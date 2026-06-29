# Semantics.Color Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build `ktsu.Semantics.Color`, a rigorous, dependency-light color-science library (canonical linear-RGB `Color` + space structs + WCAG/Oklab operations) that fixes the sRGB/linear gamma bug present in the existing ThemeProvider/ImGuiStyler color code.

**Architecture:** A canonical `readonly record struct Color(double R, double G, double B, double A)` stores **linear** RGB + alpha and is the type consumers pass around. Lightweight `readonly record struct` space types (`Srgb`, `Hsl`, `Hsv`, `Oklab`, `Oklch`) are conversion targets/math intermediates. The sRGB↔linear gamma transfer is confined to the `Srgb`↔`Color` boundary, so perceptual math can never run on gamma-encoded values. No UI-framework dependency (the ImGui adapter is a separate package in another repo).

**Tech Stack:** C# / .NET, ktsu.Sdk, multi-target `net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1`, MSTest (test project, net10.0 only), Polyfill (`Ensure`).

**Spec:** `docs/superpowers/specs/2026-06-29-semantics-color-design.md`

## Global Constraints

Every task implicitly includes these (copied verbatim from project conventions / the spec):

- **File header** on every `.cs` file (generator note N/A — these are hand-written):
  ```csharp
  // Copyright (c) ktsu.dev
  // All rights reserved.
  // Licensed under the MIT license.
  ```
- **Tabs** for indentation. **CRLF** line endings.
- **File-scoped namespace** (`namespace ktsu.Semantics.Color;`), `using` directives **inside** the namespace, no `this.` qualifier, explicit accessibility modifiers, braces on all control flow.
- **Nullable reference types enabled; warnings as errors** (inherited from ktsu.Sdk).
- Library namespace: `ktsu.Semantics.Color`. Test namespace: `ktsu.Semantics.Test.Colors` (NOT `...Color` — a `Color` namespace segment would shadow the `Color` type). Test files live in `Semantics.Test/Colors/`.
- **Channel storage is `double`; interop helpers return `float`.** Not generic over `INumber<T>`.
- **Canonical `Color` is linear RGB.** Only `Srgb`↔`Color` crosses the gamma boundary.
- Tests: explicit types (no `var`); float comparisons use `Assert.AreEqual(expected, actual, delta)`.
- Validation failures throw `ArgumentException` (most specific type available); use `Ensure.NotNull` (Polyfill) for null checks.
- Build the library: `dotnet build Semantics.Color/Semantics.Color.csproj`.
- Run a test class: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~<ClassName>"`.

## File Structure

Library (`Semantics.Color/`):

| File | Responsibility |
|---|---|
| `Semantics.Color.csproj` | Project file (mirrors `Semantics.Music.csproj`). |
| `Color.cs` | Canonical `Color` struct: fields, `FromLinear`, `WithAlpha`, clamp, `Vector4`/`Vector3` interop. |
| `Color.Conversions.cs` | `partial Color`: `Srgb`/hex/bytes/Oklab/Oklch/Hsl/Hsv `From*`/`To*`. |
| `Color.Operations.cs` | `partial Color`: `RelativeLuminance`, `ContrastRatio`, `AccessibilityLevelAgainst`, `AdjustForContrast`, `DistanceTo`, `MixOklab`, `Lerp`, `Gradient`. |
| `Srgb.cs` | `Srgb` struct + gamma transfer (`ToLinear`/`FromLinear`). |
| `Oklab.cs` | `Oklab` struct + linear↔Oklab matrices + `cbrt` helper. |
| `Oklch.cs` | `Oklch` struct + `Oklab`↔`Oklch` polar conversion. |
| `Hsl.cs` | `Hsl` struct + sRGB↔HSL. |
| `Hsv.cs` | `Hsv` struct + sRGB↔HSV. |
| `AccessibilityLevel.cs` | `AccessibilityLevel` enum. |
| `NamedColors.cs` | CSS/X11 named-color table. |

Tests (`Semantics.Test/Colors/`): one file per task as noted.

---

### Task 1: Project scaffold + `Color` core

**Files:**
- Create: `Semantics.Color/Semantics.Color.csproj`
- Create: `Semantics.Color/Color.cs`
- Modify: `Semantics.sln` (add project + config rows)
- Modify: `Semantics.Test/Semantics.Test.csproj` (add ProjectReference)
- Test: `Semantics.Test/Colors/ColorCoreTests.cs`

**Interfaces:**
- Produces: `readonly partial record struct Color(double R, double G, double B, double A)` with statics `Color FromLinear(double r, double g, double b, double a = 1.0)`; instance `Color WithAlpha(double a)`, `Color Clamp()`, `Vector4 ToLinearVector4()`, `Vector3 ToLinearVector3()`. (Space-specific conversions and ops are added as `partial` in later tasks.)

- [ ] **Step 1: Create the project file**

Create `Semantics.Color/Semantics.Color.csproj`:

```xml
<Project>
  <Sdk Name="Microsoft.NET.Sdk" />
  <Sdk Name="ktsu.Sdk" />

  <PropertyGroup>
    <TargetFrameworks>net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Polyfill" PrivateAssets="all" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Add the project to the solution**

In `Semantics.sln`, add this project declaration immediately after the `Semantics.Music` `EndProject` (line ~23):

```
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Semantics.Color", "Semantics.Color\Semantics.Color.csproj", "{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}"
EndProject
```

And add these rows to the `GlobalSection(ProjectConfigurationPlatforms)` block (after the `Semantics.Music` rows, before `EndGlobalSection`):

```
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Debug|x64.ActiveCfg = Debug|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Debug|x64.Build.0 = Debug|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Debug|x86.ActiveCfg = Debug|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Debug|x86.Build.0 = Debug|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Release|Any CPU.Build.0 = Release|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Release|x64.ActiveCfg = Release|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Release|x64.Build.0 = Release|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Release|x86.ActiveCfg = Release|Any CPU
		{D4E5F6A7-B8C9-0123-DEF0-456789ABCDEF}.Release|x86.Build.0 = Release|Any CPU
```

- [ ] **Step 3: Reference the project from the test project**

In `Semantics.Test/Semantics.Test.csproj`, add inside the existing `<ItemGroup>` of project references (after the `Semantics.Music` line):

```xml
    <ProjectReference Include="..\Semantics.Color\Semantics.Color.csproj" />
```

- [ ] **Step 4: Write the failing test**

Create `Semantics.Test/Colors/ColorCoreTests.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using System.Numerics;

using ktsu.Semantics.Color;

[TestClass]
public class ColorCoreTests
{
	[TestMethod]
	public void FromLinear_StoresChannelsAndDefaultsAlphaToOne()
	{
		Color c = Color.FromLinear(0.1, 0.2, 0.3);
		Assert.AreEqual(0.1, c.R, 1e-12);
		Assert.AreEqual(0.2, c.G, 1e-12);
		Assert.AreEqual(0.3, c.B, 1e-12);
		Assert.AreEqual(1.0, c.A, 1e-12);
	}

	[TestMethod]
	public void WithAlpha_ReplacesAlphaOnly()
	{
		Color c = Color.FromLinear(0.1, 0.2, 0.3, 1.0).WithAlpha(0.5);
		Assert.AreEqual(0.1, c.R, 1e-12);
		Assert.AreEqual(0.5, c.A, 1e-12);
	}

	[TestMethod]
	public void Clamp_BringsChannelsIntoUnitRange()
	{
		Color c = Color.FromLinear(-0.5, 0.5, 1.5, 2.0).Clamp();
		Assert.AreEqual(0.0, c.R, 1e-12);
		Assert.AreEqual(0.5, c.G, 1e-12);
		Assert.AreEqual(1.0, c.B, 1e-12);
		Assert.AreEqual(1.0, c.A, 1e-12);
	}

	[TestMethod]
	public void ToLinearVector4_ReturnsFloatChannels()
	{
		Vector4 v = Color.FromLinear(0.1, 0.2, 0.3, 0.4).ToLinearVector4();
		Assert.AreEqual(0.1f, v.X, 1e-6f);
		Assert.AreEqual(0.4f, v.W, 1e-6f);
	}
}
```

- [ ] **Step 5: Run the test to verify it fails**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~ColorCoreTests"`
Expected: FAIL — `Color` does not exist / does not compile.

- [ ] **Step 6: Implement `Color.cs`**

Create `Semantics.Color/Color.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;
using System.Numerics;

/// <summary>
/// A color stored as linear (not gamma-encoded) RGB plus straight alpha, each in the range 0..1.
/// This is the canonical color type; conversions to and from other color spaces live in the
/// <c>Color.Conversions</c> partial, and color-science operations in <c>Color.Operations</c>.
/// </summary>
/// <param name="R">Linear red channel.</param>
/// <param name="G">Linear green channel.</param>
/// <param name="B">Linear blue channel.</param>
/// <param name="A">Straight (non-premultiplied) alpha.</param>
public readonly partial record struct Color(double R, double G, double B, double A)
{
	/// <summary>Creates a color from linear RGB channels, defaulting alpha to fully opaque.</summary>
	/// <param name="r">Linear red channel.</param>
	/// <param name="g">Linear green channel.</param>
	/// <param name="b">Linear blue channel.</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>A linear-RGB color.</returns>
	public static Color FromLinear(double r, double g, double b, double a = 1.0) => new(r, g, b, a);

	/// <summary>Returns a copy of this color with a replaced alpha.</summary>
	/// <param name="a">The new straight alpha.</param>
	/// <returns>A color with the same RGB and the given alpha.</returns>
	public Color WithAlpha(double a) => new(R, G, B, a);

	/// <summary>Returns a copy with every channel clamped to the 0..1 range.</summary>
	/// <returns>A gamut- and alpha-clamped color.</returns>
	public Color Clamp() => new(Clamp01(R), Clamp01(G), Clamp01(B), Clamp01(A));

	/// <summary>Converts to a linear-RGBA <see cref="Vector4"/> (float).</summary>
	/// <returns>A float vector of the linear channels.</returns>
	public Vector4 ToLinearVector4() => new((float)R, (float)G, (float)B, (float)A);

	/// <summary>Converts to a linear-RGB <see cref="Vector3"/> (float), dropping alpha.</summary>
	/// <returns>A float vector of the linear RGB channels.</returns>
	public Vector3 ToLinearVector3() => new((float)R, (float)G, (float)B);

	internal static double Clamp01(double value) => value < 0.0 ? 0.0 : value > 1.0 ? 1.0 : value;
}
```

- [ ] **Step 7: Run the test to verify it passes**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~ColorCoreTests"`
Expected: PASS (4 tests).

- [ ] **Step 8: Commit**

```bash
git add Semantics.Color/ Semantics.sln Semantics.Test/Semantics.Test.csproj Semantics.Test/Colors/ColorCoreTests.cs
git commit -m "feat(color): scaffold Semantics.Color with canonical linear Color type"
```

---

### Task 2: `Srgb` struct + gamma boundary

**Files:**
- Create: `Semantics.Color/Srgb.cs`
- Create: `Semantics.Color/Color.Conversions.cs`
- Test: `Semantics.Test/Colors/SrgbConversionTests.cs`

**Interfaces:**
- Consumes: `Color.FromLinear`, `Color` ctor.
- Produces: `readonly record struct Srgb(double R, double G, double B)` with `Color ToLinear(double a = 1.0)` and `static Srgb FromLinear(Color color)`; on `Color` (partial): `static Color FromSrgb(Srgb srgb, double a = 1.0)`, `static Color FromSrgb(double r, double g, double b, double a = 1.0)`, `Srgb ToSrgb()`, `Vector4 ToSrgbVector4()`, `Vector3 ToSrgbVector3()`.

- [ ] **Step 1: Write the failing test**

Create `Semantics.Test/Colors/SrgbConversionTests.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using System.Numerics;

using ktsu.Semantics.Color;

[TestClass]
public class SrgbConversionTests
{
	[TestMethod]
	public void SrgbToLinearToSrgb_RoundTripsToIdentity()
	{
		for (int i = 0; i <= 100; i++)
		{
			double channel = i / 100.0;
			Srgb original = new(channel, channel, channel);
			Srgb roundTripped = original.ToLinear().ToSrgb();
			Assert.AreEqual(original.R, roundTripped.R, 1e-9);
		}
	}

	[TestMethod]
	public void Srgb_MidGray_DecodesToSmallerLinearValue()
	{
		// sRGB 0.5 is perceptual mid-gray; its linear value is ~0.214 (proves gamma decode happens).
		Color c = Color.FromSrgb(0.5, 0.5, 0.5);
		Assert.AreEqual(0.21404, c.R, 1e-4);
	}

	[TestMethod]
	public void Srgb_EndpointsMapToLinearEndpoints()
	{
		Assert.AreEqual(0.0, Color.FromSrgb(0.0, 0.0, 0.0).R, 1e-12);
		Assert.AreEqual(1.0, Color.FromSrgb(1.0, 1.0, 1.0).R, 1e-12);
	}

	[TestMethod]
	public void ToSrgbVector4_ReturnsGammaEncodedChannels()
	{
		Color c = Color.FromSrgb(0.5, 0.5, 0.5, 1.0);
		Vector4 v = c.ToSrgbVector4();
		Assert.AreEqual(0.5f, v.X, 1e-4f);
		Assert.AreEqual(1.0f, v.W, 1e-6f);
	}
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~SrgbConversionTests"`
Expected: FAIL — `Srgb` / `FromSrgb` / `ToSrgbVector4` not defined.

- [ ] **Step 3: Implement `Srgb.cs`**

Create `Semantics.Color/Srgb.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in the gamma-encoded sRGB space, each channel 0..1. This is the only color space that
/// crosses the gamma boundary to and from the linear <see cref="Color"/>.
/// </summary>
/// <param name="R">Gamma-encoded red channel.</param>
/// <param name="G">Gamma-encoded green channel.</param>
/// <param name="B">Gamma-encoded blue channel.</param>
public readonly record struct Srgb(double R, double G, double B)
{
	/// <summary>Converts this sRGB color to a linear <see cref="Color"/>.</summary>
	/// <param name="a">Straight alpha for the resulting color (default 1.0).</param>
	/// <returns>The linear-RGB equivalent.</returns>
	public Color ToLinear(double a = 1.0) => new(DecodeChannel(R), DecodeChannel(G), DecodeChannel(B), a);

	/// <summary>Converts a linear <see cref="Color"/> to gamma-encoded sRGB (alpha dropped).</summary>
	/// <param name="color">The linear color.</param>
	/// <returns>The sRGB equivalent.</returns>
	public static Srgb FromLinear(Color color) =>
		new(EncodeChannel(color.R), EncodeChannel(color.G), EncodeChannel(color.B));

	private static double DecodeChannel(double s) =>
		s <= 0.04045 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);

	private static double EncodeChannel(double linear) =>
		linear <= 0.0031308 ? 12.92 * linear : (1.055 * Math.Pow(linear, 1.0 / 2.4)) - 0.055;
}
```

- [ ] **Step 4: Implement the sRGB members of `Color.Conversions.cs`**

Create `Semantics.Color/Color.Conversions.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System.Numerics;

public readonly partial record struct Color
{
	/// <summary>Creates a linear color from a gamma-encoded <see cref="Srgb"/>.</summary>
	/// <param name="srgb">The sRGB color.</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromSrgb(Srgb srgb, double a = 1.0) => srgb.ToLinear(a);

	/// <summary>Creates a linear color from gamma-encoded sRGB channels.</summary>
	/// <param name="r">sRGB red channel (0..1).</param>
	/// <param name="g">sRGB green channel (0..1).</param>
	/// <param name="b">sRGB blue channel (0..1).</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromSrgb(double r, double g, double b, double a = 1.0) => new Srgb(r, g, b).ToLinear(a);

	/// <summary>Converts this linear color to gamma-encoded <see cref="Srgb"/>.</summary>
	/// <returns>The sRGB equivalent (alpha dropped).</returns>
	public Srgb ToSrgb() => Srgb.FromLinear(this);

	/// <summary>Converts to a gamma-encoded sRGB <see cref="Vector4"/> (float) — the value ImGui expects.</summary>
	/// <returns>A float vector of sRGB RGB plus alpha.</returns>
	public Vector4 ToSrgbVector4()
	{
		Srgb s = ToSrgb();
		return new Vector4((float)s.R, (float)s.G, (float)s.B, (float)A);
	}

	/// <summary>Converts to a gamma-encoded sRGB <see cref="Vector3"/> (float), dropping alpha.</summary>
	/// <returns>A float vector of sRGB RGB.</returns>
	public Vector3 ToSrgbVector3()
	{
		Srgb s = ToSrgb();
		return new Vector3((float)s.R, (float)s.G, (float)s.B);
	}
}
```

- [ ] **Step 5: Run the test to verify it passes**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~SrgbConversionTests"`
Expected: PASS (4 tests).

- [ ] **Step 6: Commit**

```bash
git add Semantics.Color/Srgb.cs Semantics.Color/Color.Conversions.cs Semantics.Test/Colors/SrgbConversionTests.cs
git commit -m "feat(color): add Srgb space and gamma-correct sRGB<->linear boundary"
```

---

### Task 3: Hex and byte conversions

**Files:**
- Modify: `Semantics.Color/Color.Conversions.cs` (add members)
- Test: `Semantics.Test/Colors/HexConversionTests.cs`

**Interfaces:**
- Consumes: `Color.FromSrgb`, `Color.ToSrgb`.
- Produces: on `Color` (partial): `static Color FromHex(string hex)`, `string ToHex()` (uppercase `#RRGGBB`, or `#RRGGBBAA` when alpha < 1), `static Color FromBytes(byte r, byte g, byte b, byte a = 255)`, `(byte R, byte G, byte B, byte A) ToBytes()`.

- [ ] **Step 1: Write the failing test**

Create `Semantics.Test/Colors/HexConversionTests.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using System;

using ktsu.Semantics.Color;

[TestClass]
public class HexConversionTests
{
	[TestMethod]
	public void FromHex_ParsesSixDigitAsOpaqueSrgb()
	{
		Color red = Color.FromHex("#FF0000");
		Assert.AreEqual(1.0, red.A, 1e-12);
		(byte r, byte g, byte b, byte a) = red.ToBytes();
		Assert.AreEqual((byte)255, r);
		Assert.AreEqual((byte)0, g);
		Assert.AreEqual((byte)0, b);
		Assert.AreEqual((byte)255, a);
	}

	[TestMethod]
	public void FromHex_ParsesEightDigitAlpha()
	{
		Color half = Color.FromHex("#00000080");
		Assert.AreEqual(128.0 / 255.0, half.A, 1e-6);
	}

	[TestMethod]
	public void FromHex_ParsesThreeDigitShorthand()
	{
		Color a = Color.FromHex("#F00");
		Color b = Color.FromHex("#FF0000");
		Assert.AreEqual(b.R, a.R, 1e-12);
	}

	[TestMethod]
	public void FromHex_AcceptsNoLeadingHash()
	{
		(byte r, _, _, _) = Color.FromHex("00FF00").ToBytes();
		Assert.AreEqual((byte)0, r);
	}

	[TestMethod]
	public void HexRoundTrip_IsStable()
	{
		Assert.AreEqual("#3A7BD5", Color.FromHex("#3A7BD5").ToHex());
	}

	[TestMethod]
	public void ToHex_EmitsAlphaWhenNotOpaque()
	{
		Assert.AreEqual("#3A7BD580", Color.FromHex("#3A7BD580").ToHex());
	}

	[TestMethod]
	public void FromHex_InvalidLength_Throws() =>
		Assert.ThrowsException<ArgumentException>(() => Color.FromHex("#12345"));
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~HexConversionTests"`
Expected: FAIL — `FromHex`/`ToHex`/`FromBytes`/`ToBytes` not defined.

- [ ] **Step 3: Add hex/byte members to `Color.Conversions.cs`**

Add inside the `public readonly partial record struct Color` body in `Semantics.Color/Color.Conversions.cs`, and add `using System;`, `using System.Globalization;` to that file's `using` block:

```csharp
	/// <summary>Creates a linear color from a hex string: <c>#RGB</c>, <c>#RRGGBB</c>, or <c>#RRGGBBAA</c> (leading '#' optional). Channels are interpreted as sRGB.</summary>
	/// <param name="hex">The hex color string.</param>
	/// <returns>The linear-RGB color.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="hex"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="hex"/> is not a recognised hex length.</exception>
	public static Color FromHex(string hex)
	{
		Ensure.NotNull(hex);

		string h = hex.StartsWith("#", StringComparison.Ordinal) ? hex.Substring(1) : hex;

		if (h.Length == 3)
		{
			h = new string([h[0], h[0], h[1], h[1], h[2], h[2]]);
		}

		if (h.Length is not (6 or 8))
		{
			throw new ArgumentException("Hex color must be #RGB, #RRGGBB, or #RRGGBBAA.", nameof(hex));
		}

		byte r = ParseByte(h, 0);
		byte g = ParseByte(h, 2);
		byte b = ParseByte(h, 4);
		byte a = h.Length == 8 ? ParseByte(h, 6) : (byte)255;
		return FromBytes(r, g, b, a);
	}

	/// <summary>Converts to an uppercase hex string: <c>#RRGGBB</c>, or <c>#RRGGBBAA</c> when alpha is not fully opaque.</summary>
	/// <returns>The hex string.</returns>
	public string ToHex()
	{
		(byte r, byte g, byte b, byte a) = ToBytes();
		return a == 255
			? $"#{r:X2}{g:X2}{b:X2}"
			: $"#{r:X2}{g:X2}{b:X2}{a:X2}";
	}

	/// <summary>Creates a linear color from 8-bit sRGB channels.</summary>
	/// <param name="r">sRGB red byte.</param>
	/// <param name="g">sRGB green byte.</param>
	/// <param name="b">sRGB blue byte.</param>
	/// <param name="a">Alpha byte (default 255).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromBytes(byte r, byte g, byte b, byte a = 255) =>
		FromSrgb(r / 255.0, g / 255.0, b / 255.0, a / 255.0);

	/// <summary>Converts to 8-bit sRGB channels plus an alpha byte.</summary>
	/// <returns>The rounded sRGB byte tuple.</returns>
	public (byte R, byte G, byte B, byte A) ToBytes()
	{
		Srgb s = ToSrgb();
		return (ToByte(s.R), ToByte(s.G), ToByte(s.B), ToByte(A));
	}

	private static byte ParseByte(string hex, int index) =>
		byte.Parse(hex.Substring(index, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

	private static byte ToByte(double channel)
	{
		double scaled = Math.Round(Clamp01(channel) * 255.0);
		return (byte)scaled;
	}
```

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~HexConversionTests"`
Expected: PASS (7 tests).

- [ ] **Step 5: Commit**

```bash
git add Semantics.Color/Color.Conversions.cs Semantics.Test/Colors/HexConversionTests.cs
git commit -m "feat(color): add hex and byte conversions (sRGB-interpreted)"
```

---

### Task 4: `Oklab` and `Oklch`

**Files:**
- Create: `Semantics.Color/Oklab.cs`
- Create: `Semantics.Color/Oklch.cs`
- Modify: `Semantics.Color/Color.Conversions.cs`
- Test: `Semantics.Test/Colors/OklabConversionTests.cs`

**Interfaces:**
- Consumes: `Color.FromLinear`, `Color` ctor.
- Produces: `readonly record struct Oklab(double L, double A, double B)` with `Color ToColor(double a = 1.0)`, `static Oklab FromColor(Color color)`, `Oklch ToOklch()`; `readonly record struct Oklch(double L, double C, double H)` with `Oklab ToOklab()`. On `Color` (partial): `Oklab ToOklab()`, `static Color FromOklab(Oklab oklab, double a = 1.0)`, `Oklch ToOklch()`, `static Color FromOklch(Oklch oklch, double a = 1.0)`.

- [ ] **Step 1: Write the failing test**

Create `Semantics.Test/Colors/OklabConversionTests.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class OklabConversionTests
{
	[TestMethod]
	public void White_HasLightnessOneAndNoChroma()
	{
		// Reference (Ottosson): linear white -> Oklab L=1, a=0, b=0.
		Oklab lab = Color.FromLinear(1.0, 1.0, 1.0).ToOklab();
		Assert.AreEqual(1.0, lab.L, 1e-4);
		Assert.AreEqual(0.0, lab.A, 1e-4);
		Assert.AreEqual(0.0, lab.B, 1e-4);
	}

	[TestMethod]
	public void OklabRoundTrip_IsIdentity()
	{
		Color original = Color.FromSrgb(0.2, 0.6, 0.9);
		Color roundTripped = Color.FromOklab(original.ToOklab());
		Assert.AreEqual(original.R, roundTripped.R, 1e-9);
		Assert.AreEqual(original.G, roundTripped.G, 1e-9);
		Assert.AreEqual(original.B, roundTripped.B, 1e-9);
	}

	[TestMethod]
	public void Oklch_RedHasPositiveChroma()
	{
		Oklch lch = Color.FromSrgb(1.0, 0.0, 0.0).ToOklch();
		Assert.IsTrue(lch.C > 0.1, $"expected chroma > 0.1 but was {lch.C}");
	}

	[TestMethod]
	public void OklchRoundTrip_IsIdentity()
	{
		Color original = Color.FromSrgb(0.2, 0.6, 0.9);
		Color roundTripped = Color.FromOklch(original.ToOklch());
		Assert.AreEqual(original.R, roundTripped.R, 1e-9);
		Assert.AreEqual(original.G, roundTripped.G, 1e-9);
		Assert.AreEqual(original.B, roundTripped.B, 1e-9);
	}
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~OklabConversionTests"`
Expected: FAIL — `Oklab`/`Oklch`/`ToOklab` not defined.

- [ ] **Step 3: Implement `Oklab.cs`**

Create `Semantics.Color/Oklab.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in the Oklab perceptual color space (Björn Ottosson, 2020), derived from linear RGB.
/// </summary>
/// <param name="L">Perceived lightness.</param>
/// <param name="A">Green–red axis (negative green, positive red/magenta).</param>
/// <param name="B">Blue–yellow axis (negative blue, positive yellow).</param>
public readonly record struct Oklab(double L, double A, double B)
{
	/// <summary>Converts a linear <see cref="Color"/> to Oklab.</summary>
	/// <param name="color">The linear color.</param>
	/// <returns>The Oklab equivalent.</returns>
	public static Oklab FromColor(Color color)
	{
		double l = (0.4122214708 * color.R) + (0.5363325363 * color.G) + (0.0514459929 * color.B);
		double m = (0.2119034982 * color.R) + (0.6806995451 * color.G) + (0.1073969566 * color.B);
		double s = (0.0883024619 * color.R) + (0.2817188376 * color.G) + (0.6299787005 * color.B);

		double l_ = Cbrt(l);
		double m_ = Cbrt(m);
		double s_ = Cbrt(s);

		return new Oklab(
			(0.2104542553 * l_) + (0.7936177850 * m_) - (0.0040720468 * s_),
			(1.9779984951 * l_) - (2.4285922050 * m_) + (0.4505937099 * s_),
			(0.0259040371 * l_) + (0.7827717662 * m_) - (0.8086757660 * s_));
	}

	/// <summary>Converts this Oklab color to a linear <see cref="Color"/>.</summary>
	/// <param name="a">Straight alpha for the result (default 1.0).</param>
	/// <returns>The linear-RGB equivalent.</returns>
	public Color ToColor(double a = 1.0)
	{
		double l_ = L + (0.3963377774 * A) + (0.2158037573 * B);
		double m_ = L - (0.1055613458 * A) - (0.0638541728 * B);
		double s_ = L - (0.0894841775 * A) - (1.2914855480 * B);

		double l = l_ * l_ * l_;
		double m = m_ * m_ * m_;
		double s = s_ * s_ * s_;

		return new Color(
			(+4.0767416621 * l) - (3.3077115913 * m) + (0.2309699292 * s),
			(-1.2684380046 * l) + (2.6097574011 * m) - (0.3413193965 * s),
			(-0.0041960863 * l) - (0.7034186147 * m) + (1.7076147010 * s),
			a);
	}

	/// <summary>Converts this Oklab color to its polar <see cref="Oklch"/> form.</summary>
	/// <returns>The Oklch equivalent.</returns>
	public Oklch ToOklch()
	{
		double c = Math.Sqrt((A * A) + (B * B));
		double h = Math.Atan2(B, A) * (180.0 / Math.PI);
		if (h < 0.0)
		{
			h += 360.0;
		}

		return new Oklch(L, c, h);
	}

	// netstandard2.0 lacks Math.Cbrt; a sign-aware Pow is correct for all inputs.
	private static double Cbrt(double value) => Math.Sign(value) * Math.Pow(Math.Abs(value), 1.0 / 3.0);
}
```

- [ ] **Step 4: Implement `Oklch.cs`**

Create `Semantics.Color/Oklch.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in the polar (lightness, chroma, hue) form of Oklab. Hue is in degrees, 0..360.
/// </summary>
/// <param name="L">Perceived lightness.</param>
/// <param name="C">Chroma (colourfulness).</param>
/// <param name="H">Hue angle in degrees, 0..360.</param>
public readonly record struct Oklch(double L, double C, double H)
{
	/// <summary>Converts this polar color back to Cartesian <see cref="Oklab"/>.</summary>
	/// <returns>The Oklab equivalent.</returns>
	public Oklab ToOklab()
	{
		double hRad = H * (Math.PI / 180.0);
		return new Oklab(L, C * Math.Cos(hRad), C * Math.Sin(hRad));
	}
}
```

- [ ] **Step 5: Add Oklab/Oklch members to `Color.Conversions.cs`**

Add inside the `Color` partial in `Semantics.Color/Color.Conversions.cs`:

```csharp
	/// <summary>Converts this linear color to <see cref="Oklab"/>.</summary>
	/// <returns>The Oklab equivalent.</returns>
	public Oklab ToOklab() => Oklab.FromColor(this);

	/// <summary>Creates a linear color from an <see cref="Oklab"/> value.</summary>
	/// <param name="oklab">The Oklab color.</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromOklab(Oklab oklab, double a = 1.0) => oklab.ToColor(a);

	/// <summary>Converts this linear color to <see cref="Oklch"/>.</summary>
	/// <returns>The Oklch equivalent.</returns>
	public Oklch ToOklch() => Oklab.FromColor(this).ToOklch();

	/// <summary>Creates a linear color from an <see cref="Oklch"/> value.</summary>
	/// <param name="oklch">The Oklch color.</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromOklch(Oklch oklch, double a = 1.0) => oklch.ToOklab().ToColor(a);
```

- [ ] **Step 6: Run the test to verify it passes**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~OklabConversionTests"`
Expected: PASS (4 tests).

- [ ] **Step 7: Commit**

```bash
git add Semantics.Color/Oklab.cs Semantics.Color/Oklch.cs Semantics.Color/Color.Conversions.cs Semantics.Test/Colors/OklabConversionTests.cs
git commit -m "feat(color): add Oklab and Oklch perceptual spaces"
```

---

### Task 5: `Hsl` and `Hsv`

**Files:**
- Create: `Semantics.Color/Hsl.cs`
- Create: `Semantics.Color/Hsv.cs`
- Modify: `Semantics.Color/Color.Conversions.cs`
- Test: `Semantics.Test/Colors/HslHsvConversionTests.cs`

**Interfaces:**
- Consumes: `Color.FromSrgb`, `Color.ToSrgb`, `Srgb`.
- Produces: `readonly record struct Hsl(double H, double S, double L)` with `static Hsl FromSrgb(Srgb srgb)`, `Srgb ToSrgb()`; `readonly record struct Hsv(double H, double S, double V)` with `static Hsv FromSrgb(Srgb srgb)`, `Srgb ToSrgb()`. On `Color` (partial): `Hsl ToHsl()`, `static Color FromHsl(Hsl hsl, double a = 1.0)`, `Hsv ToHsv()`, `static Color FromHsv(Hsv hsv, double a = 1.0)`. Hue is degrees 0..360.

- [ ] **Step 1: Write the failing test**

Create `Semantics.Test/Colors/HslHsvConversionTests.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class HslHsvConversionTests
{
	[TestMethod]
	public void Red_HasZeroHueFullSaturation()
	{
		Hsl hsl = Color.FromSrgb(1.0, 0.0, 0.0).ToHsl();
		Assert.AreEqual(0.0, hsl.H, 1e-6);
		Assert.AreEqual(1.0, hsl.S, 1e-6);
		Assert.AreEqual(0.5, hsl.L, 1e-6);
	}

	[TestMethod]
	public void Green_HasHue120()
	{
		Hsv hsv = Color.FromSrgb(0.0, 1.0, 0.0).ToHsv();
		Assert.AreEqual(120.0, hsv.H, 1e-4);
		Assert.AreEqual(1.0, hsv.S, 1e-6);
		Assert.AreEqual(1.0, hsv.V, 1e-6);
	}

	[TestMethod]
	public void HslRoundTrip_IsIdentity()
	{
		Color original = Color.FromSrgb(0.2, 0.6, 0.9);
		Color roundTripped = Color.FromHsl(original.ToHsl());
		Assert.AreEqual(original.R, roundTripped.R, 1e-9);
		Assert.AreEqual(original.G, roundTripped.G, 1e-9);
		Assert.AreEqual(original.B, roundTripped.B, 1e-9);
	}

	[TestMethod]
	public void HsvRoundTrip_IsIdentity()
	{
		Color original = Color.FromSrgb(0.7, 0.3, 0.55);
		Color roundTripped = Color.FromHsv(original.ToHsv());
		Assert.AreEqual(original.R, roundTripped.R, 1e-9);
		Assert.AreEqual(original.G, roundTripped.G, 1e-9);
		Assert.AreEqual(original.B, roundTripped.B, 1e-9);
	}
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~HslHsvConversionTests"`
Expected: FAIL — `Hsl`/`Hsv`/`ToHsl`/`ToHsv` not defined.

- [ ] **Step 3: Implement `Hsl.cs`**

Create `Semantics.Color/Hsl.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in HSL (hue, saturation, lightness), defined over the gamma-encoded sRGB channels.
/// Hue is in degrees, 0..360; saturation and lightness are 0..1.
/// </summary>
/// <param name="H">Hue angle in degrees, 0..360.</param>
/// <param name="S">Saturation, 0..1.</param>
/// <param name="L">Lightness, 0..1.</param>
public readonly record struct Hsl(double H, double S, double L)
{
	/// <summary>Converts a gamma-encoded <see cref="Srgb"/> color to HSL.</summary>
	/// <param name="srgb">The sRGB color.</param>
	/// <returns>The HSL equivalent.</returns>
	public static Hsl FromSrgb(Srgb srgb)
	{
		double max = Math.Max(srgb.R, Math.Max(srgb.G, srgb.B));
		double min = Math.Min(srgb.R, Math.Min(srgb.G, srgb.B));
		double l = (max + min) / 2.0;
		double h = 0.0;
		double s = 0.0;

		if (max > min)
		{
			double d = max - min;
			s = l > 0.5 ? d / (2.0 - max - min) : d / (max + min);
			h = HueDegrees(srgb, max, d);
		}

		return new Hsl(h, s, l);
	}

	/// <summary>Converts this HSL color to a gamma-encoded <see cref="Srgb"/>.</summary>
	/// <returns>The sRGB equivalent.</returns>
	public Srgb ToSrgb()
	{
		double h = NormalizeHue(H) / 360.0;
		if (S <= 0.0)
		{
			return new Srgb(L, L, L);
		}

		double q = L < 0.5 ? L * (1.0 + S) : L + S - (L * S);
		double p = (2.0 * L) - q;
		return new Srgb(
			HueToChannel(p, q, h + (1.0 / 3.0)),
			HueToChannel(p, q, h),
			HueToChannel(p, q, h - (1.0 / 3.0)));
	}

	internal static double NormalizeHue(double h)
	{
		double r = h % 360.0;
		return r < 0.0 ? r + 360.0 : r;
	}

	internal static double HueDegrees(Srgb srgb, double max, double d)
	{
		double h;
		if (max == srgb.R)
		{
			h = ((srgb.G - srgb.B) / d) + (srgb.G < srgb.B ? 6.0 : 0.0);
		}
		else if (max == srgb.G)
		{
			h = ((srgb.B - srgb.R) / d) + 2.0;
		}
		else
		{
			h = ((srgb.R - srgb.G) / d) + 4.0;
		}

		return h * 60.0;
	}

	private static double HueToChannel(double p, double q, double t)
	{
		if (t < 0.0)
		{
			t += 1.0;
		}

		if (t > 1.0)
		{
			t -= 1.0;
		}

		if (t < 1.0 / 6.0)
		{
			return p + ((q - p) * 6.0 * t);
		}

		if (t < 1.0 / 2.0)
		{
			return q;
		}

		if (t < 2.0 / 3.0)
		{
			return p + ((q - p) * ((2.0 / 3.0) - t) * 6.0);
		}

		return p;
	}
}
```

- [ ] **Step 4: Implement `Hsv.cs`**

Create `Semantics.Color/Hsv.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

/// <summary>
/// A color in HSV (hue, saturation, value), defined over the gamma-encoded sRGB channels.
/// Hue is in degrees, 0..360; saturation and value are 0..1.
/// </summary>
/// <param name="H">Hue angle in degrees, 0..360.</param>
/// <param name="S">Saturation, 0..1.</param>
/// <param name="V">Value (brightness), 0..1.</param>
public readonly record struct Hsv(double H, double S, double V)
{
	/// <summary>Converts a gamma-encoded <see cref="Srgb"/> color to HSV.</summary>
	/// <param name="srgb">The sRGB color.</param>
	/// <returns>The HSV equivalent.</returns>
	public static Hsv FromSrgb(Srgb srgb)
	{
		double max = Math.Max(srgb.R, Math.Max(srgb.G, srgb.B));
		double min = Math.Min(srgb.R, Math.Min(srgb.G, srgb.B));
		double d = max - min;
		double h = d > 0.0 ? Hsl.HueDegrees(srgb, max, d) : 0.0;
		double s = max > 0.0 ? d / max : 0.0;
		return new Hsv(h, s, max);
	}

	/// <summary>Converts this HSV color to a gamma-encoded <see cref="Srgb"/>.</summary>
	/// <returns>The sRGB equivalent.</returns>
	public Srgb ToSrgb()
	{
		double h = Hsl.NormalizeHue(H) / 60.0;
		double c = V * S;
		double x = c * (1.0 - Math.Abs((h % 2.0) - 1.0));
		double m = V - c;

		(double r, double g, double b) = h switch
		{
			< 1.0 => (c, x, 0.0),
			< 2.0 => (x, c, 0.0),
			< 3.0 => (0.0, c, x),
			< 4.0 => (0.0, x, c),
			< 5.0 => (x, 0.0, c),
			_ => (c, 0.0, x),
		};

		return new Srgb(r + m, g + m, b + m);
	}
}
```

- [ ] **Step 5: Add HSL/HSV members to `Color.Conversions.cs`**

Add inside the `Color` partial in `Semantics.Color/Color.Conversions.cs`:

```csharp
	/// <summary>Converts this linear color to <see cref="Hsl"/> (via sRGB).</summary>
	/// <returns>The HSL equivalent.</returns>
	public Hsl ToHsl() => Hsl.FromSrgb(ToSrgb());

	/// <summary>Creates a linear color from an <see cref="Hsl"/> value (via sRGB).</summary>
	/// <param name="hsl">The HSL color.</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromHsl(Hsl hsl, double a = 1.0) => FromSrgb(hsl.ToSrgb(), a);

	/// <summary>Converts this linear color to <see cref="Hsv"/> (via sRGB).</summary>
	/// <returns>The HSV equivalent.</returns>
	public Hsv ToHsv() => Hsv.FromSrgb(ToSrgb());

	/// <summary>Creates a linear color from an <see cref="Hsv"/> value (via sRGB).</summary>
	/// <param name="hsv">The HSV color.</param>
	/// <param name="a">Straight alpha (default 1.0).</param>
	/// <returns>The linear-RGB color.</returns>
	public static Color FromHsv(Hsv hsv, double a = 1.0) => FromSrgb(hsv.ToSrgb(), a);
```

- [ ] **Step 6: Run the test to verify it passes**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~HslHsvConversionTests"`
Expected: PASS (4 tests).

- [ ] **Step 7: Commit**

```bash
git add Semantics.Color/Hsl.cs Semantics.Color/Hsv.cs Semantics.Color/Color.Conversions.cs Semantics.Test/Colors/HslHsvConversionTests.cs
git commit -m "feat(color): add HSL and HSV conversions"
```

---

### Task 6: WCAG luminance, contrast, accessibility

**Files:**
- Create: `Semantics.Color/AccessibilityLevel.cs`
- Create: `Semantics.Color/Color.Operations.cs`
- Test: `Semantics.Test/Colors/AccessibilityTests.cs`

**Interfaces:**
- Consumes: `Color`, `Color.ToOklab`, `Color.FromOklab`, `Color.Clamp`, `Oklab`.
- Produces: `enum AccessibilityLevel { Fail, AA, AAA }`. On `Color` (partial): `double RelativeLuminance` (property), `double ContrastRatio(Color other)`, `AccessibilityLevel AccessibilityLevelAgainst(Color background, bool largeText = false)`, `Color AdjustForContrast(Color background, AccessibilityLevel target, bool largeText = false)`.

- [ ] **Step 1: Write the failing test**

Create `Semantics.Test/Colors/AccessibilityTests.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class AccessibilityTests
{
	[TestMethod]
	public void BlackOnWhite_HasMaximumContrast()
	{
		Color black = Color.FromSrgb(0.0, 0.0, 0.0);
		Color white = Color.FromSrgb(1.0, 1.0, 1.0);
		Assert.AreEqual(21.0, black.ContrastRatio(white), 1e-2);
	}

	[TestMethod]
	public void SameColor_HasUnitContrast()
	{
		Color gray = Color.FromSrgb(0.5, 0.5, 0.5);
		Assert.AreEqual(1.0, gray.ContrastRatio(gray), 1e-9);
	}

	[TestMethod]
	public void BlackOnWhite_RatesAaa()
	{
		Color black = Color.FromSrgb(0.0, 0.0, 0.0);
		Color white = Color.FromSrgb(1.0, 1.0, 1.0);
		Assert.AreEqual(AccessibilityLevel.AAA, black.AccessibilityLevelAgainst(white));
	}

	[TestMethod]
	public void AdjustForContrast_ReachesRequestedLevel()
	{
		Color background = Color.FromSrgb(1.0, 1.0, 1.0);
		Color faint = Color.FromSrgb(0.85, 0.85, 0.2);
		Color adjusted = faint.AdjustForContrast(background, AccessibilityLevel.AA);
		Assert.IsTrue(
			adjusted.AccessibilityLevelAgainst(background) >= AccessibilityLevel.AA,
			$"contrast was {adjusted.ContrastRatio(background)}");
	}
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~AccessibilityTests"`
Expected: FAIL — `AccessibilityLevel`/`ContrastRatio` not defined.

- [ ] **Step 3: Implement `AccessibilityLevel.cs`**

Create `Semantics.Color/AccessibilityLevel.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

/// <summary>WCAG 2.x contrast conformance levels, ordered so that higher is stricter.</summary>
public enum AccessibilityLevel
{
	/// <summary>Does not meet the AA contrast threshold.</summary>
	Fail = 0,

	/// <summary>Meets the WCAG AA contrast threshold.</summary>
	AA = 1,

	/// <summary>Meets the WCAG AAA contrast threshold.</summary>
	AAA = 2,
}
```

- [ ] **Step 4: Implement `Color.Operations.cs`**

Create `Semantics.Color/Color.Operations.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;

public readonly partial record struct Color
{
	/// <summary>Gets the WCAG relative luminance of this color (computed on the linear channels).</summary>
	public double RelativeLuminance => (0.2126 * R) + (0.7152 * G) + (0.0722 * B);

	/// <summary>Computes the WCAG contrast ratio (1..21) between this color and another.</summary>
	/// <param name="other">The other color.</param>
	/// <returns>The contrast ratio, from 1 (identical luminance) to 21 (black vs white).</returns>
	public double ContrastRatio(Color other)
	{
		double l1 = RelativeLuminance;
		double l2 = other.RelativeLuminance;
		double lighter = Math.Max(l1, l2);
		double darker = Math.Min(l1, l2);
		return (lighter + 0.05) / (darker + 0.05);
	}

	/// <summary>Rates the contrast of this color against a background per WCAG.</summary>
	/// <param name="background">The background color.</param>
	/// <param name="largeText">True for large text (lower thresholds).</param>
	/// <returns>The highest <see cref="AccessibilityLevel"/> the pair satisfies.</returns>
	public AccessibilityLevel AccessibilityLevelAgainst(Color background, bool largeText = false)
	{
		double contrast = ContrastRatio(background);
		if (contrast >= (largeText ? 4.5 : 7.0))
		{
			return AccessibilityLevel.AAA;
		}

		return contrast >= (largeText ? 3.0 : 4.5) ? AccessibilityLevel.AA : AccessibilityLevel.Fail;
	}

	/// <summary>
	/// Adjusts this color's Oklab lightness (preserving hue and chroma) until it meets the requested
	/// contrast level against a background. Returns this color unchanged if already sufficient or if
	/// no adjustment can reach the target.
	/// </summary>
	/// <param name="background">The background color.</param>
	/// <param name="target">The desired conformance level.</param>
	/// <param name="largeText">True for large text (lower thresholds).</param>
	/// <returns>An adjusted color, clamped to gamut.</returns>
	public Color AdjustForContrast(Color background, AccessibilityLevel target, bool largeText = false)
	{
		double required = target switch
		{
			AccessibilityLevel.AAA => largeText ? 4.5 : 7.0,
			AccessibilityLevel.AA => largeText ? 3.0 : 4.5,
			_ => 1.0,
		};

		if (ContrastRatio(background) >= required)
		{
			return this;
		}

		Oklab lab = ToOklab();
		bool goLighter = background.RelativeLuminance < 0.5;
		double lo = goLighter ? lab.L : 0.0;
		double hi = goLighter ? 1.0 : lab.L;

		// Contrast increases monotonically as L moves toward the chosen extreme; binary-search the
		// smallest movement that meets the requirement.
		for (int i = 0; i < 30; i++)
		{
			double mid = (lo + hi) / 2.0;
			Color candidate = Candidate(lab, mid);
			bool meets = candidate.ContrastRatio(background) >= required;
			if (goLighter)
			{
				if (meets)
				{
					hi = mid;
				}
				else
				{
					lo = mid;
				}
			}
			else if (meets)
			{
				lo = mid;
			}
			else
			{
				hi = mid;
			}
		}

		Color result = Candidate(lab, goLighter ? hi : lo);
		return result.ContrastRatio(background) >= required ? result : this;

		Color Candidate(Oklab source, double lightness) =>
			FromOklab(new Oklab(lightness, source.A, source.B), A).Clamp();
	}
}
```

- [ ] **Step 5: Run the test to verify it passes**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~AccessibilityTests"`
Expected: PASS (4 tests).

- [ ] **Step 6: Commit**

```bash
git add Semantics.Color/AccessibilityLevel.cs Semantics.Color/Color.Operations.cs Semantics.Test/Colors/AccessibilityTests.cs
git commit -m "feat(color): add WCAG luminance, contrast, and accessibility adjustment"
```

---

### Task 7: Mixing, interpolation, distance, gradients

**Files:**
- Modify: `Semantics.Color/Color.Operations.cs`
- Test: `Semantics.Test/Colors/ColorOperationTests.cs`

**Interfaces:**
- Consumes: `Color.ToOklab`, `Color.FromOklab`, `Oklab`.
- Produces: on `Color` (partial): `double DistanceTo(Color other)` (Oklab Euclidean), `Color MixOklab(Color other, double t)`, `Color Lerp(Color other, double t)` (linear), `IReadOnlyList<Color> Gradient(Color to, int steps)` (perceptual; throws `ArgumentException` when `steps < 2`).

- [ ] **Step 1: Write the failing test**

Create `Semantics.Test/Colors/ColorOperationTests.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using System;
using System.Collections.Generic;

using ktsu.Semantics.Color;

[TestClass]
public class ColorOperationTests
{
	[TestMethod]
	public void DistanceTo_Self_IsZero()
	{
		Color c = Color.FromSrgb(0.3, 0.6, 0.9);
		Assert.AreEqual(0.0, c.DistanceTo(c), 1e-12);
	}

	[TestMethod]
	public void DistanceTo_IsSymmetric()
	{
		Color a = Color.FromSrgb(0.1, 0.2, 0.3);
		Color b = Color.FromSrgb(0.9, 0.8, 0.7);
		Assert.AreEqual(a.DistanceTo(b), b.DistanceTo(a), 1e-12);
	}

	[TestMethod]
	public void Lerp_AtEndpoints_ReturnsEndpoints()
	{
		Color a = Color.FromLinear(0.0, 0.0, 0.0);
		Color b = Color.FromLinear(1.0, 1.0, 1.0);
		Assert.AreEqual(0.0, a.Lerp(b, 0.0).R, 1e-12);
		Assert.AreEqual(1.0, a.Lerp(b, 1.0).R, 1e-12);
		Assert.AreEqual(0.5, a.Lerp(b, 0.5).R, 1e-12);
	}

	[TestMethod]
	public void MixOklab_Halfway_IsBetweenEndpoints()
	{
		Color a = Color.FromSrgb(0.0, 0.0, 0.0);
		Color b = Color.FromSrgb(1.0, 1.0, 1.0);
		Color mid = a.MixOklab(b, 0.5);
		Assert.IsTrue(mid.R > a.R && mid.R < b.R, $"mid.R was {mid.R}");
	}

	[TestMethod]
	public void Gradient_ReturnsRequestedStepsWithMatchingEndpoints()
	{
		Color a = Color.FromSrgb(0.0, 0.0, 0.0);
		Color b = Color.FromSrgb(1.0, 1.0, 1.0);
		IReadOnlyList<Color> gradient = a.Gradient(b, 5);
		Assert.AreEqual(5, gradient.Count);
		Assert.AreEqual(a.R, gradient[0].R, 1e-9);
		Assert.AreEqual(b.R, gradient[4].R, 1e-9);
	}

	[TestMethod]
	public void Gradient_TooFewSteps_Throws() =>
		Assert.ThrowsException<ArgumentException>(() => Color.FromSrgb(0, 0, 0).Gradient(Color.FromSrgb(1, 1, 1), 1));
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~ColorOperationTests"`
Expected: FAIL — `DistanceTo`/`MixOklab`/`Lerp`/`Gradient` not defined.

- [ ] **Step 3: Add the operations to `Color.Operations.cs`**

Add `using System.Collections.Generic;` to the file's `using` block, then add inside the `Color` partial in `Semantics.Color/Color.Operations.cs`:

```csharp
	/// <summary>Computes the perceptual (Oklab Euclidean) distance to another color.</summary>
	/// <param name="other">The other color.</param>
	/// <returns>The Oklab distance.</returns>
	public double DistanceTo(Color other)
	{
		Oklab a = ToOklab();
		Oklab b = other.ToOklab();
		double dl = a.L - b.L;
		double da = a.A - b.A;
		double db = a.B - b.B;
		return Math.Sqrt((dl * dl) + (da * da) + (db * db));
	}

	/// <summary>Mixes this color with another in Oklab space (perceptually uniform).</summary>
	/// <param name="other">The other color.</param>
	/// <param name="t">The interpolation factor, 0 = this, 1 = other.</param>
	/// <returns>The mixed color.</returns>
	public Color MixOklab(Color other, double t)
	{
		Oklab a = ToOklab();
		Oklab b = other.ToOklab();
		double inv = 1.0 - t;
		Oklab mixed = new(
			(a.L * inv) + (b.L * t),
			(a.A * inv) + (b.A * t),
			(a.B * inv) + (b.B * t));
		return FromOklab(mixed, (A * inv) + (other.A * t));
	}

	/// <summary>Linearly interpolates this color with another in linear-RGB space.</summary>
	/// <param name="other">The other color.</param>
	/// <param name="t">The interpolation factor, 0 = this, 1 = other.</param>
	/// <returns>The interpolated color.</returns>
	public Color Lerp(Color other, double t)
	{
		double inv = 1.0 - t;
		return new Color(
			(R * inv) + (other.R * t),
			(G * inv) + (other.G * t),
			(B * inv) + (other.B * t),
			(A * inv) + (other.A * t));
	}

	/// <summary>Builds a perceptually-uniform (Oklab) gradient from this color to another.</summary>
	/// <param name="to">The end color.</param>
	/// <param name="steps">The number of colors to produce (at least 2).</param>
	/// <returns>The gradient, inclusive of both endpoints.</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="steps"/> is less than 2.</exception>
	public IReadOnlyList<Color> Gradient(Color to, int steps)
	{
		if (steps < 2)
		{
			throw new ArgumentException("A gradient needs at least 2 steps.", nameof(steps));
		}

		Color[] result = new Color[steps];
		for (int i = 0; i < steps; i++)
		{
			result[i] = MixOklab(to, i / (double)(steps - 1));
		}

		return result;
	}
```

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~ColorOperationTests"`
Expected: PASS (6 tests).

- [ ] **Step 5: Commit**

```bash
git add Semantics.Color/Color.Operations.cs Semantics.Test/Colors/ColorOperationTests.cs
git commit -m "feat(color): add Oklab mix, lerp, distance, and gradient"
```

---

### Task 8: `NamedColors` + gamma-regression sweep + full build

**Files:**
- Create: `Semantics.Color/NamedColors.cs`
- Test: `Semantics.Test/Colors/NamedColorsTests.cs`
- Test: `Semantics.Test/Colors/GammaRegressionTests.cs`

**Interfaces:**
- Consumes: `Color.FromHex`, `Color.ToHex`, `Color.ToOklab`, `Color.FromLinear`.
- Produces: `static class NamedColors` with `Color` properties (`Black`, `White`, `Red`, `Green`, `Blue`, `Yellow`, `Cyan`, `Magenta`, `Gray`, `Orange`, `Purple`, `Transparent`) and `IReadOnlyDictionary<string, Color> All` (case-insensitive) + `bool TryGet(string name, out Color color)`.

- [ ] **Step 1: Write the failing tests**

Create `Semantics.Test/Colors/NamedColorsTests.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class NamedColorsTests
{
	[TestMethod]
	public void Red_MatchesPureRedHex() =>
		Assert.AreEqual("#FF0000", NamedColors.Red.ToHex());

	[TestMethod]
	public void Transparent_HasZeroAlpha() =>
		Assert.AreEqual(0.0, NamedColors.Transparent.A, 1e-12);

	[TestMethod]
	public void TryGet_IsCaseInsensitive()
	{
		bool found = NamedColors.TryGet("ReD", out Color color);
		Assert.IsTrue(found);
		Assert.AreEqual("#FF0000", color.ToHex());
	}

	[TestMethod]
	public void TryGet_UnknownName_ReturnsFalse() =>
		Assert.IsFalse(NamedColors.TryGet("notacolor", out _));
}
```

Create `Semantics.Test/Colors/GammaRegressionTests.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Colors;

using ktsu.Semantics.Color;

[TestClass]
public class GammaRegressionTests
{
	[TestMethod]
	public void HexDisplayIsStable_AcrossLinearRoundTrip()
	{
		// Proves the fix is display-safe: hex -> linear Color -> hex returns the same hex.
		string[] samples = ["#000000", "#FFFFFF", "#3A7BD5", "#7F7F7F", "#FFA500"];
		foreach (string hex in samples)
		{
			Assert.AreEqual(hex, Color.FromHex(hex).ToHex());
		}
	}

	[TestMethod]
	public void OklabFromLinear_DiffersFromNaiveSrgbAsLinear()
	{
		// The old bug fed sRGB values straight into the Oklab matrix. Confirm the correct (linear)
		// computation produces a different lightness for mid-gray.
		Color correct = Color.FromHex("#7F7F7F");
		double correctL = correct.ToOklab().L;
		double naiveL = Color.FromLinear(127.0 / 255.0, 127.0 / 255.0, 127.0 / 255.0).ToOklab().L;
		Assert.AreNotEqual(naiveL, correctL, 1e-3);
	}
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~NamedColorsTests|FullyQualifiedName~GammaRegressionTests"`
Expected: FAIL — `NamedColors` not defined (GammaRegression compiles but cannot run until the build succeeds).

- [ ] **Step 3: Implement `NamedColors.cs`**

Create `Semantics.Color/NamedColors.cs`:

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Color;

using System;
using System.Collections.Generic;

/// <summary>A small table of common named colors (CSS/X11 subset), as linear <see cref="Color"/>.</summary>
public static class NamedColors
{
	/// <summary>Opaque black (#000000).</summary>
	public static Color Black => Color.FromHex("#000000");

	/// <summary>Opaque white (#FFFFFF).</summary>
	public static Color White => Color.FromHex("#FFFFFF");

	/// <summary>Opaque red (#FF0000).</summary>
	public static Color Red => Color.FromHex("#FF0000");

	/// <summary>Opaque green (#00FF00).</summary>
	public static Color Green => Color.FromHex("#00FF00");

	/// <summary>Opaque blue (#0000FF).</summary>
	public static Color Blue => Color.FromHex("#0000FF");

	/// <summary>Opaque yellow (#FFFF00).</summary>
	public static Color Yellow => Color.FromHex("#FFFF00");

	/// <summary>Opaque cyan (#00FFFF).</summary>
	public static Color Cyan => Color.FromHex("#00FFFF");

	/// <summary>Opaque magenta (#FF00FF).</summary>
	public static Color Magenta => Color.FromHex("#FF00FF");

	/// <summary>Opaque gray (#808080).</summary>
	public static Color Gray => Color.FromHex("#808080");

	/// <summary>Opaque orange (#FFA500).</summary>
	public static Color Orange => Color.FromHex("#FFA500");

	/// <summary>Opaque purple (#800080).</summary>
	public static Color Purple => Color.FromHex("#800080");

	/// <summary>Fully transparent black (#00000000).</summary>
	public static Color Transparent => Color.FromHex("#00000000");

	private static readonly Dictionary<string, Color> Table = new(StringComparer.OrdinalIgnoreCase)
	{
		["black"] = Black,
		["white"] = White,
		["red"] = Red,
		["green"] = Green,
		["blue"] = Blue,
		["yellow"] = Yellow,
		["cyan"] = Cyan,
		["magenta"] = Magenta,
		["gray"] = Gray,
		["grey"] = Gray,
		["orange"] = Orange,
		["purple"] = Purple,
		["transparent"] = Transparent,
	};

	/// <summary>Gets all named colors keyed by name (case-insensitive lookup).</summary>
	public static IReadOnlyDictionary<string, Color> All => Table;

	/// <summary>Looks up a named color by name, case-insensitively.</summary>
	/// <param name="name">The color name.</param>
	/// <param name="color">The resolved color, if found.</param>
	/// <returns>True when the name is known.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
	public static bool TryGet(string name, out Color color)
	{
		Ensure.NotNull(name);
		return Table.TryGetValue(name, out color);
	}
}
```

- [ ] **Step 4: Run the targeted tests to verify they pass**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~NamedColorsTests|FullyQualifiedName~GammaRegressionTests"`
Expected: PASS (6 tests).

- [ ] **Step 5: Build the library across all target frameworks**

Run: `dotnet build Semantics.Color/Semantics.Color.csproj`
Expected: Build succeeded, 0 warnings, 0 errors (warnings are errors), all of `net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1`.

- [ ] **Step 6: Run the full Color test suite**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~ktsu.Semantics.Test.Colors"`
Expected: PASS (all Color tests across the 8 test classes).

- [ ] **Step 7: Commit**

```bash
git add Semantics.Color/NamedColors.cs Semantics.Test/Colors/NamedColorsTests.cs Semantics.Test/Colors/GammaRegressionTests.cs
git commit -m "feat(color): add NamedColors and gamma-regression tests"
```

---

## Self-Review

**Spec coverage** (each spec item → task):
- Package `Semantics.Color`, wide TFMs, no UI dep → Task 1.
- Canonical `Color` (linear RGB + alpha), `double` storage, interop primitives → Tasks 1 (linear), 2 (`ToSrgbVector4`).
- `Srgb` gamma boundary (the bug fix) → Task 2 + Task 8 regression.
- Hex (`#RGB`/`#RRGGBB`/`#RRGGBBAA`) + bytes → Task 3.
- `Oklab`, `Oklch` + conversions → Task 4.
- `Hsl`, `Hsv` (v1 scope addition) + conversions → Task 5.
- WCAG `RelativeLuminance`, `ContrastRatio`, `AccessibilityLevel`, `AccessibilityLevelAgainst`, `AdjustForContrast` → Task 6.
- `DistanceTo`, `MixOklab`, `Lerp`, `Gradient` → Task 7.
- `NamedColors` (v1 scope addition) → Task 8.
- Testing: round-trips, known values, gamma regression, accessibility, NamedColors → distributed across Tasks 2–8.
- The ImGui adapter (`ktsu.ImGui.Color`) and the ThemeProvider/ImGuiApp migrations are **out of scope for this plan** by design (separate repos / follow-on plans) — see the spec's shipping order.

**Placeholder scan:** none — every code step contains complete, compilable content.

**Type consistency:** `Color` is `readonly partial record struct` declared once (Task 1) and extended via `partial` in Tasks 2/4/5 (`Color.Conversions.cs`) and 6/7 (`Color.Operations.cs`). Method/property names used in tests match the produced interfaces (`FromLinear`, `FromSrgb`, `ToSrgb`, `ToSrgbVector4`, `FromHex`, `ToHex`, `FromBytes`, `ToBytes`, `ToOklab`, `FromOklab`, `ToOklch`, `FromOklch`, `ToHsl`, `FromHsl`, `ToHsv`, `FromHsv`, `RelativeLuminance`, `ContrastRatio`, `AccessibilityLevelAgainst`, `AdjustForContrast`, `DistanceTo`, `MixOklab`, `Lerp`, `Gradient`). `Hsl.HueDegrees`/`Hsl.NormalizeHue` are `internal` and reused by `Hsv` (Task 5). `AccessibilityLevel` enum ordering (`Fail<AA<AAA`) supports the `>=` comparison used in Task 6's test.

## Notes for the implementer

- `dotnet test` uses the Microsoft.Testing.Platform runner (MSTest.Sdk). The `--filter "FullyQualifiedName~..."` syntax matches by substring; a `|` separates alternatives.
- After Task 1, every subsequent task's targeted test run also recompiles the whole library, so a break in any partial surfaces immediately.
- Round-trip tolerances: sRGB↔linear and HSL/HSV↔sRGB conversions are exact-by-construction and round-trip to ~1e-9 in `double`. **Oklab/Oklch are NOT** — the published Ottosson forward and inverse matrices are independently-rounded 10-significant-digit constants that do not invert to exact identity, so Oklab round-trips carry ~1e-7 error; use a `1e-6` tolerance there (this was corrected during implementation; the original `1e-9` note was a spec error). If a non-Oklab round-trip drifts above ~1e-9, investigate rather than loosen — it indicates a transcription bug.
