// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Paths;

using System;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PathOperatorTests
{
	[TestMethod]
	public void PathOperators_NullArguments_ThrowArgumentNullException()
	{
		// Test all path combination operators with null arguments
		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>(@"test");

		FileName nullFileName = null!;

		// AbsoluteDirectoryPath operators
		Assert.ThrowsExactly<ArgumentNullException>(() => absoluteDir / nullFileName);

		// DirectoryPath operators
		Assert.ThrowsExactly<ArgumentNullException>(() => genericDir / nullFileName);

		DirectoryPath nullGenericDir = null!;
		Assert.ThrowsExactly<ArgumentNullException>(() => nullGenericDir / nullFileName);
	}

	[TestMethod]
	public void PathOperators_ComplexCombinations_WorkCorrectly()
	{
		// Test complex path combinations
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		DirectoryPath subDir1 = DirectoryPath.Create<DirectoryPath>(@"app\src");
		DirectoryPath subDir2 = DirectoryPath.Create<DirectoryPath>(@"components");
		FileName fileName = FileName.Create<FileName>("Component.tsx");

		// Chain multiple operations - combine paths using Path.Combine
		AbsoluteDirectoryPath combinedDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(
			Path.Combine(baseDir.WeakString, subDir1.WeakString, subDir2.WeakString));
		AbsoluteFilePath finalFile = combinedDir / fileName;

		Assert.IsNotNull(combinedDir);
		Assert.IsNotNull(finalFile);
		Assert.Contains(@"C:\projects", finalFile.WeakString);
		Assert.Contains(@"app", finalFile.WeakString);
		Assert.Contains(@"src", finalFile.WeakString);
		Assert.Contains(@"components", finalFile.WeakString);
		Assert.Contains("Component.tsx", finalFile.WeakString);
	}

	[TestMethod]
	public void PathOperators_EmptyPaths_HandleCorrectly()
	{
		// Test operators with empty paths
		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");
		FileName emptyFileName = FileName.Create<FileName>("");

		// These should work without throwing
		AbsoluteFilePath result3 = absoluteDir / emptyFileName;

		Assert.IsNotNull(result3);
	}

	[TestMethod]
	public void PathOperators_SpecialCharacters_HandleCorrectly()
	{
		// Test with paths containing special characters
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test folder");
		DirectoryPath specialDir = DirectoryPath.Create<DirectoryPath>(@"sub folder (1)");
		FileName specialFile = FileName.Create<FileName>("file name with spaces.txt");

		AbsoluteDirectoryPath combinedDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(
			Path.Combine(baseDir.WeakString, specialDir.WeakString));
		AbsoluteFilePath combinedFile = combinedDir / specialFile;

		Assert.IsNotNull(combinedDir);
		Assert.IsNotNull(combinedFile);
		Assert.Contains("test folder", combinedFile.WeakString);
		Assert.Contains("sub folder (1)", combinedFile.WeakString);
		Assert.Contains("file name with spaces.txt", combinedFile.WeakString);
	}

	[TestMethod]
	public void PathOperators_ReturnTypes_AreCorrect()
	{
		// Verify that operators return the correct types
		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>(@"test");
		FileName fileName = FileName.Create<FileName>("file.txt");

		// AbsoluteDirectoryPath combinations
		AbsoluteFilePath absFile2 = absoluteDir / fileName;

		// DirectoryPath combinations
		FilePath genFile2 = genericDir / fileName;

		// Verify types
		Assert.IsInstanceOfType<AbsoluteFilePath>(absFile2);
		Assert.IsInstanceOfType<FilePath>(genFile2);
	}

	[TestMethod]
	public void PathOperators_WithDotPaths_HandleCorrectly()
	{
		// Test with relative paths containing . and ..
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\app");
		RelativeDirectoryPath currentDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>(".");
		RelativeDirectoryPath parentDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("..");
		RelativeDirectoryPath complexPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"..\other\folder");

		AbsoluteDirectoryPath result1 = baseDir / currentDir;
		AbsoluteDirectoryPath result2 = baseDir / parentDir;
		AbsoluteDirectoryPath result3 = baseDir / complexPath;

		Assert.IsNotNull(result1);
		Assert.IsNotNull(result2);
		Assert.IsNotNull(result3);

		// The exact results depend on path normalization, but they should be valid
		Assert.IsTrue(result1.IsValid());
		Assert.IsTrue(result2.IsValid());
		Assert.IsTrue(result3.IsValid());
	}

	[TestMethod]
	public void PathOperators_CrossPlatformSeparators_HandleCorrectly()
	{
		// Test with mixed path separators
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");
		RelativeDirectoryPath unixStyleDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("sub/folder");
		RelativeFilePath unixStyleFile = RelativeFilePath.Create<RelativeFilePath>("sub/file.txt");

		AbsoluteDirectoryPath combinedDir = baseDir / unixStyleDir;
		AbsoluteFilePath combinedFile = baseDir / unixStyleFile;

		Assert.IsNotNull(combinedDir);
		Assert.IsNotNull(combinedFile);
		Assert.IsTrue(combinedDir.IsValid());
		Assert.IsTrue(combinedFile.IsValid());
	}

	[TestMethod]
	public void DirectoryNameOperator_WithAbsoluteDirectoryPath_CreatesCorrectPath()
	{
		// Test combining AbsoluteDirectoryPath with DirectoryName
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		DirectoryName subDir = DirectoryName.Create<DirectoryName>("myapp");

		AbsoluteDirectoryPath result = baseDir / subDir;

		Assert.IsNotNull(result);
		Assert.IsTrue(result.IsValid());
		Assert.Contains(@"C:\projects", result.WeakString);
		Assert.Contains("myapp", result.WeakString);
	}

	[TestMethod]
	public void DirectoryNameOperator_WithDirectoryPath_CreatesCorrectPath()
	{
		// Test combining DirectoryPath with DirectoryName
		DirectoryPath baseDir = DirectoryPath.Create<DirectoryPath>(@"projects");
		DirectoryName subDir = DirectoryName.Create<DirectoryName>("myapp");

		DirectoryPath result = baseDir / subDir;

		Assert.IsNotNull(result);
		Assert.IsTrue(result.IsValid());
		Assert.Contains("projects", result.WeakString);
		Assert.Contains("myapp", result.WeakString);
	}

	[TestMethod]
	public void DirectoryNameOperator_ChainedCombinations_WorkCorrectly()
	{
		// Test chaining multiple DirectoryName combinations
		DirectoryPath baseDir = DirectoryPath.Create<DirectoryPath>(@"projects");
		DirectoryName dir1 = DirectoryName.Create<DirectoryName>("app");
		DirectoryName dir2 = DirectoryName.Create<DirectoryName>("src");
		DirectoryName dir3 = DirectoryName.Create<DirectoryName>("components");

		DirectoryPath result = baseDir / dir1 / dir2 / dir3;

		Assert.IsNotNull(result);
		Assert.IsTrue(result.IsValid());
		Assert.Contains("projects", result.WeakString);
		Assert.Contains("app", result.WeakString);
		Assert.Contains("src", result.WeakString);
		Assert.Contains("components", result.WeakString);
	}

	[TestMethod]
	public void DirectoryNameOperator_WithNullDirectoryName_ThrowsArgumentNullException()
	{
		// Test null safety for DirectoryName operators
		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>(@"test");
		DirectoryName nullDirName = null!;

		Assert.ThrowsExactly<ArgumentNullException>(() => absoluteDir / nullDirName);
		Assert.ThrowsExactly<ArgumentNullException>(() => genericDir / nullDirName);
	}

	[TestMethod]
	public void DirectoryNameOperator_WithSpecialCharacters_HandlesCorrectly()
	{
		// Test DirectoryName with valid special characters
		DirectoryPath baseDir = DirectoryPath.Create<DirectoryPath>(@"projects");
		DirectoryName specialDir = DirectoryName.Create<DirectoryName>("my-app_v2 (beta)");

		DirectoryPath result = baseDir / specialDir;

		Assert.IsNotNull(result);
		Assert.IsTrue(result.IsValid());
		Assert.Contains("my-app_v2 (beta)", result.WeakString);
	}
}
