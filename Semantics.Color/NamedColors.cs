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
