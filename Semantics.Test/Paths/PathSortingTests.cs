// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers sorting collections typed as path interfaces. Those interfaces carry no
/// <see cref="IComparable{T}"/> of their own type, so LINQ and <see cref="Comparer{T}.Default"/> fall
/// back to the non-generic <see cref="IComparable.CompareTo(object)"/> and hand it a path instance
/// rather than a <see cref="string"/>. Reported downstream as ktsu-dev/ImGuiApp#273.
/// </summary>
[TestClass]
public class PathSortingTests
{
	/// <summary>
	/// An absolute root that is valid on every platform: <c>C:\</c> on Windows, <c>/</c> elsewhere.
	/// </summary>
	private static readonly string Root = System.IO.Path.Combine(
		System.IO.Path.GetPathRoot(System.IO.Path.GetTempPath())!,
		nameof(PathSortingTests));

	private static string PathFor(string name) => System.IO.Path.Combine(Root, name);

	private static AbsoluteDirectoryPath MakeDirectory(string name) =>
		AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(PathFor(name));

	private static AbsoluteFilePath MakeFile(string name) =>
		AbsoluteFilePath.Create<AbsoluteFilePath>(PathFor(name));

	[TestMethod]
	public void OrderBy_AbsolutePathInterface_SortsByPathValue()
	{
		List<IAbsolutePath> contents =
		[
			MakeFile("bfile"),
			MakeDirectory("zebra"),
			MakeFile("afile"),
			MakeDirectory("alpha"),
		];

		List<string> sorted = [.. contents.OrderBy(p => p).Select(p => p.WeakString)];

		CollectionAssert.AreEqual(
			new List<string> { PathFor("afile"), PathFor("alpha"), PathFor("bfile"), PathFor("zebra") },
			sorted);
	}

	[TestMethod]
	public void OrderBy_MixedDirectoriesAndFiles_DirectoriesFirstThenByPath()
	{
		List<IAbsolutePath> contents =
		[
			MakeFile("bfile"),
			MakeDirectory("zebra"),
			MakeFile("afile"),
			MakeDirectory("alpha"),
		];

		List<string> sorted =
		[
			.. contents
				.OrderBy(p => p is not AbsoluteDirectoryPath)
				.ThenBy(p => p)
				.Select(p => p.WeakString)
		];

		CollectionAssert.AreEqual(
			new List<string> { PathFor("alpha"), PathFor("zebra"), PathFor("afile"), PathFor("bfile") },
			sorted);
	}

	[TestMethod]
	public void DefaultComparer_PathInterface_ComparesByPathValue()
	{
		IAbsolutePath first = MakeFile("afile");
		IAbsolutePath second = MakeDirectory("bdir");

		Assert.IsLessThan(0, Comparer<IAbsolutePath>.Default.Compare(first, second));
		Assert.IsGreaterThan(0, Comparer<IAbsolutePath>.Default.Compare(second, first));
		Assert.AreEqual(0, Comparer<IAbsolutePath>.Default.Compare(first, MakeFile("afile")));
	}

	[TestMethod]
	public void Sort_ListOfPathInterface_DoesNotThrow()
	{
		List<IPath> contents =
		[
			MakeFile("bfile"),
			MakeDirectory("adir"),
			MakeFile("cfile"),
		];

		contents.Sort();

		CollectionAssert.AreEqual(
			new List<string> { PathFor("adir"), PathFor("bfile"), PathFor("cfile") },
			contents.Select(p => p.WeakString).ToList());
	}

	[TestMethod]
	public void DefaultComparer_ForPathInterface_UsesGenericComparison()
	{
		// IPath now declares IComparable<IPath>, so the default comparer must be the generic one
		// rather than ObjectComparer, which boxes into the non-generic IComparable.CompareTo(object).
		string comparerName = Comparer<IPath>.Default.GetType().Name;

		Assert.IsTrue(comparerName.StartsWith("GenericComparer", StringComparison.Ordinal), $"Actual comparer: {comparerName}");
	}
}
