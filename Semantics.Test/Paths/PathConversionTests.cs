// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Paths;

using System;
using System.IO;
using ktsu.Semantics.Paths;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PathConversionTests
{
	[TestMethod]
	public void AsAbsolute_WithNullBaseDirectory_ThrowsArgumentNullException()
	{
		// Test all AsAbsolute(baseDirectory) methods with null base
		RelativeFilePath relativeFile = RelativeFilePath.Create<RelativeFilePath>("test.txt");
		RelativeDirectoryPath relativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("test");
		FilePath genericFile = FilePath.Create<FilePath>("test.txt");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>("test");

		Assert.ThrowsExactly<ArgumentNullException>(() => relativeFile.AsAbsolute(null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => relativeDir.AsAbsolute(null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => genericFile.AsRelative(null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => genericDir.AsRelative(null!));
	}

	[TestMethod]
	public void AsRelative_WithNullBaseDirectory_ThrowsArgumentNullException()
	{
		// Test all AsRelative(baseDirectory) methods with null base
		AbsoluteFilePath absoluteFile = AbsoluteFilePath.Create<AbsoluteFilePath>(@"C:\test\file.txt");
		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");

		Assert.ThrowsExactly<ArgumentNullException>(() => absoluteFile.AsRelative(null!));
		Assert.ThrowsExactly<ArgumentNullException>(() => absoluteDir.AsRelative(null!));
	}

	[TestMethod]
	public void AsAbsolute_WithCurrentWorkingDirectory_WorksCorrectly()
	{
		// Test AsAbsolute() without base directory (uses current working directory)
		RelativeFilePath relativeFile = RelativeFilePath.Create<RelativeFilePath>("test.txt");
		RelativeDirectoryPath relativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("test");
		FilePath genericFile = FilePath.Create<FilePath>("test.txt");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>("test");

		AbsoluteFilePath absoluteFile1 = relativeFile.AsAbsolute();
		AbsoluteDirectoryPath absoluteDir1 = relativeDir.AsAbsolute();
		AbsoluteFilePath absoluteFile2 = genericFile.AsAbsolute();
		AbsoluteDirectoryPath absoluteDir2 = genericDir.AsAbsolute();

		Assert.IsNotNull(absoluteFile1);
		Assert.IsNotNull(absoluteDir1);
		Assert.IsNotNull(absoluteFile2);
		Assert.IsNotNull(absoluteDir2);

		// Should be fully qualified paths
		Assert.IsTrue(Path.IsPathFullyQualified(absoluteFile1.WeakString));
		Assert.IsTrue(Path.IsPathFullyQualified(absoluteDir1.WeakString));
		Assert.IsTrue(Path.IsPathFullyQualified(absoluteFile2.WeakString));
		Assert.IsTrue(Path.IsPathFullyQualified(absoluteDir2.WeakString));
	}

	[TestMethod]
	public void AsAbsolute_WithSpecificBaseDirectory_WorksCorrectly()
	{
		// Test AsAbsolute(baseDirectory) with specific base
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		RelativeFilePath relativeFile = RelativeFilePath.Create<RelativeFilePath>(@"app\src\file.ts");
		RelativeDirectoryPath relativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"app\src");

		AbsoluteFilePath absoluteFile = relativeFile.AsAbsolute(baseDir);
		AbsoluteDirectoryPath absoluteDir = relativeDir.AsAbsolute(baseDir);

		Assert.IsNotNull(absoluteFile);
		Assert.IsNotNull(absoluteDir);
		Assert.IsTrue(absoluteFile.WeakString.Contains(@"C:\projects"));
		Assert.IsTrue(absoluteFile.WeakString.Contains(@"app\src\file.ts"));
		Assert.IsTrue(absoluteDir.WeakString.Contains(@"C:\projects"));
		Assert.IsTrue(absoluteDir.WeakString.Contains(@"app\src"));
	}

	[TestMethod]
	public void AsRelative_WithSpecificBaseDirectory_WorksCorrectly()
	{
		// Test AsRelative(baseDirectory) conversion
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		AbsoluteFilePath absoluteFile = AbsoluteFilePath.Create<AbsoluteFilePath>(@"C:\projects\app\src\file.ts");
		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\app\src");

		RelativeFilePath relativeFile = absoluteFile.AsRelative(baseDir);
		RelativeDirectoryPath relativeDir = absoluteDir.AsRelative(baseDir);

		Assert.IsNotNull(relativeFile);
		Assert.IsNotNull(relativeDir);
		Assert.IsTrue(relativeFile.WeakString.Contains("app"));
		Assert.IsTrue(relativeFile.WeakString.Contains("file.ts"));
		Assert.IsTrue(relativeDir.WeakString.Contains("app"));
	}

	[TestMethod]
	public void AsRelative_OnAlreadyRelativePath_ReturnsSelf()
	{
		// Test that AsRelative on already relative paths returns the same instance
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\base");
		RelativeDirectoryPath relativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("test");

		RelativeDirectoryPath result = relativeDir.AsRelative(baseDir);

		Assert.AreSame(relativeDir, result);
	}

	[TestMethod]
	public void AsAbsolute_OnAlreadyAbsolutePath_ReturnsSelf()
	{
		// Test that AsAbsolute on already absolute paths returns the same instance
		AbsoluteFilePath absoluteFile = AbsoluteFilePath.Create<AbsoluteFilePath>(@"C:\test\file.txt");
		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");

		AbsoluteFilePath resultFile = absoluteFile.AsAbsolute();
		AbsoluteDirectoryPath resultDir = absoluteDir.AsAbsolute();

		Assert.AreSame(absoluteFile, resultFile);
		Assert.AreSame(absoluteDir, resultDir);
	}

	[TestMethod]
	public void PathConversion_WithComplexRelativePaths_WorksCorrectly()
	{
		// Test with complex relative paths containing .. and .
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\app\src");
		RelativeDirectoryPath complexRelative = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"..\..\..\other\project");
		RelativeFilePath complexFile = RelativeFilePath.Create<RelativeFilePath>(@"..\..\config\settings.json");

		AbsoluteDirectoryPath absoluteDir = complexRelative.AsAbsolute(baseDir);
		AbsoluteFilePath absoluteFile = complexFile.AsAbsolute(baseDir);

		Assert.IsNotNull(absoluteDir);
		Assert.IsNotNull(absoluteFile);
		Assert.IsTrue(Path.IsPathFullyQualified(absoluteDir.WeakString));
		Assert.IsTrue(Path.IsPathFullyQualified(absoluteFile.WeakString));
	}

	[TestMethod]
	public void PathConversion_RoundTrip_PreservesEquivalence()
	{
		// Test round-trip conversion: absolute -> relative -> absolute
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		AbsoluteFilePath originalFile = AbsoluteFilePath.Create<AbsoluteFilePath>(@"C:\projects\app\file.txt");
		AbsoluteDirectoryPath originalDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects\app");

		// Convert to relative
		RelativeFilePath relativeFile = originalFile.AsRelative(baseDir);
		RelativeDirectoryPath relativeDir = originalDir.AsRelative(baseDir);

		// Convert back to absolute
		AbsoluteFilePath roundTripFile = relativeFile.AsAbsolute(baseDir);
		AbsoluteDirectoryPath roundTripDir = relativeDir.AsAbsolute(baseDir);

		// Should be equivalent (though not necessarily identical due to normalization)
		Assert.IsNotNull(roundTripFile);
		Assert.IsNotNull(roundTripDir);
		Assert.IsTrue(roundTripFile.WeakString.Contains("app"));
		Assert.IsTrue(roundTripFile.WeakString.Contains("file.txt"));
		Assert.IsTrue(roundTripDir.WeakString.Contains("app"));
	}

	[TestMethod]
	public void PathConversion_WithEmptyPaths_HandlesCorrectly()
	{
		// Test conversion with empty paths
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");
		RelativeFilePath emptyFile = RelativeFilePath.Create<RelativeFilePath>("");
		RelativeDirectoryPath emptyDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("");

		AbsoluteFilePath absoluteFile = emptyFile.AsAbsolute(baseDir);
		AbsoluteDirectoryPath absoluteDir = emptyDir.AsAbsolute(baseDir);

		Assert.IsNotNull(absoluteFile);
		Assert.IsNotNull(absoluteDir);
	}

	[TestMethod]
	public void PathConversion_CrossPlatformPaths_HandlesCorrectly()
	{
		// Test conversion with mixed path separators
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		RelativeFilePath unixStyleFile = RelativeFilePath.Create<RelativeFilePath>("app/src/file.js");
		RelativeDirectoryPath unixStyleDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("app/src");

		AbsoluteFilePath absoluteFile = unixStyleFile.AsAbsolute(baseDir);
		AbsoluteDirectoryPath absoluteDir = unixStyleDir.AsAbsolute(baseDir);

		Assert.IsNotNull(absoluteFile);
		Assert.IsNotNull(absoluteDir);
		Assert.IsTrue(absoluteFile.WeakString.Contains("app"));
		Assert.IsTrue(absoluteFile.WeakString.Contains("file.js"));
		Assert.IsTrue(absoluteDir.WeakString.Contains("app"));
	}

	[TestMethod]
	public void AsAbsolute_ReturnsCorrectConcreteTypes()
	{
		// Test that AsAbsolute methods return correct concrete types
		AbsolutePath absolutePath = AbsolutePath.Create<AbsolutePath>(@"C:\test");
		RelativePath relativePath = RelativePath.Create<RelativePath>("test");
		AbsoluteFilePath absoluteFile = AbsoluteFilePath.Create<AbsoluteFilePath>(@"C:\test\file.txt");
		RelativeFilePath relativeFile = RelativeFilePath.Create<RelativeFilePath>("file.txt");
		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\test");
		RelativeDirectoryPath relativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("test");

		// Test AsAbsolute methods
		AbsolutePath result1 = absolutePath.AsAbsolute();
		AbsolutePath result2 = relativePath.AsAbsolute();
		AbsoluteFilePath result3 = absoluteFile.AsAbsolute();
		AbsoluteFilePath result4 = relativeFile.AsAbsolute();
		AbsoluteDirectoryPath result5 = absoluteDir.AsAbsolute();
		AbsoluteDirectoryPath result6 = relativeDir.AsAbsolute();

		// Verify correct types are returned
		Assert.IsInstanceOfType<AbsolutePath>(result1);
		Assert.IsInstanceOfType<AbsolutePath>(result2);
		Assert.IsInstanceOfType<AbsoluteFilePath>(result3);
		Assert.IsInstanceOfType<AbsoluteFilePath>(result4);
		Assert.IsInstanceOfType<AbsoluteDirectoryPath>(result5);
		Assert.IsInstanceOfType<AbsoluteDirectoryPath>(result6);

		// Verify they're not null and valid
		Assert.IsNotNull(result1);
		Assert.IsNotNull(result2);
		Assert.IsNotNull(result3);
		Assert.IsNotNull(result4);
		Assert.IsNotNull(result5);
		Assert.IsNotNull(result6);
	}
}
