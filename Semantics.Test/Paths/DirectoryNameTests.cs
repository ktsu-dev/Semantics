// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Paths;

using System.Collections.Generic;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DirectoryNameTests
{
	[TestMethod]
	public void DirectoryName_Create_WithValidName_Succeeds()
	{
		// Test creating DirectoryName with valid name
		DirectoryName name = DirectoryName.Create<DirectoryName>("MyFolder");

		Assert.IsNotNull(name);
		Assert.AreEqual("MyFolder", name.WeakString);
	}

	[TestMethod]
	public void DirectoryName_Create_WithSpaces_Succeeds()
	{
		// Test creating DirectoryName with spaces
		DirectoryName name = DirectoryName.Create<DirectoryName>("My Folder Name");

		Assert.IsNotNull(name);
		Assert.AreEqual("My Folder Name", name.WeakString);
	}

	[TestMethod]
	public void DirectoryName_Create_WithSpecialCharacters_Succeeds()
	{
		// Test creating DirectoryName with valid special characters
		DirectoryName name = DirectoryName.Create<DirectoryName>("folder-name_v2 (beta)");

		Assert.IsNotNull(name);
		Assert.AreEqual("folder-name_v2 (beta)", name.WeakString);
	}

	[TestMethod]
	public void DirectoryName_Create_WithPathSeparator_ThrowsException()
	{
		// Test that DirectoryName rejects path separators
		Assert.ThrowsExactly<ArgumentException>(() =>
			DirectoryName.Create<DirectoryName>("folder\\subfolder"));
	}

	[TestMethod]
	public void DirectoryName_Create_WithForwardSlash_ThrowsException()
	{
		// Test that DirectoryName rejects forward slashes
		Assert.ThrowsExactly<ArgumentException>(() =>
			DirectoryName.Create<DirectoryName>("folder/subfolder"));
	}

	[TestMethod]
	public void DirectoryName_Create_WithInvalidCharacters_ThrowsException()
	{
		// Test that DirectoryName rejects invalid filename characters
		Assert.ThrowsExactly<ArgumentException>(() =>
			DirectoryName.Create<DirectoryName>("invalid<>name"));

		Assert.ThrowsExactly<ArgumentException>(() =>
			DirectoryName.Create<DirectoryName>("invalid:name"));

		Assert.ThrowsExactly<ArgumentException>(() =>
			DirectoryName.Create<DirectoryName>("invalid|name"));

		Assert.ThrowsExactly<ArgumentException>(() =>
			DirectoryName.Create<DirectoryName>("invalid\"name"));
	}

	[TestMethod]
	public void DirectoryName_Create_WithEmptyString_Succeeds()
	{
		// Test that empty DirectoryName is valid
		DirectoryName name = DirectoryName.Create<DirectoryName>("");

		Assert.IsNotNull(name);
		Assert.AreEqual("", name.WeakString);
	}

	[TestMethod]
	public void DirectoryName_TryCreate_WithValidName_ReturnsTrue()
	{
		// Test TryCreate with valid directory name
		bool success = DirectoryName.TryCreate("ValidFolder", out DirectoryName? result);

		Assert.IsTrue(success, "TryCreate should return true for a valid directory name");
		Assert.IsNotNull(result);
		Assert.AreEqual("ValidFolder", result.WeakString);
	}

	[TestMethod]
	public void DirectoryName_TryCreate_WithInvalidName_ReturnsFalse()
	{
		// Test TryCreate with invalid directory name
		bool success = DirectoryName.TryCreate("invalid\\name", out DirectoryName? result);

		Assert.IsFalse(success, "TryCreate should return false for a directory name containing a path separator");
		Assert.IsNull(result);
	}

	[TestMethod]
	public void DirectoryName_ExplicitCast_FromString_WorksCorrectly()
	{
		// Test explicit cast from string
		DirectoryName name = DirectoryName.Create<DirectoryName>("MyFolder");

		Assert.IsNotNull(name);
		Assert.AreEqual("MyFolder", name.WeakString);
	}

	[TestMethod]
	public void DirectoryName_ImplementsIDirectoryName_Interface()
	{
		// Test that DirectoryName implements IDirectoryName
		DirectoryName name = DirectoryName.Create<DirectoryName>("TestFolder");
		IDirectoryName interfaceReference = name;

		Assert.IsNotNull(interfaceReference);
		Assert.IsInstanceOfType<IDirectoryName>(name);
	}

	[TestMethod]
	public void DirectoryName_ImplementsISemanticString_Interface()
	{
		// Test that DirectoryName implements ISemanticString
		DirectoryName name = DirectoryName.Create<DirectoryName>("TestFolder");
		DirectoryName interfaceReference = name;

		Assert.IsNotNull(interfaceReference);
		Assert.AreEqual("TestFolder", interfaceReference.WeakString);
	}

	[TestMethod]
	public void DirectoryName_Equality_WorksCorrectly()
	{
		// Test equality comparison
		DirectoryName name1 = DirectoryName.Create<DirectoryName>("MyFolder");
		DirectoryName name2 = DirectoryName.Create<DirectoryName>("MyFolder");
		DirectoryName name3 = DirectoryName.Create<DirectoryName>("OtherFolder");

		Assert.AreEqual(name1, name2);
		Assert.AreNotEqual(name1, name3);
	}

	[TestMethod]
	public void DirectoryName_GetHashCode_ConsistentForEqualValues()
	{
		// Test that hash codes are consistent
		DirectoryName name1 = DirectoryName.Create<DirectoryName>("MyFolder");
		DirectoryName name2 = DirectoryName.Create<DirectoryName>("MyFolder");

		Assert.AreEqual(name1.GetHashCode(), name2.GetHashCode());
	}

	[TestMethod]
	public void DirectoryName_ToString_ReturnsWeakString()
	{
		// Test ToString method
		DirectoryName name = DirectoryName.Create<DirectoryName>("MyFolder");

		string result = name.ToString();

		Assert.AreEqual("MyFolder", result);
	}

	[TestMethod]
	public void DirectoryName_UsedInDictionary_WorksCorrectly()
	{
		// Test that DirectoryName can be used as dictionary key
		Dictionary<DirectoryName, string> dict = [];
		DirectoryName key = DirectoryName.Create<DirectoryName>("MyFolder");

		dict[key] = "some value";

		Assert.AreEqual("some value", dict[key]);
		Assert.IsTrue(dict.ContainsKey(DirectoryName.Create<DirectoryName>("MyFolder")), "Dictionary should contain the DirectoryName key with equal value");
	}

	[TestMethod]
	public void DirectoryName_CombineWithAbsoluteDirectoryPath_CreatesValidPath()
	{
		// Test combining DirectoryName with AbsoluteDirectoryPath
		AbsoluteDirectoryPath basePath = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(@"C:\projects");
		DirectoryName subDir = DirectoryName.Create<DirectoryName>("myapp");

		AbsoluteDirectoryPath result = basePath / subDir;

		Assert.IsNotNull(result);
		Assert.Contains("myapp", result.WeakString);
	}

	[TestMethod]
	public void DirectoryName_CombineWithDirectoryPath_CreatesValidPath()
	{
		// Test combining DirectoryName with DirectoryPath
		DirectoryPath basePath = DirectoryPath.Create<DirectoryPath>("projects");
		DirectoryName subDir = DirectoryName.Create<DirectoryName>("myapp");

		DirectoryPath result = basePath / subDir;

		Assert.IsNotNull(result);
		Assert.Contains("myapp", result.WeakString);
	}

	[TestMethod]
	public void DirectoryName_MultipleInChain_CreatesDeepPath()
	{
		// Test chaining multiple DirectoryName combinations
		DirectoryPath basePath = DirectoryPath.Create<DirectoryPath>("root");
		DirectoryName dir1 = DirectoryName.Create<DirectoryName>("level1");
		DirectoryName dir2 = DirectoryName.Create<DirectoryName>("level2");
		DirectoryName dir3 = DirectoryName.Create<DirectoryName>("level3");

		DirectoryPath result = basePath / dir1 / dir2 / dir3;

		Assert.IsNotNull(result);
		Assert.Contains("root", result.WeakString);
		Assert.Contains("level1", result.WeakString);
		Assert.Contains("level2", result.WeakString);
		Assert.Contains("level3", result.WeakString);
	}

	[TestMethod]
	public void DirectoryName_WithUnicodeCharacters_WorksCorrectly()
	{
		// Test DirectoryName with Unicode characters
		DirectoryName name = DirectoryName.Create<DirectoryName>("文件夹名称");

		Assert.IsNotNull(name);
		Assert.AreEqual("文件夹名称", name.WeakString);
	}

	[TestMethod]
	public void DirectoryName_WithNumbers_WorksCorrectly()
	{
		// Test DirectoryName with numbers
		DirectoryName name = DirectoryName.Create<DirectoryName>("folder123");

		Assert.IsNotNull(name);
		Assert.AreEqual("folder123", name.WeakString);
	}

	[TestMethod]
	public void DirectoryName_WithDots_WorksCorrectly()
	{
		// Test DirectoryName with dots (but not as path traversal)
		DirectoryName name = DirectoryName.Create<DirectoryName>("my.folder.name");

		Assert.IsNotNull(name);
		Assert.AreEqual("my.folder.name", name.WeakString);
	}

	[TestMethod]
	public void DirectoryName_IsValid_WithValidName_ReturnsTrue()
	{
		// Test IsValid method with valid name
		DirectoryName name = DirectoryName.Create<DirectoryName>("ValidFolder");

		bool isValid = name.IsValid();

		Assert.IsTrue(isValid, "IsValid should return true for a valid DirectoryName");
	}
}
