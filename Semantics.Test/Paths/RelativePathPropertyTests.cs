// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using System;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class RelativePathPropertyTests
{
	[TestMethod]
	public void RelativeDirectoryPath_Name_ReturnsCorrectDirectoryName()
	{
		// Test that Name property returns the last component as DirectoryName
		RelativeDirectoryPath path = RelativeDirectoryPath.Create<RelativeDirectoryPath>(TestPaths.Relative("projects", "app", "src"));

		DirectoryName name = path.Name;

		Assert.IsNotNull(name);
		Assert.AreEqual("src", name.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_Name_WithSingleComponent_ReturnsComponent()
	{
		// Test Name property with single directory name
		RelativeDirectoryPath path = RelativeDirectoryPath.Create<RelativeDirectoryPath>("myapp");

		DirectoryName name = path.Name;

		Assert.IsNotNull(name);
		Assert.AreEqual("myapp", name.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_Name_WithEmptyPath_ReturnsEmpty()
	{
		// Test Name property with empty path
		RelativeDirectoryPath path = RelativeDirectoryPath.Create<RelativeDirectoryPath>("");

		DirectoryName name = path.Name;

		Assert.IsNotNull(name);
		Assert.AreEqual("", name.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_Parent_ReturnsCorrectParent()
	{
		// Test that Parent property returns parent directory
		RelativeDirectoryPath path = RelativeDirectoryPath.Create<RelativeDirectoryPath>(TestPaths.Relative("projects", "app", "src"));

		RelativeDirectoryPath parent = path.Parent;

		Assert.IsNotNull(parent);
		Assert.Contains("projects", parent.WeakString);
		Assert.Contains("app", parent.WeakString);
		Assert.DoesNotContain("src", parent.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_Parent_WithSingleComponent_ReturnsStartingDirectory()
	{
		RelativeDirectoryPath path = RelativeDirectoryPath.Create<RelativeDirectoryPath>("myapp");

		RelativeDirectoryPath parent = path.Parent;

		Assert.AreEqual(".", parent.WeakString);
		Assert.AreEqual(Path.GetFullPath("."), parent.AsAbsolute().WeakString);
		Assert.IsTrue(parent.IsDirectory);
	}

	[TestMethod]
	public void RelativeDirectoryPath_Parent_WithSingleComponent_ResolvesAgainstABase()
	{
		AbsoluteDirectoryPath baseDirectory = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(Path.GetTempPath());

		RelativeDirectoryPath parent = RelativeDirectoryPath.Create<RelativeDirectoryPath>("myapp").Parent;

		Assert.AreEqual(Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar), parent.AsAbsolute(baseDirectory).WeakString.TrimEnd(Path.DirectorySeparatorChar));
	}

	[TestMethod]
	public void RelativeDirectoryPath_Depth_CalculatesCorrectly()
	{
		// Test Depth property calculation
		RelativeDirectoryPath shallow = RelativeDirectoryPath.Create<RelativeDirectoryPath>("myapp");
		RelativeDirectoryPath medium = RelativeDirectoryPath.Create<RelativeDirectoryPath>(TestPaths.Relative("projects", "myapp"));
		RelativeDirectoryPath deep = RelativeDirectoryPath.Create<RelativeDirectoryPath>(TestPaths.Relative("projects", "myapp", "src", "components"));

		Assert.AreEqual(0, shallow.Depth);
		Assert.AreEqual(1, medium.Depth);
		Assert.AreEqual(3, deep.Depth);
	}

	[TestMethod]
	public void RelativeDirectoryPath_Depth_WithEmptyPath_ReturnsZero()
	{
		// Test Depth property with empty path
		RelativeDirectoryPath empty = RelativeDirectoryPath.Create<RelativeDirectoryPath>("");

		Assert.AreEqual(0, empty.Depth);
	}

	[TestMethod]
	public void RelativeFilePath_RelativeDirectoryPath_ReturnsCorrectDirectory()
	{
		// Test RelativeDirectoryPath property
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>(TestPaths.Relative("projects", "app", "Component.tsx"));

		RelativeDirectoryPath dir = file.RelativeDirectoryPath;

		Assert.IsNotNull(dir);
		Assert.Contains("projects", dir.WeakString);
		Assert.Contains("app", dir.WeakString);
		Assert.DoesNotContain("Component", dir.WeakString);
	}

	[TestMethod]
	public void RelativeFilePath_RelativeDirectoryPath_WithBareFileName_ReturnsStartingDirectory()
	{
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>("file.txt");

		RelativeDirectoryPath dir = file.RelativeDirectoryPath;

		Assert.AreEqual(".", dir.WeakString);
		Assert.AreEqual(Path.GetFullPath("."), dir.AsAbsolute().WeakString);
	}

	[TestMethod]
	public void RelativeFilePath_DirectoryPath_WithBareFileName_ReturnsStartingDirectory()
	{
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>("file.txt");

		Assert.AreEqual(".", file.DirectoryPath.WeakString);
		Assert.IsTrue(file.DirectoryPath.IsDirectory);
	}

	[TestMethod]
	public void RelativeFilePath_FileNameWithoutExtension_ReturnsCorrectName()
	{
		// Test FileNameWithoutExtension property
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>(TestPaths.Relative("projects", "Component.tsx"));

		FileName name = file.FileNameWithoutExtension;

		Assert.IsNotNull(name);
		Assert.AreEqual("Component", name.WeakString);
	}

	[TestMethod]
	public void RelativeFilePath_FileNameWithoutExtension_WithMultipleExtensions_RemovesLastOnly()
	{
		// Test FileNameWithoutExtension with multiple extensions
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>("archive.tar.gz");

		FileName name = file.FileNameWithoutExtension;

		Assert.IsNotNull(name);
		Assert.AreEqual("archive.tar", name.WeakString);
	}

	[TestMethod]
	public void RelativeFilePath_ChangeExtension_ChangesCorrectly()
	{
		// Test ChangeExtension method
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>(TestPaths.Relative("projects", "file.txt"));
		FileExtension newExt = FileExtension.Create<FileExtension>(".md");

		RelativeFilePath result = file.ChangeExtension(newExt);

		Assert.IsNotNull(result);
		Assert.Contains("file.md", result.WeakString);
		Assert.DoesNotContain(".txt", result.WeakString);
	}

	[TestMethod]
	public void RelativeFilePath_ChangeExtension_WithNullExtension_ThrowsException()
	{
		// Test ChangeExtension with null extension
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>("file.txt");

		Assert.ThrowsExactly<ArgumentNullException>(() => file.ChangeExtension(null!));
	}

	[TestMethod]
	public void RelativeFilePath_RemoveExtension_RemovesCorrectly()
	{
		// Test RemoveExtension method
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>(TestPaths.Relative("projects", "file.txt"));

		RelativeFilePath result = file.RemoveExtension();

		Assert.IsNotNull(result);
		Assert.Contains("file", result.WeakString);
		Assert.DoesNotContain(".txt", result.WeakString);
	}

	[TestMethod]
	public void RelativeFilePath_RemoveExtension_WithNoExtension_ReturnsUnchanged()
	{
		// Test RemoveExtension with file without extension
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>(TestPaths.Relative("projects", "README"));

		RelativeFilePath result = file.RemoveExtension();

		Assert.IsNotNull(result);
		Assert.Contains("README", result.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_CombineWithRelativeDirectory_WorksCorrectly()
	{
		// Test combining relative directories
		RelativeDirectoryPath base1 = RelativeDirectoryPath.Create<RelativeDirectoryPath>("projects");
		RelativeDirectoryPath sub = RelativeDirectoryPath.Create<RelativeDirectoryPath>("app");

		RelativeDirectoryPath result = base1 / sub;

		Assert.IsNotNull(result);
		Assert.Contains("projects", result.WeakString);
		Assert.Contains("app", result.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_CombineWithRelativeFile_WorksCorrectly()
	{
		// Test combining relative directory with relative file
		RelativeDirectoryPath dir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("projects");
		RelativeFilePath file = RelativeFilePath.Create<RelativeFilePath>("file.txt");

		RelativeFilePath result = dir / file;

		Assert.IsNotNull(result);
		Assert.Contains("projects", result.WeakString);
		Assert.Contains("file.txt", result.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_CombineWithFileName_WorksCorrectly()
	{
		// Test combining relative directory with file name
		RelativeDirectoryPath dir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("projects");
		FileName file = FileName.Create<FileName>("Component.tsx");

		RelativeFilePath result = dir / file;

		Assert.IsNotNull(result);
		Assert.Contains("projects", result.WeakString);
		Assert.Contains("Component.tsx", result.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_AsRelative_ReturnsSelf()
	{
		// Test that AsRelative returns self for already relative paths
		RelativeDirectoryPath path = RelativeDirectoryPath.Create<RelativeDirectoryPath>("projects");
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(TestPaths.Absolute("temp"));

		RelativeDirectoryPath result = path.AsRelative(baseDir);

		Assert.AreSame(path, result);
	}

	[TestMethod]
	public void RelativeFilePath_AsRelative_ReturnsSelf()
	{
		// Test that AsRelative returns self for already relative file paths
		RelativeFilePath path = RelativeFilePath.Create<RelativeFilePath>("file.txt");
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(TestPaths.Absolute("temp"));

		RelativeFilePath result = path.AsRelative(baseDir);

		Assert.AreSame(path, result);
	}

	[TestMethod]
	public void RelativeDirectoryPath_AsAbsoluteWithBase_ResolvesCorrectly()
	{
		// Test AsAbsolute with explicit base directory
		RelativeDirectoryPath relative = RelativeDirectoryPath.Create<RelativeDirectoryPath>("projects");
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(TestPaths.Absolute("work"));

		AbsoluteDirectoryPath result = relative.AsAbsolute(baseDir);

		Assert.IsNotNull(result);
		Assert.Contains(TestPaths.Absolute("work"), result.WeakString);
		Assert.Contains("projects", result.WeakString);
	}

	[TestMethod]
	public void RelativeFilePath_AsAbsoluteWithBase_ResolvesCorrectly()
	{
		// Test AsAbsolute with explicit base directory for files
		RelativeFilePath relative = RelativeFilePath.Create<RelativeFilePath>("file.txt");
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(TestPaths.Absolute("work"));

		AbsoluteFilePath result = relative.AsAbsolute(baseDir);

		Assert.IsNotNull(result);
		Assert.Contains(TestPaths.Absolute("work"), result.WeakString);
		Assert.Contains("file.txt", result.WeakString);
	}

	[TestMethod]
	public void RelativeDirectoryPath_AsAbsoluteWithBase_WithNullBase_ThrowsException()
	{
		// Test AsAbsolute with null base directory
		RelativeDirectoryPath relative = RelativeDirectoryPath.Create<RelativeDirectoryPath>("projects");

		Assert.ThrowsExactly<ArgumentNullException>(() => relative.AsAbsolute(null!));
	}

	[TestMethod]
	public void RelativeFilePath_AsAbsoluteWithBase_WithNullBase_ThrowsException()
	{
		// Test AsAbsolute with null base directory for files
		RelativeFilePath relative = RelativeFilePath.Create<RelativeFilePath>("file.txt");

		Assert.ThrowsExactly<ArgumentNullException>(() => relative.AsAbsolute(null!));
	}

	[TestMethod]
	[DataRow("..", "../..")]
	[DataRow("../..", "../../..")]
	[DataRow(".", "..")]
	[DataRow("", "..")]
	[DataRow("a/..", "..")]
	[DataRow("a/../..", "../..")]
	[DataRow("a", ".")]
	[DataRow("a/b", "a")]
	[DataRow("a/./b", "a")]
	[DataRow("../a", "..")]
	public void RelativeDirectoryPath_Parent_ResolvesDotSegments(string path, string expected)
	{
		RelativeDirectoryPath parent = RelativeDirectoryPath.Create<RelativeDirectoryPath>(Native(path)).Parent;

		Assert.AreEqual(Native(expected), parent.WeakString);
	}

	[TestMethod]
	[DataRow("")]
	[DataRow(".")]
	[DataRow("..")]
	[DataRow("../..")]
	[DataRow("a")]
	[DataRow("a/b")]
	[DataRow("a/..")]
	[DataRow("a/../..")]
	[DataRow("../a")]
	[DataRow("a/./b")]
	public void RelativeDirectoryPath_Parent_AgreesWithAbsoluteParent(string path)
	{
		AbsoluteDirectoryPath baseDirectory = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(TestPaths.Absolute("home", "u", "w"));
		RelativeDirectoryPath relative = RelativeDirectoryPath.Create<RelativeDirectoryPath>(Native(path));

		Assert.AreEqual(relative.AsAbsolute(baseDirectory).Parent.WeakString, relative.Parent.AsAbsolute(baseDirectory).WeakString);
	}

	private static string Native(string? path) => (path ?? string.Empty).Replace('/', Path.DirectorySeparatorChar);
}
