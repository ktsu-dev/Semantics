// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Strings;

using BenchmarkDotNet.Attributes;

using ktsu.Semantics.Strings.Identifiers;

/// <summary>
/// Measures what a semantic string costs after it has been created.
/// </summary>
/// <remarks>
/// <para>
/// Creation is where a semantic string spends its cost, and the rest of this suite says so. This
/// class is the other half of that claim: once a value exists, an operation on it should be the
/// underlying <see cref="string"/>'s own work plus a wrapper, and the rows here are how far that
/// holds.
/// </para>
/// <para>
/// <b>Two of them are creation in disguise, which is the point of including them.</b>
/// <see cref="AsConversion"/> and <see cref="WithSuffix"/> both route back through
/// <c>Create</c> on the target type, so each pays the full reflection and validation cost a
/// <c>StringCreationBenchmarks</c> row measures. They read as ordinary member calls at a call
/// site, and they are not, and that is worth being able to point at.
/// </para>
/// <para>
/// <see cref="WithSuffix"/> runs on <see cref="PlainText"/> rather than on an identifier, because
/// appending to a <see cref="Uuid"/> produces a value its own validator rejects. Measuring the
/// throw is <c>StringCreationBenchmarks.CreateThrows</c>'s job, not this one's.
/// </para>
/// </remarks>
[MemoryDiagnoser]
public class StringOperationBenchmarks
{
	private Uuid left = null!;
	private Uuid right = null!;
	private PlainText plain = null!;
	private string suffix = "";

	/// <summary>Builds the operands once, outside the measurement.</summary>
	[GlobalSetup]
	public void Setup()
	{
		left = Uuid.Create(StringSpecimens.UuidText);
		right = Uuid.Create(StringSpecimens.UuidText);
		plain = PlainText.Create(StringSpecimens.PlainInput);
		suffix = "-suffixed";
	}

	/// <summary>Record equality over two equal values, which is the worst case for it.</summary>
	/// <returns>Whether the two are equal, which is always true here.</returns>
	[Benchmark]
	public bool EqualityOperator() => left == right;

	/// <summary>Ordering, which routes through the underlying string's comparison.</summary>
	/// <returns>The comparison result.</returns>
	[Benchmark]
	public int CompareTo() => left.CompareTo(right);

	/// <summary>Hashing, which a dictionary of semantic strings pays on every lookup.</summary>
	/// <returns>The hash code.</returns>
	[Benchmark]
	public int HashCode() => left.GetHashCode();

	/// <summary>Cross-type conversion, which is a full creation against the target type.</summary>
	/// <returns>The converted value.</returns>
	[Benchmark]
	public PlainText AsConversion() => left.As<PlainText>();

	/// <summary>Appending, which is also a full creation against the same type.</summary>
	/// <returns>The extended value.</returns>
	[Benchmark]
	public PlainText WithSuffix() => plain.WithSuffix(suffix);

	/// <summary>The implicit conversion back out, which should be a field read.</summary>
	/// <returns>The underlying string.</returns>
	[Benchmark]
	public string ToStringImplicit() => left;
}
