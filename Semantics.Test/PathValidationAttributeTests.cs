// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PathValidationAttributeTests
{
	[TestMethod]
	public void IsPathAttribute_ValidPath_ShouldPass()
	{
		// Arrange
		TestPath validPath = TestPath.Create<TestPath>("C:\\valid\\path");

		// Act & Assert
		Assert.IsTrue(validPath.IsValid());
	}

	[TestMethod]
	public void IsPathAttribute_EmptyPath_ShouldPass()
	{
		// Arrange
		TestPath emptyPath = TestPath.Create<TestPath>("");

		// Act & Assert
		Assert.IsTrue(emptyPath.IsValid());
	}

	[TestMethod]
	public void IsPathAttribute_PathWithInvalidChars_ShouldFail()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			TestPath.Create<TestPath>("C:\\invalid<>path"));
	}

	[TestMethod]
	public void IsPathAttribute_ExcessivelyLongPath_ShouldFail()
	{
		// Arrange
		string longPath = "C:\\" + new string('a', 300);

		// Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			TestPath.Create<TestPath>(longPath));
	}

	[TestMethod]
	public void IsAbsolutePathAttribute_WithAbsolutePath_ShouldPass()
	{
		// Arrange
		TestAbsolutePath absolutePath = TestAbsolutePath.Create<TestAbsolutePath>("C:\\test\\path");

		// Act & Assert
		Assert.IsTrue(absolutePath.IsValid());
	}

	[TestMethod]
	public void IsAbsolutePathAttribute_WithRelativePath_ShouldFail()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			TestAbsolutePath.Create<TestAbsolutePath>("relative\\path"));
	}

	[TestMethod]
	public void IsAbsolutePathAttribute_WithUNCPath_ShouldPass()
	{
		// Arrange
		TestAbsolutePath uncPath = TestAbsolutePath.Create<TestAbsolutePath>("\\\\server\\share\\file");

		// Act & Assert
		Assert.IsTrue(uncPath.IsValid());
	}

	[TestMethod]
	public void IsAbsolutePathAttribute_EmptyPath_ShouldPass()
	{
		// Arrange
		TestAbsolutePath emptyPath = TestAbsolutePath.Create<TestAbsolutePath>("");

		// Act & Assert
		Assert.IsTrue(emptyPath.IsValid());
	}

	[TestMethod]
	public void IsRelativePathAttribute_WithRelativePath_ShouldPass()
	{
		// Arrange
		TestRelativePath relativePath = TestRelativePath.Create<TestRelativePath>("relative\\path");

		// Act & Assert
		Assert.IsTrue(relativePath.IsValid());
	}

	[TestMethod]
	public void IsRelativePathAttribute_WithAbsolutePath_ShouldFail()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			TestRelativePath.Create<TestRelativePath>("C:\\absolute\\path"));
	}

	[TestMethod]
	public void IsRelativePathAttribute_WithDotPath_ShouldPass()
	{
		// Arrange
		TestRelativePath dotPath = TestRelativePath.Create<TestRelativePath>("./relative/path");

		// Act & Assert
		Assert.IsTrue(dotPath.IsValid());
	}

	[TestMethod]
	public void IsRelativePathAttribute_WithParentPath_ShouldPass()
	{
		// Arrange
		TestRelativePath parentPath = TestRelativePath.Create<TestRelativePath>("../parent/path");

		// Act & Assert
		Assert.IsTrue(parentPath.IsValid());
	}

	[TestMethod]
	public void IsRelativePathAttribute_EmptyPath_ShouldPass()
	{
		// Arrange
		TestRelativePath emptyPath = TestRelativePath.Create<TestRelativePath>("");

		// Act & Assert
		Assert.IsTrue(emptyPath.IsValid());
	}

	[TestMethod]
	public void IsFileNameAttribute_ValidFileName_ShouldPass()
	{
		// Arrange
		TestFileName fileName = TestFileName.Create<TestFileName>("valid_file_name.txt");

		// Act & Assert
		Assert.IsTrue(fileName.IsValid());
	}

	[TestMethod]
	public void IsFileNameAttribute_FileNameWithInvalidChars_ShouldFail()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			TestFileName.Create<TestFileName>("invalid<>file.txt"));
	}

	[TestMethod]
	public void IsFileNameAttribute_EmptyFileName_ShouldPass()
	{
		// Arrange
		TestFileName emptyFileName = TestFileName.Create<TestFileName>("");

		// Act & Assert
		Assert.IsTrue(emptyFileName.IsValid());
	}

	[TestMethod]
	public void IsDirectoryPathAttribute_NonExistentPath_ShouldPass()
	{
		// Arrange
		TestDirectoryPath directoryPath = TestDirectoryPath.Create<TestDirectoryPath>("C:\\nonexistent\\directory");

		// Act & Assert
		Assert.IsTrue(directoryPath.IsValid());
	}

	[TestMethod]
	public void IsDirectoryPathAttribute_EmptyPath_ShouldPass()
	{
		// Arrange
		TestDirectoryPath emptyPath = TestDirectoryPath.Create<TestDirectoryPath>("");

		// Act & Assert
		Assert.IsTrue(emptyPath.IsValid());
	}

	[TestMethod]
	public void IsFilePathAttribute_NonExistentPath_ShouldPass()
	{
		// Arrange
		TestFilePath filePath = TestFilePath.Create<TestFilePath>("C:\\nonexistent\\file.txt");

		// Act & Assert
		Assert.IsTrue(filePath.IsValid());
	}

	[TestMethod]
	public void IsFilePathAttribute_EmptyPath_ShouldPass()
	{
		// Arrange
		TestFilePath emptyPath = TestFilePath.Create<TestFilePath>("");

		// Act & Assert
		Assert.IsTrue(emptyPath.IsValid());
	}

	[TestMethod]
	public void DoesExistAttribute_NonExistentPath_ShouldFail()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			TestExistingPath.Create<TestExistingPath>("C:\\definitely\\does\\not\\exist"));
	}

	[TestMethod]
	public void DoesExistAttribute_EmptyPath_ShouldFail()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			TestExistingPath.Create<TestExistingPath>(""));
	}

	[TestMethod]
	public void IsExtensionAttribute_ValidExtension_ShouldPass()
	{
		// Arrange
		TestExtension extension = TestExtension.Create<TestExtension>(".txt");

		// Act & Assert
		Assert.IsTrue(extension.IsValid());
	}

	[TestMethod]
	public void IsExtensionAttribute_ExtensionWithoutDot_ShouldFail()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			TestExtension.Create<TestExtension>("txt"));
	}

	[TestMethod]
	public void IsExtensionAttribute_EmptyExtension_ShouldPass()
	{
		// Arrange
		TestExtension emptyExtension = TestExtension.Create<TestExtension>("");

		// Act & Assert
		Assert.IsTrue(emptyExtension.IsValid());
	}

	[TestMethod]
	public void IsExtensionAttribute_MultipleDotsExtension_ShouldPass()
	{
		// Arrange
		TestExtension multiExtension = TestExtension.Create<TestExtension>(".tar.gz");

		// Act & Assert
		Assert.IsTrue(multiExtension.IsValid());
	}

	// Platform-specific tests
	[TestMethod]
	public void IsAbsolutePathAttribute_UnixStylePath_ShouldPassOnUnix()
	{
		if (!OperatingSystem.IsWindows())
		{
			// Arrange
			TestAbsolutePath unixPath = TestAbsolutePath.Create<TestAbsolutePath>("/usr/local/bin");

			// Act & Assert
			Assert.IsTrue(unixPath.IsValid());
		}
	}

	[TestMethod]
	public void IsRelativePathAttribute_UnixStyleRelativePath_ShouldPass()
	{
		// Arrange
		TestRelativePath unixRelativePath = TestRelativePath.Create<TestRelativePath>("usr/local/bin");

		// Act & Assert
		Assert.IsTrue(unixRelativePath.IsValid());
	}

	// Edge case tests for boundary conditions
	[TestMethod]
	public void IsPathAttribute_PathAtExactLimit_ShouldPass()
	{
		// Arrange - create a path at exactly 256 characters
		string exactLimitPath = "C:\\" + new string('a', 252); // 2 for C:\ + 252 = 254, add 2 more for file

		// Act
		TestPath path = TestPath.Create<TestPath>(exactLimitPath);

		// Assert
		Assert.IsTrue(path.IsValid());
	}

	[TestMethod]
	public void IsPathAttribute_PathOverLimit_ShouldFail()
	{
		// Arrange - create a path over 256 characters
		string overLimitPath = "C:\\" + new string('a', 255); // This will be over 256

		// Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			TestPath.Create<TestPath>(overLimitPath));
	}

	[TestMethod]
	public void IsFileNameAttribute_FileNameWithValidSpecialChars_ShouldPass()
	{
		// Arrange - use valid special characters in filename
		TestFileName fileName = TestFileName.Create<TestFileName>("file-name_with.special-chars.txt");

		// Act & Assert
		Assert.IsTrue(fileName.IsValid());
	}
}

// Test record types for validation attribute testing
[IsPath]
public record TestPath : SemanticString<TestPath> { }

[IsAbsolutePath]
public record TestAbsolutePath : SemanticString<TestAbsolutePath> { }

[IsRelativePath]
public record TestRelativePath : SemanticString<TestRelativePath> { }

[IsFileName]
public record TestFileName : SemanticString<TestFileName> { }

[IsDirectoryPath]
public record TestDirectoryPath : SemanticString<TestDirectoryPath> { }

[IsFilePath]
public record TestFilePath : SemanticString<TestFilePath> { }

[DoesExist]
public record TestExistingPath : SemanticString<TestExistingPath> { }

[IsExtension]
public record TestExtension : SemanticString<TestExtension> { }
