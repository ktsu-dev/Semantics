// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Default factory implementation for creating semantic string instances.
/// Implements the Factory pattern to separate creation concerns from the main semantic string class.
/// </summary>
/// <typeparam name="T">The semantic string type to create</typeparam>
public sealed class SemanticStringFactory<T> : ISemanticStringFactory<T>
	where T : SemanticString<T>
{
	/// <summary>
	/// Gets the singleton instance of the factory for the specified type.
	/// </summary>
	public static SemanticStringFactory<T> Default { get; } = new();

	/// <inheritdoc/>
	public T FromString(string? value) => SemanticString<T>.FromString<T>(value);

	/// <inheritdoc/>
	public T FromCharArray(char[]? value) => SemanticString<T>.FromCharArray<T>(value);

	/// <inheritdoc/>
	public T FromReadOnlySpan(ReadOnlySpan<char> value) => SemanticString<T>.FromReadOnlySpan<T>(value);

	/// <inheritdoc/>
	public bool TryFromString(string? value, out T? result)
	{
		result = null;
		try
		{
			if (value is null)
			{
				return false;
			}

			result = FromString(value);
			return true;
		}
		catch (FormatException)
		{
			return false;
		}
		catch (ArgumentNullException)
		{
			return false;
		}
	}
}
