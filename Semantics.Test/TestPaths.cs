// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test;

using System;
using System.IO;

/// <summary>
/// Builds path fixtures that mean the same thing on every platform.
/// </summary>
/// <remarks>
/// <para>
/// The path tests were written against Windows and hard-coded its spelling of everything: <c>C:\</c>
/// for the root, <c>\</c> for the separator. None of that is portable. On Linux <c>C:\projects</c>
/// is a <em>relative</em> path whose first segment happens to contain a colon and a backslash, so
/// <c>AbsoluteDirectoryPath.Create</c> rejects it — correctly — and the test fails for a reason that
/// has nothing to do with what it was written to check.
/// </para>
/// <para>
/// Composing fixtures here instead keeps each test asserting the thing it was about. The library
/// itself is already platform-correct: it takes its separators from <see cref="Path"/>, and treats
/// <c>\</c> as an ordinary filename character on platforms where that is what it is.
/// </para>
/// </remarks>
internal static class TestPaths
{
	/// <summary>
	/// Gets the platform's absolute-path root — <c>C:\</c> on Windows, <c>/</c> everywhere else.
	/// </summary>
	internal static string Root { get; } = OperatingSystem.IsWindows() ? "C:" + Separator : "/";

	/// <summary>Gets the platform's directory separator.</summary>
	internal static char Separator => Path.DirectorySeparatorChar;

	/// <summary>
	/// Gets the platform's alternate directory separator.
	/// </summary>
	/// <remarks>
	/// On Windows this is <c>/</c>, distinct from <see cref="Separator"/>. On Unix both are <c>/</c>:
	/// there is no second spelling, because the other candidate is a legal filename character. A
	/// "mixed separators" fixture built from both therefore stays a genuine mixed-separator case on
	/// Windows and degrades to an ordinary path on Unix, which is the honest translation — rather
	/// than one that smuggles a backslash in and tests something else entirely.
	/// </remarks>
	internal static char AlternateSeparator => Path.AltDirectorySeparatorChar;

	/// <summary>
	/// Joins segments into an absolute path rooted at <see cref="Root"/>.
	/// </summary>
	/// <param name="segments">The path segments, in order.</param>
	/// <returns>An absolute path this platform recognises as fully qualified.</returns>
	internal static string Absolute(params string[] segments) => Root + Relative(segments);

	/// <summary>
	/// Joins segments into a relative path using the platform separator.
	/// </summary>
	/// <param name="segments">The path segments, in order.</param>
	/// <returns>A relative path.</returns>
	internal static string Relative(params string[] segments) =>
		string.Join(Separator.ToString(), segments);

	/// <summary>
	/// Joins segments using <see cref="AlternateSeparator"/>, for fixtures that are specifically
	/// about a path written the other way round.
	/// </summary>
	/// <param name="segments">The path segments, in order.</param>
	/// <returns>A relative path spelled with the alternate separator.</returns>
	internal static string AltRelative(params string[] segments) =>
		string.Join(AlternateSeparator.ToString(), segments);

	/// <summary>
	/// Joins segments into an absolute path spelled with <see cref="AlternateSeparator"/>.
	/// </summary>
	/// <param name="segments">The path segments, in order.</param>
	/// <returns>An absolute path spelled with the alternate separator.</returns>
	internal static string AltAbsolute(params string[] segments) =>
		Root.Replace(Separator, AlternateSeparator) + AltRelative(segments);
}
