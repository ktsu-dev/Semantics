// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

using ktsu.Coder.Ast;
using ktsu.Coder.Languages;
using ktsu.Semantics.Vocabulary;

/// <summary>
/// Projects the quantity metadata into C++.
/// </summary>
/// <remarks>
/// The vocabulary is two layers and this generates the upper one. Underneath is
/// <c>Quantity&lt;D&gt;</c> over an eight-exponent <c>Dimension</c>, which is shipped rather than
/// generated because no part of it is derived from the metadata. On top is one class per dimension,
/// per vector form and per named overload -- <c>Length</c>, <c>Displacement3D</c>, <c>Weight</c> --
/// because the exponents cannot tell every pair of quantities apart: 72 dimensions share 63
/// exponent vectors, so <c>Area</c> and <c>NuclearCrossSection</c> are one vector between two
/// names, and naming them is the only thing that separates them.
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
/// <item>A componentwise operation expands at compile time rather than looping over an index. A
/// loop over a runtime subscript is what took the same spike from 1.01 to 4.51 on MSVC.</item>
/// </list>
/// <para>
/// Rule four is the one the vector forms brought, and it costs this generator nothing, which is
/// worth saying because it costs a hand-written library a great deal. A library writes
/// <c>Vector3&lt;Q&gt;</c> once over every <c>Q</c>, so its componentwise operations have to be
/// written once too, and expanding them at compile time rather than looping means an index-sequence
/// fold and the machinery around it. A generator has the components in hand when it writes the
/// class, so it writes them out: <c>Displacement3D{ a.x() * s, a.y() * s, a.z() * s }</c> is the
/// fully expanded form already, with no fold to arrange and no index to be a runtime value.
/// </para>
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
	private const string ComponentAlias = "component";
	private const string ValueName = "value";
	private const string DimensionTemplate = "Dimension";
	private const string QuantityTemplate = "Quantity";
	private const string PreludeNamespaceToken = "@NAMESPACE@";
	private const string PreludeBannerToken = "@BANNER@";

	/// <summary>
	/// What a vector's components are called, in order.
	/// </summary>
	/// <remarks>
	/// Names rather than an index, because <c>x</c> and <c>y</c> are what a reader wants and
	/// <c>[0]</c> is not -- and because a component reached by name is reached at compile time,
	/// which is rule four.
	/// </remarks>
	private static readonly string[] ComponentNames = ["x", "y", "z", "w"];

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

		QuantityVocabulary vocabulary = QuantityVocabulary.FromDimensions(metadata.ToDeclarations());
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
	/// What a type's components are called: one <c>value</c> for a scalar, and <c>x</c> through
	/// <c>w</c> for a vector.
	/// </summary>
	private static string[] Components(QuantityType type) =>
		type.IsVector ? ComponentNames[..type.Form] : [ValueName];

	/// <summary>The member behind one component's accessor.</summary>
	private static string Field(string component) => $"{component}_";

	/// <summary>
	/// What one component is spelled as: a scalar's whole value, or one of a vector's several.
	/// </summary>
	private static string Storage(QuantityType type) => type.IsVector ? ComponentAlias : UnderlyingAlias;

	/// <summary>
	/// One quantity class: a distinct type wrapping one <c>Quantity</c> of its dimension, or as
	/// many of them as it has components.
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

		if (type.Form > 0)
		{
			// Every form answers its size with the magnitude form of the same dimension, which is
			// the one place the signed half of the vocabulary reaches back into the unsigned half.
			includes.Add($"\"{type.MagnitudeType}{Options.HeaderExtension}\"");
		}

		declaration.Members.Add(new UsingAlias(
			Storage(type),
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

		declaration.Members.Add(FromComponents(type));

		foreach (string component in Components(type))
		{
			declaration.Members.Add(Accessor(type, component));
		}

		if (type.Form > 0)
		{
			declaration.Members.Add(MagnitudeOf(type));
		}

		if (type.IsVector)
		{
			declaration.Members.Add(MagnitudeSquared(type));
		}

		if (type.Refines is not null)
		{
			declaration.Members.Add(Widening(type));
			declaration.Members.Add(Narrowing(type));
		}

		if (type.Magnitude is Magnitude.Signed)
		{
			// Arithmetic belongs to the signed forms and stops at them. A magnitude has the
			// question of what `Length - Length` means when the answer would be negative, which
			// the .NET side settled as the absolute difference; that is a decision about the
			// magnitude form rather than about the vector forms, and it is not made here.
			foreach (AstNode member in Arithmetic(type))
			{
				declaration.Members.Add(member);
			}
		}

		declaration.Members.Add(Comparison(type.Name, "==", "bool"));

		if (!type.IsVector)
		{
			// A vector has no order: there is no reading in which one displacement is less than
			// another, so it gets equality and nothing more.
			declaration.Members.Add(Comparison(type.Name, "<=>", "auto"));
		}

		foreach (string component in Components(type))
		{
			declaration.Members.Add(
				new FieldDeclaration(Field(component), Storage(type)) { Visibility = Visibility.Private });
		}

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
	private static FunctionDeclaration FromComponents(QuantityType type)
	{
		FunctionDeclaration constructor = new(type.Name)
		{
			Kind = FunctionKind.Constructor,
			IsExplicit = true,
			IsCompileTimeEvaluable = true,
			IsNoThrow = true,
		};

		string[] components = Components(type);

		constructor.Documentation.Add(type.IsVector
			? $"Explicit: {components.Length.ToString(CultureInfo.InvariantCulture)} bare values never become {Article(type.Name)} {type.Name} by accident."
			: $"Explicit: a bare value never becomes {Article(type.Name)} {type.Name} by accident.");

		foreach (string component in components)
		{
			constructor.Parameters.Add(new Parameter(component, Storage(type)));
			constructor.Initialisers.Add(
				new MemberInitialiser(Field(component), new VariableReference(Guarded(type, component))));
		}

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
	private static string Guarded(QuantityType type, string component)
	{
		if (type.Magnitude is Magnitude.Signed)
		{
			return component;
		}

		string comparison = type.Magnitude == Magnitude.Positive ? ">" : ">=";
		string says = type.Magnitude == Magnitude.Positive
			? $"{Article(type.Name)} {type.Name} of zero is not a physical value"
			: $"{Article(type.Name)} {type.Name} cannot be negative";

		return $"(assert({component}.count() {comparison} 0 && \"{says}\"), {component})";
	}

	private static FunctionDeclaration Accessor(QuantityType type, string component)
	{
		FunctionDeclaration accessor = new(component)
		{
			// A reference rather than a copy, which on MSVC is the difference between a register
			// and a spill. Rule two.
			ReturnType = $"const {Storage(type)}&",
			IsPure = true,
			IsCompileTimeEvaluable = true,
			IsReadOnly = true,
			IsNoThrow = true,
		};

		accessor.Documentation.Add(type.IsVector
			? $"The {component} component."
			: "Named, because getting the value back out is a decision too.");

		accessor.Body.Add(new ReturnStatement(new VariableReference(Field(component))));
		return accessor;
	}

	/// <summary>
	/// How big this is, without its direction: the bridge from a signed form back to the magnitude
	/// form of the same dimension.
	/// </summary>
	/// <remarks>
	/// A vector's length is not constant-evaluable because a square root is not, so only the
	/// one-component form -- where the answer is an absolute value -- is <c>constexpr</c>.
	/// <para>
	/// The dimension works out on its own and that is worth noticing rather than arranging: the sum
	/// of the squares of the components has twice the dimension of one of them, and halving it
	/// again is what <c>sqrt</c> does, so the result is a component's dimension whatever that was.
	/// The magnitude form's constructor takes exactly that, so a mistake here would not compile.
	/// </para>
	/// </remarks>
	private static FunctionDeclaration MagnitudeOf(QuantityType type)
	{
		FunctionDeclaration magnitude = new("magnitude")
		{
			ReturnType = type.MagnitudeType,
			IsPure = true,
			IsCompileTimeEvaluable = !type.IsVector,
			IsReadOnly = true,
			IsNoThrow = true,
		};

		magnitude.Documentation.Add(type.IsVector
			? $"How long this is, as {Article(type.MagnitudeType)} {type.MagnitudeType}."
			: $"How big this is without its sign, as {Article(type.MagnitudeType)} {type.MagnitudeType}.");

		string size = type.IsVector ? "sqrt(magnitude_squared())" : $"abs({Field(ValueName)})";

		magnitude.Body.Add(new ReturnStatement(new ConstructionExpression(type.MagnitudeType)
		{
			Arguments = { new VariableReference(size) },
		}));

		return magnitude;
	}

	/// <summary>
	/// The square of the length, which needs no square root and so stays constant-evaluable.
	/// </summary>
	/// <remarks>
	/// It answers with a bare <c>Quantity</c> rather than a named type, and that is the honest
	/// answer rather than a shortcut: the square of a dimension usually has no name in the
	/// metadata, and where it does -- a length squared is an area -- the name is not unique, since
	/// <c>Area</c> and <c>NuclearCrossSection</c> are the same exponents. The structural layer is
	/// exactly what exists for a value whose dimension is real and whose name is not.
	/// </remarks>
	private static FunctionDeclaration MagnitudeSquared(QuantityType type)
	{
		FunctionDeclaration squared = new("magnitude_squared")
		{
			ReturnType = $"{QuantityTemplate}<{(type.Dimension + type.Dimension).ToCpp(DimensionTemplate)}>",
			IsPure = true,
			IsCompileTimeEvaluable = true,
			IsReadOnly = true,
			IsNoThrow = true,
		};

		squared.Documentation.Add("The square of the length, for a comparison that does not need the root.");
		squared.Body.Add(new ReturnStatement(new VariableReference(
			string.Join(" + ", Components(type).Select(component => $"{Field(component)} * {Field(component)}")))));

		return squared;
	}

	/// <summary>
	/// What a signed form can do with another of itself: add, subtract, negate and scale.
	/// </summary>
	/// <remarks>
	/// Componentwise and written out, which is rule four with nothing clever about it. A generated
	/// class knows how many components it has while it is being written, so the expanded form is
	/// simply what there is to write.
	/// </remarks>
	private static IEnumerable<AstNode> Arithmetic(QuantityType type)
	{
		yield return Binary(type, "+");
		yield return Binary(type, "-");
		yield return Negation(type);
		yield return Scaled(type, "*", scaleFirst: false);
		yield return Scaled(type, "*", scaleFirst: true);
		yield return Scaled(type, "/", scaleFirst: false);
	}

	private static FunctionDeclaration Binary(QuantityType type, string symbol)
	{
		FunctionDeclaration declaration = Friend(symbol, type.Name);
		declaration.Parameters.Add(new Parameter("lhs", type.Name));
		declaration.Parameters.Add(new Parameter("rhs", type.Name));
		declaration.Body.Add(Built(type, component => $"lhs.{Field(component)} {symbol} rhs.{Field(component)}"));
		return declaration;
	}

	private static FunctionDeclaration Negation(QuantityType type)
	{
		FunctionDeclaration declaration = Friend("-", type.Name);
		declaration.Parameters.Add(new Parameter("operand", type.Name));
		declaration.Body.Add(Built(type, component => $"-operand.{Field(component)}"));
		return declaration;
	}

	/// <summary>
	/// Scaling by a bare number, which leaves the dimension alone.
	/// </summary>
	/// <remarks>
	/// Both orders, because a reader writes <c>2.0f * v</c> as readily as <c>v * 2.0f</c> and C++
	/// will not find the second overload from the first. Division has only the one order: a number
	/// divided by a displacement is not a displacement.
	/// </remarks>
	private static FunctionDeclaration Scaled(QuantityType type, string symbol, bool scaleFirst)
	{
		FunctionDeclaration declaration = Friend(symbol, type.Name);
		string scale = "scale";
		string storage = $"{Storage(type)}::rep";

		if (scaleFirst)
		{
			declaration.Parameters.Add(new Parameter(scale, storage));
			declaration.Parameters.Add(new Parameter("operand", type.Name));
		}
		else
		{
			declaration.Parameters.Add(new Parameter("operand", type.Name));
			declaration.Parameters.Add(new Parameter(scale, storage));
		}

		declaration.Body.Add(Built(type, component => scaleFirst
			? $"{scale} {symbol} operand.{Field(component)}"
			: $"operand.{Field(component)} {symbol} {scale}"));

		return declaration;
	}

	/// <summary>
	/// A result built from all of its components at once, which is rule one.
	/// </summary>
	private static ReturnStatement Built(QuantityType type, Func<string, string> perComponent)
	{
		ConstructionExpression construction = new(type.Name);

		foreach (string component in Components(type))
		{
			construction.Arguments.Add(new VariableReference(perComponent(component)));
		}

		return new ReturnStatement(construction);
	}

	private static FunctionDeclaration Friend(string symbol, string returnType) => new(symbol)
	{
		Kind = FunctionKind.Operator,
		ReturnType = returnType,
		IsPure = true,
		IsFriend = true,
		IsCompileTimeEvaluable = true,
		IsNoThrow = true,
	};

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

		ConstructionExpression construction = new(type.Refines);
		foreach (string component in Components(type))
		{
			construction.Arguments.Add(new VariableReference(Field(component)));
		}

		widening.Body.Add(new ReturnStatement(construction));
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

		ConstructionExpression construction = new(type.Name);
		foreach (string component in Components(type))
		{
			construction.Arguments.Add(new VariableReference($"{ValueName}.{component}()"));
		}

		narrowing.Body.Add(new ReturnStatement(construction));
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

			operators.Add(Operator(relationship));
		}

		Preamble(file, includes);
		file.HeaderComment.Add(string.Empty);
		file.HeaderComment.Add($"{vocabulary.Relationships.Count} relationships, from what dimensions.json declares as");
		file.HeaderComment.Add("integrals, derivatives and cross products. Each is checked against the exponents");
		file.HeaderComment.Add("before it is written, so every operator here is one the dimensions agree with.");

		file.Members.Add(Namespaced([.. operators]));
		return file;
	}

	/// <summary>
	/// One declared relationship, at one form.
	/// </summary>
	/// <remarks>
	/// Rule three: the arithmetic stays in <c>Quantity</c> space. That is also what makes the
	/// declared relationship checkable -- if the exponents disagreed with the result type this
	/// would not compile, which is why a relationship that disagrees is never generated in the
	/// first place.
	/// </remarks>
	private static FunctionDeclaration Operator(QuantityRelationship relationship)
	{
		FunctionDeclaration declaration = new(relationship.Symbol)
		{
			Kind = relationship.IsOperator ? FunctionKind.Operator : FunctionKind.Method,
			ReturnType = relationship.Result,
			IsPure = true,
			IsCompileTimeEvaluable = true,
			IsNoThrow = true,
		};

		declaration.Documentation.Add(relationship.ToString());
		declaration.Parameters.Add(new Parameter("lhs", relationship.Left));
		declaration.Parameters.Add(new Parameter("rhs", relationship.Right));

		ConstructionExpression construction = new(relationship.Result);
		foreach (string term in relationship.Kind == RelationshipKind.Cross
			? Crossed()
			: Scaling(relationship))
		{
			construction.Arguments.Add(new VariableReference(term));
		}

		declaration.Body.Add(new ReturnStatement(construction));
		return declaration;
	}

	/// <summary>
	/// Every component of the left operand against the whole of the right, which is what scaling a
	/// vector by a magnitude is.
	/// </summary>
	private static IEnumerable<string> Scaling(QuantityRelationship relationship) =>
		Reached(relationship.Form).Select(component =>
			$"lhs.{component}() {relationship.Symbol} rhs.{ValueName}()");

	/// <summary>
	/// The three components of a cross product, written out.
	/// </summary>
	/// <remarks>
	/// Only in three dimensions, which is the definition rather than a limitation of this
	/// generator: the cross product exists in 3D and 7D and nowhere else, and the metadata
	/// declares it at <c>forms: [3]</c> for that reason.
	/// </remarks>
	private static IEnumerable<string> Crossed()
	{
		string[] axes = ["x", "y", "z"];

		for (int axis = 0; axis < axes.Length; axis++)
		{
			string next = axes[(axis + 1) % axes.Length];
			string after = axes[(axis + 2) % axes.Length];
			yield return $"lhs.{next}() * rhs.{after}() - lhs.{after}() * rhs.{next}()";
		}
	}

	/// <summary>What the accessors of a type at one form are called.</summary>
	private static string[] Reached(int form) =>
		form >= 2 ? ComponentNames[..form] : [ValueName];

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
		file.HeaderComment.Add($"{vocabulary.Types.Count} quantity types over {DistinctDimensions(vocabulary)} distinct dimensions,");
		file.HeaderComment.Add($"in {Forms(vocabulary)}.");
		return file;
	}

	private static string Forms(QuantityVocabulary vocabulary)
	{
		IEnumerable<string> counted = vocabulary.Types
			.GroupBy(type => type.Form)
			.OrderBy(group => group.Key)
			.Select(group => $"{group.Count().ToString(CultureInfo.InvariantCulture)} at vector{group.Key.ToString(CultureInfo.InvariantCulture)}");

		return string.Join(", ", counted);
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
/// What the metadata asked for that the generator will not honour, each named with the reason
/// written out. Empty is the expected state; anything here is a metadata bug rather than a
/// generator limitation.
/// </param>
public sealed record CppQuantityOutput(IReadOnlyDictionary<string, string> Files, IReadOnlyList<string> Refused);
