# Design: Concrete semantic strings — `Semantics.Strings.Identifiers`

Status: approved (2026-07-01). Implements **Phase 0** of `docs/roadmap-semantic-domains.md`, scoped
to the *identifier* subset. Read alongside `docs/architecture.md` (semantic-string framework) and
`CLAUDE.md` (project layout, code standards).

## Problem

`Semantics.Strings` ships a complete validation framework (`SemanticString<TDerived>`, composable
validation attributes, validation strategies) but **zero concrete domain string types**. Consumers who
want an `Email` or a `Uuid` must hand-roll the CRTP record and validation themselves. The framework is
idle without batteries-included types.

Phase 0 of the roadmap proposes filling this gap with two audiences — web/contact strings and
identifiers. This spec covers the **identifiers** cut only, and deliberately restricts the roster to
types that genuinely *identify an entity*. Encodings (`Base64`, `HexString`) and notations (`SemVer`,
`HexColor`) are explicitly deferred to a later `Semantics.Strings.Formats` package so that the
`Identifiers` package name stays honest.

## Goals

- Ship a `Semantics.Strings.Identifiers` package with six validated identifier types.
- Reuse the existing framework: each type is a thin CRTP record; validation composes/extends the
  existing attribute machinery.
- Keep the framework package (`Semantics.Strings`) generic — no domain-specific validators leak into
  it.
- Pragmatic, well-documented validation (matches the existing `IsEmailAddress` precedent): structural
  format plus real check-digit math where a checksum exists, with XML docs stating each type's limits.

## Non-goals

- No web/contact types (`Email`, `Url`, `Hostname`, `IpV4/6`, `Phone`, `Slug`) — separate future
  package.
- No encodings/notations (`Base64`, `Base64Url`, `HexString`, `SemVer`, `HexColor`) — future
  `Semantics.Strings.Formats` package. `HexColor` will become a string type there, consumed by
  `Semantics.Color`.
- No parsed accessors in v1 (`Uuid.ToGuid()`, JWT claim decoding, ISBN-10↔13 conversion). These are a
  clean follow-up; validation and type-safety are the v1 deliverable.
- No signature verification for JWTs (see `JwtToken` below).

## Framework mechanics this design relies on

From `Semantics.Strings/SemanticString.cs`:

- `protected virtual string MakeCanonical(string input) => input;` — a hook invoked by `Create`
  *before* validation. Derived types override it to normalize (strip separators, fix case). Validation
  then runs against the canonical form, and the canonical form is what gets stored.
- `Create<TDerived>(string)` / `Create(string)` factories apply `MakeCanonical`, then `PerformValidation`
  (which calls `IsValid()` → `ValidateAttributes()`), throwing `ArgumentException` on failure.
- Validation attributes derive from the public `NativeSemanticStringValidationAttribute` (which wraps a
  `ValidationAdapter` returning `ValidationResult`). This is the extension point the Identifiers package
  builds on.

Important: `Create` does **not** reject empty strings — `IsValid()` only checks the underlying string
is non-null, then defers to the validation attributes. Some existing framework attributes (e.g.
`IsEmailAddress`) return `Success()` on empty so they compose with a separate length rule; that means
`EmailAddress.Create("")` currently succeeds. For identifiers an empty string is **never** a valid
value, so the new attributes deliberately do **not** adopt that convention: their format / structural /
checksum checks reject empty naturally (an anchored regex or a segment/length check fails on `""`), so
`Uuid.Create("")` throws `ArgumentException`. Each identifier type carries exactly one such attribute
as its sole validator.

## Package & project

New project `Semantics.Strings.Identifiers/`:

- **Namespace:** `ktsu.Semantics.Strings.Identifiers`.
- **NuGet id:** `ktsu.Semantics.Strings.Identifiers`.
- **TFMs:** `net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1` (mirrors `Semantics.Strings`).
- **SDK:** `Microsoft.NET.Sdk` + `ktsu.Sdk` (same header as `Semantics.Strings.csproj`).
- **References:**
  - `ProjectReference` → `Semantics.Strings/Semantics.Strings.csproj` (public types + attribute base
    classes).
  - `PackageReference` → `Polyfill` (`PrivateAssets="all"`), `System.Text.Json` (JWT payload parsing),
    `System.Memory` conditioned on `netstandard2.0`.
- `AssemblyInfo.cs` holding assembly attributes (per `CLAUDE.md` convention; mirror
  `Semantics.Paths/AssemblyInfo.cs`).
- `InternalsVisibleTo("ktsu.Semantics.Test")`.
- Added to `Semantics.sln`; referenced by `Semantics.Test/Semantics.Test.csproj`.
- **Package validation:** start with SDK defaults. If per-TFM public-surface differences appear (e.g.
  span-based base64url paths only on newer TFMs), set `EnablePackageValidation=false` with the same
  justification comment `Semantics.Strings.csproj` uses. Expected surface here is uniform, so default
  should hold — flagged so the implementer checks the pack step rather than assuming.

## Type roster

Every type is `public sealed record <Name> : SemanticString<<Name>>` decorated with a single
identifier attribute, plus a `MakeCanonical` override where normalization applies. File header is the
standard three-line ktsu MIT header. XML docs on each type state the accepted forms and the documented
limit.

| Type | `MakeCanonical` | Validation attribute | Rule | Documented limit |
|---|---|---|---|---|
| `Uuid` | lowercase; strip surrounding `{}`/`()` | `[IsUuid]` | `^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$` on canonical form | any RFC-4122 variant/version accepted; not version-checked; nil UUID allowed |
| `Ulid` | uppercase | `[IsUlid]` | 26 chars from Crockford base32 `0-9A-HJKMNP-TV-Z`; first char ≤ `7` (48-bit timestamp bound) | charset + length + timestamp-high-bit only |
| `Iban` | strip whitespace; uppercase | `[IsIban]` | `^[A-Z]{2}[0-9]{2}[A-Z0-9]+$`, total length 15–34, ISO 7064 mod-97-10 == 1 | checksum + generic structure; per-country BBAN length/format tables **not** enforced |
| `Isbn` | strip hyphens/whitespace; uppercase | `[IsIsbn]` | ISBN-10 (10 chars, weighted mod-11, final may be `X`) **or** ISBN-13 (13 digits, mod-10) | both forms accepted; registration-group/publisher ranges not validated |
| `CreditCardNumber` | strip whitespace/hyphens | `[IsCreditCardNumber]` | 13–19 digits, Luhn (mod-10) valid | Luhn only — no BIN/issuer/network detection, no PCI guarantees. Doc: **do not log PANs** |
| `JwtToken` | none (opaque, case-sensitive) | `[IsJwtToken]` | exactly three `.`-separated segments; header + payload are non-empty base64url that decode to JSON **objects**; signature segment may be empty (`alg=none`) | **signature is not verified**; `alg`/claims not inspected; expiry not checked |

Canonicalization note: because `MakeCanonical` runs before validation and its result is stored,
`Iban.Create("gb82 west 1234 5698 7654 32")` stores `GB82WEST12345698765432`, and
`CreditCardNumber.Create("4111 1111 1111 1111")` stores `4111111111111111`. `JwtToken` is intentionally
left byte-for-byte (any transform would corrupt the signature).

## New validation attributes

Six attributes in the Identifiers package, namespace `ktsu.Semantics.Strings.Identifiers`, each
`public sealed class Is<Name>Attribute : NativeSemanticStringValidationAttribute` with a private nested
`ValidationAdapter`. Names: `IsUuidAttribute`, `IsUlidAttribute`, `IsIbanAttribute`, `IsIsbnAttribute`,
`IsCreditCardNumberAttribute`, `IsJwtTokenAttribute`.

- Regex-only rules (`IsUuid`, `IsUlid`) compile a single anchored pattern with a 1-second timeout
  (matches `RegexMatchAttribute`), operating on the already-canonical value.
- Check-digit rules (`IsIban`, `IsIsbn`, `IsCreditCardNumber`) do a cheap structural pre-check, then run
  the checksum. Checksum helpers are private static methods on the adapter — no shared public math
  surface in v1.
- `IsJwtToken` splits on `.`, requires exactly three segments with non-empty header and payload
  segments (the signature segment may be empty, e.g. `alg=none`), base64url-decodes the header and
  payload (padding restored, `-_` → `+/`), and confirms each decodes to a JSON object via
  `System.Text.Json` (`JsonDocument.Parse` with `RootElement.ValueKind == JsonValueKind.Object`). It
  does **not** decode or verify the signature segment.
- Every adapter rejects empty/`""` (empty is never a valid identifier — the format, structural, or
  checksum check fails on it, so no empty→`Success()` guard is added) and returns a specific
  `ValidationResult.Failure(message)` on any failure. Failure messages name the rule that failed
  (e.g. "The value must be a valid IBAN (mod-97 checksum failed).").

Rationale for package-local attributes: these are domain-specific and only meaningful next to the
types they validate. The framework exposes the base classes precisely so downstream packages can add
their own validators; keeping IBAN/Luhn/ISBN/JWT logic out of `Semantics.Strings` preserves that
package's generality (matching how `Semantics.Paths` owns its path validators).

## Testing

New folder `Semantics.Test/Identifiers/`, one MSTest class per type
(`UuidTests`, `UlidTests`, `IbanTests`, `IsbnTests`, `CreditCardNumberTests`, `JwtTokenTests`).
Conventions from `CLAUDE.md`: explicit types (no `var`), semantic asserts.

Each class covers:

- **Canonical valid** — a known-good canonical vector constructs successfully.
- **Canonicalization** — an input with separators/mixed case constructs and stores the canonical form
  (assert on `.WeakString`). (N/A for `JwtToken`.)
- **Invalid checksum** — a value that passes the structural regex but fails the check digit throws
  `ArgumentException` (IBAN/ISBN/CreditCard).
- **Structural invalid** — wrong length, illegal characters, wrong segment count (JWT) throw
  `ArgumentException`.
- **`As<T>` round-trip** — converting from a plain `SemanticString` or between compatible types
  preserves the value.

Reference vectors (real, well-known): `Uuid` — `123e4567-e89b-12d3-a456-426614174000` and nil
`00000000-0000-0000-0000-000000000000`; `Ulid` — `01ARZ3NDEKTSV4RRFFQ69G5FAV`; `Iban` —
`GB82WEST12345698765432` (valid), `GB82WEST12345698765431` (bad checksum); `Isbn` —
`0-306-40615-2` (ISBN-10), `978-0-306-40615-7` (ISBN-13); `CreditCardNumber` — `4111111111111111`
(valid Luhn), `4111111111111112` (invalid); `JwtToken` — a standard three-segment sample with a JSON
header/payload.

## Documentation & wiring (late step)

- Add a `Semantics.Strings.Identifiers` row to the `CLAUDE.md` project-layout table.
- Update the README/complete-library-guide via the `update-docs` skill once the code lands.
- Do **not** hand-edit `VERSION.md`/`CHANGELOG.md` (auto-generated). The commit that ships the package
  carries the appropriate version tag per the ktsu convention.

## Risks & mitigations

- **netstandard2.0 base64url decode** — `Convert.FromBase64String` needs `+/` and padding; the JWT
  adapter must map `-_`→`+/` and re-pad. Straightforward, covered by tests on the ns2.0 leg (CI builds
  all TFMs).
- **Regex catastrophic backtracking** — all patterns are anchored and linear; reuse the 1-second
  timeout convention from `RegexMatchAttribute`.
- **Line endings** — the repo mandates CRLF + no BOM + final newline (warnings-as-errors). Normalize new
  `.cs` files to CRLF/no-BOM after writing (verify first bytes are `2f 2f`, not `ef bb bf`).
- **Package validation surprise** — if the pack step flags per-TFM surface diffs, apply the documented
  `EnablePackageValidation=false` escape hatch as `Semantics.Strings` does.

## Out-of-scope follow-ups (recorded, not committed)

1. `Semantics.Strings.Web` — Email/Url/Hostname/IP/Phone/Slug.
2. `Semantics.Strings.Formats` — Base64/Base64Url/HexString/SemVer/HexColor; `HexColor` consumed by
   `Semantics.Color`.
3. Parsed accessors — `Uuid.ToGuid()`, `JwtToken` header/payload/claims, ISBN-10↔13 conversion,
   credit-card network detection.
