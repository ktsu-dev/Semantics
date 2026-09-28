// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using System;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for the containment checks <see cref="AbsoluteDirectoryPath.IsChildOf"/>,
/// <see cref="AbsoluteDirectoryPath.IsParentOf"/> and <see cref="AbsoluteFilePath.IsChildOf"/>.
/// </summary>
[TestClass]
public class PathContainmentTests
{
	private static AbsoluteDirectoryPath Dir(string path) => AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(path);

	private static AbsoluteFilePath File(string path) => AbsoluteFilePath.Create<AbsoluteFilePath>(path);

	// Windows filesystems ignore case, so there a differently-cased directory is the same directory.
	// Everywhere else the library compares paths case-sensitively, and so do containment checks.
	private static bool CaseInsensitive => OperatingSystem.IsWindows();

	[TestMethod]
	public void DirectoryUnderDifferentlyCasedSibling_IsChildOnlyWhereCaseIsIgnored()
	{
		AbsoluteDirectoryPath parent = Dir(TestPaths.Absolute("home", "user", "docs"));
		AbsoluteDirectoryPath other = Dir(TestPaths.Absolute("home", "user", "DOCS", "report"));

		Assert.AreEqual(CaseInsensitive, other.IsChildOf(parent));
		Assert.AreEqual(CaseInsensitive, parent.IsParentOf(other));
	}

	[TestMethod]
	public void FileUnderDifferentlyCasedSibling_IsChildOnlyWhereCaseIsIgnored()
	{
		AbsoluteDirectoryPath parent = Dir(TestPaths.Absolute("home", "user", "docs"));
		AbsoluteFilePath other = File(TestPaths.Absolute("home", "user", "DOCS", "report.txt"));

		Assert.AreEqual(CaseInsensitive, other.IsChildOf(parent));
	}

	[TestMethod]
	public void DifferentlyCasedSpellingOfTheSameDirectory_IsNeverAChild()
	{
		AbsoluteDirectoryPath lower = Dir(TestPaths.Absolute("home", "user", "docs"));
		AbsoluteDirectoryPath upper = Dir(TestPaths.Absolute("home", "user", "DOCS"));

		Assert.IsFalse(upper.IsChildOf(lower));
		Assert.IsFalse(lower.IsParentOf(upper));
	}

	[TestMethod]
	public void DirectoryDirectlyUnderRoot_IsChildOfRoot()
	{
		AbsoluteDirectoryPath root = Dir(TestPaths.Root);
		AbsoluteDirectoryPath home = Dir(TestPaths.Absolute("home"));

		Assert.IsTrue(home.IsChildOf(root));
		Assert.IsTrue(root.IsParentOf(home));
		Assert.AreEqual(root, home.Parent);
	}

	[TestMethod]
	public void FileDirectlyUnderRoot_IsChildOfRoot()
	{
		AbsoluteDirectoryPath root = Dir(TestPaths.Root);

		Assert.IsTrue(File(TestPaths.Absolute("etc.txt")).IsChildOf(root));
	}

	[TestMethod]
	public void DeepPath_IsChildOfRoot()
	{
		AbsoluteDirectoryPath root = Dir(TestPaths.Root);

		Assert.IsTrue(Dir(TestPaths.Absolute("home", "user")).IsChildOf(root));
		Assert.IsTrue(File(TestPaths.Absolute("home", "user", "a.txt")).IsChildOf(root));
	}

	[TestMethod]
	public void Root_IsNotAChildOfItself()
	{
		AbsoluteDirectoryPath root = Dir(TestPaths.Root);

		Assert.IsFalse(root.IsChildOf(root));
		Assert.IsFalse(root.IsParentOf(root));
	}

	[TestMethod]
	public void SiblingSharingANamePrefix_IsNotAChild()
	{
		AbsoluteDirectoryPath home = Dir(TestPaths.Absolute("home"));

		Assert.IsFalse(Dir(TestPaths.Absolute("home2")).IsChildOf(home));
		Assert.IsFalse(Dir(TestPaths.Absolute("home2", "user")).IsChildOf(home));
		Assert.IsFalse(File(TestPaths.Absolute("home2.txt")).IsChildOf(home));
		Assert.IsFalse(home.IsParentOf(Dir(TestPaths.Absolute("home2"))));
	}
}
