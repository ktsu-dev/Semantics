// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

#if !NET6_0_OR_GREATER
using System;

/// <summary>
/// Polyfill for ArgumentNullException.ThrowIfNull for older .NET versions
/// </summary>
internal static class ArgumentNullExceptionPolyfill
{
	/// <summary>
	/// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
	/// </summary>
	/// <param name="argument">The reference type argument to validate as non-null.</param>
	/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
	public static void ThrowIfNull(object? argument, string? paramName = null)
	{
		if (argument is null)
		{
			throw new ArgumentNullException(paramName);
		}
	}
}
#endif
