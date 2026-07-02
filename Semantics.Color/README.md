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
- **Color spaces**: sRGB (`Srgb`), HSL (`Hsl`), HSV (`Hsv`), Oklab (`Oklab`), and Oklch (`Oklch`), each with `From*` / `To*` conversions on `Color`.
- **Interop**: hex parse and format (`#RGB`, `#RRGGBB`, `#RRGGBBAA`), 8-bit byte tuples, and linear or sRGB `Vector3` / `Vector4` output (the sRGB vectors are what ImGui expects).
- **WCAG accessibility**: relative luminance, contrast ratio (1..21), conformance rating against a background, and `AdjustForContrast` which binary-searches Oklab lightness to hit a target while preserving hue and chroma.
- **Perceptual operations**: Oklab distance (`DistanceTo`), perceptually uniform mixing (`MixOklab`), and Oklab gradients (`Gradient`), alongside plain linear `Lerp`.
- **Named colors**: a CSS/X11 subset with case-insensitive lookup.

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
| `ContrastRatio(other)` | `double` | WCAG ratio, 1..21. |
| `AccessibilityLevelAgainst(background, largeText = false)` | `AccessibilityLevel` | Conformance rating. |
| `AdjustForContrast(background, target, largeText = false)` | `Color` | Nudge lightness to meet a target level. |
| `DistanceTo(other)` | `double` | Perceptual Oklab distance. |
| `MixOklab(other, t)` | `Color` | Perceptually uniform mix (`t = 0` returns this, `t = 1` returns other). |
| `Lerp(other, t)` | `Color` | Linear-RGB interpolation. |
| `Gradient(to, steps)` | `IReadOnlyList<Color>` | Oklab gradient, inclusive of endpoints (`steps >= 2`). |

### Supporting types

| Type | Description |
|------|-------------|
| `Srgb` | Gamma-encoded sRGB, the only gamma-boundary crossing. `ToLinear()`, `FromLinear(Color)`. |
| `Hsl` / `Hsv` | Hue in degrees, saturation and lightness/value in 0..1, defined over sRGB. `FromSrgb` / `ToSrgb`. |
| `Oklab` | Perceptual color space (Ottosson 2020). `FromColor` / `ToColor` / `ToOklch`. |
| `Oklch` | Polar form of Oklab. `ToOklab`. |
| `AccessibilityLevel` | enum: `Fail = 0`, `AA = 1`, `AAA = 2`. |
| `NamedColors` | Common colors (`Black`, `White`, `Red`, `Orange`, `Transparent`, ...), plus `All` and `TryGet(name, out color)` with case-insensitive keys. |

`FromHex` throws `ArgumentException` for lengths other than 3, 6, or 8, and `Gradient` throws `ArgumentException` for `steps < 2`.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE.md](../LICENSE.md) file for details.
