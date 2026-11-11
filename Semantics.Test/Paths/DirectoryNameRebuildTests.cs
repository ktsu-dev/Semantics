// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// A directory's <c>Name</c> is a <see cref="DirectoryName"/> on every directory type, so rebuilding a
/// directory from its own parts keeps it a directory. <see cref="AbsoluteDirectoryPath"/> and
/// <see cref="DirectoryPath"/> used to return a <see cref="FileName"/>, which turned
/// <c>dir.Parent / dir.Name</c> into a file path (#498).
/// </summary>
[TestClass]
public class DirectoryNameRebuildTests
{
	[TestMethod]
	public void AbsoluteDirectoryPath_ParentSlashName_IsTheSameDirectory()
	{
		AbsoluteDirectoryPath dir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(TestPaths.Absolute("home", "user", "docs"));

		DirectoryName name = dir.Name;
		AbsoluteDirectoryPath rebuilt = dir.Parent / dir.Name;

		Assert.AreEqual("docs", name.WeakString);
		Assert.AreEqual(dir, rebuilt);
	}

	[TestMethod]
	public void DirectoryPath_ParentSlashName_IsTheSameDirectory()
	{
		DirectoryPath dir = DirectoryPath.Create<DirectoryPath>(TestPaths.Absolute("home", "user", "docs"));

		DirectoryName name = dir.Name;
		DirectoryPath rebuilt = dir.Parent / dir.Name;

		Assert.AreEqual("docs", name.WeakString);
		Assert.AreEqual(dir, rebuilt);
	}

	[TestMethod]
	public void DirectoryPath_SingleSegment_ParentSlashName_IsTheSameDirectory()
	{
		// The parent of "a" is "." (#504); combining it back must not give "./a" (#539).
		DirectoryPath dir = DirectoryPath.Create<DirectoryPath>("a");

		DirectoryPath rebuilt = dir.Parent / dir.Name;

		Assert.AreEqual("a", rebuilt.WeakString);
		Assert.AreEqual(dir, rebuilt);
	}

	[TestMethod]
	public void FilePath_BareFileName_DirectorySlashFileName_IsTheSameFile()
	{
		FilePath file = FilePath.Create<FilePath>("x.txt");

		FilePath rebuilt = file.DirectoryPath / file.FileName;

		Assert.AreEqual("x.txt", rebuilt.WeakString);
		Assert.AreEqual(file, rebuilt);
	}

	[TestMethod]
	public void DirectoryPath_Dot_SlashRelativeFilePath_IsTheRelativeFilePath()
	{
		FilePath combined = DirectoryPath.Create<DirectoryPath>(".") / RelativeFilePath.Create<RelativeFilePath>("x.txt");

		Assert.AreEqual("x.txt", combined.WeakString);
	}

	[TestMethod]
	public void DirectoryPath_Dot_SlashRelativeDirectoryPath_IsTheRelativeDirectoryPath()
	{
		DirectoryPath combined = DirectoryPath.Create<DirectoryPath>(".") / RelativeDirectoryPath.Create<RelativeDirectoryPath>("a");

		Assert.AreEqual("a", combined.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_ParentSlashName_IsTheSameDirectory()
	{
		RelativeDirectoryPath dir = RelativeDirectoryPath.Create<RelativeDirectoryPath>(TestPaths.Relative("projects", "app", "src"));

		DirectoryName name = dir.Name;
		RelativeDirectoryPath rebuilt = dir.Parent / dir.Name;

		Assert.AreEqual("src", name.WeakString);
		Assert.AreEqual(dir, rebuilt);
	}

	[TestMethod]
	public void AbsoluteDirectoryPath_Name_IsStillTheLastComponentAsWritten()
	{
		// The documented "as written, not as resolved" behaviour is unchanged.
		AbsoluteDirectoryPath dir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(TestPaths.Absolute("home", "user", ".."));

		Assert.AreEqual("..", dir.Name.WeakString);
	}
}
