// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SemanticPathTests
{
	[TestMethod]
	public void SemanticPath_BasicCreation_ShouldWork()
	{
		// Arrange & Act
		Path path = Path.FromString<Path>("C:\\test\\path");

		// Assert
		Assert.IsNotNull(path);
		Assert.AreEqual("C:\\test\\path", path.ToString());
	}

	[TestMethod]
	public void SemanticAbsolutePath_WithAbsolutePath_ShouldBeValid()
	{
		// Arrange & Act
		AbsolutePath path = AbsolutePath.FromString<AbsolutePath>("C:\\test\\path");

		// Assert
		Assert.IsTrue(path.IsValid());
	}

	[TestMethod]
	public void SemanticAbsolutePath_WithRelativePath_ShouldThrowException()
	{
		// Arrange & Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			AbsolutePath.FromString<AbsolutePath>("test\\path"));
	}

	[TestMethod]
	public void SemanticRelativePath_WithRelativePath_ShouldBeValid()
	{
		// Arrange & Act
		RelativePath path = RelativePath.FromString<RelativePath>("test\\path");

		// Assert
		Assert.IsTrue(path.IsValid());
	}

	[TestMethod]
	public void SemanticRelativePath_WithAbsolutePath_ShouldThrowException()
	{
		// Arrange & Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			RelativePath.FromString<RelativePath>("C:\\test\\path"));
	}

	[TestMethod]
	public void SemanticFilePath_FileExtension_ShouldReturnCorrectExtension()
	{
		// Arrange
		FilePath filePath = FilePath.FromString<FilePath>("test.txt");

		// Act
		FileExtension extension = filePath.FileExtension;

		// Assert
		Assert.AreEqual(".txt", extension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FileExtension_NoExtension_ShouldReturnEmpty()
	{
		// Arrange
		FilePath filePath = FilePath.FromString<FilePath>("test");

		// Act
		FileExtension extension = filePath.FileExtension;

		// Assert
		Assert.AreEqual("", extension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FullFileExtension_MultipleExtensions_ShouldReturnAll()
	{
		// Arrange
		FilePath filePath = FilePath.FromString<FilePath>("test.tar.gz");

		// Act
		FileExtension fullExtension = filePath.FullFileExtension;

		// Assert
		Assert.AreEqual(".tar.gz", fullExtension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FileName_ShouldReturnCorrectFileName()
	{
		// Arrange
		FilePath filePath = FilePath.FromString<FilePath>("C:\\folder\\test.txt");

		// Act
		FileName fileName = filePath.FileName;

		// Assert
		Assert.AreEqual("test.txt", fileName.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_DirectoryPath_ShouldReturnCorrectDirectory()
	{
		// Arrange
		FilePath filePath = FilePath.FromString<FilePath>("C:\\folder\\test.txt");

		// Act
		DirectoryPath directoryPath = filePath.DirectoryPath;

		// Assert
		Assert.AreEqual("C:\\folder", directoryPath.ToString());
	}

	[TestMethod]
	public void SemanticFileName_WithValidFileName_ShouldBeValid()
	{
		// Arrange & Act
		FileName fileName = FileName.FromString<FileName>("test.txt");

		// Assert
		Assert.IsTrue(fileName.IsValid());
	}

	[TestMethod]
	public void SemanticFileName_WithInvalidChars_ShouldThrowException()
	{
		// Arrange & Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			FileName.FromString<FileName>("test<>.txt"));
	}

	[TestMethod]
	public void SemanticFileExtension_WithValidExtension_ShouldBeValid()
	{
		// Arrange & Act
		FileExtension extension = FileExtension.FromString<FileExtension>(".txt");

		// Assert
		Assert.IsTrue(extension.IsValid());
	}

	[TestMethod]
	public void SemanticFileExtension_WithoutDot_ShouldThrowException()
	{
		// Arrange & Act & Assert
		Assert.ThrowsException<FormatException>(() =>
			FileExtension.FromString<FileExtension>("txt"));
	}

	[TestMethod]
	public void SemanticPath_MakeCanonical_ShouldNormalizeSeparators()
	{
		// Arrange & Act
		Path path = Path.FromString<Path>("C:/test\\path/");

		// Assert
		// Should normalize to platform-specific separators and remove trailing separator
		string expected = "C:" + System.IO.Path.DirectorySeparatorChar + "test" + System.IO.Path.DirectorySeparatorChar + "path";
		Assert.AreEqual(expected, path.ToString());
	}

	[TestMethod]
	public void SemanticPath_Exists_WithNonExistentPath_ShouldReturnFalse()
	{
		// Arrange
		Path path = Path.FromString<Path>("C:\\nonexistent\\path");

		// Act & Assert
		Assert.IsFalse(path.Exists);
		Assert.IsFalse(path.IsDirectory);
		Assert.IsFalse(path.IsFile);
	}

	[TestMethod]
	public void SemanticRelativePath_Make_ShouldCreateCorrectRelativePath()
	{
		// Arrange
		AbsolutePath from = AbsolutePath.FromString<AbsolutePath>("C:\\base\\folder");
		AbsolutePath to = AbsolutePath.FromString<AbsolutePath>("C:\\base\\other\\file.txt");

		// Act
		RelativePath relativePath = RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(from, to);

		// Assert
		Assert.IsNotNull(relativePath);
		// The exact result depends on the platform, but it should be a valid relative path
		Assert.IsTrue(relativePath.IsValid());
	}

	// Additional comprehensive tests for edge cases and missing coverage
	[TestMethod]
	public void SemanticPath_MakeCanonical_WithRootPath_ShouldPreserveTrailingSeparator()
	{
		// This test checks that root paths like "C:\" keep their trailing separator
		if (OperatingSystem.IsWindows())
		{
			// On Windows, test root drive paths
			Path rootPath = Path.FromString<Path>("C:\\");
			Assert.AreEqual("C:\\", rootPath.ToString());
		}
		else
		{
			// On Unix-like systems, test root path
			Path rootPath = Path.FromString<Path>("/");
			Assert.AreEqual("/", rootPath.ToString());
		}
	}

	[TestMethod]
	public void SemanticPath_MakeCanonical_WithMixedSeparators_ShouldNormalize()
	{
		// Test path with mixed separators
		Path path = Path.FromString<Path>("folder/subfolder\\file");
		string expected = "folder" + System.IO.Path.DirectorySeparatorChar + "subfolder" + System.IO.Path.DirectorySeparatorChar + "file";
		Assert.AreEqual(expected, path.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FileExtension_WithMultipleDots_ShouldReturnLastExtension()
	{
		// Test file with multiple dots in the name
		FilePath filePath = FilePath.FromString<FilePath>("file.backup.v2.txt");
		FileExtension extension = filePath.FileExtension;
		Assert.AreEqual(".txt", extension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FullFileExtension_WithSingleExtension_ShouldReturnSameAsFileExtension()
	{
		// Test that FullFileExtension works correctly with single extensions
		FilePath filePath = FilePath.FromString<FilePath>("document.pdf");
		FileExtension fullExtension = filePath.FullFileExtension;
		Assert.AreEqual(".pdf", fullExtension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FileName_WithPathSeparators_ShouldReturnOnlyFileName()
	{
		// Test filename extraction from complex paths
		FilePath filePath = FilePath.FromString<FilePath>("C:\\very\\deep\\folder\\structure\\document.docx");
		FileName fileName = filePath.FileName;
		Assert.AreEqual("document.docx", fileName.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_DirectoryPath_WithRootFile_ShouldReturnRootDirectory()
	{
		// Test directory extraction when file is in root
		if (OperatingSystem.IsWindows())
		{
			FilePath filePath = FilePath.FromString<FilePath>("C:\\file.txt");
			DirectoryPath directoryPath = filePath.DirectoryPath;
			Assert.AreEqual("C:\\", directoryPath.ToString());
		}
	}

	[TestMethod]
	public void SemanticFilePath_DirectoryPath_WithEmptyResult_ShouldReturnEmpty()
	{
		// Test when GetDirectoryName returns null/empty
		FilePath filePath = FilePath.FromString<FilePath>("file.txt");
		DirectoryPath directoryPath = filePath.DirectoryPath;
		Assert.AreEqual("", directoryPath.ToString());
	}

	[TestMethod]
	public void SemanticRelativePath_Make_WithNullArguments_ShouldThrowArgumentNullException()
	{
		// Test null argument handling
		AbsolutePath validPath = AbsolutePath.FromString<AbsolutePath>("C:\\test");

		Assert.ThrowsException<ArgumentNullException>(() =>
			RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(null!, validPath));

		Assert.ThrowsException<ArgumentNullException>(() =>
			RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(validPath, null!));
	}

	[TestMethod]
	public void SemanticRelativePath_Make_WithDirectoryPaths_ShouldHandleCorrectly()
	{
		// Test relative path creation between directories
		AbsolutePath from = AbsolutePath.FromString<AbsolutePath>("C:\\base\\folder1");
		AbsolutePath to = AbsolutePath.FromString<AbsolutePath>("C:\\base\\folder2");

		RelativePath relativePath = RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(from, to);
		Assert.IsNotNull(relativePath);
		Assert.IsTrue(relativePath.IsValid());
	}

	[TestMethod]
	public void SemanticPath_EmptyPath_ShouldBeValid()
	{
		// Test that empty paths are considered valid per the IsPath attribute
		Path emptyPath = Path.FromString<Path>("");
		Assert.IsTrue(emptyPath.IsValid());
		Assert.AreEqual("", emptyPath.ToString());
	}

	[TestMethod]
	public void SemanticAbsolutePath_EmptyPath_ShouldBeValid()
	{
		// Test that empty absolute paths are valid per the IsAbsolutePath attribute
		AbsolutePath emptyPath = AbsolutePath.FromString<AbsolutePath>("");
		Assert.IsTrue(emptyPath.IsValid());
	}

	[TestMethod]
	public void SemanticRelativePath_EmptyPath_ShouldBeValid()
	{
		// Test that empty relative paths are valid per the IsRelativePath attribute
		RelativePath emptyPath = RelativePath.FromString<RelativePath>("");
		Assert.IsTrue(emptyPath.IsValid());
	}

	[TestMethod]
	public void SemanticPath_Exists_EmptyPath_ShouldReturnFalse()
	{
		// Test filesystem existence check for empty path
		Path emptyPath = Path.FromString<Path>("");
		Assert.IsFalse(emptyPath.Exists);
		Assert.IsFalse(emptyPath.IsDirectory);
		Assert.IsFalse(emptyPath.IsFile);
	}

	[TestMethod]
	public void SemanticPath_WithValidLength_ShouldPass()
	{
		// Test path length validation (should pass for reasonable lengths)
		string longButValidPath = "C:\\" + new string('a', 200) + "\\file.txt";
		Path path = Path.FromString<Path>(longButValidPath);
		Assert.IsTrue(path.IsValid());
	}

	[TestMethod]
	public void SemanticPath_WithExcessiveLength_ShouldFail()
	{
		// Test path length validation (should fail for paths over 256 characters)
		string excessivelyLongPath = "C:\\" + new string('a', 300) + "\\file.txt";
		Assert.ThrowsException<FormatException>(() =>
			Path.FromString<Path>(excessivelyLongPath));
	}

	[TestMethod]
	public void SemanticFileName_EmptyFileName_ShouldBeValid()
	{
		// Test that empty filenames are valid per the IsFileName attribute
		FileName emptyFileName = FileName.FromString<FileName>("");
		Assert.IsTrue(emptyFileName.IsValid());
	}

	[TestMethod]
	public void SemanticFileExtension_EmptyExtension_ShouldBeValid()
	{
		// Test that empty extensions are valid (no extension case)
		FileExtension emptyExtension = FileExtension.FromString<FileExtension>("");
		Assert.IsTrue(emptyExtension.IsValid());
	}
}
