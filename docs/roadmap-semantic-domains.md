# Roadmap: new semantic domains

Status: proposed (2026-06-29). This is a planning document, not a commitment. It sequences the
candidate domains discussed for `ktsu.Semantics` and records the design decisions each one needs
before implementation starts. Read alongside `docs/strategy-unified-vector-quantities.md` (the
quantity model) and `CLAUDE.md` (project layout).

## Where the library is today

Four semantic domains ship:

| Domain | Project | Maturity |
|---|---|---|
| Strings | `Semantics.Strings` | Framework + validation attributes only — **no concrete domain string types ship** |
| Paths | `Semantics.Paths` | Complete (12 interfaces, 4 concrete types, 12 validation attributes) |
| Physical quantities | `Semantics.Quantities` (+ generators) | Deep: 72 dimensions, ~189 units, 73 relationships, 8 log scales, constants |
| Music | `Semantics.Music` | Hand-written: pitch/interval/scale/key/chord/rhythm; **no aggregate (Score) layer** |

The biggest leverage is therefore *filling in* Strings and Music as much as adding new domains.

## Guiding principles

1. **Each domain is its own package** (`Semantics.<Domain>`), so consumers pull only what they need
   and the multi-target matrix stays tractable. Strings/Paths target `netstandard2.0`+; anything
   that needs `INumber<T>` is `net8.0`+.
2. **Reuse the quantity system where the domain is genuinely dimensional** (Geo distance → `Length`,
   Data-rate → a new dimension). Keep non-dimensional domains (Money, Color channels) separate from
   the `INumber<T>` vector model — forcing them in distorts both.
3. **Consolidate before you create.** Color already exists twice in the ecosystem; the Color work is
   primarily a migration, not a green-field build.
4. **Validation attributes are an existing asset.** Concrete strings should lean on the casing /
   format / regex attributes already in `Semantics.Strings` rather than re-deriving parsing.

---

## Phase 0 — Concrete semantic strings (`Semantics.Strings.Web` / `.Identifiers`)

**Why first:** lowest effort, highest visible payoff. The framework is idle without batteries-included
types, and these are pure compositions of attributes that already exist.

**Scope**
- Web/contact: `EmailAddress`, `Url`/`HttpUrl`, `Hostname`, `DomainName`, `IpV4`, `IpV6`,
  `MacAddress`, `PhoneNumber` (E.164), `Slug`.
- Identifiers: `Uuid`/`Guid`, `Ulid`, `Base64`/`Base64Url`, `HexString`, `JwtToken`,
  `CreditCardNumber` (Luhn), `Iban`, `Isbn`, `SemVer`, `HexColor`.

**Decisions to make**
- One package or split (`.Web` vs `.Identifiers`)? Recommend split — different audiences.
- `HexColor` overlaps Phase 3 (Color). Define it here as a *string* type; Color consumes/produces it.
- Which validators need new attributes vs. compose existing ones (Luhn and IBAN check-digits are new).

**Effort:** S. **Depends on:** nothing.

---

## Phase 1 — Music aggregate layer (`Semantics.Music`)

**Why early:** the domain already exists and has momentum; it's missing only the container layer that
makes it usable for real scores.

**Scope**
- Containers: `Score`, `Track`/`Part`, `Measure`/`Bar`, `Voice` — ordered `IMusicalEvent` sequences
  with bar/beat math derived from `TimeSignature` + `Tempo`.
- `Progression` — sequence of `Chord` with functional analysis over a `Key` (extends existing
  roman-numeral support).
- `Tuning`/`Temperament` (equal, just, Pythagorean) so `Pitch.FromFrequency` is no longer hardwired
  to A440 equal temperament.
- Notation niceties: `Clef`, `KeySignature`, `Articulation`, `Dynamic` scale (`pp`..`ff`, bridging to
  existing `Velocity`).

**Decisions to make**
- Is `Score` immutable (record-style, rebuild on edit) or a mutable builder? Recommend immutable core
  + a builder, consistent with the rest of the library.
- Does `Tuning` tie into the quantities `Frequency` type, or stay self-contained? Recommend bridging
  to `Frequency` since that type already exists.

**Effort:** M. **Depends on:** nothing (extends current Music).

---

## Phase 2 — Color (`Semantics.Color`) — **consolidation, not green-field**

**Why this matters:** color science already exists **twice** in the ecosystem, with overlap and at
least one latent correctness bug:

- `ktsu.ThemeProvider` has the real implementation: `RgbColor`, `SRgbColor` (a genuine linear/sRGB
  split), `OklabColor` (+ LCh polar), `PerceptualColor`, and `ColorMath` (RGB↔Oklab, WCAG relative
  luminance, contrast ratio, `AccessibilityLevel`, accessibility-driven lightness adjustment via
  binary search, perceptually-uniform gradients).
- `ktsu.ImGuiStyler` has a *second, weaker* copy: its own `FromHex`, `FromRGB/RGBA`, `FromHSL/HSLA`
  (a hand-rolled `HueToRGB`), all producing `Hexa.NET.ImGui.ImColor`, and it depends on
  ThemeProvider's `RgbColor`/`PerceptualColor` on top.

**Latent bug to fix in the move:** `RgbColor.FromHex` parses sRGB hex bytes straight into a struct
documented as *linear* RGB with no gamma decode, and `ColorMath.RgbToOklab` (whose matrix assumes
linear input) then consumes it. A rigorous `Semantics.Color` makes the sRGB↔linear boundary explicit
and the mistake unrepresentable.

**Target architecture**
- `Semantics.Color` owns **color value types + color science only**:
  - Spaces: `SRgb`, `LinearRgb`, `Hsl`, `Hsv`, `Oklab`, `Oklch`, (stretch: `Lab`, `Xyz`); enforced
    conversions with the gamma boundary correct.
  - Operations: hex parse/format, WCAG luminance + contrast ratio, `AccessibilityLevel`, contrast
    adjustment, perceptual distance, perceptual gradients/lerp, mixing.
  - Stretch synergy with quantities: spectral `Wavelength → Color`, tie WCAG luminance to the
    photometry dimensions (`Luminance`/`Illuminance`) that already exist.
- `ktsu.ThemeProvider` keeps the **semantic theming layer** only (`SemanticMeaning`,
  `SemanticColorRequest`, `IPaletteMapper`, `SemanticColorMapper`, `ISemanticTheme`, `ThemeRegistry`,
  the ~40 bundled themes) and takes a dependency on `Semantics.Color`.
- `ktsu.ImGuiStyler` deletes its bespoke color math and keeps only a thin
  `Semantics.Color ↔ ImColor` adapter at the ImGui boundary.

**Migration plan (own sub-roadmap)** — detailed in
`superpowers/specs/2026-06-29-semantics-color-design.md`. Direct migration, **no shims**:
1. Build & publish `Semantics.Color` (+ `Semantics.Color.ImGui` adapter) with the value types +
   science ported from `ColorMath`/`OklabColor`/etc., gamma boundary fixed, full tests.
2. Re-point `ThemeProvider` at `Semantics.Color` (`PerceptualColor → Color`, breaking; major bump).
3. Re-point `ImGuiStyler` at `Semantics.Color(.ImGui)`; reduce its `Color` class to ImColor adapters.

**Decisions to make** *(these block the build — see open questions)*
- **Channel storage:** float-only (matches existing code + ImGui), or generic `Color<T>` over
  `INumber<T>` (consistent with quantities, heavier)? Recommend **float/double, not the `INumber<T>`
  vector model** — color channels aren't a physics vector and the generic buys little here.
- **Coordination with the repo owners** of ThemeProvider/ImGuiStyler on the shim/deprecation timeline
  (cross-repo change).
- Does `HexColor` (Phase 0 string) become the canonical parse entry point?

**Effort:** L (spans three repos). **Depends on:** Phase 0 only for the optional `HexColor` link.

---

## Phase 3 — Money / Currency (`Semantics.Money`)

**Why:** the canonical "primitive obsession" target and the clearest showcase of the semantic-type
thesis after strings.

**Scope**
- `Currency` (ISO 4217), `Money` (decimal amount + currency), arithmetic that *refuses* cross-currency
  add/subtract at compile or runtime, rounding policies, allocation/splitting without losing pennies,
  formatting per culture.

**Decisions to make**
- Explicitly **not** part of the quantity generator — currency isn't convertible by a fixed constant.
  Document why (mirrors the log-scale "doesn't obey linear arithmetic" carve-out).
- FX/exchange-rate handling: in-scope as an explicit `ExchangeRate` conversion, or out of scope?
  Recommend a minimal `ExchangeRate` type, no rate *sourcing*.

**Effort:** M. **Depends on:** nothing.

---

## Phase 4 — Geo (`Semantics.Geo`)

**Why:** strong synergy with the quantity system — it consumes `Length`, `Bearing`/`Heading`
(already angular dimensions), and `Velocity`.

**Scope**
- `Latitude`, `Longitude`, `Coordinate` (lat/long pair), `Altitude` (already a `Length` overload),
  haversine/great-circle distance → `Length`, initial bearing → `Bearing`, `GeoHash`, bounding boxes.

**Decisions to make**
- Datum/projection scope: WGS84 spherical only (recommend), or pluggable ellipsoid/projection (large)?
- Reuse `Distance`/`Bearing` quantity types directly as return values (recommend) vs. bespoke types.

**Effort:** M. **Depends on:** Quantities (already present).

---

## Phase 5 — Calendar / Temporal (`Semantics.Calendar`)

**Why:** common primitive-obsession area; distinct from the physics `Time` quantity (duration vs.
calendar position).

**Scope**
- `Date`, `TimeOfDay`, `DayOfWeek`, `Month`, `Quarter`, ISO week, `DateRange`/`Interval`, business-day
  math, recurrence helpers.

**Decisions to make**
- **Name collision:** the physics `Time` dimension already exists. Package/namespace must disambiguate
  (recommend `Semantics.Calendar`, never `Semantics.Time`).
- Build on `DateOnly`/`TimeOnly`/`NodaTime`, or self-contained? Recommend wrapping BCL
  `DateOnly`/`TimeOnly` (net6+), with care for the netstandard targets.

**Effort:** M. **Depends on:** nothing (but coordinate naming with Quantities).

---

## Phase 6 — Data size & rate (`Semantics.Data`)

**Why:** small, high-utility, and a good test of whether the quantity generator can absorb a new
dimension cleanly.

**Scope**
- `DataSize` (bytes) with binary (KiB/MiB) **and** decimal (KB/MB) prefixes, `DataRate` (bit/s),
  the integral/derivative relationship between them over `Time`.

**Decisions to make**
- **Generator vs. hand-written:** strongly consider adding `Information` as a dimension in
  `dimensions.json` (unit = byte/bit) so `DataSize`/`DataRate` fall out of the existing machinery,
  including the `DataRate = DataSize / Time` relationship. This is the cleanest fit and validates the
  generator's extensibility.

**Effort:** S–M. **Depends on:** Quantities generator.

---

## Suggested sequence & rationale

```
Phase 0  Concrete strings      ── ship value immediately, no deps
Phase 1  Music aggregates      ── finish a domain already in flight
Phase 2  Color (consolidation) ── retire duplication + fix gamma bug (cross-repo, start early)
Phase 3  Money                 ── flagship new domain
Phase 4  Geo                   ── leans on quantities
Phase 5  Calendar              ── careful naming vs. Time
Phase 6  Data size/rate        ── exercise the generator
```

Phases 0/1 are independent and could run in parallel. Phase 2 is the largest and touches three repos,
so kick off its design (the channel-storage decision + repo-owner coordination) early even if coding
lands later.

## Open questions (resolve before committing)

1. ~~**Color channel storage**~~ — **Resolved:** `double` internally, `float` interop helpers; not
   generic over `INumber<T>`. See `superpowers/specs/2026-06-29-semantics-color-design.md`.
2. ~~**Color migration ownership & timeline**~~ — **Resolved:** direct migration, **no shims**;
   ThemeProvider/ImGuiStyler updated in lockstep and shipped immediately in publish order
   (`Semantics.Color` → ThemeProvider → ImGuiStyler).
3. **Money in or out of the quantity model** — confirm the deliberate carve-out (recommended: out).
4. **Data size in the generator** — add `Information` as a dimension vs. hand-write. (Recommendation:
   generator.)
5. **String package granularity** — one `Semantics.Strings.*` package or split Web/Identifiers.
6. **Calendar naming** — lock a namespace that never collides with the physics `Time` dimension.
```
