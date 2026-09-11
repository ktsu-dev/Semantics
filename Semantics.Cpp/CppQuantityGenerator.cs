// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

using ktsu.Coder.Ast;
using ktsu.Coder.Languages;

/// <summary>
/// Projects the quantity metadata into C++.
/// </summary>
/// <remarks>
/// The vocabulary is two layers and this generates the upper one. Underneath is
/// <c>Quantity&lt;D&gt;</c> over an eight-exponent <c>Dimension</c>, which is shipped rather than
/// generated because no part of it is derived from the metadata. On top is one class per dimension
/// and per named overload -- <c>Length</c>, <c>Speed</c>, <c>Weight</c> -- because the exponents
/// cannot tell every pair of quantities apart: 72 dimensions share 63 exponent vectors, so
/// <c>Area</c> and <c>NuclearCrossSection</c> are one vector between two names, and naming them is
/// the only thing that separates them.
/// <para>
/// <b>How the generated code is written is not a matter of taste, and this is the part to read
/// before changing anything here.</b> The same vocabulary written two ways measured 0.9896 and
/// 1.4004 against bare floats on MSVC, while GCC and clang folded both away completely -- so the
/// wrong formulation passes on three compilers of four, and a generator emits whichever one it was
/// written to emit, once per type, hundreds of times. Four rules came out of measuring that:
/// </para>
/// <list type="number">
/// <item>A produced value is built in its own initialiser, never default-constructed and then
/// assigned into. The second shape cost 22% on MSVC on its own.</item>
/// <item>An accessor returns a reference, not a copy.</item>
/// <item>Arithmetic stays in <c>Quantity</c> space rather than unwrapping to a number and
/// rewrapping the result, which is work the optimiser then has to undo.</item>
/// <item>A componentwise operation expands at compile time rather than looping over an index.
/// That one applies to the vector forms, which this does not generate yet.</item>
/// </list>
/// <para>
/// Rule three is also why a relationship is checked before it is emitted. The operator is written
/// as <c>Energy{ f.value() * d.value() }</c>, so the exponents have to agree with the declared
/// result or it does not compile -- which makes the metadata's claims checkable, and means a claim
/// that is not true is refused by name rather than emitted as something broken. See
/// <see cref="QuantityVocabulary"/>.
/// </para>
/// </remarks>
/// <param name="options">What the target says about how it wants this spelled.</param>
public sealed class CppQuantityGenerator(CppQuantityOptions options)
{
	private const string UnderlyingAlias = "underlying";
	private const string ValueField = "value_";
	private const string ValueName = "value";
	private const string DimensionTemplate = "Dimension";
	private const string QuantityTemplate = "Quantity";
	private const string PreludeNamespaceToken = "@NAMESPACE@";
	private const string PreludeBannerToken = "@BANNER@";

	/// <summary>
	/// Initializes a new instance of the <see cref="CppQuantityGenerator"/> class with the
	/// defaults.
	/// </summary>
	public CppQuantityGenerator()
		: this(new CppQuantityOptions())
	{
	}

	/// <summary>Gets what the target said.</summary>
	public CppQuantityOptions Options { get; } = options;

	/// <summary>
	/// Generates the whole vocabulary.
	/// </summary>
	/// <param name="metadata">The deserialised <c>dimensions.json</c>.</param>
	/// <returns>Every file to write, keyed by name, and everything the metadata asked for that
	/// could not be honoured.</returns>
	public CppQuantityOutput Generate(QuantityMetadata metadata)
	{
		Ensure.NotNull(metadata);

		QuantityVocabulary vocabulary = QuantityVocabulary.FromMetadata(metadata);
		Dictionary<string, string> files = [];

		foreach ((string name, string text) in Prelude())
		{
			files[name] = text;
		}

		CppGenerator writer = new();

		foreach (QuantityType type in vocabulary.Types)
		{
			files[$"{type.Name}{Options.HeaderExtension}"] = writer.Generate(Quantity(type));
		}

		files[$"relationships{Options.HeaderExtension}"] = writer.Generate(Relationships(vocabulary));
		files[$"quantities{Options.HeaderExtension}"] = writer.Generate(Umbrella(vocabulary));

		return new CppQuantityOutput(
			new ReadOnlyDictionary<string, string>(files),
			[.. vocabulary.Refused.Select(issue => issue.ToString())]);
	}

	/// <summary>
	/// The two headers that are shipped rather than generated, with the target's namespace put in.
	/// </summary>
	private IEnumerable<(string Name, string Text)> Prelude()
	{
		Assembly assembly = typeof(CppQuantityGenerator).Assembly;

		foreach (string resource in assembly.GetManifestResourceNames().Where(name => name.EndsWith(".hpp", StringComparison.Ordinal)))
		{
			using Stream stream = assembly.GetManifestResourceStream(resource)!;
			using StreamReader reader = new(stream);

			string text = reader.ReadToEnd()
				.Replace(PreludeNamespaceToken, Options.Namespace, StringComparison.Ordinal)
				.Replace(PreludeBannerToken, Banner(), StringComparison.Ordinal);

			// The resource name is the project's default namespace, a folder and the file name;
			// only the last two parts are the file.
			string[] parts = resource.Split('.');
			yield return ($"{parts[^2]}.{parts[^1]}", text.ReplaceLineEndings("\n"));
		}
	}

	private string Banner() => $"Generated by {Options.GeneratedBy}. Do not edit.";

	/// <summary>
	/// One quantity class: a distinct type wrapping a <c>Quantity</c> of its dimension.
	/// </summary>
	private SourceFile Quantity(QuantityType type)
	{
		ClassDeclaration declaration = new(type.Name);

		if (!string.IsNullOrEmpty(type.Description))
		{
			declaration.Documentation.Add(type.Description);
		}

		declaration.Documentation.Add($"dimension: {type.Dimension}");

		SortedSet<string> includes = [$"\"quantity{Options.HeaderExtension}\""];

		if (type.Magnitude is not Magnitude.Signed)
		{
			includes.Add("<cassert>");
		}

		declaration.Members.Add(new UsingAlias(
			UnderlyingAlias,
			$"{QuantityTemplate}<{type.Dimension.ToCpp(DimensionTemplate)}>"));

		if (type.Refines is not null)
		{
			includes.Add($"\"{type.Refines}{Options.HeaderExtension}\"");
			declaration.Members.Add(new UsingAlias("refines", type.Refines));
		}

		declaration.Members.Add(new FunctionDeclaration(type.Name)
		{
			Kind = FunctionKind.Constructor,
			IsCompileTimeEvaluable = true,
			IsNoThrow = true,
			Definition = FunctionDefinition.Defaulted,
		});

		declaration.Members.Add(FromUnderlying(type));
		declaration.Members.Add(Accessor());

		if (type.Refines is not null)
		{
			declaration.Members.Add(Widening(type));
			declaration.Members.Add(Narrowing(type));
		}

		declaration.Members.Add(Comparison(type.Name, "==", "bool"));
		declaration.Members.Add(Comparison(type.Name, "<=>", "auto"));
		declaration.Members.Add(new FieldDeclaration(ValueField, UnderlyingAlias) { Visibility = Visibility.Private });

		SourceFile file = new(type.Name) { IsHeader = true };
		Preamble(file, includes);
		file.Members.Add(Namespaced(declaration));
		return file;
	}

	/// <summary>
	/// The explicit constructor, which is also where a magnitude's floor is enforced.
	/// </summary>
	/// <remarks>
	/// Debug only. A magnitude that goes negative is a bug upstream of here, so the build people
	/// work in should say so and the build the zero-cost claim is measured in should have nothing
	/// to elide. <c>NDEBUG</c> is what tells the two apart, and <c>assert</c> is already compiled
	/// out by it, so no guard of our own is needed around it.
	/// </remarks>
	private static FunctionDeclaration FromUnderlying(QuantityType type)
	{
		FunctionDeclaration constructor = new(type.Name)
		{
			Kind = FunctionKind.Constructor,
			IsExplicit = true,
			IsCompileTimeEvaluable = true,
			IsNoThrow = true,
		};

		constructor.Documentation.Add($"Explicit: a bare value never becomes {Article(type.Name)} {type.Name} by accident.");
		constructor.Parameters.Add(new Parameter(ValueName, UnderlyingAlias));
		constructor.Initialisers.Add(new MemberInitialiser(ValueField, new VariableReference(Guarded(type))));

		return constructor;
	}

	/// <summary>
	/// The member initialiser, with a magnitude's floor checked on the way past.
	/// </summary>
	/// <remarks>
	/// The check rides in the initialiser rather than sitting in the constructor's body, and that
	/// is a limitation rather than a preference: <c>ktsu.Coder</c> has no expression-statement
	/// node, so a call made for its effect -- <c>assert(...)</c>, and every other void call -- is
	/// not something the AST can currently say. A comma expression in the initialiser is the
	/// recognised way to check a precondition in a <c>constexpr</c> constructor, so this is a
	/// legitimate spelling rather than a workaround wearing a disguise, but the reason it was
	/// chosen is that the alternative could not be written.
	/// <para>
	/// <c>assert</c> is already compiled out by <c>NDEBUG</c>, so no guard of our own is needed
	/// around it: the build people work in says so, and the build the zero-cost claim is measured
	/// in has nothing to elide.
	/// </para>
	/// </remarks>
	private static string Guarded(QuantityType type)
	{
		if (type.Magnitude is Magnitude.Signed)
		{
			return ValueName;
		}

		string comparison = type.Magnitude == Magnitude.Positive ? ">" : ">=";
		string says = type.Magnitude == Magnitude.Positive
			? $"{Article(type.Name)} {type.Name} of zero is not a physical value"
			: $"{Article(type.Name)} {type.Name} cannot be negative";

		return $"(assert({ValueName}.count() {comparison} 0 && \"{says}\"), {ValueName})";
	}

	private static FunctionDeclaration Accessor()
	{
		FunctionDeclaration accessor = new(ValueName)
		{
			// A reference rather than a copy, which on MSVC is the difference between a register
			// and a spill. Rule two.
			ReturnType = $"const {UnderlyingAlias}&",
			IsPure = true,
			IsCompileTimeEvaluable = true,
			IsReadOnly = true,
			IsNoThrow = true,
		};

		accessor.Documentation.Add("Named, because getting the value back out is a decision too.");
		accessor.Body.Add(new ReturnStatement(new VariableReference(ValueField)));
		return accessor;
	}

	private static FunctionDeclaration Widening(QuantityType type)
	{
		FunctionDeclaration widening = new(type.Refines!)
		{
			Kind = FunctionKind.ConversionOperator,
			ReturnType = type.Refines,
			IsPure = true,
			IsCompileTimeEvaluable = true,
			IsReadOnly = true,
			IsNoThrow = true,
		};

		widening.Documentation.Add($"Widening is implicit: this is {Article(type.Refines!)} {type.Refines}.");
		widening.Body.Add(new ReturnStatement(
			new ConstructionExpression(type.Refines) { Arguments = { new VariableReference(ValueField) } }));

		return widening;
	}

	private static FunctionDeclaration Narrowing(QuantityType type)
	{
		FunctionDeclaration narrowing = new("from")
		{
			ReturnType = type.Name,
			IsPure = true,
			IsStatic = true,
			IsCompileTimeEvaluable = true,
			IsNoThrow = true,
		};

		narrowing.Documentation.Add(
			$"Narrowing is explicit and named: not every {type.Refines} is {Article(type.Name)} {type.Name}.");
		narrowing.Parameters.Add(new Parameter(ValueName, type.Refines!));
		narrowing.Body.Add(new ReturnStatement(new ConstructionExpression(type.Name)
		{
			Arguments = { new VariableReference($"{ValueName}.{ValueName}()") },
		}));

		return narrowing;
	}

	private static FunctionDeclaration Comparison(string name, string symbol, string returnType)
	{
		FunctionDeclaration comparison = new(symbol)
		{
			Kind = FunctionKind.Operator,
			ReturnType = returnType,
			IsPure = true,
			IsFriend = true,
			IsCompileTimeEvaluable = true,
			IsNoThrow = true,
			Definition = FunctionDefinition.Defaulted,
		};

		comparison.Parameters.Add(new Parameter(string.Empty, name));
		comparison.Parameters.Add(new Parameter(string.Empty, name));
		return comparison;
	}

	/// <summary>
	/// The operators the metadata declares, as free functions.
	/// </summary>
	/// <remarks>
	/// In one file rather than beside their operands, because a relationship belongs to neither of
	/// the two types it joins and putting it in one of their headers would decide arbitrarily
	/// which of them a program has to include to multiply.
	/// </remarks>
	private SourceFile Relationships(QuantityVocabulary vocabulary)
	{
		SourceFile file = new("relationships") { IsHeader = true };

		SortedSet<string> includes = [];
		List<AstNode> operators = [];

		foreach (QuantityRelationship relationship in vocabulary.Relationships)
		{
			foreach (string named in (string[])[relationship.Left, relationship.Right, relationship.Result])
			{
				includes.Add($"\"{named}{Options.HeaderExtension}\"");
			}

			FunctionDeclaration declaration = new(relationship.Symbol)
			{
				Kind = FunctionKind.Operator,
				ReturnType = relationship.Result,
				IsPure = true,
				IsCompileTimeEvaluable = true,
				IsNoThrow = true,
			};

			declaration.Documentation.Add(relationship.ToString());
			declaration.Parameters.Add(new Parameter("lhs", relationship.Left));
			declaration.Parameters.Add(new Parameter("rhs", relationship.Right));

			// Rule three: the arithmetic stays in Quantity space. That is also what makes the
			// declared relationship checkable -- if the exponents disagreed with the result type
			// this would not compile, which is why a relationship that disagrees is never
			// generated in the first place.
			declaration.Body.Add(new ReturnStatement(new ConstructionExpression(relationship.Result)
			{
				Arguments = { new VariableReference($"lhs.{ValueName}() {relationship.Symbol} rhs.{ValueName}()") },
			}));

			operators.Add(declaration);
		}

		Preamble(file, includes);
		file.HeaderComment.Add(string.Empty);
		file.HeaderComment.Add($"{vocabulary.Relationships.Count} relationships, from the integrals and derivatives");
		file.HeaderComment.Add("dimensions.json declares. Each is checked against the exponents before it is");
		file.HeaderComment.Add("written, so every operator here is one the dimensions agree with.");

		file.Members.Add(Namespaced([.. operators]));
		return file;
	}

	/// <summary>
	/// One header that includes the whole vocabulary, for a program that does not want to track
	/// which quantity lives where.
	/// </summary>
	private SourceFile Umbrella(QuantityVocabulary vocabulary)
	{
		SourceFile file = new("quantities") { IsHeader = true };

		SortedSet<string> includes = [$"\"relationships{Options.HeaderExtension}\""];
		foreach (QuantityType type in vocabulary.Types)
		{
			includes.Add($"\"{type.Name}{Options.HeaderExtension}\"");
		}

		Preamble(file, includes);
		file.HeaderComment.Add(string.Empty);
		file.HeaderComment.Add($"{vocabulary.Types.Count} quantity types over {DistinctDimensions(vocabulary)} distinct dimensions.");
		return file;
	}

	private static int DistinctDimensions(QuantityVocabulary vocabulary) =>
		vocabulary.Types.Select(type => type.Dimension).Distinct().Count();

	private void Preamble(SourceFile file, IEnumerable<string> includes)
	{
		file.HeaderComment.Add(Banner());
		file.HeaderComment.Add(string.Empty);
		file.HeaderComment.Add("Editing this file is editing the wrong thing: it is derived from the quantity");
		file.HeaderComment.Add("metadata, and the next build overwrites it. Change dimensions.json instead.");

		foreach (string include in includes)
		{
			file.Imports.Add(include);
		}
	}

	private NamespaceDeclaration Namespaced(params AstNode[] members)
	{
		NamespaceDeclaration space = new(Options.Namespace);
		foreach (AstNode member in members)
		{
			space.Members.Add(member);
		}

		return space;
	}

	private static string Article(string name) =>
		"AEIOU".Contains(char.ToUpperInvariant(name[0]), StringComparison.Ordinal) ? "an" : "a";
}

/// <summary>
/// What the generator produced, and what it would not.
/// </summary>
/// <param name="Files">Every file to write, keyed by name.</param>
/// <param name="Refused">
/// What the metadata asked for that the exponents contradict, each named with both dimensions
/// written out. Empty is the expected state; anything here is a metadata bug rather than a
/// generator limitation.
/// </param>
public sealed record CppQuantityOutput(IReadOnlyDictionary<string, string> Files, IReadOnlyList<string> Refused);
