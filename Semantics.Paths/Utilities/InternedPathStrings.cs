// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

/// <summary>
/// Provides interned common strings for path operations to reduce memory allocations.
/// </summary>
internal static class InternedPathStrings
{
	/// <summary>
	/// Interned directory separator character as string.
	/// </summary>
	public static readonly string DirectorySeparator = string.Intern(Path.DirectorySeparatorChar.ToString());

	/// <summary>
	/// Interned alternative directory separator character as string.
	/// </summary>
	public static readonly string AltDirectorySeparator = string.Intern(Path.AltDirectorySeparatorChar.ToString());

	/// <summary>
	/// Interned empty string.
	/// </summary>
	public static readonly string Empty = string.Intern(string.Empty);

	/// <summary>
	/// Interned common path roots.
	/// </summary>
	public static readonly string WindowsRoot = string.Intern(@"C:\");
	public static readonly string UnixRoot = string.Intern("/");
	public static readonly string WindowsUncRoot = string.Intern(@"\\");

	/// <summary>
	/// Interns a string if it matches common path patterns, otherwise returns the original string.
	/// </summary>
	/// <param name="value">The string to potentially intern.</param>
	/// <returns>An interned string if it matches common patterns, otherwise the original string.</returns>
	public static string InternIfCommon(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return Empty;
		}

		// Intern common separators
		if (value == Path.DirectorySeparatorChar.ToString())
		{
			return DirectorySeparator;
		}
		if (value == Path.AltDirectorySeparatorChar.ToString())
		{
			return AltDirectorySeparator;
		}

		// Intern common roots
		return value.Length <= 4
			? value switch
			{
				@"C:\" => WindowsRoot,
				"/" => UnixRoot,
				@"\\" => WindowsUncRoot,
				_ => value
			}
			: value;
	}
}
