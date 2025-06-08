// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SemanticPathInterfaceTests
{
	[TestMethod]
	public void Path_ImplementsIPath()
	{
		// Arrange & Act
		Path path = Path.FromString<Path>("test\\path");
		IPath iPath = path;

		// Assert
		Assert.IsNotNull(iPath);
		Assert.IsInstanceOfType<IPath>(iPath);
		Assert.AreSame(path, iPath);
	}

	[TestMethod]
	public void AbsolutePath_ImplementsIAbsolutePathAndIPath()
	{
		// Arrange & Act
		AbsolutePath absolutePath = AbsolutePath.FromString<AbsolutePath>("C:\\test\\path");
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
		RelativePath relativePath = RelativePath.FromString<RelativePath>("test\\path");
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
		FilePath filePath = FilePath.FromString<FilePath>("test\\file.txt");
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
		DirectoryPath directoryPath = DirectoryPath.FromString<DirectoryPath>("test\\directory");
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
		AbsoluteFilePath absoluteFilePath = AbsoluteFilePath.FromString<AbsoluteFilePath>("C:\\test\\file.txt");
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
		RelativeFilePath relativeFilePath = RelativeFilePath.FromString<RelativeFilePath>("test\\file.txt");
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
		AbsoluteDirectoryPath absoluteDirectoryPath = AbsoluteDirectoryPath.FromString<AbsoluteDirectoryPath>("C:\\test\\directory");
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
		RelativeDirectoryPath relativeDirectoryPath = RelativeDirectoryPath.FromString<RelativeDirectoryPath>("test\\directory");
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
		FileName fileName = FileName.FromString<FileName>("test.txt");
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
		FileExtension fileExtension = FileExtension.FromString<FileExtension>(".txt");
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
		paths.Add(Path.FromString<Path>("test\\path"));
		paths.Add(AbsolutePath.FromString<AbsolutePath>("C:\\absolute\\path"));
		paths.Add(RelativePath.FromString<RelativePath>("relative\\path"));
		paths.Add(FilePath.FromString<FilePath>("file.txt"));
		paths.Add(DirectoryPath.FromString<DirectoryPath>("directory"));
		paths.Add(AbsoluteFilePath.FromString<AbsoluteFilePath>("C:\\file.txt"));
		paths.Add(RelativeFilePath.FromString<RelativeFilePath>("relative\\file.txt"));
		paths.Add(AbsoluteDirectoryPath.FromString<AbsoluteDirectoryPath>("C:\\directory"));
		paths.Add(RelativeDirectoryPath.FromString<RelativeDirectoryPath>("relative\\directory"));

		filePaths.Add(FilePath.FromString<FilePath>("file.txt"));
		filePaths.Add(AbsoluteFilePath.FromString<AbsoluteFilePath>("C:\\file.txt"));
		filePaths.Add(RelativeFilePath.FromString<RelativeFilePath>("relative\\file.txt"));

		directoryPaths.Add(DirectoryPath.FromString<DirectoryPath>("directory"));
		directoryPaths.Add(AbsoluteDirectoryPath.FromString<AbsoluteDirectoryPath>("C:\\directory"));
		directoryPaths.Add(RelativeDirectoryPath.FromString<RelativeDirectoryPath>("relative\\directory"));

		absolutePaths.Add(AbsolutePath.FromString<AbsolutePath>("C:\\absolute\\path"));
		absolutePaths.Add(AbsoluteFilePath.FromString<AbsoluteFilePath>("C:\\file.txt"));
		absolutePaths.Add(AbsoluteDirectoryPath.FromString<AbsoluteDirectoryPath>("C:\\directory"));

		relativePaths.Add(RelativePath.FromString<RelativePath>("relative\\path"));
		relativePaths.Add(RelativeFilePath.FromString<RelativeFilePath>("relative\\file.txt"));
		relativePaths.Add(RelativeDirectoryPath.FromString<RelativeDirectoryPath>("relative\\directory"));

		// Assert
		Assert.AreEqual(9, paths.Count);
		Assert.AreEqual(3, filePaths.Count);
		Assert.AreEqual(3, directoryPaths.Count);
		Assert.AreEqual(3, absolutePaths.Count);
		Assert.AreEqual(3, relativePaths.Count);

		// Verify all items are of correct types
		Assert.IsTrue(paths.All(p => p is not null));
		Assert.IsTrue(filePaths.All(f => f is not null));
		Assert.IsTrue(directoryPaths.All(d => d is not null));
		Assert.IsTrue(absolutePaths.All(a => a is not null));
		Assert.IsTrue(relativePaths.All(r => r is not null));
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

		AbsoluteFilePath absoluteFilePath = AbsoluteFilePath.FromString<AbsoluteFilePath>("C:\\test\\file.txt");
		RelativeDirectoryPath relativeDirectoryPath = RelativeDirectoryPath.FromString<RelativeDirectoryPath>("test\\directory");

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
		AbsoluteFilePath absoluteFilePath = AbsoluteFilePath.FromString<AbsoluteFilePath>("C:\\test\\file.txt");

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
			AbsoluteFilePath.FromString<AbsoluteFilePath>("C:\\file.txt"),
			RelativeDirectoryPath.FromString<RelativeDirectoryPath>("directory")
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
		FileName fileName = FileName.FromString<FileName>("test.txt");
		FileExtension fileExtension = FileExtension.FromString<FileExtension>(".txt");

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
}
