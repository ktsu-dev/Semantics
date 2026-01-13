// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Paths;

using System.Collections.Generic;
using ktsu.Semantics.Paths;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PathIntegrationTests
{
	[TestMethod]
	public void MixedPathTypes_InCollection_WorkCorrectly()
	{
		// Test that different path types can coexist in polymorphic collections
		List<IPath> paths =
		[
			AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects"),
			AbsoluteFilePath.Create<AbsoluteFilePath>(@"C:\file.txt"),
			RelativeDirectoryPath.Create<RelativeDirectoryPath>("subfolder"),
			RelativeFilePath.Create<RelativeFilePath>("file.txt"),
			DirectoryPath.Create<DirectoryPath>("any"),
			FilePath.Create<FilePath>("any.txt")
		];

		Assert.HasCount(6, paths);
		foreach (IPath path in paths)
		{
			Assert.IsNotNull(path);
			// All created paths are valid by construction
		}
	}

	[TestMethod]
	public void DirectoryNames_InCollection_WorkCorrectly()
	{
		// Test DirectoryName in collections
		List<DirectoryName> names =
		[
			DirectoryName.Create<DirectoryName>("folder1"),
			DirectoryName.Create<DirectoryName>("folder2"),
			DirectoryName.Create<DirectoryName>("folder3")
		];

		Assert.HasCount(3, names);
		Assert.Contains(DirectoryName.Create<DirectoryName>("folder1"), names);
	}

	[TestMethod]
	public void DirectoryNames_AsSet_WorkCorrectly()
	{
		// Test DirectoryName in HashSet
		HashSet<DirectoryName> uniqueNames =
		[
			DirectoryName.Create<DirectoryName>("folder1"),
			DirectoryName.Create<DirectoryName>("folder2"),
			DirectoryName.Create<DirectoryName>("folder1") // Duplicate
		];

		Assert.HasCount(2, uniqueNames); // Duplicate should be ignored
	}

	[TestMethod]
	public void ComplexPathConstruction_WithAllTypes_WorksCorrectly()
	{
		// Test complex path construction scenario
		AbsoluteDirectoryPath root = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		DirectoryName appDir = DirectoryName.Create<DirectoryName>("myapp");
		DirectoryName srcDir = DirectoryName.Create<DirectoryName>("src");
		FileName componentFile = FileName.Create<FileName>("Component.tsx");

		// Build path step by step
		AbsoluteDirectoryPath appPath = root / appDir;
		AbsoluteDirectoryPath srcPath = appPath / srcDir;
		AbsoluteFilePath filePath = srcPath / componentFile;

		Assert.IsNotNull(filePath);
		Assert.Contains(@"C:\projects", filePath.WeakString);
		Assert.Contains("myapp", filePath.WeakString);
		Assert.Contains("src", filePath.WeakString);
		Assert.Contains("Component.tsx", filePath.WeakString);
	}

	[TestMethod]
	public void RelativeToAbsolute_RoundTrip_WorksCorrectly()
	{
		// Test converting between relative and absolute paths
		RelativeDirectoryPath relative = RelativeDirectoryPath.Create<RelativeDirectoryPath>("projects");
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\work");

		// Convert to absolute with specific base
		AbsoluteDirectoryPath absolute = relative.AsAbsolute(baseDir);

		Assert.IsNotNull(absolute);
		Assert.Contains(@"C:\work", absolute.WeakString);
		Assert.Contains("projects", absolute.WeakString);
	}

	[TestMethod]
	public void AbsoluteToRelative_RoundTrip_WorksCorrectly()
	{
		// Test converting from absolute to relative paths
		AbsoluteDirectoryPath absolute = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\work\projects");
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\work");

		// Convert to relative
		RelativeDirectoryPath relative = absolute.AsRelative(baseDir);

		Assert.IsNotNull(relative);
		Assert.Contains("projects", relative.WeakString);
		Assert.DoesNotContain(@"C:\work", relative.WeakString);
	}

	[TestMethod]
	public void PathHierarchy_ParentTraversal_WorksCorrectly()
	{
		// Test traversing up the directory hierarchy
		RelativeDirectoryPath deep = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"a\b\c\d");

		RelativeDirectoryPath parent1 = deep.Parent;
		RelativeDirectoryPath parent2 = parent1.Parent;
		RelativeDirectoryPath parent3 = parent2.Parent;
		RelativeDirectoryPath parent4 = parent3.Parent;

		Assert.Contains("c", parent1.WeakString);
		Assert.Contains("b", parent2.WeakString);
		Assert.AreEqual("a", parent3.WeakString);
		Assert.AreEqual("", parent4.WeakString); // Should be empty at root
	}

	[TestMethod]
	public void PathNormalization_WithDotComponents_ResolvesCorrectly()
	{
		// Test path normalization with . and .. components
		RelativeDirectoryPath pathWithDots = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"a\.\b\..\c");

		RelativeDirectoryPath normalized = pathWithDots.Normalize();

		Assert.IsNotNull(normalized);
		// After normalization, . and .. should be resolved
		// a\.\b\..\c -> a\c
		Assert.Contains("a", normalized.WeakString);
		Assert.Contains("c", normalized.WeakString);
	}

	[TestMethod]
	public void PathNormalization_WithParentTraversal_ResolvesCorrectly()
	{
		// Test path normalization with parent directory traversal
		RelativeDirectoryPath path = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"a\b\..\..\c");

		RelativeDirectoryPath normalized = path.Normalize();

		Assert.IsNotNull(normalized);
		// a\b\..\..\c -> c
		Assert.AreEqual("c", normalized.WeakString);
	}

	[TestMethod]
	public void FilePathExtensions_MultipleOperations_WorkCorrectly()
	{
		// Test multiple extension operations on same file
		RelativeFilePath original = RelativeFilePath.Create<RelativeFilePath>("document.txt");
		FileExtension mdExt = FileExtension.Create<FileExtension>(".md");
		FileExtension pdfExt = FileExtension.Create<FileExtension>(".pdf");

		RelativeFilePath md = original.ChangeExtension(mdExt);
		RelativeFilePath pdf = md.ChangeExtension(pdfExt);
		RelativeFilePath noExt = pdf.RemoveExtension();

		Assert.Contains(".md", md.WeakString);
		Assert.Contains(".pdf", pdf.WeakString);
		Assert.DoesNotContain(".pdf", noExt.WeakString);
		Assert.Contains("document", noExt.WeakString);
	}

	[TestMethod]
	public void DirectoryDepth_Comparison_WorksCorrectly()
	{
		// Test comparing depths of different paths
		RelativeDirectoryPath shallow = RelativeDirectoryPath.Create<RelativeDirectoryPath>("a");
		RelativeDirectoryPath medium = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"a\b");
		RelativeDirectoryPath deep = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"a\b\c");

		Assert.IsLessThan(medium.Depth, shallow.Depth);
		Assert.IsLessThan(deep.Depth, medium.Depth);
		Assert.AreEqual(0, shallow.Depth);
		Assert.AreEqual(1, medium.Depth);
		Assert.AreEqual(2, deep.Depth);
	}

	[TestMethod]
	public void InterfaceBasedPathOperations_WorkPolymorphically()
	{
		// Test that interface-based operations work polymorphically
		IDirectoryPath dir1 = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");
		IDirectoryPath dir2 = RelativeDirectoryPath.Create<RelativeDirectoryPath>("test");
		IDirectoryPath dir3 = DirectoryPath.Create<DirectoryPath>("test");

		IPath[] paths = [dir1, dir2, dir3];

		foreach (IPath path in paths)
		{
			Assert.IsNotNull(path);
			ISemanticString semanticPath = (ISemanticString)path;
			Assert.IsNotNull(semanticPath.WeakString);
			// All paths are valid by construction
		}
	}

	[TestMethod]
	public void CombiningDirectoryNames_BuildsDeepHierarchy()
	{
		// Test building deep directory hierarchies with DirectoryName
		DirectoryPath root = DirectoryPath.Create<DirectoryPath>("root");
		DirectoryName[] levels =
		[
			DirectoryName.Create<DirectoryName>("level1"),
			DirectoryName.Create<DirectoryName>("level2"),
			DirectoryName.Create<DirectoryName>("level3"),
			DirectoryName.Create<DirectoryName>("level4"),
			DirectoryName.Create<DirectoryName>("level5")
		];

		DirectoryPath current = root;
		foreach (DirectoryName level in levels)
		{
			current /= level;
		}

		// Verify all levels are in the final path
		Assert.Contains("root", current.WeakString);
		Assert.Contains("level1", current.WeakString);
		Assert.Contains("level2", current.WeakString);
		Assert.Contains("level3", current.WeakString);
		Assert.Contains("level4", current.WeakString);
		Assert.Contains("level5", current.WeakString);
	}

	[TestMethod]
	public void EmptyPaths_HandleCorrectly()
	{
		// Test that empty paths are handled correctly
		DirectoryPath emptyDir = DirectoryPath.Create<DirectoryPath>("");
		FilePath emptyFile = FilePath.Create<FilePath>("");
		DirectoryName emptyName = DirectoryName.Create<DirectoryName>("");
		FileName emptyFileName = FileName.Create<FileName>("");

		Assert.AreEqual("", emptyDir.WeakString);
		Assert.AreEqual("", emptyFile.WeakString);
		Assert.AreEqual("", emptyName.WeakString);
		Assert.AreEqual("", emptyFileName.WeakString);
	}

	[TestMethod]
	public void RelativePaths_WithDotDot_AreValid()
	{
		// Test that relative paths with .. components are valid
		RelativeDirectoryPath parentRef = RelativeDirectoryPath.Create<RelativeDirectoryPath>("..");
		RelativeDirectoryPath multiParent = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"..\..\..");
		RelativeFilePath fileInParent = RelativeFilePath.Create<RelativeFilePath>(@"..\file.txt");

		Assert.IsNotNull(parentRef);
		Assert.IsNotNull(multiParent);
		Assert.IsNotNull(fileInParent);
		Assert.IsTrue(parentRef.IsValid());
		Assert.IsTrue(multiParent.IsValid());
		Assert.IsTrue(fileInParent.IsValid());
	}

	[TestMethod]
	public void RelativePaths_WithDot_AreValid()
	{
		// Test that relative paths with . components are valid
		RelativeDirectoryPath currentRef = RelativeDirectoryPath.Create<RelativeDirectoryPath>(".");
		RelativeDirectoryPath withCurrent = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@".\subfolder");
		RelativeFilePath fileInCurrent = RelativeFilePath.Create<RelativeFilePath>(@".\file.txt");

		Assert.IsNotNull(currentRef);
		Assert.IsNotNull(withCurrent);
		Assert.IsNotNull(fileInCurrent);
		Assert.IsTrue(currentRef.IsValid());
		Assert.IsTrue(withCurrent.IsValid());
		Assert.IsTrue(fileInCurrent.IsValid());
	}
}
