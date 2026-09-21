// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Checks that the storage-type alias packages ship their props where the binding stops at the
/// project that asked for it.
/// </summary>
/// <remarks>
/// <para>
/// Each <c>Semantics.Quantities.{Double,Float,Decimal,Precise}</c> package is props only: its
/// payload is one file of roughly 220 <c>&lt;Using&gt;</c> items, which the SDK turns into
/// project-wide <c>global using Mass = …&lt;double&gt;;</c> aliases keyed on the bare type name. Two
/// storage types reaching one project therefore define every alias twice, and the compile fails with
/// one <c>CS1537</c> per quantity against <c>GlobalUsings.g.cs</c> — a generated file the author
/// never wrote, in an error that names no package.
/// </para>
/// <para>
/// <c>buildTransitive/</c> is by definition the folder whose contents flow past the referencing
/// project to everything downstream of it, so packing there made that collision reachable from a
/// project that references no alias package at all: the documented rule, "one alias package per
/// project", was satisfied by every project in the graph and the build still died. <c>build/</c>
/// binds where the reference is and nowhere else, which is the rule exactly, and removes the failure
/// mode rather than diagnosing it.
/// </para>
/// <para>
/// The folder is one word in a <c>PackagePath</c>, invisible in every build in this repository —
/// nothing here consumes the alias packages as packages — and only wrong once a consumer composes
/// two of them. So it is asserted here, where a regression costs a test rather than a release.
/// </para>
/// </remarks>
[TestClass]
public class AliasPropsPackagingTests
{
	/// <summary>The storage-type suffixes that name the four satellite packages.</summary>
	private static readonly string[] StorageSuffixes = ["Double", "Float", "Decimal", "Precise"];

	[TestMethod]
	public void EveryAliasPackagePacksItsPropsIntoBuildRatherThanBuildTransitive()
	{
		List<string> offenders = [];

		foreach ((string suffix, XElement packed) in PackedPropsItems())
		{
			string packagePath = packed.Attribute("PackagePath")?.Value ?? "";
			if (Normalize(packagePath) is not "build/")
			{
				offenders.Add($"Semantics.Quantities.{suffix}: PackagePath=\"{packagePath}\"");
			}
		}

		Assert.IsEmpty(
			offenders,
			"An alias package packs its props outside build/, so the storage-type binding flows past " +
			"the project that referenced it. Two alias packages anywhere in one dependency graph then " +
			"collide on every alias name. Offenders: " + string.Join("; ", offenders));
	}

	[TestMethod]
	public void EveryAliasPackagePacksThePropsFileThatExistsOnDisk()
	{
		List<string> offenders = [];

		foreach ((string suffix, XElement packed) in PackedPropsItems())
		{
			string include = packed.Attribute("Include")?.Value ?? "";
			string expected = $"build/ktsu.Semantics.Quantities.{suffix}.props";

			if (Normalize(include) != expected)
			{
				offenders.Add($"Semantics.Quantities.{suffix}: Include=\"{include}\", expected \"{expected}\"");
				continue;
			}

			string onDisk = Path.Combine(
				RepositoryRoot(),
				$"Semantics.Quantities.{suffix}",
				"build",
				$"ktsu.Semantics.Quantities.{suffix}.props");

			if (!File.Exists(onDisk))
			{
				offenders.Add($"Semantics.Quantities.{suffix}: {expected} is packed but missing on disk");
			}
		}

		Assert.IsEmpty(
			offenders,
			"An alias package packs a props path that does not name the generated file under build/. " +
			"scripts/Generate-AliasProps.ps1 writes them there, and the verify-generated workflow " +
			"regenerates and diffs them. Offenders: " + string.Join("; ", offenders));
	}

	/// <summary>
	/// Yields the single packed props item from each alias project, failing if a project does not
	/// have exactly one — the whole package is that file, so none or several is itself the defect.
	/// </summary>
	private static IEnumerable<(string Suffix, XElement Packed)> PackedPropsItems()
	{
		foreach (string suffix in StorageSuffixes)
		{
			string project = Path.Combine(
				RepositoryRoot(),
				$"Semantics.Quantities.{suffix}",
				$"Semantics.Quantities.{suffix}.csproj");

			Assert.IsTrue(File.Exists(project), $"Could not find {project}.");

			List<XElement> packed =
			[
				.. XDocument.Load(project)
					.Descendants()
					.Where(element => element.Name.LocalName == "None")
					.Where(element => (element.Attribute("Pack")?.Value ?? "").Equals("true", StringComparison.OrdinalIgnoreCase))
					.Where(element => Normalize(element.Attribute("Include")?.Value ?? "").EndsWith(".props", StringComparison.Ordinal))
			];

			Assert.HasCount(
				1,
				packed,
				$"Semantics.Quantities.{suffix} packs {packed.Count} props files; the package is exactly one.");

			yield return (suffix, packed[0]);
		}
	}

	/// <summary>
	/// Rewrites an MSBuild path to forward slashes so a Windows-spelled attribute compares equal on
	/// every platform, keeping the trailing separator that distinguishes a folder from a file.
	/// </summary>
	/// <param name="path">The attribute value as written in the project file.</param>
	/// <returns>The same path spelled with forward slashes.</returns>
	private static string Normalize(string path) => path.Replace('\\', '/');

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
