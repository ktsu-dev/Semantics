// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using System;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Issue #554: <c>.</c> and <c>..</c> name a directory, never a file, so they are not valid file names and a
/// file path cannot end in one. Directory names and paths are unaffected.
/// </summary>
[TestClass]
public class DotFileNameTests
{
	[TestMethod]
	[DataRow(".")]
	[DataRow("..")]
	public void FileName_Create_RejectsADotSegment(string name)
	{
		Assert.ThrowsExactly<ArgumentException>(() => FileName.Create<FileName>(name));
		Assert.IsFalse(FileName.TryCreate(name, out FileName? result));
		Assert.IsNull(result);
	}

	[TestMethod]
	[DataRow("...")]
	[DataRow(".bashrc")]
	[DataRow("..hidden")]
	[DataRow("file.")]
	public void FileName_Create_AcceptsNamesThatOnlyContainDots(string name) =>
		Assert.AreEqual(name, FileName.Create<FileName>(name).WeakString);

	[TestMethod]
	public void AbsoluteFilePath_Create_RejectsADotFinalSegment()
	{
		foreach (string path in new[] { TestPaths.Absolute("home", "user", ".."), TestPaths.Absolute("home", "user", ".") })
		{
			Assert.ThrowsExactly<ArgumentException>(() => AbsoluteFilePath.Create<AbsoluteFilePath>(path), path);
			Assert.IsFalse(AbsoluteFilePath.TryCreate(path, out _), path);
		}
	}

	[TestMethod]
	[DataRow(".")]
	[DataRow("..")]
	public void RelativeFilePath_Create_RejectsADotFinalSegment(string name)
	{
		foreach (string path in new[] { name, TestPaths.Relative("a", name), "a/" + name, "a\\" + name })
		{
			Assert.ThrowsExactly<ArgumentException>(() => RelativeFilePath.Create<RelativeFilePath>(path), path);
			Assert.IsFalse(RelativeFilePath.TryCreate(path, out _), path);
		}
	}

	[TestMethod]
	[DataRow(".")]
	[DataRow("..")]
	public void FilePath_Create_RejectsADotFinalSegment(string name)
	{
		foreach (string path in new[] { name, TestPaths.Relative("a", name), TestPaths.Absolute("a", name) })
		{
			Assert.ThrowsExactly<ArgumentException>(() => FilePath.Create<FilePath>(path), path);
			Assert.IsFalse(FilePath.TryCreate(path, out _), path);
		}
	}

	[TestMethod]
	public void FilePaths_AcceptLeadingAndInnerDotSegments()
	{
		Assert.AreEqual(TestPaths.Relative("..", "a.txt"), RelativeFilePath.Create<RelativeFilePath>(TestPaths.Relative("..", "a.txt")).WeakString);
		Assert.AreEqual(TestPaths.Relative(".", "a", "..", "b"), FilePath.Create<FilePath>(TestPaths.Relative(".", "a", "..", "b")).WeakString);
		Assert.AreEqual(TestPaths.Absolute("home", "..", "...", ".bashrc"), AbsoluteFilePath.Create<AbsoluteFilePath>(TestPaths.Absolute("home", "..", "...", ".bashrc")).WeakString);
	}

	[TestMethod]
	public void EveryValidFilePath_HasAValidFileName()
	{
		AbsoluteDirectoryPath home = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(TestPaths.Absolute("home", "user"));
		AbsoluteFilePath file = home / FileName.Create<FileName>("notes.txt");

		Assert.AreEqual("notes.txt", file.FileName.WeakString);
		Assert.IsTrue(file.IsChildOf(home));
	}

	[TestMethod]
	[DataRow(".")]
	[DataRow("..")]
	public void DirectoryNamesAndPaths_StillAcceptADotSegment(string name)
	{
		Assert.AreEqual(name, DirectoryName.Create<DirectoryName>(name).WeakString);
		Assert.AreEqual(TestPaths.Relative("a", name), RelativeDirectoryPath.Create<RelativeDirectoryPath>(TestPaths.Relative("a", name)).WeakString);
		Assert.AreEqual(TestPaths.Absolute("a", name), AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(TestPaths.Absolute("a", name)).WeakString);
	}
}
