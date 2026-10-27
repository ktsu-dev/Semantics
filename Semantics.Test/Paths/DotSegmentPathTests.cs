// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests that <c>.</c> and <c>..</c> segments are resolved by <see cref="AbsoluteDirectoryPath.IsRoot"/>,
/// <see cref="DirectoryPath.Parent"/> and both <c>Depth</c> properties, and that a directory listing survives an
/// entry the library cannot represent.
/// </summary>
[TestClass]
public class DotSegmentPathTests
{
	private static AbsoluteDirectoryPath Absolute(string path) => AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(path);

	private static RelativeDirectoryPath Relative(string path) => RelativeDirectoryPath.Create<RelativeDirectoryPath>(path);

	private static DirectoryPath Polymorphic(string path) => DirectoryPath.Create<DirectoryPath>(path);

	private static readonly string[] ValidEntries = ["ok.txt", "sub"];

	private static readonly string Sep = Path.DirectorySeparatorChar.ToString();

	/// <summary>Issue #479: a path that resolves to a root is a root.</summary>
	[TestMethod]
	public void IsRoot_IsTrueExactlyWhenParentIsThePathItself()
	{
		string[] paths =
		[
			TestPaths.Root,
			TestPaths.Absolute("home", ".."),
			TestPaths.Absolute("home", "user", "..", ".."),
			TestPaths.Absolute("home"),
			TestPaths.Absolute("home", "user", ".."),
		];

		foreach (string path in paths)
		{
			AbsoluteDirectoryPath dir = Absolute(path);
			Assert.AreEqual(ReferenceEquals(dir.Parent, dir), dir.IsRoot, path);
		}

		Assert.IsTrue(Absolute(TestPaths.Absolute("home", "..")).IsRoot);
		Assert.IsFalse(Absolute(TestPaths.Absolute("home", "user", "..")).IsRoot);
	}

	/// <summary>Issue #479: the documented upward walk stops at the root from a dotted path.</summary>
	[TestMethod]
	public void WalkingUpwards_FromADottedPath_StopsAtTheRoot()
	{
		foreach (string start in new[] { TestPaths.Absolute("home", ".."), TestPaths.Absolute("a", "b", "..", "c", ".") })
		{
			AbsoluteDirectoryPath dir = Absolute(start);
			int steps = 0;
			while (!dir.IsRoot)
			{
				dir = dir.Parent;
				Assert.IsLessThan(10, ++steps, $"Walking up from '{start}' did not stop.");
			}

			Assert.AreEqual(TestPaths.Root, Path.GetFullPath(dir.WeakString));
		}
	}

	/// <summary>Issue #478: the polymorphic primitive resolves dot segments like the typed paths do.</summary>
	[TestMethod]
	public void DirectoryPathParent_ResolvesDotSegments()
	{
		Assert.AreEqual(TestPaths.Root, Polymorphic(TestPaths.Absolute("home", "user", "..")).Parent.WeakString);
		Assert.AreEqual(TestPaths.Absolute("home"), Polymorphic(TestPaths.Absolute("home", "user", ".")).Parent.WeakString);
		Assert.AreEqual("..", Polymorphic("a" + Sep + "..").Parent.WeakString);
		Assert.AreEqual(".." + Sep + "..", Polymorphic("..").Parent.WeakString);
		Assert.AreEqual("a", Polymorphic("a" + Sep + "." + Sep + "b").Parent.WeakString);
	}

	[TestMethod]
	public void DirectoryPathParent_OfARoot_IsEmpty()
	{
		Assert.AreEqual("", Polymorphic(TestPaths.Root).Parent.WeakString);
		Assert.AreEqual("", Polymorphic(TestPaths.Absolute("home", "..")).Parent.WeakString);
	}

	/// <summary>Issue #466: depth counts directories below the root, not separator characters.</summary>
	[TestMethod]
	public void AbsoluteDepth_CountsDirectoriesBelowTheRoot()
	{
		Assert.AreEqual(0, Absolute(TestPaths.Root).Depth);
		Assert.AreEqual(1, Absolute(TestPaths.Absolute("home")).Depth);
		Assert.AreEqual(1, Absolute(TestPaths.Absolute("home") + Sep).Depth);
		Assert.AreEqual(2, Absolute(TestPaths.Absolute("home", "user")).Depth);
		Assert.AreEqual(1, Absolute(TestPaths.Absolute("home", "user", "..")).Depth);
		Assert.AreEqual(0, Absolute(TestPaths.Absolute("home", "..")).Depth);
	}

	[TestMethod]
	public void AbsoluteDepth_IsOneMoreThanTheParents()
	{
		AbsoluteDirectoryPath dir = Absolute(TestPaths.Absolute("a", "b", "c"));
		while (!dir.IsRoot)
		{
			Assert.AreEqual(dir.Parent.Depth + 1, dir.Depth, dir.WeakString);
			dir = dir.Parent;
		}
	}

	[TestMethod]
	[OSCondition(OperatingSystems.Windows)]
	[TestCategory("OS-Specific")]
	public void AbsoluteDepth_OnWindowsRoots()
	{
		Assert.AreEqual(0, Absolute(@"C:\").Depth);
		Assert.AreEqual(1, Absolute(@"C:\Users").Depth);
	}

	[TestMethod]
	public void RelativeDepth_IsTakenFromTheNormalizedPath()
	{
		Assert.AreEqual(0, Relative("a").Depth);
		Assert.AreEqual(1, Relative("a" + Sep + "b").Depth);
		Assert.AreEqual(0, Relative("." + Sep + "a").Depth);
		Assert.AreEqual(0, Relative("a" + Sep).Depth);
		Assert.AreEqual(0, Relative("a" + Sep + "..").Depth);
		Assert.AreEqual(0, Relative(".").Depth);
		Assert.AreEqual(1, Relative("a" + Sep + "." + Sep + "b").Depth);
	}

	/// <summary>
	/// A directory type whose entries fail validation by name, standing in for the names and lengths only some
	/// platforms reject, so the listing contract is tested on every platform.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Created through SemanticString.Create<T>")]
	private sealed record PickyDirectoryPath : SemanticDirectoryPath<PickyDirectoryPath>
	{
		protected override IFilePath CreateFilePath(string filePath) =>
			Path.GetFileName(filePath) == "bad.txt"
				? throw new ArgumentException("Rejected for the test.", nameof(filePath))
				: base.CreateFilePath(filePath);
	}

	/// <summary>Issue #445: one entry that fails validation does not hide the rest.</summary>
	[TestMethod]
	public void GetContents_SkipsAnEntryThatFailsValidation()
	{
		string root = Path.Join(Path.GetTempPath(), "semantics-contents-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(root);
		try
		{
			File.WriteAllText(Path.Join(root, "ok.txt"), "ok");
			Directory.CreateDirectory(Path.Join(root, "sub"));
			File.WriteAllText(Path.Join(root, "bad.txt"), "rejected");

			PickyDirectoryPath dir = PickyDirectoryPath.Create<PickyDirectoryPath>(root);
			List<string> names = [.. dir.GetContents().Select(p => Path.GetFileName(p.ToString()) ?? "")];

			CollectionAssert.AreEquivalent(ValidEntries, names);
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	/// <summary>Issue #445, with the real Windows length limit rather than a stand-in.</summary>
	[TestMethod]
	[OSCondition(OperatingSystems.Windows)]
	[TestCategory("OS-Specific")]
	public void GetContents_SkipsAnEntryPastTheWindowsLengthLimit()
	{
		string root = Path.Join(Path.GetTempPath(), "semantics-contents-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(root);
		try
		{
			File.WriteAllText(Path.Join(root, "ok.txt"), "ok");
			Directory.CreateDirectory(Path.Join(root, "sub"));
			File.WriteAllText(Path.Join(root, new string('x', 250) + ".txt"), "too long");

			List<string> names = [.. Absolute(root).GetContents().Select(p => Path.GetFileName(p.ToString()) ?? "")];

			CollectionAssert.AreEquivalent(ValidEntries, names);
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}
}
