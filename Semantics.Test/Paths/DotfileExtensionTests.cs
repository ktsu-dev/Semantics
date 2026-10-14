// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Issue #325: a dotfile (<c>.bashrc</c>) has no extension, so <c>RemoveExtension</c> leaves it alone
/// and <c>ChangeExtension</c> appends to it, instead of treating the whole name as an extension.
/// </summary>
[TestClass]
public sealed class DotfileExtensionTests
{
	private static FileExtension Bak => FileExtension.Create<FileExtension>(".bak");

	// Absolute and relative dotfiles from the issue, iterated inside each test rather than passed
	// through [DataRow]: a public test method taking a string trips CA1062.
	private static readonly string[] AbsoluteDotfiles =
	[
		TestPaths.Absolute("home", "user", ".bashrc"),
		TestPaths.Absolute(".profile"),
	];

	private static readonly string[] RelativeDotfiles =
	[
		TestPaths.Relative(".gitignore"),
		TestPaths.Relative("src", ".editorconfig"),
	];

	[TestMethod]
	public void AbsoluteFilePath_Dotfile_RemoveExtension_KeepsName()
	{
		foreach (string path in AbsoluteDotfiles)
		{
			Assert.AreEqual(path, AbsoluteFilePath.Create<AbsoluteFilePath>(path).RemoveExtension().WeakString);
		}
	}

	[TestMethod]
	public void AbsoluteFilePath_Dotfile_ChangeExtension_Appends()
	{
		foreach (string path in AbsoluteDotfiles)
		{
			Assert.AreEqual(path + ".bak", AbsoluteFilePath.Create<AbsoluteFilePath>(path).ChangeExtension(Bak).WeakString);
		}
	}

	[TestMethod]
	public void RelativeFilePath_Dotfile_RemoveExtension_KeepsName()
	{
		foreach (string path in RelativeDotfiles)
		{
			Assert.AreEqual(path, RelativeFilePath.Create<RelativeFilePath>(path).RemoveExtension().WeakString);
		}
	}

	[TestMethod]
	public void RelativeFilePath_Dotfile_ChangeExtension_Appends()
	{
		foreach (string path in RelativeDotfiles)
		{
			Assert.AreEqual(path + ".bak", RelativeFilePath.Create<RelativeFilePath>(path).ChangeExtension(Bak).WeakString);
		}
	}

	[TestMethod]
	public void Dotfile_FileNameWithoutExtension_IsWholeName()
	{
		Assert.AreEqual(".bashrc", AbsoluteFilePath.Create<AbsoluteFilePath>(TestPaths.Absolute("home", "user", ".bashrc")).FileNameWithoutExtension.WeakString);
		Assert.AreEqual(".gitignore", RelativeFilePath.Create<RelativeFilePath>(".gitignore").FileNameWithoutExtension.WeakString);
	}

	[TestMethod]
	public void DotfileWithExtension_RemoveAndChangeExtension_TouchOnlyTheExtension()
	{
		RelativeFilePath path = RelativeFilePath.Create<RelativeFilePath>(TestPaths.Relative("src", ".config.json"));

		Assert.AreEqual(TestPaths.Relative("src", ".config"), path.RemoveExtension().WeakString);
		Assert.AreEqual(TestPaths.Relative("src", ".config.bak"), path.ChangeExtension(Bak).WeakString);
		Assert.AreEqual(".config", path.FileNameWithoutExtension.WeakString);
	}

	[TestMethod]
	public void DotInDirectoryName_IsNotAnExtension()
	{
		RelativeFilePath path = RelativeFilePath.Create<RelativeFilePath>(TestPaths.Relative("app.v2", "README"));

		Assert.AreEqual(path.WeakString, path.RemoveExtension().WeakString);
		Assert.AreEqual(TestPaths.Relative("app.v2", "README.bak"), path.ChangeExtension(Bak).WeakString);
	}
}
