// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Factory interface for creating semantic string instances with validation.
/// Separates creation logic from the main semantic string implementation.
/// </summary>
/// <typeparam name="T">The semantic string type to create</typeparam>
public interface ISemanticStringFactory<T> where T : SemanticString<T>
{
	/// <summary>
	/// Creates a new semantic string instance from a string value.
	/// </summary>
	/// <param name="value">The string value to convert</param>
	/// <returns>A validated semantic string instance</returns>
	/// <exception cref="FormatException">The value doesn't meet validation criteria</exception>
	public T FromString(string? value);

	/// <summary>
	/// Creates a new semantic string instance from a character array.
	/// </summary>
	/// <param name="value">The character array to convert</param>
	/// <returns>A validated semantic string instance</returns>
	/// <exception cref="FormatException">The value doesn't meet validation criteria</exception>
	public T FromCharArray(char[]? value);

	/// <summary>
	/// Creates a new semantic string instance from a read-only span.
	/// </summary>
	/// <param name="value">The read-only span to convert</param>
	/// <returns>A validated semantic string instance</returns>
	/// <exception cref="FormatException">The value doesn't meet validation criteria</exception>
	public T FromReadOnlySpan(ReadOnlySpan<char> value);

	/// <summary>
	/// Attempts to create a semantic string instance without throwing exceptions.
	/// </summary>
	/// <param name="value">The string value to convert</param>
	/// <param name="result">The resulting semantic string if successful</param>
	/// <returns>True if creation was successful, false otherwise</returns>
	public bool TryFromString(string? value, out T? result);
}
