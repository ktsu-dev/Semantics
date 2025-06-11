// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SemanticPathTests
{
	[TestMethod]
	public void SemanticPath_BasicUsage()
	{
		// Test basic path creation and string conversion
		AbsolutePath path = AbsolutePath.Create<AbsolutePath>("C:\\test\\path");
		Assert.IsNotNull(path);
		Assert.AreEqual("C:\\test\\path", path.ToString());
	}

	[TestMethod]
	public void SemanticAbsolutePath_WithAbsolutePath_ShouldBeValid()
	{
		// Arrange & Act
		AbsolutePath path = AbsolutePath.Create<AbsolutePath>("C:\\test\\path");

		// Assert
		Assert.IsTrue(path.IsValid());
	}

	[TestMethod]
	public void SemanticAbsolutePath_WithRelativePath_ShouldThrowException()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			AbsolutePath.Create<AbsolutePath>("test\\path"));
	}

	[TestMethod]
	public void SemanticRelativePath_WithRelativePath_ShouldBeValid()
	{
		// Arrange & Act
		RelativePath path = RelativePath.Create<RelativePath>("test\\path");

		// Assert
		Assert.IsTrue(path.IsValid());
	}

	[TestMethod]
	public void SemanticRelativePath_WithAbsolutePath_ShouldThrowException()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			RelativePath.Create<RelativePath>("C:\\test\\path"));
	}

	[TestMethod]
	public void SemanticFilePath_FileExtension_ShouldReturnCorrectExtension()
	{
		// Arrange
		FilePath filePath = FilePath.Create<FilePath>("test.txt");

		// Act
		FileExtension extension = filePath.FileExtension;

		// Assert
		Assert.AreEqual(".txt", extension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FileExtension_NoExtension_ShouldReturnEmpty()
	{
		// Arrange
		FilePath filePath = FilePath.Create<FilePath>("test");

		// Act
		FileExtension extension = filePath.FileExtension;

		// Assert
		Assert.AreEqual("", extension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FullFileExtension_MultipleExtensions_ShouldReturnAll()
	{
		// Arrange
		FilePath filePath = FilePath.Create<FilePath>("test.tar.gz");

		// Act
		FileExtension fullExtension = filePath.FullFileExtension;

		// Assert
		Assert.AreEqual(".tar.gz", fullExtension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FileName_ShouldReturnCorrectFileName()
	{
		// Arrange
		FilePath filePath = FilePath.Create<FilePath>("C:\\folder\\test.txt");

		// Act
		FileName fileName = filePath.FileName;

		// Assert
		Assert.AreEqual("test.txt", fileName.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_DirectoryPath_ShouldReturnCorrectDirectory()
	{
		// Arrange
		FilePath filePath = FilePath.Create<FilePath>("C:\\folder\\test.txt");

		// Act
		DirectoryPath directoryPath = filePath.DirectoryPath;

		// Assert
		Assert.AreEqual("C:\\folder", directoryPath.ToString());
	}

	[TestMethod]
	public void SemanticFileName_WithValidFileName_ShouldBeValid()
	{
		// Arrange & Act
		FileName fileName = FileName.Create<FileName>("test.txt");

		// Assert
		Assert.IsTrue(fileName.IsValid());
	}

	[TestMethod]
	public void SemanticFileName_WithInvalidChars_ShouldThrowException()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			FileName.Create<FileName>("test<>.txt"));
	}

	[TestMethod]
	public void SemanticFileExtension_WithValidExtension_ShouldBeValid()
	{
		// Arrange & Act
		FileExtension extension = FileExtension.Create<FileExtension>(".txt");

		// Assert
		Assert.IsTrue(extension.IsValid());
	}

	[TestMethod]
	public void SemanticFileExtension_WithoutDot_ShouldThrowException()
	{
		// Arrange & Act & Assert
		Assert.ThrowsExactly<FormatException>(() =>
			FileExtension.Create<FileExtension>("txt"));
	}

	[TestMethod]
	public void SemanticPath_NormalizePath()
	{
		// Test path normalization with mixed separators
		AbsolutePath path = AbsolutePath.Create<AbsolutePath>("C:/test\\path/");
		Assert.IsNotNull(path);
		// Path should be normalized regardless of input format
		Assert.IsTrue(path.ToString().Contains("test") && path.ToString().Contains("path"));
	}

	[TestMethod]
	public void SemanticPath_NonExistentPath()
	{
		// Test that non-existent paths can be created but marked appropriately
		AbsolutePath path = AbsolutePath.Create<AbsolutePath>("C:\\nonexistent\\path");
		Assert.IsNotNull(path);
		Assert.AreEqual("C:\\nonexistent\\path", path.ToString());
	}

	[TestMethod]
	public void SemanticPath_RootPath()
	{
		// Test root path creation
		AbsolutePath rootPath = AbsolutePath.Create<AbsolutePath>("C:\\");
		Assert.IsNotNull(rootPath);
		Assert.AreEqual("C:\\", rootPath.ToString());
	}

	[TestMethod]
	public void SemanticPath_RootPath_Unix()
	{
		// Test Unix root path
		AbsolutePath rootPath = AbsolutePath.Create<AbsolutePath>("/");
		Assert.IsNotNull(rootPath);

		// On Windows, the MakeCanonical method converts "/" to "\"
		// On Unix systems, it should remain "/"
		if (OperatingSystem.IsWindows())
		{
			Assert.AreEqual("\\", rootPath.ToString());
		}
		else
		{
			Assert.AreEqual("/", rootPath.ToString());
		}
	}

	[TestMethod]
	public void SemanticPath_PathTypes()
	{
		// Test different path type recognition with relative path
		RelativePath relativePath = RelativePath.Create<RelativePath>("folder/subfolder\\file");
		Assert.IsNotNull(relativePath);
		// These methods should exist based on the path type
		Assert.IsTrue(relativePath.IsValid());

		// Test with absolute path
		AbsolutePath absolutePath = AbsolutePath.Create<AbsolutePath>("C:\\folder/subfolder\\file");
		Assert.IsNotNull(absolutePath);
		Assert.IsTrue(absolutePath.IsValid());
	}

	[TestMethod]
	public void SemanticPath_EmptyPath_ShouldBeValid()
	{
		// Test that empty paths are considered valid per the IsPath attribute
		AbsolutePath emptyPath = AbsolutePath.Create<AbsolutePath>("");
		Assert.IsTrue(emptyPath.IsValid());
		Assert.AreEqual("", emptyPath.ToString());
	}

	[TestMethod]
	public void SemanticPath_PathLength_Long()
	{
		// Test long but valid path
		string longButValidPath = "C:\\" + string.Join("\\", Enumerable.Repeat("folder", 20));
		AbsolutePath path = AbsolutePath.Create<AbsolutePath>(longButValidPath);
		Assert.IsNotNull(path);
		Assert.AreEqual(longButValidPath, path.ToString());
	}

	[TestMethod]
	public void SemanticPath_PathLength_TooLong()
	{
		// Test excessively long path (over typical OS limits)
		string excessivelyLongPath = "C:\\" + string.Join("\\", Enumerable.Repeat("verylongfoldernamethatexceedstypicallimits", 50));
		Assert.ThrowsExactly<FormatException>(() =>
			AbsolutePath.Create<AbsolutePath>(excessivelyLongPath));
	}

	[TestMethod]
	public void SemanticRelativePath_Make_ShouldCreateCorrectRelativePath()
	{
		// Arrange
		AbsolutePath from = AbsolutePath.Create<AbsolutePath>("C:\\base\\folder");
		AbsolutePath to = AbsolutePath.Create<AbsolutePath>("C:\\base\\other\\file.txt");

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
			AbsolutePath rootPath = AbsolutePath.Create<AbsolutePath>("C:\\");
			Assert.AreEqual("C:\\", rootPath.ToString());
		}
		else
		{
			// On Unix-like systems, test root path
			AbsolutePath rootPath = AbsolutePath.Create<AbsolutePath>("/");
			Assert.AreEqual("/", rootPath.ToString());
		}
	}

	[TestMethod]
	public void SemanticPath_MakeCanonical_WithMixedSeparators_ShouldNormalize()
	{
		// Test path with mixed separators - use absolute path
		AbsolutePath path = AbsolutePath.Create<AbsolutePath>("C:/folder/subfolder\\file");
		string expected = "C:" + Path.DirectorySeparatorChar + "folder" + Path.DirectorySeparatorChar + "subfolder" + Path.DirectorySeparatorChar + "file";
		Assert.AreEqual(expected, path.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FileExtension_WithMultipleDots_ShouldReturnLastExtension()
	{
		// Test file with multiple dots in the name
		FilePath filePath = FilePath.Create<FilePath>("file.backup.v2.txt");
		FileExtension extension = filePath.FileExtension;
		Assert.AreEqual(".txt", extension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FullFileExtension_WithSingleExtension_ShouldReturnSameAsFileExtension()
	{
		// Test that FullFileExtension works correctly with single extensions
		FilePath filePath = FilePath.Create<FilePath>("document.pdf");
		FileExtension fullExtension = filePath.FullFileExtension;
		Assert.AreEqual(".pdf", fullExtension.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_FileName_WithPathSeparators_ShouldReturnOnlyFileName()
	{
		// Test filename extraction from complex paths
		FilePath filePath = FilePath.Create<FilePath>("C:\\very\\deep\\folder\\structure\\document.docx");
		FileName fileName = filePath.FileName;
		Assert.AreEqual("document.docx", fileName.ToString());
	}

	[TestMethod]
	public void SemanticFilePath_DirectoryPath_WithRootFile_ShouldReturnRootDirectory()
	{
		// Test directory extraction when file is in root
		if (OperatingSystem.IsWindows())
		{
			FilePath filePath = FilePath.Create<FilePath>("C:\\file.txt");
			DirectoryPath directoryPath = filePath.DirectoryPath;
			Assert.AreEqual("C:\\", directoryPath.ToString());
		}
	}

	[TestMethod]
	public void SemanticFilePath_DirectoryPath_WithEmptyResult_ShouldReturnEmpty()
	{
		// Test when GetDirectoryName returns null/empty
		FilePath filePath = FilePath.Create<FilePath>("file.txt");
		DirectoryPath directoryPath = filePath.DirectoryPath;
		Assert.AreEqual("", directoryPath.ToString());
	}

	[TestMethod]
	public void SemanticRelativePath_Make_WithNullArguments_ShouldThrowArgumentNullException()
	{
		// Test null argument handling
		AbsolutePath validPath = AbsolutePath.Create<AbsolutePath>("C:\\test");

		Assert.ThrowsExactly<ArgumentNullException>(() =>
			RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(null!, validPath));

		Assert.ThrowsExactly<ArgumentNullException>(() =>
			RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(validPath, null!));
	}

	[TestMethod]
	public void SemanticRelativePath_Make_WithDirectoryPaths_ShouldHandleCorrectly()
	{
		// Test relative path creation between directories
		AbsolutePath from = AbsolutePath.Create<AbsolutePath>("C:\\base\\folder1");
		AbsolutePath to = AbsolutePath.Create<AbsolutePath>("C:\\base\\folder2");

		RelativePath relativePath = RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(from, to);
		Assert.IsNotNull(relativePath);
		Assert.IsTrue(relativePath.IsValid());
	}

	[TestMethod]
	public void SemanticFileName_EmptyFileName_ShouldBeValid()
	{
		// Test that empty filenames are valid per the IsFileName attribute
		FileName emptyFileName = FileName.Create<FileName>("");
		Assert.IsTrue(emptyFileName.IsValid());
	}

	[TestMethod]
	public void SemanticFileExtension_EmptyExtension_ShouldBeValid()
	{
		// Test that empty extensions are valid (no extension case)
		FileExtension emptyExtension = FileExtension.Create<FileExtension>("");
		Assert.IsTrue(emptyExtension.IsValid());
	}
}
