// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using System.IO;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class RelativePathMakeTests
{
	// '%' is an ordinary file-name character on every platform, so none of these is an escape.
	[TestMethod]
	[DataRow("%2E%2E")]
	[DataRow("v1%2E0")]
	[DataRow("100%41")]
	[DataRow("%7Ebackup")]
	[DataRow("50%20off")]
	public void Make_KeepsPercentSequencesInNamesAsWritten(string folder)
	{
		string fromValue = TestPaths.Absolute("home", "u", "proj");
		string toValue = TestPaths.Absolute("home", "u", "proj", folder, "notes.txt");
		AbsoluteDirectoryPath from = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(fromValue);
		AbsoluteFilePath to = AbsoluteFilePath.Create<AbsoluteFilePath>(toValue);

		RelativePath relative = RelativePath.Make<RelativePath, AbsoluteDirectoryPath, AbsoluteFilePath>(from, to);

		Assert.AreEqual(Path.Join(folder, "notes.txt"), relative.WeakString);
		Assert.AreEqual(toValue, Path.GetFullPath(Path.Join(fromValue, relative.WeakString)));
	}

	[TestMethod]
	public void Make_FromAFile_IsRelativeToItsDirectory()
	{
		AbsoluteFilePath from = AbsoluteFilePath.Create<AbsoluteFilePath>(TestPaths.Absolute("base", "folder", "a.txt"));
		AbsoluteFilePath to = AbsoluteFilePath.Create<AbsoluteFilePath>(TestPaths.Absolute("base", "other", "b.txt"));

		RelativePath relative = RelativePath.Make<RelativePath, AbsoluteFilePath, AbsoluteFilePath>(from, to);

		Assert.AreEqual(Path.Join("..", "other", "b.txt"), relative.WeakString);
	}

	[TestMethod]
	[DataRow(new[] { "home", "u", "proj" }, new[] { "home", "u", "proj", "%2E%2E", "notes.txt" })]
	[DataRow(new[] { "home", "u", "proj" }, new[] { "home", "u", "proj", "src", "file.cs" })]
	[DataRow(new[] { "home", "u", "proj" }, new[] { "home", "u", "proj", "src" })]
	[DataRow(new[] { "home", "u", "proj" }, new[] { "home", "u", "proj" })]
	[DataRow(new[] { "home", "u", "proj", "a", "b" }, new[] { "home", "u", "other", "c" })]
	[DataRow(new[] { "home", "u", "proj" }, new[] { "home" })]
	[DataRow(new[] { "home", "u", "proj" }, new[] { "home", "u", "proj with space", "x" })]
	[DataRow(new string[0], new[] { "home", "u" })]
	public void SegmentFallback_MatchesPathGetRelativePath(string[] from, string[] to)
	{
		string fromValue = TestPaths.Absolute(from);
		string toValue = TestPaths.Absolute(to);

		Assert.AreEqual(
			Path.GetRelativePath(fromValue, toValue),
			PathPolyfill.GetRelativePathBySegments(fromValue, toValue));
	}
}
