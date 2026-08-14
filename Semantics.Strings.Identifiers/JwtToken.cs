// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// A JSON Web Token in compact serialization: three '.'-separated base64url segments. The value is
/// stored verbatim (no normalization) because it is opaque and case-sensitive. Validation confirms the
/// header and payload decode to JSON objects; the <b>signature is not verified</b>, and <c>alg</c>,
/// claims, and expiry are not inspected.
/// </summary>
[IsJwtToken]
public sealed record JwtToken : SemanticString<JwtToken>
{
}
