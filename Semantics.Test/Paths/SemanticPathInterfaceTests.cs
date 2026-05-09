// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Paths;

using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SemanticPathInterfaceTests
{
	[TestMethod]
	public void DirectoryPath_CombineWithFileName_WorksCorrectly()
	{
		// Arrange
		FileName fileName = FileName.Create<FileName>("test.txt");

		AbsoluteDirectoryPath absoluteDir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\temp");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>(@"temp");

		// Act
		AbsoluteFilePath absoluteResult = absoluteDir / fileName;
		FilePath genericResult = genericDir / fileName;

		// Assert
		Assert.IsNotNull(absoluteResult);
		Assert.IsNotNull(genericResult);

		Assert.AreEqual(@"C:\temp\test.txt", absoluteResult.WeakString);
		Assert.AreEqual(@"temp\test.txt", genericResult.WeakString);
	}
}
