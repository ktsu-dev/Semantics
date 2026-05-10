// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Quantities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Locks in invariants on the source generator's output. Issue #57 raised the concern that
/// the generator's dedup keys could let two methods (or operators) with the same name and
/// parameter types land on the same type — which the C# compiler already rejects, but this
/// test makes the property explicit and regression-proof if the dedup logic changes.
/// </summary>
[TestClass]
public sealed class GeneratorOutputInvariantTests
{
	/// <summary>
	/// For every generated quantity type in <c>ktsu.Semantics.Quantities</c>, no two public
	/// static methods (including operators) share both the same name and the same parameter
	/// type list. Walks the runtime assembly rather than re-parsing the .g.cs files because
	/// the compiled types are the source of truth — anything the test sees is what consumers
	/// would call.
	/// </summary>
	[TestMethod]
	public void NoDuplicatePublicStaticMethodsOrOperatorsPerGeneratedType()
	{
		Assembly assembly = typeof(Mass<>).Assembly;
		List<Type> generatedQuantityTypes = [.. CollectGeneratedQuantityTypes(assembly)];

		// Sanity: we should be looking at a non-trivial set, otherwise the test is silently
		// vacuous (e.g. namespace got renamed and the filter dropped everything).
		Assert.IsTrue(
			generatedQuantityTypes.Count > 50,
			$"Expected to find many generated quantity types (got {generatedQuantityTypes.Count}). The filter likely needs updating.");

		List<string> failures = [];
		foreach (Type type in generatedQuantityTypes)
		{
			MethodInfo[] staticMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

			IEnumerable<IGrouping<string, MethodInfo>> groups = staticMethods.GroupBy(SignatureKey);
			foreach (IGrouping<string, MethodInfo> group in groups)
			{
				int count = group.Count();
				if (count > 1)
				{
					failures.Add($"{type.Name}: {group.Key} appears {count} times");
				}
			}
		}

		if (failures.Count > 0)
		{
			Assert.Fail(
				$"Found duplicate public static method signatures on generated quantity types:\n  " +
				string.Join("\n  ", failures));
		}
	}

	/// <summary>
	/// Cross-dimensional <c>operator *</c> overloads should exist in both operand orders so
	/// either-order user code (<c>mass * accel</c> and <c>accel * mass</c>) compiles. The
	/// generator's <c>CollectAllOperators</c> emits both directions; this test asserts the
	/// commutativity property explicitly so a regression in the dedup keys would fail here
	/// before it reaches a downstream consumer.
	/// </summary>
	[TestMethod]
	public void EveryCrossDimensionalMultiplicationHasBothOperandOrders()
	{
		Assembly assembly = typeof(Mass<>).Assembly;
		List<Type> types = [.. CollectGeneratedQuantityTypes(assembly)];

		// Collect every observed operator * signature as a tuple (left, right, returnType).
		HashSet<(string Left, string Right, string Result)> observed = [];
		foreach (Type type in types)
		{
			foreach (MethodInfo m in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
			{
				if (m.Name != "op_Multiply")
				{
					continue;
				}

				ParameterInfo[] pars = m.GetParameters();
				if (pars.Length != 2)
				{
					continue;
				}

				observed.Add((pars[0].ParameterType.Name, pars[1].ParameterType.Name, m.ReturnType.Name));
			}
		}

		// For every cross-dimensional product (operands of distinct types), the swapped pair
		// should also be present with the same return. Same-type products (T * T) are exempt
		// because there is no swap — they're idempotent under reorder.
		List<string> missing = [];
		foreach ((string left, string right, string result) in observed)
		{
			if (left == right)
			{
				continue;
			}

			if (!observed.Contains((right, left, result)))
			{
				missing.Add($"missing reverse pair: {right} * {left} -> {result} (forward {left} * {right} -> {result} exists)");
			}
		}

		if (missing.Count > 0)
		{
			Assert.Fail(
				"Cross-dimensional multiplication should be emitted in both operand orders, but found " +
				$"{missing.Count} unmatched forward(s):\n  " +
				string.Join("\n  ", missing));
		}
	}

	private static IEnumerable<Type> CollectGeneratedQuantityTypes(Assembly assembly)
	{
		foreach (Type type in assembly.GetTypes())
		{
			if (type.Namespace != "ktsu.Semantics.Quantities")
			{
				continue;
			}

			if (!type.IsGenericTypeDefinition)
			{
				continue;
			}

			// Generated quantity types implement one of IVector0..IVector4 (closed over TSelf, T).
			bool isQuantity = type.GetInterfaces().Any(static i =>
				i.IsGenericType && i.Name.StartsWith("IVector", StringComparison.Ordinal));
			if (!isQuantity)
			{
				continue;
			}

			yield return type;
		}
	}

	private static string SignatureKey(MethodInfo m)
	{
		string parameterList = string.Join(",", m.GetParameters().Select(static p => p.ParameterType.FullName ?? p.ParameterType.Name));
		return $"{m.Name}({parameterList})";
	}
}
