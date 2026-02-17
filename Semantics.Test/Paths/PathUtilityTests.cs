// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Paths;

using System;
using System.IO;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PathUtilityTests
{
	[TestMethod]
	public void IsChildOf_WithValidChildPath_ReturnsTrue()
	{
		// Test IsChildOf with valid parent-child relationship
		AbsoluteDirectoryPath parent = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		AbsoluteDirectoryPath child = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\app\src");

		bool result = child.IsChildOf(parent);

		Assert.IsTrue(result);
	}

	[TestMethod]
	public void IsChildOf_WithSamePath_ReturnsFalse()
	{
		// Test IsChildOf with identical paths (should return false)
		AbsoluteDirectoryPath path1 = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		AbsoluteDirectoryPath path2 = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");

		bool result = path1.IsChildOf(path2);

		Assert.IsFalse(result);
	}

	[TestMethod]
	public void IsChildOf_WithNonChildPath_ReturnsFalse()
	{
		// Test IsChildOf with unrelated paths
		AbsoluteDirectoryPath path1 = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\app");
		AbsoluteDirectoryPath path2 = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\other\folder");

		bool result = path1.IsChildOf(path2);

		Assert.IsFalse(result);
	}

	[TestMethod]
	public void IsChildOf_WithParentAsChild_ReturnsFalse()
	{
		// Test IsChildOf with parent-child relationship reversed
		AbsoluteDirectoryPath parent = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		AbsoluteDirectoryPath child = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\app");

		bool result = parent.IsChildOf(child);

		Assert.IsFalse(result);
	}

	[TestMethod]
	public void IsChildOf_WithNullArgument_ThrowsArgumentNullException()
	{
		// Test IsChildOf with null argument
		AbsoluteDirectoryPath path = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");

		Assert.ThrowsExactly<ArgumentNullException>(() => path.IsChildOf(null!));
	}

	[TestMethod]
	public void IsChildOf_WithMixedSeparators_WorksCorrectly()
	{
		// Test IsChildOf with mixed path separators
		AbsoluteDirectoryPath parent = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		AbsoluteDirectoryPath child = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:/projects/app/src");

		bool result = child.IsChildOf(parent);

		Assert.IsTrue(result);
	}

	[TestMethod]
	public void GetRelativePathTo_WithValidPaths_ReturnsCorrectRelativePath()
	{
		// Test GetRelativePathTo with valid directory paths
		AbsoluteDirectoryPath from = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\app");
		AbsoluteDirectoryPath to = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\lib\utils");

		RelativeDirectoryPath result = from.GetRelativePathTo(to);

		Assert.IsNotNull(result);
		Assert.IsTrue(result.WeakString.Contains(".."));
		Assert.IsTrue(result.WeakString.Contains("lib"));
		Assert.IsTrue(result.WeakString.Contains("utils"));
	}

	[TestMethod]
	public void GetRelativePathTo_WithSamePath_ReturnsCurrentDirectory()
	{
		// Test GetRelativePathTo with identical paths
		AbsoluteDirectoryPath path1 = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		AbsoluteDirectoryPath path2 = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");

		RelativeDirectoryPath result = path1.GetRelativePathTo(path2);

		Assert.IsNotNull(result);
		// Should be "." or empty depending on implementation
		Assert.IsTrue(result.WeakString is "." || string.IsNullOrEmpty(result.WeakString));
	}

	[TestMethod]
	public void GetRelativePathTo_WithNullArgument_ThrowsArgumentNullException()
	{
		// Test GetRelativePathTo with null argument
		AbsoluteDirectoryPath path = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");

		Assert.ThrowsExactly<ArgumentNullException>(() => path.GetRelativePathTo(null!));
	}

	[TestMethod]
	public void GetRelativePathTo_WithChildPath_ReturnsSimpleRelativePath()
	{
		// Test GetRelativePathTo from parent to child
		AbsoluteDirectoryPath parent = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		AbsoluteDirectoryPath child = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\app\src");

		RelativeDirectoryPath result = parent.GetRelativePathTo(child);

		Assert.IsNotNull(result);
		Assert.IsTrue(result.WeakString.Contains("app"));
		Assert.IsTrue(result.WeakString.Contains("src"));
		Assert.IsFalse(result.WeakString.Contains(".."));
	}

	[TestMethod]
	public void Normalize_WithDotPaths_ResolvesCorrectly()
	{
		// Test Normalize with . and .. components
		RelativeDirectoryPath complexPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"app\.\src\..\lib\utils");

		RelativeDirectoryPath normalized = complexPath.Normalize();

		Assert.IsNotNull(normalized);
		Assert.IsTrue(normalized.WeakString.Contains("app"));
		Assert.IsTrue(normalized.WeakString.Contains("lib"));
		Assert.IsTrue(normalized.WeakString.Contains("utils"));
		Assert.IsFalse(normalized.WeakString.Contains('.'));
	}

	[TestMethod]
	public void Normalize_WithEmptyPath_ReturnsEmpty()
	{
		// Test Normalize with empty path
		RelativeDirectoryPath emptyPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>("");

		RelativeDirectoryPath normalized = emptyPath.Normalize();

		Assert.AreSame(emptyPath, normalized);
	}

	[TestMethod]
	public void Normalize_WithSimplePath_ReturnsSame()
	{
		// Test Normalize with already normalized path
		RelativeDirectoryPath simplePath = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"app\src");

		RelativeDirectoryPath normalized = simplePath.Normalize();

		Assert.IsNotNull(normalized);
		Assert.AreEqual("app" + Path.DirectorySeparatorChar + "src", normalized.WeakString);
	}

	[TestMethod]
	public void Normalize_WithOnlyDots_ResolvesCorrectly()
	{
		// Test Normalize with only . and .. components
		RelativeDirectoryPath dotPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@".\..\.\folder");

		RelativeDirectoryPath normalized = dotPath.Normalize();

		Assert.IsNotNull(normalized);
		Assert.IsTrue(normalized.WeakString.Contains("folder"));
	}

	[TestMethod]
	public void RemoveExtension_WithValidExtension_RemovesCorrectly()
	{
		// Test RemoveExtension on RelativeFilePath
		RelativeFilePath filePath = RelativeFilePath.Create<RelativeFilePath>(@"app\src\component.tsx");

		RelativeFilePath result = filePath.RemoveExtension();

		Assert.IsNotNull(result);
		Assert.IsTrue(result.WeakString.Contains("component"));
		Assert.IsFalse(result.WeakString.Contains(".tsx"));
	}

	[TestMethod]
	public void RemoveExtension_WithMultipleExtensions_RemovesLastOnly()
	{
		// Test RemoveExtension with multiple extensions
		RelativeFilePath filePath = RelativeFilePath.Create<RelativeFilePath>(@"backup.tar.gz");

		RelativeFilePath result = filePath.RemoveExtension();

		Assert.IsNotNull(result);
		Assert.IsTrue(result.WeakString.Contains("backup.tar"));
		Assert.IsFalse(result.WeakString.Contains(".gz"));
	}

	[TestMethod]
	public void RemoveExtension_WithNoExtension_ReturnsUnchanged()
	{
		// Test RemoveExtension with file that has no extension
		RelativeFilePath filePath = RelativeFilePath.Create<RelativeFilePath>(@"app\src\README");

		RelativeFilePath result = filePath.RemoveExtension();

		Assert.IsNotNull(result);
		Assert.IsTrue(result.WeakString.Contains("README"));
	}

	[TestMethod]
	public void PathDepth_CalculatesCorrectly()
	{
		// Test depth calculation for relative directory paths
		RelativeDirectoryPath shallowPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>("folder");
		RelativeDirectoryPath deepPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"app\src\components\ui");

		int shallowDepth = shallowPath.Depth;
		int deepDepth = deepPath.Depth;

		Assert.AreEqual(0, shallowDepth); // No separators
		Assert.AreEqual(3, deepDepth); // Three separators
	}

	[TestMethod]
	public void PathDepth_WithEmptyPath_ReturnsZero()
	{
		// Test depth calculation for empty path
		RelativeDirectoryPath emptyPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>("");

		int depth = emptyPath.Depth;

		Assert.AreEqual(0, depth);
	}

	[TestMethod]
	public void PathDepth_WithMixedSeparators_CountsCorrectly()
	{
		// Test depth calculation with mixed separators
		RelativeDirectoryPath mixedPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"app/src\components");

		int depth = mixedPath.Depth;

		Assert.AreEqual(2, depth); // Two separators regardless of type
	}
}
