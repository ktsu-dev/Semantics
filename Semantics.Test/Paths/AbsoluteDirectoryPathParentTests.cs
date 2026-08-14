// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for walking upwards from an <see cref="AbsoluteDirectoryPath"/>.
/// </summary>
[TestClass]
public class AbsoluteDirectoryPathParentTests
{
	private static AbsoluteDirectoryPath Dir(string path) => AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(path);

	private static string Nested => OperatingSystem.IsWindows() ? @"C:\Users\user\Documents" : "/home/user/Documents";

	private static string Middle => OperatingSystem.IsWindows() ? @"C:\Users\user" : "/home/user";

	private static string BelowRoot => OperatingSystem.IsWindows() ? @"C:\Users" : "/home";

	private static string Root => OperatingSystem.IsWindows() ? @"C:\" : "/";

	[TestMethod]
	public void Parent_ReturnsTheContainingDirectory()
	{
		Assert.AreEqual(Middle, Dir(Nested).Parent.WeakString);
		Assert.AreEqual(BelowRoot, Dir(Middle).Parent.WeakString);
		Assert.AreEqual(Root, Dir(BelowRoot).Parent.WeakString);
	}

	[TestMethod]
	public void Parent_OfRoot_ReturnsTheRootItself()
	{
		AbsoluteDirectoryPath root = Dir(Root);

		Assert.IsTrue(root.IsRoot, $"'{root}' should be a root.");
		Assert.AreEqual(root, root.Parent);
	}

	[TestMethod]
	public void Parent_OfUncShareRoot_ReturnsTheShareItself()
	{
		if (!OperatingSystem.IsWindows())
		{
			Assert.Inconclusive("UNC paths are Windows-only.");
			return;
		}

		Assert.AreEqual(@"\\server\share", Dir(@"\\server\share\folder").Parent.WeakString);

		AbsoluteDirectoryPath share = Dir(@"\\server\share");
		Assert.AreEqual(share, share.Parent);
	}

	/// <summary>
	/// Regression test for ktsu-dev/ImGuiApp#281. A root used to report an empty parent, which still passes
	/// <see cref="AbsoluteDirectoryPath"/> validation but is not a location: passed to the filesystem it reads
	/// as "no contents" rather than failing, so callers walking upwards silently landed nowhere.
	/// </summary>
	[TestMethod]
	public void Parent_IsAlwaysAUsableAbsolutePath()
	{
		AbsoluteDirectoryPath current = Dir(Environment.CurrentDirectory);
		int stepsPastRoot = current.Depth + 2;

		for (int step = 0; step < stepsPastRoot; step++)
		{
			AbsoluteDirectoryPath parent = current.Parent;

			Assert.IsFalse(string.IsNullOrEmpty(parent.WeakString), $"The parent of '{current}' should not be empty.");
			Assert.IsTrue(
				Path.IsPathFullyQualified(parent.WeakString),
				$"The parent of '{current}' should be fully qualified, but was '{parent}'.");

			current = parent;
		}

		Assert.IsTrue(current.IsRoot, $"Walking upwards should settle on a root, but settled on '{current}'.");
	}

	[TestMethod]
	public void Parent_OfRoot_IsAFixedPointSoWalkingUpwardsTerminates()
	{
		AbsoluteDirectoryPath current = Dir(Environment.CurrentDirectory);
		int steps = 0;

		while (!current.IsRoot)
		{
			current = current.Parent;
			Assert.IsLessThan(1000, ++steps, "Walking upwards should terminate at a root.");
		}

		Assert.AreEqual(current, current.Parent);
	}

	[TestMethod]
	public void GetAncestors_EndsAtTheRootWithoutRepeatingIt()
	{
		List<AbsoluteDirectoryPath> ancestors = [.. Dir(Nested).GetAncestors()];

		// Compare the underlying string values: AbsoluteDirectoryPath implements IEnumerable<char>,
		// and Assert.AreSequenceEqual recurses into elements that are themselves sequences rather
		// than using the record's value equality.
		List<string> expected = [Middle, BelowRoot, Root];

		Assert.AreSequenceEqual(
			expected,
			ancestors.Select(a => a.WeakString),
			$"Expected the chain up to the root, got [{string.Join(", ", ancestors.Select(a => a.WeakString))}].");
	}

	[TestMethod]
	public void GetAncestors_OfRoot_IsEmpty()
	{
		Assert.IsEmpty(Dir(Root).GetAncestors());
	}
}
