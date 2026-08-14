// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using ktsu.Semantics.Paths;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers the string value exposed on the path and path-component interfaces, so polymorphic code
/// can read a value without downcasting to a concrete type.
/// </summary>
[TestClass]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance", Justification = "Reading the value through the interface is exactly what these tests cover.")]
public class PathInterfaceMemberTests
{
	/// <summary>
	/// An absolute root that is valid on every platform: <c>C:\</c> on Windows, <c>/</c> elsewhere.
	/// </summary>
	private static readonly string Root = Path.Combine(
		Path.GetPathRoot(Path.GetTempPath())!,
		nameof(PathInterfaceMemberTests));

	private static string PathFor(string name) => Path.Combine(Root, name);
	[TestMethod]
	public void IPath_ExposesWeakString()
	{
		IPath path = AbsoluteFilePath.Create<AbsoluteFilePath>(PathFor("afile"));

		Assert.AreEqual(PathFor("afile"), path.WeakString);
	}

	[TestMethod]
	public void IAbsolutePath_ExposesWeakString()
	{
		IAbsolutePath path = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(PathFor("adir"));

		Assert.AreEqual(PathFor("adir"), path.WeakString);
	}

	[TestMethod]
	public void IFileName_ExposesWeakString()
	{
		IFileName fileName = FileName.Create<FileName>("afile.txt");

		Assert.AreEqual("afile.txt", fileName.WeakString);
	}

	[TestMethod]
	public void IFileExtension_ExposesWeakString()
	{
		IFileExtension extension = FileExtension.Create<FileExtension>(".txt");

		Assert.AreEqual(".txt", extension.WeakString);
	}

	[TestMethod]
	public void IDirectoryName_ExposesWeakString()
	{
		IDirectoryName directoryName = DirectoryName.Create<DirectoryName>("adir");

		Assert.AreEqual("adir", directoryName.WeakString);
	}

	[TestMethod]
	public void GetContents_ResultsReadableWithoutDowncasting()
	{
		string root = Path.Combine(Path.GetTempPath(), $"{nameof(PathInterfaceMemberTests)}_{nameof(GetContents_ResultsReadableWithoutDowncasting)}");
		Directory.CreateDirectory(Path.Combine(root, "child"));
		File.WriteAllText(Path.Combine(root, "afile.txt"), "content");

		try
		{
			AbsoluteDirectoryPath directory = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(root);

			List<string> contents = [.. directory.GetContents().Select(p => p.WeakString).OrderBy(p => p, StringComparer.Ordinal)];

			List<string> expected = [Path.Combine(root, "afile.txt"), Path.Combine(root, "child")];

			Assert.AreSequenceEqual(expected, contents);
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	/// <summary>
	/// Guards the reason <see cref="IComparable{T}"/> of <see cref="IPath"/> is implemented explicitly:
	/// a public overload taking <see cref="IPath"/> would make this call ambiguous against the inherited
	/// overload taking <see cref="ISemanticString"/>, since a concrete path satisfies both. This test
	/// failing to compile is the regression signal.
	/// </summary>
	[TestMethod]
	public void CompareTo_BetweenConcretePaths_IsNotAmbiguous()
	{
		AbsoluteFilePath first = AbsoluteFilePath.Create<AbsoluteFilePath>(PathFor("afile"));
		AbsoluteFilePath second = AbsoluteFilePath.Create<AbsoluteFilePath>(PathFor("bfile"));

		Assert.IsLessThan(0, first.CompareTo(second));
	}

	[TestMethod]
	public void CompareTo_ThroughIComparableOfIPath_ComparesByValue()
	{
		IComparable<IPath> first = AbsoluteFilePath.Create<AbsoluteFilePath>(PathFor("afile"));
		IPath second = AbsoluteFilePath.Create<AbsoluteFilePath>(PathFor("bfile"));

		Assert.IsLessThan(0, first.CompareTo(second));
		Assert.IsGreaterThan(0, first.CompareTo(null));
	}

	/// <summary>
	/// The relative path types implement <see cref="IRelativePath.AsAbsolute()"/> explicitly so the
	/// interface returns the base <see cref="AbsolutePath"/> while the public overload returns the
	/// specific file or directory type. Only the explicit implementations are exercised here; the
	/// public overloads are covered elsewhere.
	/// </summary>
	[TestMethod]
	public void AsAbsolute_ThroughIRelativePath_ReturnsBaseAbsolutePath()
	{
		IRelativePath relativeFile = RelativeFilePath.Create<RelativeFilePath>(Path.Combine("sub", "afile.txt"));
		IRelativePath relativeDirectory = RelativeDirectoryPath.Create<RelativeDirectoryPath>(Path.Combine("sub", "child"));

		AbsolutePath absoluteFile = relativeFile.AsAbsolute();
		AbsolutePath absoluteDirectory = relativeDirectory.AsAbsolute();

		Assert.AreEqual(Path.GetFullPath(Path.Combine("sub", "afile.txt")), absoluteFile.WeakString);
		Assert.AreEqual(Path.GetFullPath(Path.Combine("sub", "child")), absoluteDirectory.WeakString);
	}
}
