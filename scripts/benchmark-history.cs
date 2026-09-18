// Copyright (c) 2023-2026 ktsu-dev contributors

// Accumulates benchmark results per release and draws them for the README.
//
//   dotnet run scripts/benchmark-history.cs -- ingest --history <f> --results <dir> --version <v>
//   dotnet run scripts/benchmark-history.cs -- render --history <f> --out <svg>
//
// A file-based app rather than a project: it is tooling, it is the same language as the library,
// and the SDK that builds the library already runs it with nothing else installed. The work still
// lives in a class rather than in top-level statements, so the analyzers judge each method.

using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

return BenchmarkHistory.Run(args);

/// <summary>Reads BenchmarkDotNet reports into a per-release history, and draws it.</summary>
internal static partial class BenchmarkHistory
{
	private const int SchemaVersion = 1;
	private const string BaselineKey = "BaselineBenchmarks.ReferenceWork";
	private const int CellWidth = 228;
	private const int CellHeight = 132;
	private const int Left = 56;

	/// <summary>One drawable chart: its title, its grid width, and the panels it shows.</summary>
	/// <param name="Title">The library the chart is about, used in the heading and the aria-label.</param>
	/// <param name="Columns">Panels per row.</param>
	/// <param name="Headline">The panels of the allocation and time sections, in reading order.</param>
	/// <param name="Cost">
	/// The paired benchmarks the cost section draws. Empty for a subject whose history carries no
	/// bare-baseline pair, and that subject's chart then has two sections rather than three.
	/// </param>
	private sealed record Subject(string Title, int Columns, PanelSpec[] Headline, CostPair[] Cost);

	/// <summary>One panel: which benchmark it draws, and what to call it.</summary>
	/// <param name="Key">The benchmark key as <c>ingest</c> stores it.</param>
	/// <param name="Parameters">The parameter case to draw, or null when the benchmark has none.</param>
	/// <param name="Label">The panel heading.</param>
	private sealed record PanelSpec(string Key, string? Parameters, string Label);

	/// <summary>One cost panel: a measured benchmark over the bare baseline it is paired with.</summary>
	/// <param name="Key">The benchmark measuring the wrapped type.</param>
	/// <param name="Baseline">The benchmark measuring the same work on the bare type.</param>
	/// <param name="Label">The panel heading.</param>
	private sealed record CostPair(string Key, string Baseline, string Label);

	/// <summary>The charts this script can draw, by the name <c>--subject</c> takes.</summary>
	/// <remarks>
	/// <para>
	/// Everything measured is stored; this only decides what each picture shows, so it can change
	/// without re-running anything.
	/// </para>
	/// <para>
	/// <b>Quantities</b> is drawn as a grid of one operation per storage type rather than of every
	/// operation at one storage type. A quantity is a value type over <c>T</c> and does almost
	/// nothing of its own, so what a release changes it changes per storage type — and the same
	/// line of user code costs four different things depending on the <c>T</c> it was written
	/// against. The top row is one construction across the four, so that row reads as the
	/// comparison it is.
	/// </para>
	/// <para>
	/// None of the eight is a bare operator, although the suite measures those too. A relationship
	/// operator over a binary floating point type is a single machine instruction on values the
	/// loop does not change, so the JIT hoists it out and BenchmarkDotNet reports it as
	/// indistinguishable from an empty method. That is a true answer about the library and a
	/// useless one to plot: a panel of it would chart the harness's resolution rather than any
	/// release. What is drawn instead is the work around an operator — building a quantity from a
	/// unit, reading it back out in one, a vector length, a comparison — all of which are far
	/// enough above that floor to move when the library does.
	/// </para>
	/// <para>
	/// <b>Strings</b> is drawn along validation weight, because that is the axis there is. A
	/// semantic string is a record wrapping a <see cref="string"/> and its cost is concentrated at
	/// creation, so the top row walks from the reflection machinery alone up through a character
	/// set check, a format check and a mod-97 check. The bottom row is what a caller pays around
	/// that: both failure paths, the cross-type conversion that is secretly another creation, and
	/// the hash a dictionary of semantic strings pays on every lookup.
	/// </para>
	/// <para>
	/// Ordering is measured and stored, and deliberately not drawn. A semantic string's
	/// <c>CompareTo</c> is the underlying string's own, over operands a loop does not change, so
	/// the JIT hoists it and BenchmarkDotNet reports it as indistinguishable from an empty method —
	/// the same reason no bare quantity operator is drawn above. A panel of it would chart the
	/// harness's resolution rather than any release.
	/// </para>
	/// <para>
	/// <b>Paths</b> is the same shape: build each kind, then operate on one. The two file name
	/// panels sit next to each other because one caches into a field and one rebuilds and
	/// revalidates on every read, and both look like field access at a call site.
	/// </para>
	/// </remarks>
	private static readonly Dictionary<string, Subject> Subjects = new(StringComparer.Ordinal)
	{
		["quantities"] = new("Semantics.Quantities", 4,
		[
			new PanelSpec("ConstructionBenchmarks<Double>.FromNauticalMile", null, "Construct (double)"),
			new PanelSpec("ConstructionBenchmarks<Single>.FromNauticalMile", null, "Construct (float)"),
			new PanelSpec("ConstructionBenchmarks<Decimal>.FromNauticalMile", null, "Construct (decimal)"),
			new PanelSpec("ConstructionBenchmarks<PreciseNumber>.FromNauticalMile", null, "Construct (precise)"),
			new PanelSpec("UnitConversionBenchmarks<Decimal>.InNauticalMile", null, "Read back (decimal)"),
			new PanelSpec("UnitConversionBenchmarks<PreciseNumber>.InNauticalMile", null, "Read back (precise)"),
			new PanelSpec("VectorBenchmarks<Decimal>.Length", null, "Vector length (decimal)"),
			new PanelSpec("ComparisonBenchmarks<Double>.CompareToInterface", null, "CompareTo (double)"),
		],
		[
			// Both halves of each pair are stored like any other benchmark; the ratio is computed at
			// render time rather than recorded, so an entry gathered before this section existed draws
			// as soon as its run includes the pair, and no history has to be rewritten to change what
			// the section shows.
			new CostPair("AbstractionCostBenchmarks<Double>.QuantityAdd", "AbstractionCostBenchmarks<Double>.BareAdd", "Add (double)"),
			new CostPair("AbstractionCostBenchmarks<Double>.QuantityMultiply", "AbstractionCostBenchmarks<Double>.BareMultiply", "Multiply (double)"),
			new CostPair("AbstractionCostBenchmarks<Decimal>.QuantityMultiply", "AbstractionCostBenchmarks<Decimal>.BareMultiply", "Multiply (decimal)"),
			new CostPair("AbstractionCostBenchmarks<PreciseNumber>.QuantityMultiply", "AbstractionCostBenchmarks<PreciseNumber>.BareMultiply", "Multiply (precise)"),
		]),
		["strings"] = new("Semantics.Strings", 4,
		[
			new("StringCreationBenchmarks.Unvalidated", null, "Create (no validation)"),
			new("StringCreationBenchmarks.CharsetRegex", null, "Create (charset regex)"),
			new("StringCreationBenchmarks.FormatRegex", null, "Create (format regex)"),
			new("StringCreationBenchmarks.Mod97", null, "Create (mod-97)"),
			new("StringCreationBenchmarks.TryCreateRejects", null, "TryCreate (rejects)"),
			new("StringCreationBenchmarks.CreateThrows", null, "Create (throws)"),
			new("StringOperationBenchmarks.AsConversion", null, "As<T> conversion"),
			new("StringOperationBenchmarks.HashCode", null, "GetHashCode"),
		],
			[]),
		["paths"] = new("Semantics.Paths", 4,
		[
			new("PathCreationBenchmarks.AbsoluteFilePath", null, "Create (absolute file)"),
			new("PathCreationBenchmarks.RelativeFilePath", null, "Create (relative file)"),
			new("PathCreationBenchmarks.FileNameType", null, "Create (file name)"),
			new("PathOperationBenchmarks.FileName", null, "FileName (uncached)"),
			new("PathOperationBenchmarks.FileNameWithoutExtension", null, "FileName (cached)"),
			new("PathOperationBenchmarks.AsAbsolute", null, "AsAbsolute (from relative)"),
			new("PathOperationBenchmarks.AsRelative", null, "AsRelative (from absolute)"),
			new("PathOperationBenchmarks.RemoveExtension", null, "RemoveExtension"),
		],
			[]),
	};

	/// <summary>
	/// Validated for colour-vision separation against both surfaces: every check passes, worst
	/// adjacent pair dE 24.7 light and 26.8 dark.
	/// </summary>
	private static readonly Dictionary<string, Theme> Themes = new(StringComparer.Ordinal)
	{
		["light"] = new("#fcfcfb", "#0b0b0b", "#52514e", "#e4e3df", "#2a78d6", "#eb6834", "#2e8b57"),
		["dark"] = new("#1a1a19", "#ffffff", "#c3c2b7", "#333330", "#3987e5", "#d95926", "#3faa71"),
	};

	private sealed record Theme(
		string Surface, string Ink, string Muted, string Grid, string Alloc, string Time, string Cost);

	/// <summary>Which of the three things a section draws.</summary>
	private enum Measure
	{
		/// <summary>Bytes allocated per operation, as stored.</summary>
		Allocation,

		/// <summary>Time, divided by the reference workload from the same job.</summary>
		Time,

		/// <summary>Time, divided by the paired bare-double benchmark from the same entry.</summary>
		Cost,
	}

	internal static int Run(string[] args)
	{
		if (args.Length == 0)
		{
			Console.Error.WriteLine("Expected 'ingest', 'render', or 'baseline'.");
			return 2;
		}

		Dictionary<string, string> options = ReadOptions(args.Skip(1));
		try
		{
			return args[0] switch
			{
				"ingest" => Ingest(options),
				"render" => Render(options),
				"baseline" => PrintBaseline(options),
				_ => Unknown(args[0]),
			};
		}
		catch (InvalidOperationException problem)
		{
			Console.Error.WriteLine(problem.Message);
			return 2;
		}
	}

	/// <summary>Prints the reference workload's mean, for a workflow to carry between steps.</summary>
	private static int PrintBaseline(Dictionary<string, string> options)
	{
		string directory = Required(options, "results");
		string[] reports = Directory.GetFiles(directory, "*-report-full.json", SearchOption.AllDirectories);
		if (reports.Length == 0)
		{
			Console.Error.WriteLine($"No *-report-full.json under {directory}");
			return 1;
		}

		(var measured, _, _) = ReadReports(reports);
		double? baseline = Baseline(measured, "");
		if (baseline is null)
		{
			Console.Error.WriteLine($"No {BaselineKey} measurement under {directory}");
			return 1;
		}

		Console.WriteLine(baseline.Value.ToString(CultureInfo.InvariantCulture));
		return 0;
	}

	private static int Unknown(string command)
	{
		Console.Error.WriteLine($"Unknown command '{command}'.");
		return 2;
	}

	private static Dictionary<string, string> ReadOptions(IEnumerable<string> rest)
	{
		Dictionary<string, string> found = new(StringComparer.Ordinal);
		string? name = null;
		foreach (string argument in rest)
		{
			if (argument.StartsWith("--", StringComparison.Ordinal))
			{
				name = argument[2..];
				found[name] = "";
			}
			else if (name is not null)
			{
				found[name] = argument;
				name = null;
			}
		}

		return found;
	}

	private static string Required(Dictionary<string, string> options, string name) =>
		options.TryGetValue(name, out string? value) && value.Length > 0
			? value
			: throw new InvalidOperationException($"--{name} is required");

	private static string Optional(Dictionary<string, string> options, string name, string fallback = "") =>
		options.TryGetValue(name, out string? value) && value.Length > 0 ? value : fallback;

	private static int Ingest(Dictionary<string, string> options)
	{
		string resultsDirectory = Required(options, "results");
		string[] reports = Directory.GetFiles(resultsDirectory, "*-report-full.json", SearchOption.AllDirectories);
		Array.Sort(reports, StringComparer.Ordinal);
		if (reports.Length == 0)
		{
			Console.Error.WriteLine($"No *-report-full.json under {resultsDirectory}");
			return 1;
		}

		(var measured, string cpu, string runtime) = ReadReports(reports);
		double? baseline = Baseline(measured, Optional(options, "baseline-ns"));
		if (baseline is null)
		{
			Console.Error.WriteLine(
				$"warning: no {BaselineKey} measurement and no --baseline-ns; " +
				"this entry's times will not be comparable across runners");
		}

		string version = Required(options, "version");
		JsonObject benchmarks = Benchmarks(measured);
		if (benchmarks.Count == 0)
		{
			// Reports with every row reading NA: the harness built and ran, and each benchmark
			// threw. An older package whose Parse is a NotSupportedException does exactly this.
			// Recording it would put a release on the axis with nothing under it, which reads as
			// a release that was measured and found to cost nothing.
			Console.Error.WriteLine(
				$"No benchmark in {resultsDirectory} produced a measurement; {version} not recorded");
			return 1;
		}

		JsonObject record = new()
		{
			["version"] = version,
			["commit"] = Optional(options, "commit"),
			["date"] = Optional(options, "date", DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
			["cpu"] = cpu,
			["runtime"] = runtime,
			["baselineNs"] = baseline,
			["runId"] = Optional(options, "run-id"),
			["benchmarks"] = benchmarks,
		};

		string historyPath = Required(options, "history");
		JsonObject history = LoadHistory(historyPath);
		JsonArray entries = history["entries"]!.AsArray();

		// A version is measured once. Re-running a release replaces its entry rather than doubling it.
		for (int index = entries.Count - 1; index >= 0; index--)
		{
			if (string.Equals(entries[index]?["version"]?.GetValue<string>(), version, StringComparison.Ordinal))
			{
				entries.RemoveAt(index);
			}
		}

		entries.Add((JsonNode?)record);
		Reorder(entries);

		Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(historyPath))!);
		File.WriteAllText(historyPath, history.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");

		Console.WriteLine(
			$"ingested {version}: {record["benchmarks"]!.AsObject().Count} benchmarks, " +
			$"baseline {baseline?.ToString(CultureInfo.InvariantCulture) ?? "none"} ns, " +
			$"cpu {(cpu.Length > 0 ? cpu : "unknown")}");
		return 0;
	}

	private static (SortedDictionary<string, List<ParameterCase>> Measured, string Cpu, string Runtime)
		ReadReports(string[] reports)
	{
		SortedDictionary<string, List<ParameterCase>> measured = new(StringComparer.Ordinal);
		string cpu = "";
		string runtime = "";

		foreach (JsonNode document in reports.Select(report => JsonNode.Parse(File.ReadAllText(report))!))
		{
			if (cpu.Length == 0 && document["HostEnvironmentInfo"] is JsonNode environment)
			{
				cpu = (environment["ProcessorName"]?.GetValue<string>() ?? "").Trim();
				runtime = (environment["RuntimeVersion"]?.GetValue<string>() ?? "").Trim();
			}

			foreach (JsonNode? entry in document["Benchmarks"]?.AsArray() ?? [])
			{
				if (entry?["Statistics"]?["Mean"] is not JsonNode mean)
				{
					continue;
				}

				string key = BenchmarkKey(entry["FullName"]?.GetValue<string>() ?? "");
				string parameters = (entry["Parameters"]?.GetValue<string>() ?? "").Trim();
				if (!measured.TryGetValue(key, out List<ParameterCase>? cases))
				{
					cases = [];
					measured[key] = cases;
				}

				Measurement measurement = new(
					Math.Round(mean.GetValue<double>(), 4),
					entry["Memory"]?["BytesAllocatedPerOperation"]?.GetValue<long>() ?? 0);
				int existing = cases.FindIndex(one => string.Equals(one.Parameters, parameters, StringComparison.Ordinal));
				if (existing >= 0)
				{
					cases[existing] = new(parameters, measurement);
				}
				else
				{
					cases.Add(new(parameters, measurement));
				}
			}
		}

		return (measured, cpu, runtime);
	}

	private sealed record Measurement(double MeanNs, long AllocatedBytes);

	private sealed record ParameterCase(string Parameters, Measurement Value);

	private static double? Baseline(
		SortedDictionary<string, List<ParameterCase>> measured, string given)
	{
		if (measured.TryGetValue(BaselineKey, out List<ParameterCase>? cases) && cases.Count > 0)
		{
			return cases[0].Value.MeanNs;
		}

		return double.TryParse(given, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed)
			? parsed
			: null;
	}

	private static JsonObject Benchmarks(
		SortedDictionary<string, List<ParameterCase>> measured)
	{
		JsonObject benchmarks = [];
		foreach ((string key, List<ParameterCase> cases) in measured)
		{
			if (string.Equals(key, BaselineKey, StringComparison.Ordinal))
			{
				continue;
			}

			JsonObject byParameters = [];
			foreach (ParameterCase one in cases)
			{
				byParameters[one.Parameters] = new JsonObject
				{
					["meanNs"] = one.Value.MeanNs,
					["allocatedBytes"] = one.Value.AllocatedBytes,
				};
			}

			benchmarks[key] = byParameters;
		}

		return benchmarks;
	}

	private static void Reorder(JsonArray entries)
	{
		JsonNode[] ordered =
		[
			.. entries
				.Select(node => node!.DeepClone())
				.OrderBy(node => node["version"]?.GetValue<string>() ?? "", VersionOrder.Instance),
		];

		entries.Clear();
		foreach (JsonNode node in ordered)
		{
			entries.Add((JsonNode?)node);
		}
	}

	private static string BenchmarkKey(string fullName)
	{
		string bare = fullName.Split('(')[0];
		string[] parts = bare.Split('.');
		return parts.Length >= 2 ? $"{parts[^2]}.{parts[^1]}" : bare;
	}

	private static JsonObject LoadHistory(string path)
	{
		if (!File.Exists(path))
		{
			return new JsonObject { ["schemaVersion"] = SchemaVersion, ["entries"] = new JsonArray() };
		}

		JsonObject history = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
		history["schemaVersion"] ??= SchemaVersion;
		history["entries"] ??= new JsonArray();
		return history;
	}

	/// <summary>Orders versions numerically, keeping anything unparseable first in name order.</summary>
	private sealed class VersionOrder : IComparer<string>
	{
		internal static readonly VersionOrder Instance = new();

		public int Compare(string? left, string? right)
		{
			int[] first = Numbers(left);
			int[] second = Numbers(right);
			for (int index = 0; index < Math.Min(first.Length, second.Length); index++)
			{
				if (first[index] != second[index])
				{
					return first[index].CompareTo(second[index]);
				}
			}

			return first.Length != second.Length
				? first.Length.CompareTo(second.Length)
				: string.CompareOrdinal(left, right);
		}

		private static int[] Numbers(string? text) =>
			[.. DigitRun().Matches(text ?? "").Select(match => int.Parse(match.Value, CultureInfo.InvariantCulture))];
	}

	[GeneratedRegex("[0-9]+")]
	private static partial Regex DigitRun();

	private static int Render(Dictionary<string, string> options)
	{
		string historyPath = Required(options, "history");
		string subjectName = Required(options, "subject");
		if (!Subjects.TryGetValue(subjectName, out Subject? subject))
		{
			throw new InvalidOperationException(
				$"Unknown subject '{subjectName}'. Known: {string.Join(", ", Subjects.Keys)}");
		}

		JsonArray entries = LoadHistory(historyPath)["entries"]!.AsArray();
		if (entries.Count == 0)
		{
			Console.Error.WriteLine($"{historyPath} has no entries to draw");
			return 1;
		}

		string output = Required(options, "out");
		string extension = Path.GetExtension(output);
		string stem = output[..^extension.Length];
		Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);

		List<string> written = [];
		foreach (string name in (string[])["light", "dark"])
		{
			string path = string.Equals(name, "light", StringComparison.Ordinal)
				? output
				: $"{stem}-dark{extension}";
			File.WriteAllText(path, Draw(entries, Themes[name], subject));
			written.Add(path);
		}

		Console.WriteLine($"rendered {entries.Count} releases to {string.Join(", ", written)}");
		return 0;
	}

	private static string Draw(JsonArray entries, Theme theme, Subject subject)
	{
		string[] labels = [.. entries.Select(entry => entry!["version"]?.GetValue<string>() ?? "?")];
		int width = Left + (subject.Columns * CellWidth) + 24;
		int rows = (subject.Headline.Length + subject.Columns - 1) / subject.Columns;
		int costRows = (subject.Cost.Length + subject.Columns - 1) / subject.Columns;

		// A subject with no cost pairs draws two sections rather than three, and reserves no height
		// for the third. Only the quantities suite measures a bare-storage-type baseline beside each
		// operation; the strings and paths cost classes exist but are not run by the release workflow,
		// so their histories carry no pair to divide.
		Measure[] measures = subject.Cost.Length > 0
			? [Measure.Allocation, Measure.Time, Measure.Cost]
			: [Measure.Allocation, Measure.Time];
		int height = 72 + ((34 + (rows * CellHeight)) * 2)
			+ (costRows > 0 ? 34 + (costRows * CellHeight) : 0) + 54;

		StringBuilder svg = new();
		Preamble(svg, theme, width, height, entries, subject);

		int y = 72;
		foreach (Measure measure in measures)
		{
			Section(svg, theme, entries, labels.Length, y, measure, subject);
			y += 34 + ((measure == Measure.Cost ? costRows : rows) * CellHeight);
		}

		Footer(svg, entries, labels, y - 4);
		svg.AppendLine("</svg>");
		return svg.ToString();
	}

	private static void Preamble(
		StringBuilder svg, Theme theme, int width, int height, JsonArray entries, Subject subject)
	{
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<svg xmlns="http://www.w3.org/2000/svg" width="{width}" height="{height}" viewBox="0 0 {width} {height}" role="img" aria-label="{Escape(subject.Title)} allocation and relative time per release">""");
		svg.AppendLine("<style>");
		svg.AppendLine(CultureInfo.InvariantCulture, $"  text {{ font-family: ui-sans-serif, -apple-system, 'Segoe UI', Roboto, sans-serif; fill: {theme.Ink}; }}");
		svg.AppendLine("  .title { font-size: 15px; font-weight: 600; }");
		svg.AppendLine("  .section { font-size: 12.5px; font-weight: 600; }");
		svg.AppendLine(CultureInfo.InvariantCulture, $"  .panel-title {{ font-size: 11px; font-weight: 600; fill: {theme.Ink}; }}");
		svg.AppendLine(CultureInfo.InvariantCulture, $"  .muted, .muted-value, .caption {{ font-size: 9.5px; fill: {theme.Muted}; }}");
		svg.AppendLine(CultureInfo.InvariantCulture, $"  .value {{ font-size: 10px; font-weight: 600; fill: {theme.Ink}; }}");
		svg.AppendLine(CultureInfo.InvariantCulture, $"  .tick {{ font-size: 9px; fill: {theme.Muted}; }}");
		svg.AppendLine(CultureInfo.InvariantCulture, $"  .axis {{ stroke: {theme.Grid}; stroke-width: 1; }}");
		svg.AppendLine("</style>");
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<rect width="{width}" height="{height}" fill="{theme.Surface}" />""");
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{Left}" y="28" class="title">{Escape(subject.Title)} performance by release</text>""");

		JsonNode latest = entries[^1]!;
		string date = latest["date"]?.GetValue<string>() ?? "";
		string suffix = date.Length > 0 ? " · " + Escape(date) : "";
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{Left}" y="45" class="caption">{entries.Count} releases · newest {Escape(latest["version"]?.GetValue<string>() ?? "?")}{suffix}</text>""");
	}

	private static void Section(
		StringBuilder svg, Theme theme, JsonArray entries, int points, int y, Measure measure, Subject subject)
	{
		string colour = measure switch
		{
			Measure.Time => theme.Time,
			Measure.Cost => theme.Cost,
			_ => theme.Alloc,
		};
		string title = measure switch
		{
			Measure.Time => "Time, as a multiple of a fixed reference workload",
			Measure.Cost => "Cost of the quantity wrapper over the bare storage type",
			_ => "Allocated bytes per operation",
		};
		string note = measure switch
		{
			Measure.Time => "Divided by a reference loop measured in the same job, which cancels most of the difference between CI runners. Lower is faster.",
			Measure.Cost => "Divided by the identical loop on the bare storage type, measured beside it. A quantity holds one value and adds no work of its own, so this belongs at 1.",
			_ => "Deterministic: the same code allocates the same bytes on any machine.",
		};

		svg.AppendLine(CultureInfo.InvariantCulture, $"""<rect x="{Left}" y="{y - 10}" width="9" height="9" rx="2" fill="{colour}" />""");
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{Left + 15}" y="{y - 2}" class="section">{Escape(title)}</text>""");
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{Left + 15}" y="{y + 12}" class="caption">{Escape(note)}</text>""");

		if (measure == Measure.Cost)
		{
			for (int position = 0; position < subject.Cost.Length; position++)
			{
				(string key, string baseline, string label) = subject.Cost[position];
				double?[] values = [.. entries.Select(entry => Cost(entry!, key, baseline))];
				Panel(
					svg,
					Left + (position % subject.Columns * CellWidth),
					y + 26 + (position / subject.Columns * CellHeight),
					label,
					points,
					values,
					true,
					colour,
					theme);
			}

			return;
		}

		bool isTime = measure == Measure.Time;
		for (int position = 0; position < subject.Headline.Length; position++)
		{
			(string key, string? parameters, string label) = subject.Headline[position];
			double?[] values = [.. entries.Select(entry => Value(entry!, key, parameters, isTime))];
			Panel(
				svg,
				Left + (position % subject.Columns * CellWidth),
				y + 26 + (position / subject.Columns * CellHeight),
				label + (parameters is null ? "" : $" ({parameters} digits)"),
				points,
				values,
				isTime,
				colour,
				theme);
		}
	}

	/// <summary>
	/// One benchmark's mean divided by the mean of the bare-double benchmark beside it.
	/// </summary>
	/// <remarks>
	/// Both were measured in the same job on the same machine, so unlike the time section this
	/// needs no reference workload to be comparable across runs: the denominator is the reference.
	/// </remarks>
	private static double? Cost(JsonNode entry, string key, string baseline)
	{
		double? measured = Mean(entry, key);
		double? divisor = Mean(entry, baseline);
		return measured is not null && divisor is > 0 ? measured / divisor : null;
	}

	private static double? Mean(JsonNode entry, string key) =>
		entry["benchmarks"]?[key] is JsonObject cases && cases.Count > 0
			? cases.First().Value?["meanNs"]?.GetValue<double>()
			: null;

	private static double? Value(JsonNode entry, string key, string? parameters, bool isTime)
	{
		if (entry["benchmarks"]?[key] is not JsonObject cases || cases.Count == 0)
		{
			return null;
		}

		JsonNode? measurement = parameters is null
			? cases.First().Value
			: cases.FirstOrDefault(pair => pair.Key.Contains(parameters, StringComparison.Ordinal)).Value;
		if (measurement is null)
		{
			return null;
		}

		if (!isTime)
		{
			return measurement["allocatedBytes"]!.GetValue<double>();
		}

		double? baseline = entry["baselineNs"]?.GetValue<double?>();
		return baseline is > 0 ? measurement["meanNs"]!.GetValue<double>() / baseline : null;
	}

	private static void Footer(StringBuilder svg, JsonArray entries, string[] labels, int axisY)
	{
		List<string> ticks = [];
		for (int index = 0; index < labels.Length; index++)
		{
			// Every label while they fit. Thinning them reads as the whole list, which would say
			// there were fewer releases than there were.
			if (labels.Length > 12 && index > 0 && index < labels.Length - 1 && index % 2 == 1)
			{
				continue;
			}

			ticks.Add(Escape(labels[index]));
		}

		svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{Left}" y="{axisY}" class="tick">releases, oldest to newest: {string.Join(" → ", ticks)}</text>""");

		string[] cpus =
		[
			.. entries
				.Select(entry => entry!["cpu"]?.GetValue<string>() ?? "")
				.Where(name => name.Length > 0)
				.Distinct(StringComparer.Ordinal)
				.OrderBy(name => name, StringComparer.Ordinal),
		];
		string measured = cpus.Length > 0 ? string.Join(", ", cpus) : "an unrecorded CPU";
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{Left}" y="{axisY + 16}" class="caption">Measured on {Escape(measured)}. Full tables: Semantics.Benchmarks.</text>""");
	}

	/// <summary>One small multiple: a single series, so colour carries no identity of its own.</summary>
	private static void Panel(
		StringBuilder svg, int x0, int y0, string title, int points, double?[] values,
		bool isTime, string colour, Theme theme)
	{
		double plotTop = y0 + 22;
		double plotBottom = y0 + CellHeight - 12 - 20;
		double plotLeft = x0 + 6;
		double plotRight = x0 + CellWidth - 16 - 10;

		svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{x0}" y="{y0 + 10}" class="panel-title">{Escape(title)}</text>""");

		(int Index, double Value)[] present =
		[
			.. values
				.Select((value, index) => (Index: index, Value: value))
				.Where(point => point.Value.HasValue)
				.Select(point => (point.Index, point.Value!.Value)),
		];

		if (present.Length == 0)
		{
			svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{x0}" y="{F((plotTop + plotBottom) / 2)}" class="muted">not measured</text>""");
			return;
		}

		// Zero-based: these are magnitudes, and a clipped axis would exaggerate every wobble.
		double highest = present.Max(point => point.Value);
		double top = highest > 0 ? highest * 1.25 : 1.0;

		double X(int index) => points == 1
			? (plotLeft + plotRight) / 2
			: plotLeft + ((plotRight - plotLeft) * index / (points - 1));
		double Y(double value) => plotBottom - ((plotBottom - plotTop) * (value / top));

		svg.AppendLine(CultureInfo.InvariantCulture, $"""<line x1="{plotLeft}" y1="{F(plotBottom)}" x2="{plotRight}" y2="{F(plotBottom)}" class="axis" />""");

		if (present.Length > 1)
		{
			string line = string.Join(" ", present.Select(point => $"{F(X(point.Index))},{F(Y(point.Value))}"));
			svg.AppendLine(CultureInfo.InvariantCulture, $"""<polyline points="{line}" fill="none" stroke="{colour}" stroke-width="2" stroke-linejoin="round" stroke-linecap="round" />""");
		}

		foreach ((int index, double value) in present)
		{
			// A 2px surface ring keeps markers legible where the line passes behind them.
			svg.AppendLine(CultureInfo.InvariantCulture, $"""<circle cx="{F(X(index))}" cy="{F(Y(value))}" r="4" fill="{colour}" stroke="{theme.Surface}" stroke-width="2" />""");
		}

		(int lastIndex, double lastValue) = present[^1];
		string anchor = lastIndex == points - 1 ? "end" : "middle";
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{F(X(lastIndex))}" y="{F(Y(lastValue) - 9)}" class="value" text-anchor="{anchor}">{Escape(Label(lastValue, isTime))}</text>""");

		(int firstIndex, double firstValue) = present[0];
		if (firstIndex != lastIndex)
		{
			svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{F(X(firstIndex))}" y="{F(Y(firstValue) - 9)}" class="muted-value" text-anchor="middle">{Escape(Label(firstValue, isTime))}</text>""");
		}
	}

	/// <summary>One decimal is ample for an SVG coordinate, and keeps the committed diff small.</summary>
	private static string F(double value) => value.ToString("0.0", CultureInfo.InvariantCulture);

	private static string Label(double value, bool isTime) =>
		isTime ? RatioLabel(value) : ByteLabel(value);

	private static string ByteLabel(double value) =>
		value <= 0 ? "0 B"
		: value >= 1024 ? (value / 1024).ToString("0.0", CultureInfo.InvariantCulture) + " KB"
		: value.ToString("0", CultureInfo.InvariantCulture) + " B";

	/// <summary>Three significant figures, so a 0.0331x and a 15.3x are both legible.</summary>
	private static string RatioLabel(double value) =>
		value <= 0 ? "0×"
		: value >= 100 ? value.ToString("0", CultureInfo.InvariantCulture) + "×"
		: value >= 10 ? value.ToString("0.0", CultureInfo.InvariantCulture) + "×"
		: value >= 1 ? value.ToString("0.00", CultureInfo.InvariantCulture) + "×"
		: value >= 0.1 ? value.ToString("0.000", CultureInfo.InvariantCulture) + "×"
		: value.ToString("0.0000", CultureInfo.InvariantCulture) + "×";

	private static string Escape(string text) =>
		text
			.Replace("&", "&amp;", StringComparison.Ordinal)
			.Replace("<", "&lt;", StringComparison.Ordinal)
			.Replace(">", "&gt;", StringComparison.Ordinal)
			.Replace("\"", "&quot;", StringComparison.Ordinal);
}
