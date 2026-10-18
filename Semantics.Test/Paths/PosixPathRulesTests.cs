// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// The reserved characters &lt;, &gt; and | and the 256-character limit are Windows rules. Linux and
/// macOS allow both, and git reports such paths there, so the path types must accept them.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Linux | OperatingSystems.OSX)]
[TestCategory("OS-Specific")]
public class PosixPathRulesTests
{
	[TestMethod]
	[DataRow("x|y.txt")]
	[DataRow("a<b>.txt")]
	[DataRow("dir/x|y.txt")]
	public void RelativeFilePath_WithWindowsReservedCharacter_IsAccepted(string path)
	{
		Assert.IsTrue(RelativeFilePath.TryCreate(path, out RelativeFilePath? result));
		Assert.AreEqual(path, result!.WeakString);
	}

	[TestMethod]
	public void RelativeFilePath_LongerThan260Characters_IsAccepted()
	{
		string path = string.Concat(System.Linq.Enumerable.Repeat("dir/", 30)) + new string('y', 150) + ".txt";
		Assert.IsGreaterThan(260, path.Length);

		Assert.IsTrue(RelativeFilePath.TryCreate(path, out RelativeFilePath? result));
		Assert.AreEqual(path, result!.WeakString);
	}

	[TestMethod]
	public void AbsoluteFilePath_WithWindowsReservedCharacter_IsAccepted()
	{
		Assert.IsTrue(AbsoluteFilePath.TryCreate("/home/user/x|y.txt", out AbsoluteFilePath? result));
		Assert.AreEqual("/home/user/x|y.txt", result!.WeakString);
	}

	[TestMethod]
	public void RelativeFilePath_WithNul_IsRefused() =>
		Assert.IsFalse(RelativeFilePath.TryCreate("x\0y.txt", out _));
}
