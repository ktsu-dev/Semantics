// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Strings;

using ktsu.Semantics.Strings;

/// <summary>
/// A semantic string that declares no validation at all.
/// </summary>
/// <remarks>
/// The one specimen in this suite that is not a shipped type, and it has to be.
/// <c>Semantics.Strings</c> ships the validation framework and no concrete types, so nothing
/// shipped occupies the no-validation rung — and that rung is what every other measurement is read
/// against, being the reflection machinery alone with no validator behind it. Declared against the
/// public API only, like everything else here.
/// </remarks>
public sealed record PlainText : SemanticString<PlainText>;

/// <summary>
/// The input strings the string benchmarks run against.
/// </summary>
/// <remarks>
/// Every value here is valid for the type it is paired with, except <see cref="UuidRejected"/>,
/// and each is held as a constant so that a benchmark body reads as one call rather than as a
/// literal the JIT might fold. The benchmark classes copy these into fields in
/// <c>[GlobalSetup]</c> for the same reason.
/// </remarks>
internal static class StringSpecimens
{
	/// <summary>Input for the unvalidated rung. Length is in the same range as the others.</summary>
	internal const string PlainInput = "0123456789abcdef0123456789abcdef0123";

	/// <summary>A canonical lowercase RFC 4122 identifier, which <c>IsUuid</c> accepts as written.</summary>
	internal const string UuidText = "123e4567-e89b-12d3-a456-426614174000";

	/// <summary>
	/// The pattern <c>IsUuidAttribute</c> uses, repeated here for the hand-written side of the
	/// cost pairs in <c>StringAbstractionCostBenchmarks</c>. It must stay identical to the one in
	/// <c>Semantics.Strings.Identifiers</c>, or that pair stops being a comparison.
	/// </summary>
	internal const string UuidPattern =
		"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$";

	/// <summary>The same identifier one character short, so the pattern rejects it.</summary>
	internal const string UuidRejected = "123e4567-e89b-12d3-a456-42661417400";

	/// <summary>A 26-character Crockford base32 identifier, which <c>IsUlid</c> accepts.</summary>
	internal const string UlidText = "01ARZ3NDEKTSV4RRFFQ69G5FAV";

	/// <summary>A 16-digit number that passes the Luhn check. Not a real card.</summary>
	internal const string CardText = "4111111111111111";

	/// <summary>The standard example account number, which passes the mod-97 check.</summary>
	internal const string IbanText = "GB82WEST12345698765432";
}
