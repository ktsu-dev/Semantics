// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Strings;

using System;

using BenchmarkDotNet.Attributes;

using ktsu.Semantics.Strings.Identifiers;

/// <summary>
/// Measures creating a semantic string, across the range of validation a type can declare.
/// </summary>
/// <remarks>
/// <para>
/// <b>The axis here is validation weight.</b> A semantic string is a record wrapping a
/// <see cref="string"/>, so its cost is concentrated at creation, and what differs between types
/// is what their attributes do. <see cref="Unvalidated"/> is the floor: the reflection machinery
/// alone, which is <c>Activator.CreateInstance</c>, a <c>GetProperty</c>, a
/// <c>PropertyInfo.SetValue</c>, and a strategy lookup that finds no attributes. Every other row
/// is that floor plus one validator, so the difference between rows is the validator.
/// </para>
/// <para>
/// The validators are shipped types rather than fixtures, so the numbers describe types a user
/// actually holds. They also happen to span the interesting ground: <see cref="CharsetRegex"/> and
/// <see cref="FormatRegex"/> are interpreted regular expressions looked up from the static cache
/// on every call and carrying a one-second timeout, <see cref="Checksum"/> is a hand-written Luhn
/// pass over the same sort of input, and <see cref="Mod97"/> is the heaviest shipped validator.
/// </para>
/// <para>
/// <b>Both failure paths are here, and they cost the same.</b> <see cref="TryCreateRejects"/> and
/// <see cref="CreateThrows"/> reject the same input, and measuring them side by side turns up
/// something the names hide: <c>SemanticString.TryFromString</c> is implemented as
/// <c>try { Create(...) } catch (ArgumentException) { return false; }</c>, so <c>TryCreate</c>
/// throws and catches internally on every rejection. Both rows therefore pay a full .NET exception,
/// and both cost far more than any success rung above. <c>TryCreate</c> is exception-free in the
/// caller's control flow and not in the caller's cost, which is worth knowing at a boundary that
/// rejects often. <see cref="CreateThrows"/> catches inside the benchmark on purpose: the throw and
/// the catch together are what a caller pays either way.
/// </para>
/// </remarks>
[MemoryDiagnoser]
public class StringCreationBenchmarks
{
	private string plain = "";
	private string uuid = "";
	private string uuidRejected = "";
	private string ulid = "";
	private string card = "";
	private string iban = "";

	/// <summary>Copies the inputs into fields, so no benchmark body holds a literal.</summary>
	[GlobalSetup]
	public void Setup()
	{
		plain = StringSpecimens.PlainInput;
		uuid = StringSpecimens.UuidText;
		uuidRejected = StringSpecimens.UuidRejected;
		ulid = StringSpecimens.UlidText;
		card = StringSpecimens.CardText;
		iban = StringSpecimens.IbanText;
	}

	/// <summary>The reflection machinery with no validator behind it.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public PlainText Unvalidated() => PlainText.Create(plain);

	/// <summary>That floor plus an interpreted regular expression over a fixed character set.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public Ulid CharsetRegex() => Ulid.Create(ulid);

	/// <summary>The same, over a pattern with groups and separators.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public Uuid FormatRegex() => Uuid.Create(uuid);

	/// <summary>That floor plus a hand-written Luhn pass, for regular expressions against arithmetic.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public CreditCardNumber Checksum() => CreditCardNumber.Create(card);

	/// <summary>The heaviest shipped validator: rearrangement, expansion, modular arithmetic.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public Iban Mod97() => Iban.Create(iban);

	/// <summary>The failure path that does not throw.</summary>
	/// <returns>Whether creation succeeded, which is always false here.</returns>
	[Benchmark]
	public bool TryCreateRejects() => Uuid.TryCreate(uuidRejected, out _);

	/// <summary>The failure path that does, throw and catch together.</summary>
	/// <returns>Whether the expected exception was raised, which is always true here.</returns>
	[Benchmark]
	public bool CreateThrows()
	{
		try
		{
			_ = Uuid.Create(uuidRejected);
			return false;
		}
		catch (ArgumentException)
		{
			return true;
		}
	}
}
