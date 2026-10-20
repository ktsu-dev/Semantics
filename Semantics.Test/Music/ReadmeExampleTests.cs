// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using System.Text.RegularExpressions;

using ktsu.Semantics.Music;

/// <summary>
/// Checks the music examples in the READMEs against the library, so that an example a reader copies
/// from the package's front page runs.
/// </summary>
[TestClass]
public partial class ReadmeExampleTests
{
	private static readonly string[] Readmes = ["Root.md", "Music.md"];
	private static readonly string[] RootExampleNumerals = ["ii7", "V7", "Imaj7"];

	private static string ReadmeDirectory => Path.Combine(AppContext.BaseDirectory, "Readmes");

	[GeneratedRegex("""Progression\.Parse\("([^"]*)"\)""")]
	private static partial Regex ProgressionLiteral();

	private static string[] ProgressionLiterals(string readme) =>
		[.. ProgressionLiteral().Matches(File.ReadAllText(Path.Combine(ReadmeDirectory, readme))).Select(match => match.Groups[1].Value)];

	[TestMethod]
	public void EveryProgressionLiteralInTheReadmesParses()
	{
		foreach (string readme in Readmes)
		{
			string[] literals = ProgressionLiterals(readme);
			Assert.IsNotEmpty(literals, readme);

			foreach (string literal in literals)
			{
				Assert.IsTrue(Progression.TryParse(literal, out _), $"{readme}: \"{literal}\"");
			}
		}
	}

	[TestMethod]
	public void RootReadmeMusicExample_GivesTheOutputsItsCommentsShow()
	{
		Progression prog = Progression.Parse(ProgressionLiterals("Root.md")[0]);

		Key? key = prog.InferKey();
		Assert.IsNotNull(key);
		Assert.AreEqual(0, key.Tonic.Value);
		Assert.AreEqual(Mode.Major, key.Mode);
		Assert.AreSequenceEqual(RootExampleNumerals, prog.RomanNumerals(key));
	}
}
