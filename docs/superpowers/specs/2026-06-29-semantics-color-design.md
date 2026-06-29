# Design: `Semantics.Color` + ThemeProvider / ImGuiStyler consolidation

Status: approved design (2026-06-29). Implements **Phase 2** of
`docs/roadmap-semantic-domains.md`. This spec is the input to the implementation plan.

## Goal

Create a single, rigorous color-science library, `Semantics.Color`, and consolidate the two existing
color implementations (`ktsu.ThemeProvider` and `ktsu.ImGuiStyler`) onto it — removing duplication and
fixing a latent gamma-correctness bug. Ship immediately with no compatibility shims; the consuming
repos are updated in lockstep.

## Background — what exists today

- **`ktsu.ThemeProvider`** holds the real color engine: `RgbColor`, `SRgbColor` (a genuine
  linear/sRGB split), `OklabColor` (+ LCh polar), `PerceptualColor`, and `ColorMath` (RGB↔Oklab, WCAG
  relative luminance, contrast ratio, `AccessibilityLevel`, accessibility-driven lightness adjustment
  via binary search, perceptually-uniform gradients). `PerceptualColor` is the **public currency type**
  — the palette is exposed as `ImmutableDictionary<SemanticColorRequest, PerceptualColor>`. ~40 theme
  files construct colors via `RgbColor.FromHex` / `PerceptualColor`.
- **`ktsu.ImGui.Styler`** (in the **ImGuiApp** monorepo, `C:\dev\ktsu-dev\ImGuiApp\ImGui.Styler\`;
  the old standalone `ktsu.ImGuiStyler` repo is retired) has a second, weaker copy: a static `Color`
  class with `FromHex`, `FromRGB/RGBA`, `FromHSL/HSLA` (a hand-rolled `HueToRGB`), `FromPerceptualColor`,
  and a themed `Palette`, all producing `Hexa.NET.ImGui.ImColor`. It also depends on ThemeProvider's
  `RgbColor`/`PerceptualColor`. The ImGuiApp repo already consumes `ktsu.Semantics.Paths/Strings/
  Quantities` as NuGet packages, so adding a `ktsu.Semantics.Color` reference fits its existing pattern.

### The latent bug being fixed

`RgbColor.FromHex` parses sRGB hex bytes directly into a struct documented as **linear** RGB, with no
gamma decode. `ColorMath.RgbToOklab` (whose matrix assumes **linear** input) then consumes those
sRGB-valued numbers. Two consequences:

- **Display happened to be correct**: ImGui interprets `ImColor` values as straight sRGB, and the
  pipeline passed sRGB values through unchanged, so rendered colors looked right.
- **All perceptual math was wrong**: Oklab distance, gradients, WCAG relative luminance, contrast
  ratios, and accessibility matching were computed on sRGB-valued numbers instead of linear ones.

The fix makes the canonical representation truly linear and confines gamma conversion to explicit type
boundaries. **Displayed palette colors do not change** (hex→linear→sRGB round-trips to identity);
**perceptual computations become correct**.

## Decisions (resolved during brainstorming)

1. **Channel storage:** `double` internally, `float` interop helpers. Not generic over `INumber<T>`.
2. **API shape:** hybrid — one canonical `Color` (linear RGB + alpha) is the type consumers pass
   around; lightweight space structs (`Srgb`, `Hsl`, `Hsv`, `Oklab`, `Oklch`) exist as conversion
   targets / math intermediates.
3. **v1 scope:** the migration-complete set **plus** `Hsv` and a `NamedColors` (CSS/X11) table.
   Deferred to later: `Lab`, `Xyz`, `Cmyk`, spectral `Wavelength→Color`.
4. **Canonical = linear RGB.** ImGui interop goes through an explicit sRGB-encoding boundary, never a
   raw linear cast.
5. **ImGui ergonomics:** `ToImColor()`/`FromImColor()` (↔ `ImColor`) and
   `ToImGuiVector4()`/`FromImGuiVector4()` (↔ a strong `ImGuiVector4` carrying the sRGB-encoded value).
   `ImGuiVector4` **implicitly widens to `System.Numerics.Vector4`**, so it drops into any ImGui call
   expecting a `Vector4` with no ceremony.
6. **The ImGui adapter ships from the ImGuiApp repo, not the Semantics repo.** The Semantics repo is
   deliberately dependency-light (Polyfill, PreciseNumber); it must not take a `Hexa.NET.ImGui`
   dependency. The adapter is a new `ktsu.ImGui.Color` package in ImGuiApp (alongside `ktsu.ImGui.Styler`
   in the `ktsu.ImGui.*` family). The Semantics `Color` core stays ImGui-agnostic and exposes the
   primitive `ToSrgbVector4()`; the adapter builds the ImGui surface on top.
7. **No shims.** `PerceptualColor → Color` is an accepted breaking change to ThemeProvider's public
   API; the only consumer (`ktsu.ImGui.Styler`) is migrated in lockstep.

## Architecture

### Package layout

| Package | Repo | Targets | Depends on | Responsibility |
|---|---|---|---|---|
| `Semantics.Color` | **Semantics** | netstandard2.0/2.1 + net8–net10 | (none beyond BCL `System.Numerics`) | Color value types + color science. Hand-written, style mirrors `Semantics.Music`. |
| `ktsu.ImGui.Color` | **ImGuiApp** | net8–net10 (match ImGui consumers) | `ktsu.Semantics.Color`, `Hexa.NET.ImGui` (2.2.9) | Thin adapter: `ToImColor()`/`FromImColor()`/`ToImGuiVector4()`/`FromImGuiVector4()` + the `ImGuiVector4` strong type. Mirrors the `ThemeProvider` / `ThemeProvider.ImGui` split. |

Only `Semantics.Color` is added to `Semantics.sln`; the adapter lives in the ImGuiApp monorepo.
`Semantics.Color` can target the wide matrix because it is float/double math with no `INumber<T>`
requirement, and it takes **no** UI-framework dependency.

> **This spec's implementation plan (`docs/superpowers/plans/`) covers `Semantics.Color` only** — the
> first shippable unit, in this repo. The `ktsu.ImGui.Color` adapter + `ktsu.ImGui.Styler` migration
> (ImGuiApp repo) and the `ktsu.ThemeProvider` migration (ThemeProvider repo) are follow-on plans
> authored in their own repos once `ktsu.Semantics.Color` is published.

### Core type: `Color`

```csharp
public readonly record struct Color(double R, double G, double B, double A);
```

Stores **linear** RGB + alpha, each `0..1`. Alpha is not gamma-encoded.

- **Factories:** `FromSrgb`, `FromLinear`, `FromHex` (sRGB-assumed; `#RGB`, `#RRGGBB`, `#RRGGBBAA`),
  `FromBytes` (sRGB), `FromOklab`, `FromOklch`, `FromHsl`, `FromHsv`.
- **Conversions out:** `ToSrgb()`, `ToHex()`, `ToBytes()`, `ToOklab()`, `ToOklch()`, `ToHsl()`,
  `ToHsv()`, `WithAlpha(double)`.
- **Interop primitives:** `ToSrgbVector4()` (float, sRGB — the value ImGui wants),
  `ToLinearVector4()` (float, linear), plus `Vector3` variants dropping alpha.
- **Operations (ported from `ColorMath`):** `RelativeLuminance` (WCAG, on linear), `ContrastRatio(Color)`,
  `AccessibilityLevelAgainst(Color background, bool largeText = false)`,
  `AdjustForContrast(Color background, AccessibilityLevel target, bool largeText = false)`,
  `DistanceTo(Color)` (Oklab Euclidean), `MixOklab(Color, double t)`, `Lerp(Color, double t)` (linear),
  `Gradient(Color to, int steps)` (perceptual, Oklab).

### Space structs (math intermediates)

`readonly record struct` each, with conversions to/from `Color`:

- `Srgb(double R, double G, double B)` — the **only** struct that crosses the gamma boundary
  (`Srgb`↔`Color`). Gamma transfer functions live here.
- `Hsl(double H, double S, double L)`, `Hsv(double H, double S, double V)` — derived from sRGB.
- `Oklab(double L, double A, double B)`, `Oklch(double L, double C, double H)` — derived from linear.

`Color` is always linear; consumers never accidentally feed sRGB into perceptual math.

### Supporting types

- `NamedColors` — static class exposing CSS/X11 named colors as `Color`.
- `AccessibilityLevel` — enum `{ Fail, AA, AAA }` (moved from ThemeProvider).

### ImGui adapter (`ktsu.ImGui.Color`, ImGuiApp repo)

```csharp
// Strong type: an sRGB-encoded, ImGui-ready colour vector. Lives in ktsu.ImGui.Color.
// (No Hexa.NET.ImGui dependency needed for the type itself — it is just a Vector4 wrapper.)
public readonly record struct ImGuiVector4(float X, float Y, float Z, float W)
{
    public ImGuiVector4(Vector4 v) : this(v.X, v.Y, v.Z, v.W) { }
    public static implicit operator Vector4(ImGuiVector4 v) => new(v.X, v.Y, v.Z, v.W);
    public Vector4 ToVector4();
}

public static class ColorImGuiExtensions
{
    public static ImColor      ToImColor(this Color color);          // → ImColor (sRGB)
    public static Color        FromImColor(this ImColor color);      // treats ImColor as sRGB

    public static ImGuiVector4 ToImGuiVector4(this Color color);     // → strong sRGB vector (widens to Vector4)
    public static Color        FromImGuiVector4(ImGuiVector4 srgb);  // from the strong type
    public static Color        FromImGuiVector4(Vector4 srgb);       // ingest a raw ImGui Vector4 as sRGB
}
```

`ToImColor()` / `ToImGuiVector4()` are the "nobody needs to remember gamma" entry points requested in
brainstorming — both emit the sRGB-encoded value ImGui expects, so callers never reason about gamma.
`ImGuiVector4` widens implicitly to `System.Numerics.Vector4`. `FromImGuiVector4` is overloaded so the
raw `Vector4` you read back from ImGui (e.g. `ImColor.Value`) can be ingested directly as sRGB.

## Migration

### ThemeProvider (breaking; major version bump)

- Delete `RgbColor`, `SRgbColor`, `OklabColor`, `PerceptualColor`, `ColorMath`, `AccessibilityLevel`;
  add a dependency on `Semantics.Color`.
- **`PerceptualColor` → `Color`.** Perceptual properties (`Hue`/`Chroma`/`Lightness`) are obtained via
  `.ToOklch()`/`.ToOklab()` instead of cached fields. The public palette type becomes
  `ImmutableDictionary<SemanticColorRequest, Color>`.
- The ~40 theme files switch `RgbColor.FromHex(...)` / `PerceptualColor.FromRgb(hex)` to
  `Color.FromHex(...)` (mechanical).
- Retained semantic-mapping layer: `SemanticColorMapper`, `SemanticMeaning`, `SemanticColorRequest`,
  `IPaletteMapper`, `ISemanticTheme`, `ThemeRegistry`, `Priority`. `ColorRange` is rewritten in terms
  of `Color`/`Oklch` (its perceptual-range logic moves onto the new types; it stays in ThemeProvider as
  semantic-layer code).
- `ThemeProvider.ImGui/ImGuiPaletteMapper` updated to the new types.

### ktsu.ImGui.Color (new adapter, ImGuiApp repo)

- New package `ktsu.ImGui.Color` providing the `ImGuiVector4` strong type and the
  `ToImColor`/`FromImColor`/`ToImGuiVector4`/`FromImGuiVector4` extensions over `Semantics.Color.Color`.
  References `ktsu.Semantics.Color` + `Hexa.NET.ImGui` (2.2.9). This is the only place
  `Hexa.NET.ImGui` meets `Semantics.Color`.

### ktsu.ImGui.Styler (ImGuiApp repo)

- The static `Color` class keeps its ImGui-facing API surface (`FromHex`, `FromRGB/RGBA`,
  `FromHSL/HSLA`, `FromVector`) and the `Palette`, but delegates **all** conversion to `Semantics.Color`
  and emits `ImColor` via the `ktsu.ImGui.Color` adapter (`ToImColor()`). The hand-rolled `HueToRGB`
  is deleted.
- `FromPerceptualColor(PerceptualColor)` → `FromColor(Color)`. The `ImGuiStylerDemo` example (in
  ImGuiApp `examples/`) is updated to match. No `[Obsolete]` shim (per "ship immediately").

## Testing (MSTest, in `Semantics.Test`)

- **Round-trips:** `Srgb↔Color` (linear) identity within tolerance; `Color↔hex`; `Color↔Oklab`;
  `Color↔Oklch`; `Color↔Hsl`; `Color↔Hsv`.
- **Known values:** black/white contrast ratio = 21:1; relative luminance of pure primaries; Oklab of
  reference colors matches Björn Ottosson's published values; gradient endpoints equal the inputs.
- **Gamma regression (proves the fix):** `hex → Color → hex` is stable (display unchanged); Oklab
  computed from true-linear differs measurably from the old "sRGB-as-linear" computation.
- **Accessibility:** `AdjustForContrast` reaches the requested `AccessibilityLevel` for representative
  fg/bg pairs.
- **NamedColors:** spot-check a handful against known hex values.

## Shipping order

1. **Semantics repo:** build and publish `ktsu.Semantics.Color` with tests. *(This spec's plan.)*
2. **ThemeProvider repo:** reference `ktsu.Semantics.Color`, migrate (`PerceptualColor → Color`), ship
   (major bump).
3. **ImGuiApp repo:** add the `ktsu.ImGui.Color` adapter, then migrate `ktsu.ImGui.Styler` to the new
   ThemeProvider + `ktsu.Semantics.Color` + `ktsu.ImGui.Color`, update the demo, ship.

Each repo is independently buildable in that order. **Cross-repo caveat:** because these are separate
repositories/feeds, `ktsu.Semantics.Color` must be published before ThemeProvider/ImGuiApp can
reference it via NuGet; during development use temporary local `ProjectReference`s.

## Out of scope (deferred)

`Lab`, `Xyz`, `Cmyk` color spaces; spectral `Wavelength→Color` (would bridge to the photometry
quantities); a richer named-palette system; any color-picker UI.

## Risks

- **Cross-repo coordination:** three repos, three CI pipelines, ordered publishing. Mitigated by the
  shipping order and local ProjectReferences during dev.
- **Perceptual-math behavior change:** themed *derived* colors (nearest-match, gradients, contrast
  decisions) will shift because the math is now correct. This is intended; base palette colors are
  unaffected. Tests pin the display-stability and document the math change.
