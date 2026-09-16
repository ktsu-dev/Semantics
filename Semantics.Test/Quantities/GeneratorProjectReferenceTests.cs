// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Checks that every reference to the source generator project asks for it the same way.
/// </summary>
/// <remarks>
/// <para>
/// MSBuild keys its project-instance cache on the project path together with the global properties
/// it was asked for. <c>AdditionalProperties</c> on a <c>ProjectReference</c> becomes a global
/// property, so two consumers passing different ones are asking for two instances of the same
/// project — and both build, concurrently, into the one
/// <c>Semantics.SourceGenerators\bin\$(Configuration)\netstandard2.0</c> directory.
/// </para>
/// <para>
/// A compiler reading <c>Semantics.SourceGenerators.dll</c> as an analyzer while one of those
/// copies is in flight fails with <c>CS0006: Metadata file could not be found</c>, reported against
/// <c>Semantics.Quantities</c>, which has nothing wrong with it. Being a race it needs the build to
/// be wide enough to lose, so it passed locally and on the test runners and failed in
/// <c>Analyze &amp; Release</c>.
/// </para>
/// <para>
/// The condition is cheap to state and cheap to check, so it is checked here rather than left to a
/// comment: every reference to the generator carries the same global properties, which today means
/// none carries any. A consumer wanting something different from the generator's output filters its
/// own references instead — see <c>DropBundledGeneratorDependencies</c> in this project's file.
/// </para>
/// </remarks>
[TestClass]
public class GeneratorProjectReferenceTests
{
	private const string GeneratorProject = "Semantics.SourceGenerators.csproj";

	/// <summary>
	/// The ProjectReference attributes that turn into global properties on the referenced project,
	/// and so decide how many instances of it MSBuild builds.
	/// </summary>
	private static readonly string[] ForkingAttributes =
	[
		"AdditionalProperties",
		"SetTargetFramework",
		"GlobalPropertiesToRemove",
		"UndefineProperties",
	];

	[TestMethod]
	public void NoProjectForksTheGeneratorWithAdditionalProperties()
	{
		List<string> offenders =
		[
			.. GeneratorReferences()
				.Where(reference => reference.Element.Attribute("AdditionalProperties") is not null)
				.Select(reference =>
					$"{reference.Project}: AdditionalProperties=\"{reference.Element.Attribute("AdditionalProperties")!.Value}\"")
		];

		Assert.IsEmpty(
			offenders,
			"A ProjectReference to the source generator passes AdditionalProperties, which forks it " +
			"into a second MSBuild instance building into the same output directory. Filter the " +
			"resolved references in the consuming project instead. Offenders: " +
			string.Join("; ", offenders));
	}

	[TestMethod]
	public void EveryGeneratorReferenceAsksForTheSameGlobalProperties()
	{
		List<(string Project, string Properties)> asked =
		[
			.. GeneratorReferences()
				.Select(reference => (
					reference.Project,
					Properties: string.Join(
						";",
						ForkingAttributes
							.Select(name => reference.Element.Attribute(name)?.Value ?? "")
							.Where(value => value.Length > 0))))
		];

		Assert.IsNotEmpty(asked, $"No project references {GeneratorProject}; this test is checking nothing.");

		List<string> distinct = [.. asked.Select(one => one.Properties).Distinct(StringComparer.Ordinal)];

		Assert.HasCount(
			1,
			distinct,
			"References to the source generator ask for different global properties, so MSBuild " +
			"builds it more than once into one output directory: " +
			string.Join(", ", asked.Select(one => $"{one.Project} -> \"{one.Properties}\"")));
	}

	private static IEnumerable<(string Project, XElement Element)> GeneratorReferences()
	{
		foreach (string project in Directory.EnumerateFiles(RepositoryRoot(), "*.csproj", SearchOption.AllDirectories))
		{
			XDocument document = XDocument.Load(project);
			IEnumerable<XElement> references = document
				.Descendants()
				.Where(element => element.Name.LocalName == "ProjectReference")
				.Where(element =>
					(element.Attribute("Include")?.Value ?? "")
						.Replace('\\', '/')
						.EndsWith(GeneratorProject, StringComparison.Ordinal));

			foreach (XElement reference in references)
			{
				yield return (Path.GetFileName(project), reference);
			}
		}
	}

	private static string RepositoryRoot()
	{
		DirectoryInfo? directory = new(AppContext.BaseDirectory);
		while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "Semantics.SourceGenerators")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate the repository root from the test output directory.");
		return directory!.FullName;
	}
}
