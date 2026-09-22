// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Paths;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ktsu.Semantics.Paths;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// A path's identity is its text. Reading a derived property must not change whether the instance
/// compares equal to an identical one, nor what it hashes to.
/// </summary>
/// <remarks>
/// The path types memoize their derived properties in private instance fields. A record's generated
/// <c>Equals</c> and <c>GetHashCode</c> compare <em>every</em> instance field, so populating a cache on
/// first read used to move the instance out of its own hash bucket and stop it comparing equal to an
/// identical value — while its text stayed character-for-character the same. See
/// <see href="https://github.com/ktsu-dev/Semantics/issues/265"/>.
/// </remarks>
[TestClass]
public class PathCacheEqualityTests
{
	/// <summary>
	/// Asserts that reading <paramref name="read"/> leaves <paramref name="subject"/> equal to
	/// <paramref name="twin"/>, in every direction and by hash code.
	/// </summary>
	/// <typeparam name="T">The path type under test.</typeparam>
	/// <param name="subject">The instance the property is read from.</param>
	/// <param name="twin">An instance built from the same text, never read from.</param>
	/// <param name="read">Reads the derived property under test.</param>
	/// <param name="propertyName">The property being read, for the failure message.</param>
	private static void AssertStableAfterReading<T>(T subject, T twin, Action<T> read, string propertyName)
		where T : SemanticString<T>
	{
		Assert.AreEqual(twin, subject, $"{typeof(T).Name} was not equal to its twin before reading {propertyName}");

		read(subject);

		Assert.AreEqual(twin, subject, $"reading {typeof(T).Name}.{propertyName} broke Equals(twin, subject)");
		Assert.AreEqual(subject, twin, $"reading {typeof(T).Name}.{propertyName} broke Equals(subject, twin)");
		Assert.AreEqual(
			twin.GetHashCode(),
			subject.GetHashCode(),
			$"reading {typeof(T).Name}.{propertyName} changed the hash code");
	}

	[TestMethod]
	public void ReadingAbsoluteDirectoryPathLeavesTheFilePathEqualAndHashable()
	{
		string text = TestPaths.Absolute("tmp", "x", "y.schema.json");
		AbsoluteFilePath subject = AbsoluteFilePath.Create<AbsoluteFilePath>(text);
		AbsoluteFilePath twin = AbsoluteFilePath.Create<AbsoluteFilePath>(text);

		AssertStableAfterReading(subject, twin, path => _ = path.AbsoluteDirectoryPath, nameof(AbsoluteFilePath.AbsoluteDirectoryPath));
	}

	[TestMethod]
	public void ReadingFileNameWithoutExtensionLeavesTheAbsoluteFilePathEqualAndHashable()
	{
		string text = TestPaths.Absolute("tmp", "x", "y.schema.json");
		AbsoluteFilePath subject = AbsoluteFilePath.Create<AbsoluteFilePath>(text);
		AbsoluteFilePath twin = AbsoluteFilePath.Create<AbsoluteFilePath>(text);

		AssertStableAfterReading(subject, twin, path => _ = path.FileNameWithoutExtension, nameof(AbsoluteFilePath.FileNameWithoutExtension));
	}

	[TestMethod]
	public void ReadingEveryDerivedPropertyLeavesTheAbsoluteFilePathEqualAndHashable()
	{
		string text = TestPaths.Absolute("tmp", "x", "y.schema.json");
		AbsoluteFilePath subject = AbsoluteFilePath.Create<AbsoluteFilePath>(text);
		AbsoluteFilePath twin = AbsoluteFilePath.Create<AbsoluteFilePath>(text);

		AssertStableAfterReading(
			subject,
			twin,
			path =>
			{
				_ = path.AbsoluteDirectoryPath;
				_ = path.FileNameWithoutExtension;
				_ = path.FileName;
				_ = path.FileExtension;
			},
			"every derived property");
	}

	[TestMethod]
	public void ReadingEveryDerivedPropertyLeavesTheRelativeFilePathEqualAndHashable()
	{
		string text = TestPaths.Relative("x", "y.schema.json");
		RelativeFilePath subject = RelativeFilePath.Create<RelativeFilePath>(text);
		RelativeFilePath twin = RelativeFilePath.Create<RelativeFilePath>(text);

		AssertStableAfterReading(
			subject,
			twin,
			path =>
			{
				_ = path.RelativeDirectoryPath;
				_ = path.FileNameWithoutExtension;
				_ = path.FileName;
				_ = path.FileExtension;
			},
			"every derived property");
	}

	[TestMethod]
	public void ReadingEveryDerivedPropertyLeavesTheAbsoluteDirectoryPathEqualAndHashable()
	{
		string text = TestPaths.Absolute("tmp", "x", "src");
		AbsoluteDirectoryPath subject = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(text);
		AbsoluteDirectoryPath twin = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(text);

		AssertStableAfterReading(
			subject,
			twin,
			path =>
			{
				_ = path.Parent;
				_ = path.Name;
				_ = path.Depth;
				_ = path.IsRoot;
			},
			"every derived property");
	}

	[TestMethod]
	public void ReadingEveryDerivedPropertyLeavesTheRelativeDirectoryPathEqualAndHashable()
	{
		string text = TestPaths.Relative("projects", "app", "src");
		RelativeDirectoryPath subject = RelativeDirectoryPath.Create<RelativeDirectoryPath>(text);
		RelativeDirectoryPath twin = RelativeDirectoryPath.Create<RelativeDirectoryPath>(text);

		AssertStableAfterReading(
			subject,
			twin,
			path =>
			{
				_ = path.Parent;
				_ = path.Name;
				_ = path.Depth;
			},
			"every derived property");
	}

	[TestMethod]
	public void APathStaysFindableAsADictionaryKeyAfterItsDirectoryIsRead()
	{
		string text = TestPaths.Absolute("tmp", "x", "y.schema.json");
		AbsoluteFilePath key = AbsoluteFilePath.Create<AbsoluteFilePath>(text);
		Dictionary<AbsoluteFilePath, string> recentFiles = new() { [key] = "recent" };

		_ = key.AbsoluteDirectoryPath;

		Assert.IsTrue(
			recentFiles.ContainsKey(key),
			"the stored key could not be found in the dictionary it was stored in, after its directory was read");
		Assert.IsTrue(
			recentFiles.ContainsKey(AbsoluteFilePath.Create<AbsoluteFilePath>(text)),
			"an equal key could not be found in the dictionary after the stored key's directory was read");
	}

	[TestMethod]
	public void APathStaysFindableInASetAfterItsDirectoryIsRead()
	{
		string text = TestPaths.Absolute("tmp", "x", "y.schema.json");
		AbsoluteFilePath member = AbsoluteFilePath.Create<AbsoluteFilePath>(text);
		HashSet<AbsoluteFilePath> seen = [member];

		_ = member.AbsoluteDirectoryPath;

		Assert.IsTrue(seen.Contains(member), "the set no longer contains the member it was built from");
		Assert.IsFalse(
			seen.Add(AbsoluteFilePath.Create<AbsoluteFilePath>(text)),
			"an equal path was added to the set a second time after the first one's directory was read");
	}

	[TestMethod]
	public void DifferentPathsStayUnequal()
	{
		AbsoluteFilePath first = AbsoluteFilePath.Create<AbsoluteFilePath>(TestPaths.Absolute("tmp", "x", "y.schema.json"));
		AbsoluteFilePath second = AbsoluteFilePath.Create<AbsoluteFilePath>(TestPaths.Absolute("tmp", "x", "z.schema.json"));

		_ = first.AbsoluteDirectoryPath;
		_ = second.AbsoluteDirectoryPath;

		Assert.AreNotEqual(first, second);
	}

	/// <summary>
	/// Determines whether <paramref name="method"/> was written by hand rather than synthesized for the record.
	/// </summary>
	/// <param name="method">The candidate equality member, or <see langword="null"/> if the type declares none.</param>
	/// <returns><see langword="true"/> if the method exists and is not compiler-generated.</returns>
	/// <remarks>
	/// A record's generated <c>Equals</c> and <c>GetHashCode</c> are declared members of the type, so their presence
	/// alone proves nothing — the distinction is <see cref="CompilerGeneratedAttribute"/>. It must be read with
	/// <c>inherit: false</c>: a hand-written <c>GetHashCode</c> overrides the base record's generated one and would
	/// otherwise inherit its attribute, making every type look compiler-generated.
	/// </remarks>
	private static bool IsHandWritten(MethodInfo? method) =>
		method is not null && !method.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false);

	/// <summary>
	/// The structural rule behind the behavioural tests above, so a path type added later cannot
	/// reintroduce the defect by declaring a cache field and inheriting the generated equality.
	/// </summary>
	[TestMethod]
	public void EveryPathTypeWithInstanceFieldsDeclaresItsOwnEqualityMembers()
	{
		IEnumerable<Type> pathTypes = typeof(AbsoluteFilePath).Assembly
			.GetTypes()
			.Where(type => type is { IsClass: true, IsAbstract: false, IsPublic: true })
			.Where(typeof(ISemanticString).IsAssignableFrom);

		List<string> offenders = [];

		foreach (Type type in pathTypes)
		{
			FieldInfo[] instanceFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			if (instanceFields.Length == 0)
			{
				continue;
			}

			bool declaresGetHashCode = IsHandWritten(type.GetMethod(
				nameof(GetHashCode),
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
				binder: null,
				types: Type.EmptyTypes,
				modifiers: null));

			bool declaresEquals = IsHandWritten(type.GetMethod(
				nameof(Equals),
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
				binder: null,
				types: [type],
				modifiers: null));

			if (!declaresGetHashCode || !declaresEquals)
			{
				string fields = string.Join(", ", instanceFields.Select(field => field.Name));
				offenders.Add($"{type.Name} declares instance field(s) {fields} but no {(declaresEquals ? "" : $"Equals({type.Name}?)")}{(declaresEquals || declaresGetHashCode ? "" : " and no ")}{(declaresGetHashCode ? "" : "GetHashCode()")}");
			}
		}

		Assert.AreEqual(
			0,
			offenders.Count,
			"A record's generated equality includes every instance field, so a type that memoizes into one "
			+ "must declare Equals and GetHashCode that defer to WeakString only:"
			+ Environment.NewLine
			+ string.Join(Environment.NewLine, offenders));
	}
}
