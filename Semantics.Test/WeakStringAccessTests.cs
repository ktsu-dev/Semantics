// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test;

using System;
using System.IO;
using System.Reflection;
using ktsu.Semantics.Paths;
using ktsu.Semantics.Strings;
using ktsu.Semantics.Strings.Identifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// A semantic string's value only arrives through <c>Create</c>, which canonicalizes and validates it.
/// A public init accessor let <c>uuid with { WeakString = "not a uuid" }</c> skip both (#310).
/// </summary>
[TestClass]
public class WeakStringAccessTests
{
	[TestMethod]
	public void WeakString_HasNoPublicSetter()
	{
		Type[] types =
		[
			typeof(MySemanticString),
			typeof(Uuid),
			typeof(AbsoluteDirectoryPath),
			typeof(AbsoluteFilePath),
			typeof(RelativeDirectoryPath),
		];

		foreach (Type type in types)
		{
			PropertyInfo property = type.GetProperty(nameof(ISemanticString.WeakString))!;

			Assert.IsNotNull(property.GetGetMethod(), $"{type.Name}.WeakString should stay publicly readable.");
			Assert.IsNull(property.GetSetMethod(), $"{type.Name}.WeakString should not be publicly settable.");
		}
	}

	[TestMethod]
	public void ISemanticString_WeakString_IsReadOnly()
	{
		PropertyInfo property = typeof(ISemanticString).GetProperty(nameof(ISemanticString.WeakString))!;

		Assert.IsFalse(property.CanWrite);
	}

	[TestMethod]
	public void Create_StillSetsTheValue()
	{
		const string value = "123e4567-e89b-12d3-a456-426614174000";

		Assert.AreEqual(value, Uuid.Create(value).WeakString);
		Assert.AreEqual("test", MySemanticString.Create("test").WeakString);
	}

	[TestMethod]
	public void Create_StillSetsThePathValue()
	{
		AbsoluteDirectoryPath parent = AbsoluteDirectoryPath.Create(Path.GetTempPath());
		AbsoluteDirectoryPath child = AbsoluteDirectoryPath.Create(Path.Join(parent.WeakString, "weakstring"));

		Assert.AreEqual(parent, child.Parent);
	}
}
