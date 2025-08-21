// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Paths;

using System;
using ktsu.Semantics;
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
		RelativeDirectoryPath relativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"test");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>(@"test");

		RelativeDirectoryPath nullRelativeDir = null!;
		RelativeFilePath nullRelativeFile = null!;
		FileName nullFileName = null!;

		// AbsoluteDirectoryPath operators
		Assert.ThrowsExactly<ArgumentNullException>(() => absoluteDir / nullRelativeDir);
		Assert.ThrowsExactly<ArgumentNullException>(() => absoluteDir / nullRelativeFile);
		Assert.ThrowsExactly<ArgumentNullException>(() => absoluteDir / nullFileName);

		AbsoluteDirectoryPath nullAbsoluteDir = null!;
		Assert.ThrowsExactly<ArgumentNullException>(() => nullAbsoluteDir / relativeDir);

		// RelativeDirectoryPath operators
		Assert.ThrowsExactly<ArgumentNullException>(() => relativeDir / nullRelativeDir);
		Assert.ThrowsExactly<ArgumentNullException>(() => relativeDir / nullRelativeFile);
		Assert.ThrowsExactly<ArgumentNullException>(() => relativeDir / nullFileName);

		RelativeDirectoryPath nullRelativeDir2 = null!;
		Assert.ThrowsExactly<ArgumentNullException>(() => nullRelativeDir2 / relativeDir);

		// DirectoryPath operators
		Assert.ThrowsExactly<ArgumentNullException>(() => genericDir / nullRelativeDir);
		Assert.ThrowsExactly<ArgumentNullException>(() => genericDir / nullRelativeFile);
		Assert.ThrowsExactly<ArgumentNullException>(() => genericDir / nullFileName);

		DirectoryPath nullGenericDir = null!;
		Assert.ThrowsExactly<ArgumentNullException>(() => nullGenericDir / relativeDir);
	}

	[TestMethod]
	public void PathOperators_ComplexCombinations_WorkCorrectly()
	{
		// Test complex path combinations
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		RelativeDirectoryPath subDir1 = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"app\src");
		RelativeDirectoryPath subDir2 = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"components");
		FileName fileName = FileName.Create<FileName>("Component.tsx");

		// Chain multiple operations
		AbsoluteDirectoryPath combinedDir = baseDir / subDir1 / subDir2;
		AbsoluteFilePath finalFile = combinedDir / fileName;

		Assert.IsNotNull(combinedDir);
		Assert.IsNotNull(finalFile);
		Assert.Contains(@"C:\projects", finalFile.WeakString);
		Assert.Contains(@"app\src", finalFile.WeakString);
		Assert.Contains(@"components", finalFile.WeakString);
		Assert.Contains("Component.tsx", finalFile.WeakString);
	}

	[TestMethod]
	public void PathOperators_EmptyPaths_HandleCorrectly()
	{
		// Test operators with empty paths
		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");
		RelativeDirectoryPath emptyRelativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("");
		RelativeFilePath emptyRelativeFile = RelativeFilePath.Create<RelativeFilePath>("");
		FileName emptyFileName = FileName.Create<FileName>("");

		// These should work without throwing
		AbsoluteDirectoryPath result1 = absoluteDir / emptyRelativeDir;
		AbsoluteFilePath result2 = absoluteDir / emptyRelativeFile;
		AbsoluteFilePath result3 = absoluteDir / emptyFileName;

		Assert.IsNotNull(result1);
		Assert.IsNotNull(result2);
		Assert.IsNotNull(result3);
	}

	[TestMethod]
	public void PathOperators_SpecialCharacters_HandleCorrectly()
	{
		// Test with paths containing special characters
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test folder");
		RelativeDirectoryPath specialDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"sub folder (1)");
		FileName specialFile = FileName.Create<FileName>("file name with spaces.txt");

		AbsoluteDirectoryPath combinedDir = baseDir / specialDir;
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
		RelativeDirectoryPath relativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"sub");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>(@"test");
		RelativeFilePath relativeFile = RelativeFilePath.Create<RelativeFilePath>(@"file.txt");
		FileName fileName = FileName.Create<FileName>("file.txt");

		// AbsoluteDirectoryPath combinations
		AbsoluteDirectoryPath absDir = absoluteDir / relativeDir;
		AbsoluteFilePath absFile1 = absoluteDir / relativeFile;
		AbsoluteFilePath absFile2 = absoluteDir / fileName;

		// RelativeDirectoryPath combinations
		RelativeDirectoryPath relDir = relativeDir / relativeDir;
		RelativeFilePath relFile1 = relativeDir / relativeFile;
		RelativeFilePath relFile2 = relativeDir / fileName;

		// DirectoryPath combinations
		DirectoryPath genDir = genericDir / relativeDir;
		FilePath genFile1 = genericDir / relativeFile;
		FilePath genFile2 = genericDir / fileName;

		// Verify types
		Assert.IsInstanceOfType<AbsoluteDirectoryPath>(absDir);
		Assert.IsInstanceOfType<AbsoluteFilePath>(absFile1);
		Assert.IsInstanceOfType<AbsoluteFilePath>(absFile2);
		Assert.IsInstanceOfType<RelativeDirectoryPath>(relDir);
		Assert.IsInstanceOfType<RelativeFilePath>(relFile1);
		Assert.IsInstanceOfType<RelativeFilePath>(relFile2);
		Assert.IsInstanceOfType<DirectoryPath>(genDir);
		Assert.IsInstanceOfType<FilePath>(genFile1);
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
}
