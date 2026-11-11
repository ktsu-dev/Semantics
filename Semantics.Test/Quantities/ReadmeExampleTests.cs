// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Text.RegularExpressions;

using ktsu.Semantics.Quantities;
using ktsu.Semantics.Strings;
using ktsu.Semantics.Strings.Identifiers;

/// <summary>
/// Checks the vector example in the Quantities README, and the identifier examples in the Identifiers README,
/// against the library, so that an example a reader copies from a package's front page compiles (#543).
/// </summary>
[TestClass]
public partial class ReadmeExampleTests
{
	private const double Tolerance = 1e-12;

	/// <summary>
	/// The statements of the Quantities README's vector example, in order, without their comments.
	/// <see cref="QuantitiesReadmeVectorExample_RunsAndGivesTheValuesItsCommentsShow"/> runs the same statements,
	/// so the README cannot show a call the library does not have.
	/// </summary>
	private static readonly string[] VectorExampleStatements =
	[
		"Force3D<double> f = new() { X = 3.0, Y = 4.0, Z = 0.0 };",
		"ForceMagnitude<double> mag = f.Magnitude();",
		"Force3D<double> unit = f.Normalize();",
		"Displacement3D<double> d = new() { X = 1.0, Y = 0.0, Z = 0.0 };",
		"double work = (f.X * d.X) + (f.Y * d.Y) + (f.Z * d.Z);",
		"Torque3D<double> torque = d.Cross(f);",
		"Momentum3D<double> impulse = f * Duration<double>.Create(2.0);",
	];

	private static string ReadmeDirectory => Path.Combine(AppContext.BaseDirectory, "Readmes");

	[GeneratedRegex(@"```csharp\n(.*?)```", RegexOptions.Singleline)]
	private static partial Regex CSharpBlock();

	[GeneratedRegex("//.*$")]
	private static partial Regex LineComment();

	private static string[] CSharpBlocks(string readme) =>
		[.. CSharpBlock().Matches(File.ReadAllText(Path.Combine(ReadmeDirectory, readme)).ReplaceLineEndings("\n"))
			.Select(match => match.Groups[1].Value)];

	private static string[] Statements(string block) =>
		[.. block.Split('\n')
			.Select(line => LineComment().Replace(line, string.Empty).Trim())
			.Where(line => line.Length > 0 && !line.StartsWith("using ", StringComparison.Ordinal))];

	[TestMethod]
	public void QuantitiesReadmeVectorExample_IsTheExampleThisTestRuns()
	{
		string[] vectorBlocks = [.. CSharpBlocks("Quantities.md").Where(block => block.Contains("Force3D<double> f", StringComparison.Ordinal))];

		Assert.HasCount(1, vectorBlocks);
		Assert.AreSequenceEqual(VectorExampleStatements, Statements(vectorBlocks[0]));
	}

	[TestMethod]
	public void QuantitiesReadmeVectorExample_RunsAndGivesTheValuesItsCommentsShow()
	{
		Force3D<double> f = new() { X = 3.0, Y = 4.0, Z = 0.0 };
		ForceMagnitude<double> mag = f.Magnitude();
		Force3D<double> unit = f.Normalize();
		Displacement3D<double> d = new() { X = 1.0, Y = 0.0, Z = 0.0 };
		double work = (f.X * d.X) + (f.Y * d.Y) + (f.Z * d.Z);
		Torque3D<double> torque = d.Cross(f);
		Momentum3D<double> impulse = f * Duration<double>.Create(2.0);

		Assert.AreEqual(5.0, mag.Value, Tolerance);
		Assert.AreEqual(1.0, unit.Magnitude().Value, Tolerance);
		Assert.AreEqual(3.0, work, Tolerance);
		Assert.AreEqual(0.0, torque.X, Tolerance);
		Assert.AreEqual(0.0, torque.Y, Tolerance);
		Assert.AreEqual(4.0, torque.Z, Tolerance);
		Assert.AreEqual(6.0, impulse.X, Tolerance);
		Assert.AreEqual(8.0, impulse.Y, Tolerance);
	}

	[TestMethod]
	public void QuantitiesReadme_NamesNoTypedForceDisplacementDotProduct()
	{
		string readme = File.ReadAllText(Path.Combine(ReadmeDirectory, "Quantities.md"));

		Assert.DoesNotContain("Force3D.Dot(Displacement3D)", readme);
	}

	[TestMethod]
	public void IdentifiersReadme_EveryBlockCallingAsImportsTheNamespaceThatDeclaresIt()
	{
		string[] blocks = [.. CSharpBlocks("Identifiers.md").Where(block => block.Contains(".As<", StringComparison.Ordinal))];

		Assert.IsNotEmpty(blocks);
		foreach (string block in blocks)
		{
			Assert.Contains("using ktsu.Semantics.Strings;", block, block);
		}

		// The extension the README calls lives in ktsu.Semantics.Strings, which this file imports for it.
		CreditCardNumber card = "4111-1111-1111-1111".As<CreditCardNumber>();
		Assert.AreEqual("4111111111111111", card.WeakString);
	}
}
