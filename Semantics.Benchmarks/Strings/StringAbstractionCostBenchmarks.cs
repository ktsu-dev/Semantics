// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Strings;

using System;
using System.Text.RegularExpressions;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using ktsu.Semantics.Strings.Identifiers;

/// <summary>
/// Measures what the semantic string types cost over the code a caller would otherwise write.
/// </summary>
/// <remarks>
/// <para>
/// <b>What the baseline side is, and why it is not a bare string.</b> The quantities suite pairs
/// <c>T + T</c> against <c>Length&lt;T&gt; + Length&lt;T&gt;</c>, which is fair because both sides
/// do identical work and the only question is what the wrapper adds. There is no such pairing for
/// <c>Uuid.Create</c>: the bare-string counterpart is an assignment, which is no work at all.
/// Against that, the ratio would be a large number restating only that validation is not free,
/// which needs no benchmark to establish.
/// </para>
/// <para>
/// So the baseline side here is the code a caller would otherwise have written — the same pattern
/// matched by hand, and the same throw on failure. Read that way the ratio answers the question a
/// caller actually has: <i>I was going to validate this anyway, so what does routing it through
/// the type cost me on top?</i> The answer separates into the validation both sides pay and the
/// per-call reflection only one side does.
/// </para>
/// <para>
/// <b>The pattern is duplicated on purpose.</b> <c>StringSpecimens.UuidPattern</c> is the same
/// string <c>IsUuidAttribute</c> holds. If the two ever drift the comparison stops being one, so
/// it is worth saying here: the constant exists to be kept identical, not to be tuned.
/// </para>
/// <para>
/// <b>The last two categories are fair pairs in the quantities sense</b> and are expected near
/// 1.00, because a semantic string's equality and ordering are the underlying string's own.
/// </para>
/// <para>
/// <b>Why these are loops.</b> A single call over an operand that does not change is
/// loop-invariant and the JIT hoists it, and a ratio between two hoisted methods means nothing.
/// Each iteration here feeds the next through an accumulator, so there is nothing to hoist and
/// both sides of a pair stay measurable. Both sides also pay the same counter and branch, which
/// pulls the ratio toward 1.00 rather than away from it, so a ratio above 1.00 is a floor on the
/// real cost rather than the whole of it.
/// </para>
/// </remarks>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class StringAbstractionCostBenchmarks
{
	/// <summary>
	/// Operations per invocation. Enough that the loop's own cost is a small share of the work.
	/// </summary>
	private const int Operations = 64;

	private string text = "";
	private string rejected = "";
	private string pattern = "";
	private Uuid left = null!;
	private Uuid right = null!;

	/// <summary>Prepares both sides of every pair.</summary>
	[GlobalSetup]
	public void Setup()
	{
		text = StringSpecimens.UuidText;
		rejected = StringSpecimens.UuidRejected;
		pattern = StringSpecimens.UuidPattern;
		left = Uuid.Create(StringSpecimens.UuidText);
		right = Uuid.Create(StringSpecimens.UuidText);
	}

	/// <summary>Validating at a boundary by hand, throwing on rejection.</summary>
	/// <returns>The accumulated length, returned so nothing here is dead code.</returns>
	[BenchmarkCategory("Validate")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareValidate()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (!Regex.IsMatch(text, pattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
			{
				throw new ArgumentException("unreachable: the specimen is valid");
			}

			accumulator += text.Length;
		}

		return accumulator;
	}

	/// <summary>Validating at a boundary through the type.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("Validate")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticValidate()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += Uuid.Create(text).Length;
		}

		return accumulator;
	}

	/// <summary>Rejecting by hand without throwing.</summary>
	/// <returns>The count of rejections, which is every iteration.</returns>
	[BenchmarkCategory("Reject")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareReject()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (!Regex.IsMatch(rejected, pattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Rejecting through the type without throwing.</summary>
	/// <returns>The count of rejections, which is every iteration.</returns>
	[BenchmarkCategory("Reject")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticReject()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (!Uuid.TryCreate(rejected, out _))
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Ordinal equality on the bare strings.</summary>
	/// <returns>The count of matches, which is every iteration.</returns>
	[BenchmarkCategory("Equality")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareEquality()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (string.Equals(left.WeakString, right.WeakString, StringComparison.Ordinal))
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Record equality on the semantic values holding those strings.</summary>
	/// <returns>The count of matches, which is every iteration.</returns>
	[BenchmarkCategory("Equality")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticEquality()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (left == right)
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Ordering on the bare strings.</summary>
	/// <returns>The accumulated comparison results.</returns>
	[BenchmarkCategory("Ordering")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareOrdering()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += string.CompareOrdinal(left.WeakString, right.WeakString);
		}

		return accumulator;
	}

	/// <summary>Ordering on the semantic values.</summary>
	/// <returns>The accumulated comparison results.</returns>
	[BenchmarkCategory("Ordering")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticOrdering()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += left.CompareTo(right);
		}

		return accumulator;
	}
}
