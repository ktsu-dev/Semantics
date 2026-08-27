// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Runs an incremental generator over a set of metadata files and hands back what it produced.
/// </summary>
/// <remarks>
/// Standing a generator up under <see cref="CSharpGeneratorDriver"/> is fiddly — reference
/// resolution, an <see cref="AdditionalText"/> shim, and the packaging escape hatch this repository
/// needed so the test project can reference the generator as a plain library
/// (<c>BundleAnalyzerDependencies=false</c>) — and none of it is specific to these generators.
/// It is written as a standalone harness so it can move to the shared generator toolkit alongside
/// <c>GeneratorBase</c>, rather than being rediscovered by the next project that writes one.
/// </remarks>
/// <param name="metadataDirectory">The directory the real metadata files were copied to.</param>
internal sealed class GeneratorHarness(string metadataDirectory)
{
	/// <summary>
	/// Runs a generator against the real metadata.
	/// </summary>
	/// <param name="generator">The generator to run.</param>
	/// <param name="overrides">
	/// Metadata to substitute or add, keyed by file name. An entry replaces the real file's contents;
	/// a name that is not a real file is added.
	/// </param>
	/// <returns>The generator's run result.</returns>
	/// <remarks>
	/// Every metadata file in the directory is supplied, the way MSBuild's
	/// <c>AdditionalFiles Include="Metadata/*.json"</c> item group supplies them. Handing a
	/// generator only the one file the test is interested in is not how it runs for real, and a
	/// generator that reads two files would report one of them missing.
	/// </remarks>
	internal GeneratorRunResult Run(
		IIncrementalGenerator generator,
		IReadOnlyDictionary<string, string>? overrides = null) =>
		RunAll([generator], overrides).Results[0];

	/// <summary>
	/// Runs a generator against a metadata set that contains only the named files.
	/// </summary>
	/// <param name="generator">The generator to run.</param>
	/// <param name="fileNames">The metadata file names to supply.</param>
	/// <returns>The generator's run result.</returns>
	internal GeneratorRunResult RunWithOnly(IIncrementalGenerator generator, params string[] fileNames)
	{
		List<AdditionalText> texts = [];
		foreach (string fileName in fileNames)
		{
			texts.Add(new InMemoryAdditionalText(
				Path.Combine(metadataDirectory, fileName),
				File.ReadAllText(Path.Combine(metadataDirectory, fileName))));
		}

		return Drive([generator], texts).Results[0];
	}

	/// <summary>
	/// Runs generators against the real metadata and returns the whole driver result, so a caller
	/// can inspect tracked steps as well as output.
	/// </summary>
	/// <param name="generators">The generators to run.</param>
	/// <param name="overrides">Metadata to substitute or add, keyed by file name.</param>
	/// <returns>The driver's run result.</returns>
	internal GeneratorDriverRunResult RunAll(
		IReadOnlyList<IIncrementalGenerator> generators,
		IReadOnlyDictionary<string, string>? overrides = null) =>
		Drive(generators, BuildTexts(overrides));

	/// <summary>
	/// Runs a generator twice over identical metadata and reports whether the second run reused the
	/// first run's cached outputs.
	/// </summary>
	/// <param name="generator">The generator to run.</param>
	/// <returns>True when no tracked output step had to be recomputed on the second run.</returns>
	/// <remarks>
	/// These are <see cref="IIncrementalGenerator"/>s, and nothing checked that they behave like
	/// one: a generator that recomputes everything on every keystroke still passes every output
	/// assertion, it just makes the IDE slow.
	/// </remarks>
	internal bool ReusesCachedOutputOnRerun(IIncrementalGenerator generator)
	{
		List<AdditionalText> texts = BuildTexts(null);
		CSharpCompilation compilation = CreateCompilation();

		GeneratorDriver driver = CSharpGeneratorDriver.Create(
			generators: [generator.AsSourceGenerator()],
			additionalTexts: texts,
			parseOptions: null,
			optionsProvider: null,
			driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));

		driver = driver.RunGenerators(compilation);
		GeneratorDriverRunResult second = driver.RunGenerators(compilation).GetRunResult();

		foreach (IncrementalGeneratorRunStep step in second.Results[0].TrackedOutputSteps.SelectMany(pair => pair.Value))
		{
			foreach ((object Value, IncrementalStepRunReason Reason) output in step.Outputs)
			{
				if (output.Reason is not (IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged))
				{
					return false;
				}
			}
		}

		return true;
	}

	private List<AdditionalText> BuildTexts(IReadOnlyDictionary<string, string>? overrides)
	{
		Dictionary<string, string> byName = [];
		foreach (string path in Directory.GetFiles(metadataDirectory, "*.json"))
		{
			byName[Path.GetFileName(path)] = File.ReadAllText(path);
		}

		if (overrides is not null)
		{
			foreach (KeyValuePair<string, string> entry in overrides)
			{
				byName[entry.Key] = entry.Value;
			}
		}

		List<AdditionalText> texts = [];
		foreach (KeyValuePair<string, string> entry in byName)
		{
			texts.Add(new InMemoryAdditionalText(Path.Combine(metadataDirectory, entry.Key), entry.Value));
		}

		return texts;
	}

	private static GeneratorDriverRunResult Drive(IReadOnlyList<IIncrementalGenerator> generators, List<AdditionalText> texts)
	{
		GeneratorDriver driver = CSharpGeneratorDriver.Create(
			generators: [.. generators.Select(generator => generator.AsSourceGenerator())],
			additionalTexts: texts);

		return driver.RunGenerators(CreateCompilation()).GetRunResult();
	}

	private static CSharpCompilation CreateCompilation() =>
		CSharpCompilation.Create(
			assemblyName: "GeneratorHost",
			syntaxTrees: [],
			references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
			options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

	/// <summary>
	/// Supplies metadata JSON to the generators the same way MSBuild's AdditionalFiles would.
	/// </summary>
	private sealed class InMemoryAdditionalText(string path, string text) : AdditionalText
	{
		public override string Path { get; } = path;

		public override SourceText GetText(CancellationToken cancellationToken = default) =>
			SourceText.From(text);
	}
}
