// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Paths;

using ktsu.Semantics;
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
		RelativeDirectoryPath relativeDir = RelativeDirectoryPath.Create<RelativeDirectoryPath>(@"temp");
		DirectoryPath genericDir = DirectoryPath.Create<DirectoryPath>(@"temp");

		// Act
		AbsoluteFilePath absoluteResult = absoluteDir / fileName;
		RelativeFilePath relativeResult = relativeDir / fileName;
		FilePath genericResult = genericDir / fileName;

		// Assert
		Assert.IsNotNull(absoluteResult);
		Assert.IsNotNull(relativeResult);
		Assert.IsNotNull(genericResult);

		Assert.AreEqual(@"C:\temp\test.txt", absoluteResult.WeakString);
		Assert.AreEqual(@"temp\test.txt", relativeResult.WeakString);
		Assert.AreEqual(@"temp\test.txt", genericResult.WeakString);
	}
}
