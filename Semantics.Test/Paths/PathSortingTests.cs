// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Paths;

using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers sorting collections typed as path interfaces. Those interfaces carry no
/// <see cref="IComparable{T}"/>, so LINQ and <see cref="Comparer{T}.Default"/> fall back to the
/// non-generic <see cref="IComparable.CompareTo(object)"/> and hand it a path instance rather than
/// a <see cref="string"/>. Reported downstream as ktsu-dev/ImGuiApp#273.
/// </summary>
[TestClass]
public class PathSortingTests
{
	private static AbsoluteDirectoryPath Directory(string path) => AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(path);

	private static AbsoluteFilePath File(string path) => AbsoluteFilePath.Create<AbsoluteFilePath>(path);

	[TestMethod]
	public void OrderBy_AbsolutePathInterface_SortsByPathValue()
	{
		List<IAbsolutePath> contents =
		[
			File("/tmp/bfile"),
			Directory("/tmp/zebra"),
			File("/tmp/afile"),
			Directory("/tmp/alpha"),
		];

		List<string> sorted = [.. contents.OrderBy(p => p).Select(p => p.WeakString)];

		CollectionAssert.AreEqual(
			new List<string> { "/tmp/afile", "/tmp/alpha", "/tmp/bfile", "/tmp/zebra" },
			sorted);
	}

	[TestMethod]
	public void OrderBy_MixedDirectoriesAndFiles_DirectoriesFirstThenByPath()
	{
		List<IAbsolutePath> contents =
		[
			File("/tmp/bfile"),
			Directory("/tmp/zebra"),
			File("/tmp/afile"),
			Directory("/tmp/alpha"),
		];

		List<string> sorted =
		[
			.. contents
				.OrderBy(p => p is not AbsoluteDirectoryPath)
				.ThenBy(p => p)
				.Select(p => p.WeakString)
		];

		CollectionAssert.AreEqual(
			new List<string> { "/tmp/alpha", "/tmp/zebra", "/tmp/afile", "/tmp/bfile" },
			sorted);
	}

	[TestMethod]
	public void DefaultComparer_PathInterface_ComparesByPathValue()
	{
		IAbsolutePath first = File("/tmp/afile");
		IAbsolutePath second = Directory("/tmp/bdir");

		Assert.IsLessThan(0, Comparer<IAbsolutePath>.Default.Compare(first, second));
		Assert.IsGreaterThan(0, Comparer<IAbsolutePath>.Default.Compare(second, first));
		Assert.AreEqual(0, Comparer<IAbsolutePath>.Default.Compare(first, File("/tmp/afile")));
	}

	[TestMethod]
	public void Sort_ListOfPathInterface_DoesNotThrow()
	{
		List<IPath> contents =
		[
			File("/tmp/bfile"),
			Directory("/tmp/adir"),
			File("/tmp/cfile"),
		];

		contents.Sort();

		CollectionAssert.AreEqual(
			new List<string> { "/tmp/adir", "/tmp/bfile", "/tmp/cfile" },
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
