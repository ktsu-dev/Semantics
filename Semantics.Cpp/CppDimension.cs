// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp;

using System.Collections.Generic;
using System.Globalization;

using ktsu.Semantics.Vocabulary;

/// <summary>
/// How C++ writes a dimension.
/// </summary>
/// <remarks>
/// An extension rather than a member of <see cref="DimensionVector"/>, because it is the one thing
/// about a dimension that belongs to a target rather than to the physics. The vocabulary is shared
/// with the C# generator, which has no template argument list to write.
/// </remarks>
internal static class CppDimension
{
	/// <summary>
	/// Writes the dimension as the C++ template argument list, trimmed to its significant prefix.
	/// </summary>
	/// <remarks>
	/// Every exponent defaults to zero in the template, so <c>Dimension&lt;1, 0, -2&gt;</c> says
	/// what <c>Dimension&lt;1, 0, -2, 0, 0, 0, 0, 0&gt;</c> says with five fewer numbers to read
	/// past. A dimension whose exponents are all zero is the empty list.
	/// </remarks>
	/// <param name="dimension">The exponents to write.</param>
	/// <param name="template">How the target spells the dimension template.</param>
	/// <returns>The type, as C++.</returns>
	internal static string ToCpp(this DimensionVector dimension, string @template)
	{
		int significant = DimensionVector.Axes.Length;
		while (significant > 0 && dimension[significant - 1] == 0)
		{
			significant--;
		}

		if (significant == 0)
		{
			// Written out rather than left as `Dimension<>` so the degenerate case reads as a
			// deliberate choice rather than an argument list someone forgot to fill in.
			return $"{@template}<0>";
		}

		List<string> arguments = [];
		for (int axis = 0; axis < significant; axis++)
		{
			arguments.Add(dimension[axis].ToString(CultureInfo.InvariantCulture));
		}

		return $"{@template}<{string.Join(", ", arguments)}>";
	}
}
