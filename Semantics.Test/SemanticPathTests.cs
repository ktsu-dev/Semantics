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
}
