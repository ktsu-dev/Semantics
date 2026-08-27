// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using global::Semantics.SourceGenerators;

/// <summary>
/// Checks that every diagnostic the generators can report is tracked in the analyzer release files.
/// </summary>
/// <remarks>
/// <c>EnforceExtendedAnalyzerRules</c> is on, so an untracked descriptor fails the generator build
/// with RS2008 — after a push, and with an error that reads like a tooling problem rather than a
/// missing line in a markdown table. Enumerating the catalogue turns it into a test failure that
/// names the identifier.
/// </remarks>
[TestClass]
public class AnalyzerReleaseTrackingTests
{
	[TestMethod]
	public void EveryDescriptorIsTrackedInAnAnalyzerReleaseFile()
	{
		string tracked = ReadReleaseFiles();

		List<string> untracked =
		[
			.. SemanticsDiagnostics.All
				.Select(descriptor => descriptor.Id)
				.Where(id => !tracked.Contains(id, StringComparison.Ordinal))
		];

		Assert.IsEmpty(
			untracked,
			$"Add to AnalyzerReleases.Unshipped.md: {string.Join(", ", untracked)}");
	}

	[TestMethod]
	public void DescriptorIdentifiersAreUniqueAndConsecutive()
	{
		List<string> ids = [.. SemanticsDiagnostics.All.Select(descriptor => descriptor.Id)];

		Assert.HasCount(ids.Count, ids.Distinct().ToList(), "Two diagnostics share an identifier.");

		for (int index = 0; index < ids.Count; index++)
		{
			Assert.AreEqual($"SEM{index + 1:D3}", ids[index]);
		}
	}

	[TestMethod]
	public void EveryDescriptorSharesTheOneCategory()
	{
		foreach (DiagnosticDescriptor descriptor in SemanticsDiagnostics.All)
		{
			// The base generator used to report parse failures under "SourceGenerator" while
			// everything derived from it used this one.
			Assert.AreEqual("Semantics.SourceGenerators", descriptor.Category, descriptor.Id);
		}
	}

	private static string ReadReleaseFiles()
	{
		DirectoryInfo? directory = new(AppContext.BaseDirectory);
		while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "Semantics.SourceGenerators")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate the repository root from the test output directory.");

		string generatorDirectory = Path.Combine(directory!.FullName, "Semantics.SourceGenerators");
		return string.Concat(
			File.ReadAllText(Path.Combine(generatorDirectory, "AnalyzerReleases.Shipped.md")),
			File.ReadAllText(Path.Combine(generatorDirectory, "AnalyzerReleases.Unshipped.md")));
	}
}
