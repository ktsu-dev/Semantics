// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp.Test;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ktsu.Coder.Ast;
using ktsu.Coder.Languages;

using ktsu.Semantics.Cpp;
using ktsu.Semantics.Vocabulary;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Builds one quantity as a language-agnostic AST and writes it in all seven of ktsu.Coder's
/// targets, to find out what the vocabulary cannot yet say in each.
/// </summary>
/// <remarks>
/// This is a probe rather than a generator. <see cref="CppQuantityGenerator"/> emits the whole
/// vocabulary and is C++ throughout — friend operators, <c>Quantity&lt;Dimension&lt;…&gt;&gt;</c>,
/// the shipped prelude — so it answers nothing about whether the *same* declarations reach another
/// language. This builds the neutral shape instead: a record struct over a storage type, with a
/// value and a unit factory, which is what every generated quantity in this repository is.
/// <para>
/// It lives in this project because this is where the reader of <c>dimensions.json</c> is. That is
/// itself a finding: <see cref="QuantityMetadata"/> and <see cref="MetadataProjection"/> decide
/// nothing about C++ and would have to be shared the way <c>Semantics.Vocabulary</c> already is
/// before a neutral projection could be a project of its own.
/// </para>
/// <para>
/// <b>Two targets used to produce source their own toolchains refuse</b>, and each was pinned
/// rather than skipped, so the day it was fixed upstream its test failed and said so. Python was
/// fixed by ktsu-dev/Coder#64 and Go by ktsu-dev/Coder#63; both pins failed exactly as intended,
/// and both now assert the fix, so a regression upstream fails here the same way round.
/// </para>
/// </remarks>
[TestClass]
public sealed class SevenTargetProjectionTests
{
	/// <summary>
	/// The storage type every generated quantity is written over.
	/// </summary>
	private const string StorageParameter = "T : struct, INumber<T>";

	/// <summary>
	/// The one quantity, built once. Every test reads the same AST, because the point is that there
	/// is only one and the targets differ around it.
	/// </summary>
	private static ClassDeclaration Quantity { get; } = Project(MagnitudeOfLength());

	private static Dictionary<string, string> Written { get; } = Write(Quantity);

	/// <summary>
	/// Every target writes something, which is the claim the whole exercise rests on.
	/// </summary>
	[TestMethod]
	public void EveryTargetWritesTheQuantity()
	{
		Assert.HasCount(7, Written);

		foreach ((string language, string source) in Written)
		{
			Assert.IsFalse(
				string.IsNullOrWhiteSpace(source),
				$"{language} wrote nothing for a declaration every other target wrote.");

			Assert.Contains("Length", source, $"{language} lost the quantity's name.");
			Assert.Contains("FromMeter", source, $"{language} lost the unit factory.");
		}
	}

	/// <summary>
	/// C# comes out as exactly what this repository already generates by hand.
	/// </summary>
	/// <remarks>
	/// The one target where the answer is checkable against something that exists: if the AST can
	/// reach the declaration `QuantitiesGenerator` writes, the AST is expressive enough for the
	/// quantities, and the other six are a question about those languages rather than about this
	/// one.
	/// </remarks>
	[TestMethod]
	public void CSharpMatchesWhatIsGeneratedToday()
	{
		string source = Written["csharp"];

		Assert.Contains("public readonly partial record struct Length<T> : IVector0<Length<T>, T>", source);
		Assert.Contains("where T : struct, INumber<T>", source);
		Assert.Contains("public T Value { get; init; }", source);
		Assert.Contains("public static Length<T> FromMeter(T value)", source);
	}

	/// <summary>
	/// The targets that have type parameters carry the storage type onto the declaration.
	/// </summary>
	[TestMethod]
	public void TheTargetsWithTypeParametersCarryTheStorageType()
	{
		Assert.Contains("template <typename T>", Written["cpp"]);
		Assert.Contains("struct Length<T: INumber<T>>", Written["rust"].Replace("pub ", string.Empty, StringComparison.Ordinal));
	}

	/// <summary>
	/// The targets without type parameters write the storage type down rather than inventing one.
	/// </summary>
	/// <remarks>
	/// C has no generics at all and JavaScript has no types to be generic over, so a note is the
	/// honest answer — the same decision `CompileTimeAssertion` gets in the targets that have no
	/// assertion. What matters is that neither silently drops it.
	/// </remarks>
	[TestMethod]
	public void TheTargetsWithoutTypeParametersWriteItDown()
	{
		Assert.Contains(StorageParameter, Written["c"]);
		Assert.Contains(StorageParameter, Written["javascript"]);
	}

	/// <summary>
	/// Go writes the generic type down, and everything written over it with it, so nothing names a
	/// type parameter the type never declared.
	/// </summary>
	/// <remarks>
	/// A generic type is deliberately written down rather than emitted, because a method on one
	/// needs the parameters in three places and spelled two ways. The constructor used not to be
	/// given the same treatment: it returned <c>Length[T]</c> from a <c>Length</c> that takes no
	/// parameters, and its parameter's type <c>T</c> was undefined, which `go vet` reported as
	/// `undefined: T`. ktsu.Coder now types a member written over a parameter as <c>any</c> and
	/// mentions the type without arguments, so the declaration agrees with itself.
	/// <para>
	/// This was pinned as a defect until ktsu-dev/Coder#63 was fixed, which the bump from
	/// ktsu.Coder 3.14.3 to 3.18.12 brought in. The pin failed, as it was written to, and asserts
	/// the fix instead.
	/// </para>
	/// </remarks>
	[TestMethod]
	public void GoWritesTheGenericTypeDownAndEverythingOverIt()
	{
		string source = Written["go"];

		Assert.Contains("type Length struct", source, "the type is written without parameters");
		Assert.Contains("Value any", source, "the field over the parameter is typed any");
		Assert.Contains("func LengthFromMeter(value any) Length {", source, "and so is the constructor");
		Assert.DoesNotContain("[T]", source, "nothing spells the type with a parameter it does not declare");
	}

	/// <summary>
	/// Python quotes the self-type in its own base list, which is what makes the class importable.
	/// </summary>
	/// <remarks>
	/// `IVector0&lt;Length&lt;T&gt;, T&gt;` is the self-type idiom every quantity here is declared
	/// with, and it is what C# needs to give an interface a method returning the implementing type.
	/// Python evaluates a base list eagerly, so naming the class inside its own bases —
	/// `IVector0[Length[T], T]` — raises `NameError: name 'Length' is not defined` on import. A
	/// string is the forward reference Python has for exactly this, and ktsu.Coder now writes one.
	/// <para>
	/// This was pinned as a defect until ktsu-dev/Coder#64 was fixed, which the bump from
	/// ktsu.Coder 3.14.0 to 3.14.3 brought in. The pin failed, as it was written to, and asserts
	/// the fix instead. Keeping it pins the fix the same way round, so a regression upstream fails
	/// here rather than shipping a module that cannot be imported.
	/// </para>
	/// </remarks>
	[TestMethod]
	public void PythonQuotesTheClassInsideItsOwnBases()
	{
		Assert.Contains("class Length(IVector0[\"Length[T]\", T])", Written["python"]);
	}

	/// <summary>
	/// Builds the neutral declaration for one magnitude quantity.
	/// </summary>
	/// <param name="type">The quantity to project.</param>
	/// <returns>The declaration, with no target in mind.</returns>
	/// <remarks>
	/// Deliberately the whole of what a generated magnitude is and no more: the type, what it is
	/// written over, what it claims to be, the value it stores and one way to build it. Anything a
	/// single target needs and the others cannot use belongs to that target's generator, not here.
	/// </remarks>
	private static ClassDeclaration Project(QuantityType type)
	{
		ClassDeclaration declaration = new(type.Name)
		{
			Kind = TypeDeclarationKind.Struct,
			IsRecord = true,
			IsReadOnly = true,
			IsPartial = true,
			Visibility = Visibility.Public,
		};

		declaration.Documentation.Add(type.Description);
		declaration.TypeParameters.Add(TypeParameter.Parse(StorageParameter));
		declaration.Interfaces.Add(TypeReference.Parse($"IVector0<{type.Name}<T>, T>"));

		PropertyDeclaration value = new("Value")
		{
			Type = TypeReference.Parse("T"),
			Visibility = Visibility.Public,
			HasGetter = true,
			HasSetter = true,
			SetterIsInitOnly = true,
		};
		value.Documentation.Add("Gets the value, in SI base units.");
		declaration.Members.Add(value);

		FunctionDeclaration factory = new("FromMeter")
		{
			ReturnType = TypeReference.Parse($"{type.Name}<T>"),
			IsStatic = true,
			Visibility = Visibility.Public,
		};
		factory.Documentation.Add("Builds one from a value in meters.");
		factory.Parameters.Add(new Parameter("value", "T"));
		factory.Body.Add(new ReturnStatement(
			new CallExpression("Create") { Arguments = { new VariableReference("value") } }));
		declaration.Members.Add(factory);

		return declaration;
	}

	/// <summary>
	/// Writes one declaration in every target ktsu.Coder has.
	/// </summary>
	/// <param name="declaration">What to write.</param>
	/// <returns>The source, by language id.</returns>
	private static Dictionary<string, string> Write(ClassDeclaration declaration)
	{
		ILanguageGenerator[] generators =
		[
			new CSharpGenerator(),
			new CppGenerator(),
			new CGenerator(),
			new RustGenerator(),
			new GoGenerator(),
			new PythonGenerator(),
			new JavaScriptGenerator(),
		];

		return generators.ToDictionary(
			generator => generator.LanguageId,
			generator => generator.Generate(declaration),
			StringComparer.Ordinal);
	}

	/// <summary>
	/// Reads the real metadata and picks the magnitude form of Length out of it.
	/// </summary>
	/// <returns>The quantity to project.</returns>
	/// <remarks>
	/// The real vocabulary rather than a fixture, for the reason the C++ tests use it: a probe run
	/// against something invented answers a question nobody asked.
	/// </remarks>
	private static QuantityType MagnitudeOfLength()
	{
		QuantityMetadata metadata = QuantityMetadata.Parse(
			File.ReadAllText(Path.Join(AppContext.BaseDirectory, "Metadata", "dimensions.json")));

		QuantityVocabulary vocabulary = QuantityVocabulary.FromDimensions(metadata.ToDeclarations());

		return vocabulary.Types.Single(quantity =>
			string.Equals(quantity.Name, "Length", StringComparison.Ordinal));
	}
}
