// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test;

using System;
using System.Runtime.ExceptionServices;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Issue #443: rejecting a value through <c>TryCreate</c> throws nothing. Issue #444: a type's validation
/// attributes are read once rather than per value.
/// </summary>
[TestClass]
public sealed class SemanticStringCreationCostTests
{
	[TestMethod]
	public void TryCreate_RejectsWithoutThrowing()
	{
		// Tests run in parallel, so only exceptions thrown on this test's own thread count.
		int thisThread = Environment.CurrentManagedThreadId;
		int exceptionsOnThisThread = 0;
		void Count(object? sender, FirstChanceExceptionEventArgs e)
		{
			if (Environment.CurrentManagedThreadId == thisThread)
			{
				exceptionsOnThisThread++;
			}
		}

		AppDomain.CurrentDomain.FirstChanceException += Count;
		try
		{

			bool fromString = StartsWithValidSemanticString.TryCreate("Nope", out StartsWithValidSemanticString? a);
			bool fromChars = StartsWithValidSemanticString.TryCreate("Nope".ToCharArray(), out StartsWithValidSemanticString? b);
			bool fromSpan = StartsWithValidSemanticString.TryCreate("Nope".AsSpan(), out StartsWithValidSemanticString? c);
			bool fromNull = StartsWithValidSemanticString.TryCreate((string?)null, out StartsWithValidSemanticString? d);

			Assert.IsFalse(fromString || fromChars || fromSpan || fromNull);
			Assert.IsNull(a ?? b ?? c ?? d);
			Assert.AreEqual(0, exceptionsOnThisThread);
		}
		finally
		{
			AppDomain.CurrentDomain.FirstChanceException -= Count;
		}
	}

	[TestMethod]
	public void TryCreate_AcceptsAValidValue()
	{
		Assert.IsTrue(StartsWithValidSemanticString.TryCreate("Valid value", out StartsWithValidSemanticString? result));
		Assert.AreEqual("Valid value", result?.WeakString);
	}

	[TestMethod]
	public void Create_StillThrowsWithTheReasons()
	{
		ArgumentException error = Assert.ThrowsExactly<ArgumentException>(() => StartsWithValidSemanticString.Create("Nope"));
		StringAssert.Contains(error.Message, "\"Nope\"");
		StringAssert.Contains(error.Message, nameof(StartsWithValidSemanticString));
	}

	[TestMethod]
	public void ValidationAttributes_AreReadOncePerType()
	{
		Type type = typeof(StartsWithValidSemanticString);
		SemanticStringValidationAttribute[] first = ValidationAttributeCache.Attributes(type);

		Assert.HasCount(1, first);
		Assert.AreSame(first, ValidationAttributeCache.Attributes(type));
	}
}
