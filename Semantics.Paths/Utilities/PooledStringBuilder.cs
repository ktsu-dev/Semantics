// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Text;

/// <summary>
/// Provides pooled StringBuilder for efficient path operations.
/// </summary>
internal static class PooledStringBuilder
{
	[ThreadStatic]
	private static StringBuilder? t_cachedInstance;

	/// <summary>
	/// Gets a pooled StringBuilder instance.
	/// </summary>
	/// <returns>A StringBuilder instance that should be returned to the pool after use.</returns>
	public static StringBuilder Get()
	{
		StringBuilder? sb = t_cachedInstance;
		if (sb != null)
		{
			// Clear but don't reset capacity
			sb.Clear();
			t_cachedInstance = null;
			return sb;
		}
		return new StringBuilder();
	}

	/// <summary>
	/// Returns a StringBuilder to the pool.
	/// </summary>
	/// <param name="sb">The StringBuilder to return to the pool.</param>
	public static void Return(StringBuilder sb)
	{
		if (sb.Capacity <= 360) // Reasonable size limit
		{
			t_cachedInstance = sb;
		}
	}

	/// <summary>
	/// Combines multiple path components efficiently using pooled StringBuilder.
	/// </summary>
	/// <param name="components">The path components to combine.</param>
	/// <returns>The combined path string.</returns>
	public static string CombinePaths(params ReadOnlySpan<string> components)
	{
		if (components.Length == 0)
		{
			return InternedPathStrings.Empty;
		}
		if (components.Length == 1)
		{
			return components[0];
		}

		StringBuilder sb = Get();
		try
		{
			sb.Append(components[0]);
			for (int i = 1; i < components.Length; i++)
			{
				if (!SpanPathUtilities.EndsWithDirectorySeparator(sb.ToString().AsSpan()))
				{
					sb.Append(Path.DirectorySeparatorChar);
				}
				sb.Append(components[i]);
			}
			return sb.ToString();
		}
		finally
		{
			Return(sb);
		}
	}
}
