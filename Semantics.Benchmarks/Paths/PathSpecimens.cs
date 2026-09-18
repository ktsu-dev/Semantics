// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Paths;

using System;
using System.IO;

/// <summary>
/// The path strings the path benchmarks run against.
/// </summary>
/// <remarks>
/// <para>
/// <b>The absolute ones are built per platform, and they have to be.</b>
/// <c>IsAbsolutePathAttribute</c> validates through <c>Path.IsPathFullyQualified</c>, whose answer
/// depends on the operating system: <c>C:\x</c> is fully qualified on Windows and is an ordinary
/// relative path on Linux. A hardcoded Windows root would throw from every absolute benchmark on
/// the CI runner while passing on a developer's machine, which is the worst way for this to fail.
/// </para>
/// <para>
/// The two roots differ by two characters in length, so a measurement taken on Windows is not
/// exactly a measurement taken on Linux. That is smaller than the difference between CI hosts that
/// <c>BaselineBenchmarks</c> already exists to normalize, and it is why the history records a
/// baseline reading alongside every entry.
/// That normalization covers the time row only. Allocation is not divided by anything, so a
/// platform change moves it directly, and the README says so beside the paths chart.
/// </para>
/// <para>
/// Nothing here touches the filesystem, and none of these paths needs to exist. The benchmarks
/// deliberately avoid <c>IsDirectory</c> and <c>IsFile</c>, which call <c>Directory.Exists</c> and
/// <c>File.Exists</c> and would measure the disk rather than the library.
/// </para>
/// </remarks>
internal static class PathSpecimens
{
	private static readonly string Root =
		OperatingSystem.IsWindows() ? @"C:\projects" : "/projects";

	/// <summary>A fully qualified file path, four segments below the root.</summary>
	internal static readonly string AbsoluteFile =
		Path.Combine(Root, "semantics", "src", "Semantics.Paths", "FilePath.cs");

	/// <summary>The directory that file sits in, used as the base for both conversions.</summary>
	internal static readonly string AbsoluteDirectory =
		Path.Combine(Root, "semantics", "src");

	/// <summary>A relative file path. Forward slashes are accepted on both platforms.</summary>
	internal const string RelativeFile = "Semantics.Paths/FilePath.cs";

	/// <summary>A bare file name, with no separator in it.</summary>
	internal const string FileNameOnly = "FilePath.cs";
}
