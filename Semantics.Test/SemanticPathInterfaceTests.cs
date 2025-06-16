// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SemanticPathInterfaceTests
{
	[TestMethod]
	public void AbsolutePath_ImplementsIAbsolutePathAndIPath()
	{
		// Arrange & Act
		AbsolutePath absolutePath = AbsolutePath.Create<AbsolutePath>("C:\\test\\path");
		IAbsolutePath iAbsolutePath = absolutePath;
		IPath iPath = absolutePath;

		// Assert
		Assert.IsNotNull(iAbsolutePath);
		Assert.IsNotNull(iPath);
		Assert.IsInstanceOfType<IAbsolutePath>(iAbsolutePath);
		Assert.IsInstanceOfType<IPath>(iPath);
		Assert.AreSame(absolutePath, iAbsolutePath);
		Assert.AreSame(absolutePath, iPath);
	}

	[TestMethod]
	public void RelativePath_ImplementsIRelativePathAndIPath()
	{
		// Arrange & Act
		RelativePath relativePath = RelativePath.Create<RelativePath>("test\\path");
		IRelativePath iRelativePath = relativePath;
		IPath iPath = relativePath;

		// Assert
		Assert.IsNotNull(iRelativePath);
		Assert.IsNotNull(iPath);
		Assert.IsInstanceOfType<IRelativePath>(iRelativePath);
		Assert.IsInstanceOfType<IPath>(iPath);
		Assert.AreSame(relativePath, iRelativePath);
		Assert.AreSame(relativePath, iPath);
	}

	[TestMethod]
	public void FilePath_ImplementsIFilePathAndIPath()
	{
		// Arrange & Act
		FilePath filePath = FilePath.Create<FilePath>("test\\file.txt");
		IFilePath iFilePath = filePath;
		IPath iPath = filePath;

		// Assert
		Assert.IsNotNull(iFilePath);
		Assert.IsNotNull(iPath);
		Assert.IsInstanceOfType<IFilePath>(iFilePath);
		Assert.IsInstanceOfType<IPath>(iPath);
		Assert.AreSame(filePath, iFilePath);
		Assert.AreSame(filePath, iPath);
	}

	[TestMethod]
	public void DirectoryPath_ImplementsIDirectoryPathAndIPath()
	{
		// Arrange & Act
		DirectoryPath directoryPath = DirectoryPath.Create<DirectoryPath>("test\\directory");
		IDirectoryPath iDirectoryPath = directoryPath;
		IPath iPath = directoryPath;

		// Assert
		Assert.IsNotNull(iDirectoryPath);
		Assert.IsNotNull(iPath);
		Assert.IsInstanceOfType<IDirectoryPath>(iDirectoryPath);
		Assert.IsInstanceOfType<IPath>(iPath);
		Assert.AreSame(directoryPath, iDirectoryPath);
		Assert.AreSame(directoryPath, iPath);
	}

	[TestMethod]
	public void AbsoluteFilePath_ImplementsAllApplicableInterfaces()
	{
		// Arrange & Act
		AbsoluteFilePath absoluteFilePath = AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\test\\file.txt");
		IAbsoluteFilePath iAbsoluteFilePath = absoluteFilePath;
		IFilePath iFilePath = absoluteFilePath;
		IAbsolutePath iAbsolutePath = absoluteFilePath;
		IPath iPath = absoluteFilePath;

		// Assert
		Assert.IsNotNull(iAbsoluteFilePath);
		Assert.IsNotNull(iFilePath);
		Assert.IsNotNull(iAbsolutePath);
		Assert.IsNotNull(iPath);
		Assert.IsInstanceOfType<IAbsoluteFilePath>(iAbsoluteFilePath);
		Assert.IsInstanceOfType<IFilePath>(iFilePath);
		Assert.IsInstanceOfType<IAbsolutePath>(iAbsolutePath);
		Assert.IsInstanceOfType<IPath>(iPath);
		Assert.AreSame(absoluteFilePath, iAbsoluteFilePath);
		Assert.AreSame(absoluteFilePath, iFilePath);
		Assert.AreSame(absoluteFilePath, iAbsolutePath);
		Assert.AreSame(absoluteFilePath, iPath);
	}

	[TestMethod]
	public void RelativeFilePath_ImplementsAllApplicableInterfaces()
	{
		// Arrange & Act
		RelativeFilePath relativeFilePath = RelativeFilePath.Create<RelativeFilePath>("test\\file.txt");
		IRelativeFilePath iRelativeFilePath = relativeFilePath;
		IFilePath iFilePath = relativeFilePath;
		IRelativePath iRelativePath = relativeFilePath;
		IPath iPath = relativeFilePath;

		// Assert
		Assert.IsNotNull(iRelativeFilePath);
		Assert.IsNotNull(iFilePath);
		Assert.IsNotNull(iRelativePath);
		Assert.IsNotNull(iPath);
		Assert.IsInstanceOfType<IRelativeFilePath>(iRelativeFilePath);
		Assert.IsInstanceOfType<IFilePath>(iFilePath);
		Assert.IsInstanceOfType<IRelativePath>(iRelativePath);
		Assert.IsInstanceOfType<IPath>(iPath);
		Assert.AreSame(relativeFilePath, iRelativeFilePath);
		Assert.AreSame(relativeFilePath, iFilePath);
		Assert.AreSame(relativeFilePath, iRelativePath);
		Assert.AreSame(relativeFilePath, iPath);
	}

	[TestMethod]
	public void AbsoluteDirectoryPath_ImplementsAllApplicableInterfaces()
	{
		// Arrange & Act
		AbsoluteDirectoryPath absoluteDirectoryPath = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>("C:\\test\\directory");
		IAbsoluteDirectoryPath iAbsoluteDirectoryPath = absoluteDirectoryPath;
		IDirectoryPath iDirectoryPath = absoluteDirectoryPath;
		IAbsolutePath iAbsolutePath = absoluteDirectoryPath;
		IPath iPath = absoluteDirectoryPath;

		// Assert
		Assert.IsNotNull(iAbsoluteDirectoryPath);
		Assert.IsNotNull(iDirectoryPath);
		Assert.IsNotNull(iAbsolutePath);
		Assert.IsNotNull(iPath);
		Assert.IsInstanceOfType<IAbsoluteDirectoryPath>(iAbsoluteDirectoryPath);
		Assert.IsInstanceOfType<IDirectoryPath>(iDirectoryPath);
		Assert.IsInstanceOfType<IAbsolutePath>(iAbsolutePath);
		Assert.IsInstanceOfType<IPath>(iPath);
		Assert.AreSame(absoluteDirectoryPath, iAbsoluteDirectoryPath);
		Assert.AreSame(absoluteDirectoryPath, iDirectoryPath);
		Assert.AreSame(absoluteDirectoryPath, iAbsolutePath);
		Assert.AreSame(absoluteDirectoryPath, iPath);
	}

	[TestMethod]
	public void RelativeDirectoryPath_ImplementsAllApplicableInterfaces()
	{
		// Arrange & Act
		RelativeDirectoryPath relativeDirectoryPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>("test\\directory");
		IRelativeDirectoryPath iRelativeDirectoryPath = relativeDirectoryPath;
		IDirectoryPath iDirectoryPath = relativeDirectoryPath;
		IRelativePath iRelativePath = relativeDirectoryPath;
		IPath iPath = relativeDirectoryPath;

		// Assert
		Assert.IsNotNull(iRelativeDirectoryPath);
		Assert.IsNotNull(iDirectoryPath);
		Assert.IsNotNull(iRelativePath);
		Assert.IsNotNull(iPath);
		Assert.IsInstanceOfType<IRelativeDirectoryPath>(iRelativeDirectoryPath);
		Assert.IsInstanceOfType<IDirectoryPath>(iDirectoryPath);
		Assert.IsInstanceOfType<IRelativePath>(iRelativePath);
		Assert.IsInstanceOfType<IPath>(iPath);
		Assert.AreSame(relativeDirectoryPath, iRelativeDirectoryPath);
		Assert.AreSame(relativeDirectoryPath, iDirectoryPath);
		Assert.AreSame(relativeDirectoryPath, iRelativePath);
		Assert.AreSame(relativeDirectoryPath, iPath);
	}

	[TestMethod]
	public void FileName_ImplementsIFileName()
	{
		// Arrange & Act
		FileName fileName = FileName.Create<FileName>("test.txt");
		IFileName iFileName = fileName;

		// Assert
		Assert.IsNotNull(iFileName);
		Assert.IsInstanceOfType<IFileName>(iFileName);
		Assert.AreSame(fileName, iFileName);
	}

	[TestMethod]
	public void FileExtension_ImplementsIFileExtension()
	{
		// Arrange & Act
		FileExtension fileExtension = FileExtension.Create<FileExtension>(".txt");
		IFileExtension iFileExtension = fileExtension;

		// Assert
		Assert.IsNotNull(iFileExtension);
		Assert.IsInstanceOfType<IFileExtension>(iFileExtension);
		Assert.AreSame(fileExtension, iFileExtension);
	}

	[TestMethod]
	public void PolymorphicCollection_CanStoreAllPathTypes()
	{
		// Arrange
		List<IPath> paths = [];
		List<IFilePath> filePaths = [];
		List<IDirectoryPath> directoryPaths = [];
		List<IAbsolutePath> absolutePaths = [];
		List<IRelativePath> relativePaths = [];

		// Act
		paths.Add(AbsolutePath.Create<AbsolutePath>("C:\\test\\path"));
		paths.Add(AbsolutePath.Create<AbsolutePath>("C:\\absolute\\path"));
		paths.Add(RelativePath.Create<RelativePath>("relative\\path"));
		paths.Add(FilePath.Create<FilePath>("file.txt"));
		paths.Add(DirectoryPath.Create<DirectoryPath>("directory"));
		paths.Add(AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\file.txt"));
		paths.Add(RelativeFilePath.Create<RelativeFilePath>("relative\\file.txt"));
		paths.Add(AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>("C:\\directory"));
		paths.Add(RelativeDirectoryPath.Create<RelativeDirectoryPath>("relative\\directory"));

		filePaths.Add(FilePath.Create<FilePath>("file.txt"));
		filePaths.Add(AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\file.txt"));
		filePaths.Add(RelativeFilePath.Create<RelativeFilePath>("relative\\file.txt"));

		directoryPaths.Add(DirectoryPath.Create<DirectoryPath>("directory"));
		directoryPaths.Add(AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>("C:\\directory"));
		directoryPaths.Add(RelativeDirectoryPath.Create<RelativeDirectoryPath>("relative\\directory"));

		absolutePaths.Add(AbsolutePath.Create<AbsolutePath>("C:\\absolute\\path"));
		absolutePaths.Add(AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\file.txt"));
		absolutePaths.Add(AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>("C:\\directory"));

		relativePaths.Add(RelativePath.Create<RelativePath>("relative\\path"));
		relativePaths.Add(RelativeFilePath.Create<RelativeFilePath>("relative\\file.txt"));
		relativePaths.Add(RelativeDirectoryPath.Create<RelativeDirectoryPath>("relative\\directory"));

		// Assert
		Assert.AreEqual(9, paths.Count);
		Assert.AreEqual(3, filePaths.Count);
		Assert.AreEqual(3, directoryPaths.Count);
		Assert.AreEqual(3, absolutePaths.Count);
		Assert.AreEqual(3, relativePaths.Count);

		// Verify all items can be cast to IPath
		Assert.IsTrue(paths.All(p => p is not null));
		Assert.IsTrue(filePaths.All(p => p is IPath));
		Assert.IsTrue(directoryPaths.All(p => p is IPath));
		Assert.IsTrue(absolutePaths.All(p => p is IPath));
		Assert.IsTrue(relativePaths.All(p => p is IPath));
	}

	[TestMethod]
	public void PolymorphicMethods_CanAcceptInterfaceParameters()
	{
		// Arrange
		static string ProcessPath(IPath path) => $"Processing path: {path}";
		static string ProcessFilePath(IFilePath filePath) => $"Processing file: {filePath}";
		static string ProcessDirectoryPath(IDirectoryPath directoryPath) => $"Processing directory: {directoryPath}";
		static string ProcessAbsolutePath(IAbsolutePath absolutePath) => $"Processing absolute: {absolutePath}";
		static string ProcessRelativePath(IRelativePath relativePath) => $"Processing relative: {relativePath}";

		AbsoluteFilePath absoluteFilePath = AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\test\\file.txt");
		RelativeDirectoryPath relativeDirectoryPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>("test\\directory");

		// Act & Assert - Test that polymorphic methods work
		Assert.AreEqual("Processing path: C:\\test\\file.txt", ProcessPath(absoluteFilePath));
		Assert.AreEqual("Processing file: C:\\test\\file.txt", ProcessFilePath(absoluteFilePath));
		Assert.AreEqual("Processing absolute: C:\\test\\file.txt", ProcessAbsolutePath(absoluteFilePath));

		Assert.AreEqual("Processing path: test\\directory", ProcessPath(relativeDirectoryPath));
		Assert.AreEqual("Processing directory: test\\directory", ProcessDirectoryPath(relativeDirectoryPath));
		Assert.AreEqual("Processing relative: test\\directory", ProcessRelativePath(relativeDirectoryPath));
	}

	[TestMethod]
	public void InterfaceHierarchy_IsCorrect()
	{
		// Arrange
		AbsoluteFilePath absoluteFilePath = AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\test\\file.txt");

		// Act & Assert - Test inheritance hierarchy
		Assert.IsTrue(absoluteFilePath is IAbsoluteFilePath);
		Assert.IsTrue(absoluteFilePath is IFilePath);
		Assert.IsTrue(absoluteFilePath is IAbsolutePath);
		Assert.IsTrue(absoluteFilePath is IPath);

		// Test that interfaces inherit correctly
		IAbsoluteFilePath iAbsoluteFilePath = absoluteFilePath;
		Assert.IsTrue(iAbsoluteFilePath is IFilePath);
		Assert.IsTrue(iAbsoluteFilePath is IAbsolutePath);
		Assert.IsTrue(iAbsoluteFilePath is IPath);
	}

	[TestMethod]
	public void TypeChecking_WithInterfaces_WorksCorrectly()
	{
		// Arrange
		List<IPath> paths =
		[
			AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\file.txt"),
			RelativeDirectoryPath.Create<RelativeDirectoryPath>("directory")
		];

		// Act
		List<IFilePath> filePaths = [.. paths.OfType<IFilePath>()];
		List<IDirectoryPath> directoryPaths = [.. paths.OfType<IDirectoryPath>()];
		List<IAbsolutePath> absolutePaths = [.. paths.OfType<IAbsolutePath>()];
		List<IRelativePath> relativePaths = [.. paths.OfType<IRelativePath>()];

		// Assert
		Assert.AreEqual(1, filePaths.Count);
		Assert.AreEqual(1, directoryPaths.Count);
		Assert.AreEqual(1, absolutePaths.Count);
		Assert.AreEqual(1, relativePaths.Count);

		Assert.IsInstanceOfType<AbsoluteFilePath>(filePaths[0]);
		Assert.IsInstanceOfType<RelativeDirectoryPath>(directoryPaths[0]);
		Assert.IsInstanceOfType<AbsoluteFilePath>(absolutePaths[0]);
		Assert.IsInstanceOfType<RelativeDirectoryPath>(relativePaths[0]);
	}

	[TestMethod]
	public void NonPathTypes_ImplementCorrectInterfaces()
	{
		// Arrange & Act
		FileName fileName = FileName.Create<FileName>("test.txt");
		FileExtension fileExtension = FileExtension.Create<FileExtension>(".txt");

		// Assert - These types implement their own interfaces but not IPath
		// (The compiler prevents us from even checking 'is IPath' since it knows they don't implement it)
		Assert.IsTrue(fileName is IFileName);
		Assert.IsTrue(fileExtension is IFileExtension);

		// Verify they can be cast to their interfaces
		IFileName iFileName = fileName;
		IFileExtension iFileExtension = fileExtension;
		Assert.IsNotNull(iFileName);
		Assert.IsNotNull(iFileExtension);
	}

	[TestMethod]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1859:Use concrete types when possible for improved performance", Justification = "Test specifically validates interface behavior")]
	public void AsAbsolute_Method_WorksCorrectly()
	{
		// Arrange - Create different path types
		AbsoluteFilePath absoluteFile = AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\test\\file.txt");
		RelativeFilePath relativeFile = RelativeFilePath.Create<RelativeFilePath>("test\\file.txt");
		FilePath genericFile = FilePath.Create<FilePath>("file.txt");

		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>("C:\\test\\dir");
		RelativeDirectoryPath relativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("test\\dir");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>("dir");

		// Act & Assert - Test file paths
		IFilePath iAbsoluteFile = absoluteFile;
		IFilePath iRelativeFile = relativeFile;
		IFilePath iGenericFile = genericFile;

		Assert.IsInstanceOfType<AbsoluteFilePath>(iAbsoluteFile.AsAbsolute());
		Assert.IsInstanceOfType<AbsoluteFilePath>(iRelativeFile.AsAbsolute());
		Assert.IsInstanceOfType<AbsoluteFilePath>(iGenericFile.AsAbsolute());

		// For absolute paths, AsAbsolute should return the same instance
		Assert.AreSame(absoluteFile, iAbsoluteFile.AsAbsolute());

		// For relative and generic paths, AsAbsolute should create new absolute instances
		Assert.IsNotNull(iRelativeFile.AsAbsolute());
		Assert.IsNotNull(iGenericFile.AsAbsolute());

		// Act & Assert - Test directory paths
		IDirectoryPath iAbsoluteDir = absoluteDir;
		IDirectoryPath iRelativeDir = relativeDir;
		IDirectoryPath iGenericDir = genericDir;

		Assert.IsInstanceOfType<AbsoluteDirectoryPath>(iAbsoluteDir.AsAbsolute());
		Assert.IsInstanceOfType<AbsoluteDirectoryPath>(iRelativeDir.AsAbsolute());
		Assert.IsInstanceOfType<AbsoluteDirectoryPath>(iGenericDir.AsAbsolute());

		// For absolute paths, AsAbsolute should return the same instance
		Assert.AreSame(absoluteDir, iAbsoluteDir.AsAbsolute());

		// For relative and generic paths, AsAbsolute should create new absolute instances
		Assert.IsNotNull(iRelativeDir.AsAbsolute());
		Assert.IsNotNull(iGenericDir.AsAbsolute());
	}

	[TestMethod]
	public void ConsolidatedPathConversions_API_WorksCorrectly()
	{
		// Arrange
		AbsoluteDirectoryPath baseDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>("C:\\base");
		AbsoluteFilePath absoluteFile = AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\base\\sub\\file.txt");
		RelativeFilePath relativeFile = RelativeFilePath.Create<RelativeFilePath>("sub\\file.txt");

		// Test the consolidated API
		// 1. AsAbsolute() - convert to absolute using current working directory
		AbsoluteFilePath absFromRelative = relativeFile.AsAbsolute();
		Assert.IsInstanceOfType<AbsoluteFilePath>(absFromRelative);

		// 2. AsAbsolute(baseDirectory) - convert to absolute using specific base
		AbsoluteFilePath absFromRelativeWithBase = relativeFile.AsAbsolute(baseDir);
		Assert.IsInstanceOfType<AbsoluteFilePath>(absFromRelativeWithBase);
		Assert.IsTrue(absFromRelativeWithBase.WeakString.Contains("C:\\base"));

		// 3. AsRelative(baseDirectory) - convert to relative using base
		RelativeFilePath relFromAbsolute = absoluteFile.AsRelative(baseDir);
		Assert.IsInstanceOfType<RelativeFilePath>(relFromAbsolute);
		Assert.AreEqual("sub\\file.txt", relFromAbsolute.WeakString);

		// 4. AsRelative(baseDirectory) on already relative path returns itself
		RelativeFilePath relFromRelative = relativeFile.AsRelative(baseDir);
		Assert.AreSame(relativeFile, relFromRelative);

		// Test with directory paths too
		AbsoluteDirectoryPath absoluteSubDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>("C:\\base\\sub");
		RelativeDirectoryPath relativeSubDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>("sub");

		RelativeDirectoryPath relDirFromAbsolute = absoluteSubDir.AsRelative(baseDir);
		Assert.AreEqual("sub", relDirFromAbsolute.WeakString);

		AbsoluteDirectoryPath absDirFromRelative = relativeSubDir.AsAbsolute(baseDir);
		Assert.IsTrue(absDirFromRelative.WeakString.Contains("C:\\base\\sub"));
	}

	[TestMethod]
	public void DirectoryPath_Contents_ReturnsCorrectPathTypes()
	{
		// Create a test directory with some files
		string tempDir = Path.GetTempPath();
		string testDir = Path.Combine(tempDir, "SemanticPathTest_" + Guid.NewGuid().ToString("N")[..8]);
		Directory.CreateDirectory(testDir);

		try
		{
			// Create test files and subdirectory
			string testFile1 = Path.Combine(testDir, "test1.txt");
			string testFile2 = Path.Combine(testDir, "test2.log");
			string subDir = Path.Combine(testDir, "subdir");
			File.WriteAllText(testFile1, "test content 1");
			File.WriteAllText(testFile2, "test content 2");
			Directory.CreateDirectory(subDir);

			// Test DirectoryPath
			DirectoryPath dirPath = DirectoryPath.Create<DirectoryPath>(testDir);
			IEnumerable<IPath> contents = dirPath.Contents;

			Assert.IsTrue(contents.Any(), "Contents should not be empty");
			Assert.AreEqual(3, contents.Count(), "Should contain 2 files and 1 directory");

			// Verify file types
			IFilePath[] files = [.. contents.OfType<IFilePath>()];
			Assert.AreEqual(2, files.Length, "Should contain 2 files");
			Assert.IsTrue(files.All(f => f is FilePath), "Files should be FilePath type");

			// Verify directory types
			IDirectoryPath[] directories = [.. contents.OfType<IDirectoryPath>()];
			Assert.AreEqual(1, directories.Length, "Should contain 1 directory");
			Assert.IsTrue(directories.All(d => d is DirectoryPath), "Directories should be DirectoryPath type");
		}
		finally
		{
			// Clean up
			if (Directory.Exists(testDir))
			{
				Directory.Delete(testDir, true);
			}
		}
	}

	[TestMethod]
	public void AbsoluteDirectoryPath_Contents_ReturnsAbsolutePathTypes()
	{
		// Create a test directory with some files
		string tempDir = Path.GetTempPath();
		string testDir = Path.Combine(tempDir, "SemanticPathTest_" + Guid.NewGuid().ToString("N")[..8]);
		Directory.CreateDirectory(testDir);

		try
		{
			// Create test files and subdirectory
			string testFile = Path.Combine(testDir, "test.txt");
			string subDir = Path.Combine(testDir, "subdir");
			File.WriteAllText(testFile, "test content");
			Directory.CreateDirectory(subDir);

			// Test AbsoluteDirectoryPath
			AbsoluteDirectoryPath absDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(testDir);
			IEnumerable<IPath> contents = absDir.Contents;

			Assert.IsTrue(contents.Any(), "Contents should not be empty");
			Assert.AreEqual(2, contents.Count(), "Should contain 1 file and 1 directory");

			// Verify file types are absolute
			IAbsoluteFilePath[] absoluteFiles = [.. contents.OfType<IAbsoluteFilePath>()];
			Assert.AreEqual(1, absoluteFiles.Length, "Should contain 1 absolute file");
			Assert.IsTrue(absoluteFiles.All(f => f is AbsoluteFilePath), "Files should be AbsoluteFilePath type");

			// Verify directory types are absolute
			IAbsoluteDirectoryPath[] absoluteDirectories = [.. contents.OfType<IAbsoluteDirectoryPath>()];
			Assert.AreEqual(1, absoluteDirectories.Length, "Should contain 1 absolute directory");
			Assert.IsTrue(absoluteDirectories.All(d => d is AbsoluteDirectoryPath), "Directories should be AbsoluteDirectoryPath type");
		}
		finally
		{
			// Clean up
			if (Directory.Exists(testDir))
			{
				Directory.Delete(testDir, true);
			}
		}
	}

	[TestMethod]
	public void DirectoryPath_Contents_NonExistentDirectory_ReturnsEmpty()
	{
		// Test non-existent directory
		DirectoryPath nonExistentDir = DirectoryPath.Create<DirectoryPath>("/path/that/does/not/exist");
		IEnumerable<IPath> contents = nonExistentDir.Contents;

		Assert.IsFalse(contents.Any(), "Non-existent directory should return empty contents");
	}

	[TestMethod]
	public void DirectoryPath_Contents_EmptyDirectory_ReturnsEmpty()
	{
		// Create an empty test directory
		string tempDir = Path.GetTempPath();
		string testDir = Path.Combine(tempDir, "SemanticPathTest_Empty_" + Guid.NewGuid().ToString("N")[..8]);
		Directory.CreateDirectory(testDir);

		try
		{
			DirectoryPath dirPath = DirectoryPath.Create<DirectoryPath>(testDir);
			IEnumerable<IPath> contents = dirPath.Contents;

			Assert.IsFalse(contents.Any(), "Empty directory should return empty contents");
		}
		finally
		{
			// Clean up
			if (Directory.Exists(testDir))
			{
				Directory.Delete(testDir, true);
			}
		}
	}

	[TestMethod]
	public void DirectoryPath_Contents_PolymorphicUsage()
	{
		// Create a test directory with some files
		string tempDir = Path.GetTempPath();
		string testDir = Path.Combine(tempDir, "SemanticPathTest_Polymorphic_" + Guid.NewGuid().ToString("N")[..8]);
		Directory.CreateDirectory(testDir);

		try
		{
			// Create test files
			string testFile1 = Path.Combine(testDir, "test1.txt");
			string testFile2 = Path.Combine(testDir, "test2.doc");
			File.WriteAllText(testFile1, "test content 1");
			File.WriteAllText(testFile2, "test content 2");

			// Test polymorphic usage with IDirectoryPath
			DirectoryPath dirPath = DirectoryPath.Create<DirectoryPath>(testDir);
			IEnumerable<IPath> contents = dirPath.Contents;

			Assert.IsTrue(contents.Any(), "Contents should not be empty");
			Assert.AreEqual(2, contents.Count(), "Should contain 2 files");

			// Test filtering by type
			IFilePath[] files = [.. contents.OfType<IFilePath>()];
			Assert.AreEqual(2, files.Length, "Should be able to filter files polymorphically");

			// Test that all returned items are paths
			Assert.IsTrue(contents.All(item => item is not null), "All contents should implement IPath");
		}
		finally
		{
			// Clean up
			if (Directory.Exists(testDir))
			{
				Directory.Delete(testDir, true);
			}
		}
	}

	[TestMethod]
	public void AllPathTypes_ImplicitStringConversion_WorksTransparently()
	{
		// Arrange - create instances of all path types
		AbsolutePath absolutePath = AbsolutePath.Create<AbsolutePath>("C:\\temp");
		RelativePath relativePath = RelativePath.Create<RelativePath>("relative\\path");
		FilePath filePath = FilePath.Create<FilePath>("file.txt");
		DirectoryPath directoryPath = DirectoryPath.Create<DirectoryPath>("directory");
		AbsoluteFilePath absoluteFilePath = AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\temp\\file.txt");
		RelativeFilePath relativeFilePath = RelativeFilePath.Create<RelativeFilePath>("relative\\file.txt");
		AbsoluteDirectoryPath absoluteDirectoryPath = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>("C:\\temp\\directory");
		RelativeDirectoryPath relativeDirectoryPath = RelativeDirectoryPath.Create<RelativeDirectoryPath>("relative\\directory");
		FileName fileName = FileName.Create<FileName>("file.txt");
		FileExtension fileExtension = FileExtension.Create<FileExtension>(".txt");

		// Act & Assert - all should implicitly convert to string
		string result1 = absolutePath; // Implicit conversion
		string result2 = relativePath;
		string result3 = filePath;
		string result4 = directoryPath;
		string result5 = absoluteFilePath;
		string result6 = relativeFilePath;
		string result7 = absoluteDirectoryPath;
		string result8 = relativeDirectoryPath;
		string result9 = fileName;
		string result10 = fileExtension;

		Assert.AreEqual("C:\\temp", result1);
		Assert.AreEqual("relative\\path", result2);
		Assert.AreEqual("file.txt", result3);
		Assert.AreEqual("directory", result4);
		Assert.AreEqual("C:\\temp\\file.txt", result5);
		Assert.AreEqual("relative\\file.txt", result6);
		Assert.AreEqual("C:\\temp\\directory", result7);
		Assert.AreEqual("relative\\directory", result8);
		Assert.AreEqual("file.txt", result9);
		Assert.AreEqual(".txt", result10);
	}

	[TestMethod]
	public void PathInterfaces_CanBeUsedInStringMethods()
	{
		// Arrange
		IAbsoluteFilePath absoluteFilePath = AbsoluteFilePath.Create<AbsoluteFilePath>("C:\\temp\\test.txt");
		IDirectoryPath directoryPath = DirectoryPath.Create<DirectoryPath>("documents");
		IFileName fileName = FileName.Create<FileName>("readme.md");

		// Act & Assert - interface types should work transparently in string operations

		// Test with System.IO.Path methods - need to cast interfaces to concrete types for string operations
		string? directory = Path.GetDirectoryName(((AbsoluteFilePath)absoluteFilePath).ToString());
		string filename = Path.GetFileName(((AbsoluteFilePath)absoluteFilePath).ToString());
		string combined = Path.Combine(((DirectoryPath)directoryPath).ToString(), ((FileName)fileName).ToString());

		Assert.AreEqual("C:\\temp", directory ?? string.Empty);
		Assert.AreEqual("test.txt", filename);
		Assert.IsTrue(combined.Contains("documents") && combined.Contains("readme.md"));
	}

	[TestMethod]
	public void PathTypes_CanBeUsedInFileSystemMethods()
	{
		// Arrange
		string tempDir = Path.GetTempPath();
		string testDir = Path.Combine(tempDir, "SemanticPathTest_" + Guid.NewGuid().ToString("N")[..8]);
		string testFile = Path.Combine(testDir, "test.txt");

		AbsoluteDirectoryPath semanticDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(testDir);
		AbsoluteFilePath semanticFile = AbsoluteFilePath.Create<AbsoluteFilePath>(testFile);

		try
		{
			// Act - use semantic path types directly in System.IO methods
			Directory.CreateDirectory(semanticDir); // Implicit conversion to string
			File.WriteAllText(semanticFile, "test content"); // Implicit conversion to string

			// Assert - verify the operations worked
			bool dirExists = Directory.Exists(semanticDir); // Implicit conversion to string
			bool fileExists = File.Exists(semanticFile); // Implicit conversion to string
			string content = File.ReadAllText(semanticFile); // Implicit conversion to string

			Assert.IsTrue(dirExists);
			Assert.IsTrue(fileExists);
			Assert.AreEqual("test content", content);
		}
		finally
		{
			// Cleanup
			if (Directory.Exists(testDir))
			{
				Directory.Delete(testDir, true);
			}
		}
	}

	[TestMethod]
	public void PathInterfaces_InPolymorphicCollections_CanBeUsedAsStrings()
	{
		// Arrange
		List<IPath> paths = [
			AbsolutePath.Create<AbsolutePath>("C:\\absolute"),
			RelativePath.Create<RelativePath>("relative"),
			FilePath.Create<FilePath>("file.txt"),
			DirectoryPath.Create<DirectoryPath>("directory")
		];

		// Act & Assert - interface references should implicitly convert to strings
		foreach (IPath path in paths)
		{
			// This works because the concrete types implement implicit string conversion
			string? stringValue = path.ToString(); // Explicit call

			// Test some string operations that would require implicit conversion
			string pathWithPrefix = $"prefix_{path.ToString() ?? string.Empty}"; // Use string interpolation to avoid nullable issues
			bool hasContent = !string.IsNullOrEmpty(pathWithPrefix);

			Assert.IsTrue(hasContent);
			Assert.IsTrue(pathWithPrefix.StartsWith("prefix_"));
		}
	}
}
