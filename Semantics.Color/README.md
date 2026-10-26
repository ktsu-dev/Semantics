# ktsu.Semantics.Color

> A physically-grounded color type with linear-RGB math, perceptual Oklab operations, and built-in WCAG accessibility checks.

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](../LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.Color?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Color)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics.Color?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Color)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.Color?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Color)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

`ktsu.Semantics.Color` is one package in the [ktsu.Semantics](../README.md) family. Start at the [root README](../README.md) for the family overview.

## Introduction

`ktsu.Semantics.Color` treats color the way rendering and color science do: the canonical `Color` type stores linear (non gamma-encoded) RGB plus straight alpha, each channel in 0..1, and all math happens in linear space. Gamma-encoded sRGB is a separate type (`Srgb`) that you cross into only when you convert. That distinction is the difference between correct blending, mixing, and luminance and the subtly-wrong results you get from doing arithmetic on gamma-encoded values.

On top of that foundation the package adds perceptual operations in the Oklab color space, hex and byte interop, `System.Numerics` vector output, and WCAG contrast tooling that can rate a color pair and automatically nudge a color until it meets a target conformance level.

## Features

- **Linear RGB as the hub**: `Color` stores linear RGBA in 0..1, and all mixing, interpolation, and luminance math happens there.
- **Color spaces**: sRGB (`Srgb`), HSL (`Hsl`), HSV (`Hsv`), Oklab (`Oklab`), and Oklch (`Oklch`). Every color type (including `Color`) converts directly to and from every other through `From*` / `To*` methods. Each hop is routed through the nearest shared hub, so a conversion crosses the gamma boundary at most once and never takes a redundant gamma round-trip.
- **Interop**: hex parse and format (`#RGB`, `#RRGGBB`, `#RRGGBBAA`), 8-bit byte tuples, and linear or sRGB `Vector3` / `Vector4` output (the sRGB vectors are what ImGui expects).
- **WCAG accessibility**: relative luminance, contrast ratio (1..21), conformance rating against a background, and `AdjustForContrast` which binary-searches Oklab lightness to hit a target while preserving hue, reducing chroma only where the sRGB gamut requires it and mixing towards black or white when lightness alone cannot reach the target. A translucent foreground is measured as it looks composited over the background, as WCAG checkers do.
- **Color vision simulation**: `Simulate(deficiency, severity)` shows a color as someone with protan, deutan, or tritan color vision sees it, from the Machado, Oliveira, and Fernandes (2009) matrices at any severity from 0 to 1.
- **Perceptual operations**: Oklab distance (`DistanceTo`), perceptually uniform mixing (`MixOklab`), and Oklab gradients (`Gradient`), alongside plain linear `Lerp`.
- **Adjustments**: lighten/darken, saturate/desaturate, hue offset, grayscale, and invert. HSL-based on `Color`, `Hsl`, and `Srgb`; perceptually-uniform (lightness/chroma) variants on `Oklch`.
- **Named colors**: a subset of the CSS named colors, with CSS's values (`green` is `#008000`, `lime` is `#00FF00`) and case-insensitive lookup.

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.Semantics.Color
```

### .NET CLI

```bash
dotnet add package ktsu.Semantics.Color
```

### Package Reference

```xml
<PackageReference Include="ktsu.Semantics.Color" Version="x.y.z" />
```

## Usage Examples

### Basic Example: accessible text color

```csharp
using ktsu.Semantics.Color;

Color text = Color.FromHex("#777777");
Color background = NamedColors.White;

double ratio = text.ContrastRatio(background);                        // ~4.48
AccessibilityLevel level = text.AccessibilityLevelAgainst(background); // Fail for AA body text

if (level < AccessibilityLevel.AA)
{
    // darkens or lightens in Oklab, preserving hue and chroma, until AA is met
    text = text.AdjustForContrast(background, AccessibilityLevel.AA);
}

Console.WriteLine(text.ToHex());
```

### Simulating color vision deficiencies

```csharp
using ktsu.Semantics.Color;

Color red = Color.FromHex("#D62728");
Color green = Color.FromHex("#2CA02C");

// Protanopia (no working red cones): severity 1.0 is the dichromacy, lower is protanomaly
Color redSeen = red.Simulate(ColorVisionDeficiency.Protan, 1.0);
Color greenSeen = green.Simulate(ColorVisionDeficiency.Protan, 1.0);
double apart = redSeen.DistanceTo(greenSeen);   // much smaller than red.DistanceTo(green)

// The matrix itself, to apply to many pixels without re-interpolating
ColorVisionMatrix mild = ColorVision.Matrix(ColorVisionDeficiency.Deutan, 0.4);
Color seen = mild.Apply(green);
```

The matrices are the published tables of Machado, Oliveira, and Fernandes (2009), "A Physiologically-based Model for Simulation of Color Vision Deficiency", for severities 0.0 to 1.0 in steps of 0.1. A severity between two steps interpolates linearly between them. They act on linear RGB, which is what `Color` stores, and the result is clamped to 0..1 with alpha kept. Severity 0 is the identity, and a severity outside 0..1 throws `ArgumentOutOfRangeException`.

### Perceptually uniform gradients

```csharp
using ktsu.Semantics.Color;

Color start = Color.FromSrgb(0.9, 0.1, 0.2);   // sRGB input, stored as linear
Color end = NamedColors.Blue;

// Oklab interpolation, inclusive of both endpoints; steps must be >= 2
IReadOnlyList<Color> ramp = start.Gradient(end, 5);

foreach (Color c in ramp)
{
    (byte r, byte g, byte b, byte a) = c.ToBytes();
    Console.WriteLine($"{c.ToHex()}  rgba({r},{g},{b},{a})");
}
```

### Space conversions and vector interop

```csharp
using System.Numerics;
using ktsu.Semantics.Color;

Color c = NamedColors.Orange;

Hsl hsl = c.ToHsl();                                            // H in degrees, S/L in 0..1
Color complementary = Color.FromHsl(hsl with { H = (hsl.H + 180) % 360 });

Oklch lch = c.ToOklch();                                        // perceptual lightness/chroma/hue
Color brighter = Color.FromOklch(lch with { L = lch.L + 0.1 });

Vector4 imguiColor = complementary.ToSrgbVector4();             // gamma-encoded RGBA for ImGui
```

Conversions are not limited to `Color`. Any color type converts straight to any other, so you can stay in whichever space fits the task:

```csharp
using ktsu.Semantics.Color;

Hsl hsl = Color.FromHex("#3366CC").ToHsl();

Oklab lab = hsl.ToOklab();          // sRGB family -> perceptual family, through linear Color
Hsv hsv = lab.ToHsv();              // and back again
Srgb srgb = Oklch.FromHsl(hsl).ToSrgb();
Color linear = hsl.ToColor();       // every satellite also has ToColor() / FromColor()
```

### Adjusting colors

```csharp
using ktsu.Semantics.Color;

Color c = Color.FromHex("#3366CC");

Color hover = c.LightenBy(0.1);          // HSL lightness, alpha preserved
Color muted = c.DesaturateBy(0.3);       // HSL saturation
Color accent = c.OffsetHue(30);          // rotate hue 30 degrees
Color negative = c.Invert();             // per-channel negative in sRGB

// For perceptually-uniform tinting, adjust in Oklch instead of HSL
Color perceptualHover = c.ToOklch().LightenBy(0.1).ToColor(c.A);
```

## API Reference

### `Color`

The canonical color: linear RGBA, each channel `double` in 0..1. A `readonly record struct` with positional properties `R`, `G`, `B`, `A`.

#### Construction and conversion

| Name | Return Type | Description |
|------|-------------|-------------|
| `FromLinear(r, g, b, a = 1)` | `Color` | From linear RGBA. |
| `FromSrgb(r, g, b, a = 1)` / `FromSrgb(Srgb, a = 1)` | `Color` | From gamma-encoded sRGB. |
| `FromBytes(r, g, b, a = 255)` | `Color` | From 8-bit channels. |
| `FromHex(string)` | `Color` | Parse `#RGB`, `#RRGGBB`, or `#RRGGBBAA` (the `#` is optional). |
| `FromOklab` / `FromOklch` / `FromHsl` / `FromHsv` | `Color` | From the named space. |
| `ToSrgb()` / `ToOklab()` / `ToOklch()` / `ToHsl()` / `ToHsv()` | space type | Convert out. |
| `ToHex()` | `string` | `#RRGGBB`, or `#RRGGBBAA` when alpha is not full. |
| `ToBytes()` | `(byte, byte, byte, byte)` | Rounded 8-bit RGBA. |
| `ToLinearVector3/4()` / `ToSrgbVector3/4()` | `Vector3` / `Vector4` | `System.Numerics` interop. |
| `WithAlpha(a)` / `Clamp()` | `Color` | Copy helpers. |

#### Operations

| Name | Return Type | Description |
|------|-------------|-------------|
| `RelativeLuminance` | `double` | WCAG luminance. |
| `CompositeOver(background)` | `Color` | Source-over blend in gamma sRGB, as browsers blend. |
| `ContrastRatio(other)` | `double` | WCAG ratio, 1..21, with a translucent color composited over `other` first. |
| `AccessibilityLevelAgainst(background, largeText = false)` | `AccessibilityLevel` | Conformance rating. |
| `AdjustForContrast(background, target, largeText = false)` | `Color` | Nudge lightness (or mix towards black or white) to meet a target level. |
| `DistanceTo(other)` | `double` | Perceptual Oklab distance. |
| `MixOklab(other, t)` | `Color` | Perceptually uniform mix (`t = 0` returns this, `t = 1` returns other), gamut-mapped into sRGB. |
| `Lerp(other, t)` | `Color` | Linear-RGB interpolation. |
| `Gradient(to, steps)` | `IReadOnlyList<Color>` | Oklab gradient, inclusive of endpoints (`steps >= 2`). |
| `Simulate(deficiency, severity)` | `Color` | The color as seen with a color vision deficiency (Machado et al. 2009), severity 0..1. |

#### Adjustments

Convenience adjustments on `Color` operate in HSL and preserve alpha; for perceptually-uniform lightness and chroma use the `Oklch` equivalents via `ToOklch()`. All are also available natively on `Hsl` (returning `Hsl`) and `Srgb` (returning `Srgb`).

| Name | Return Type | Description |
|------|-------------|-------------|
| `WithSaturation(s)` / `SaturateBy(a)` / `DesaturateBy(a)` / `MultiplySaturation(f)` | `Color` | Set or shift HSL saturation (clamped to 0..1). |
| `WithLightness(l)` / `LightenBy(a)` / `DarkenBy(a)` / `MultiplyLightness(f)` | `Color` | Set or shift HSL lightness (clamped to 0..1). |
| `OffsetHue(degrees)` | `Color` | Rotate hue around the wheel (wraps at 360). |
| `ToGrayscale()` | `Color` | Drop saturation, keeping lightness. |
| `Invert()` | `Color` | Per-channel negative, computed in gamma-encoded sRGB. |

### Supporting types

| Type | Description | Base conversion |
|------|-------------|-----------------|
| `Srgb` | Gamma-encoded sRGB, the only gamma-boundary crossing. | `ToLinear()` / `FromLinear(Color)` (also as `ToColor()` / `FromColor()`) |
| `Hsl` / `Hsv` | Hue in degrees, saturation and lightness/value in 0..1, defined over sRGB. | `FromSrgb` / `ToSrgb` |
| `Oklab` | Perceptual color space (Ottosson 2020). | `FromColor` / `ToColor`; polar via `ToOklch` / `FromOklch` |
| `Oklch` | Polar form of Oklab. | `ToOklab` / `FromOklab` |
| `AccessibilityLevel` | enum: `Fail = 0`, `AA = 1`, `AAA = 2`. | — |
| `ColorVisionDeficiency` | enum: `Protan`, `Deutan`, `Tritan`. | — |
| `ColorVision` | `Matrix(deficiency, severity)`: the Machado et al. (2009) simulation matrix, interpolated between the published 0.1 steps. | — |
| `ColorVisionMatrix` | A 3x3 linear-RGB matrix (`M11`..`M33`) with `Apply(Color)`, which clamps to 0..1 and keeps alpha; `Identity`. | — |
| `NamedColors` | Common colors (`Black`, `White`, `Red`, `Orange`, `Transparent`, ...), plus `All` and `TryGet(name, out color)` with case-insensitive keys. | — |

The satellite spaces carry their own adjustments: `Hsl` and `Srgb` have the full HSL set (saturation/lightness/hue/grayscale; `Srgb` routes these through HSL and also adds `Invert()`), while `Oklch` exposes perceptually-uniform `WithLightness`/`LightenBy`/`DarkenBy`, chroma ops (`WithChroma`/`MultiplyChroma`/`SaturateBy`/`DesaturateBy`/`ToGrayscale`), and `OffsetHue`. `Color`'s adjustment methods forward to `Hsl`.

#### Converting between spaces

Beyond the base conversion listed above, every color type (including `Color`) exposes `To{Space}()` and `From{Space}(...)` for every other space, plus a uniform `ToColor()` / `FromColor(Color)` pair. Those extra methods are one-liners that reuse the base conversions rather than reimplementing any color math, routed through the nearest shared hub:

- **Within the sRGB family** (`Srgb`, `Hsl`, `Hsv`) conversions route through `Srgb`, so no gamma decode/encode happens.
- **Within the perceptual family** (`Oklab`, `Oklch`) conversions route through `Oklab`.
- **Across the two families** conversions route through the linear `Color` hub, crossing the gamma boundary exactly once.

So `srgb.ToHsl()` is the direct HSL definition, while `hsl.ToOklab()` goes `Hsl → Srgb → linear Color → Oklab`. Because the satellite spaces carry no alpha, cross-space conversions preserve only the color; use the `Color` overloads (or `WithAlpha`) when you need alpha.

`FromHex` throws `ArgumentException` for lengths other than 3, 6, or 8, and `Gradient` throws `ArgumentException` for `steps < 2`.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE.md](../LICENSE.md) file for details.
