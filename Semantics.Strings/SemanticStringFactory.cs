// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

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
	public T FromString(string? value) => SemanticString<T>.Create<T>(value);

	/// <inheritdoc/>
	public T FromCharArray(char[]? value) => SemanticString<T>.Create<T>(value);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
	/// <inheritdoc/>
	public T FromReadOnlySpan(ReadOnlySpan<char> value) => SemanticString<T>.Create<T>(value);
#endif

	/// <inheritdoc/>
	public bool TryFromString(string? value, out T? result) => SemanticString<T>.TryCreate<T>(value, out result);

	/// <summary>
	/// Creates a new instance of the target semantic string type from a string value.
	/// </summary>
	/// <param name="value">The string value to convert.</param>
	/// <returns>A new instance of the target semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="FormatException">The string does not meet the validation criteria for the target type.</exception>
	public T Create(string? value) => SemanticString<T>.Create(value);

	/// <summary>
	/// Creates a new instance of the target semantic string type from a character array.
	/// </summary>
	/// <param name="value">The character array to convert.</param>
	/// <returns>A new instance of the target semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="FormatException">The character array does not meet the validation criteria for the target type.</exception>
	public T Create(char[]? value) => SemanticString<T>.Create(value);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
	/// <summary>
	/// Creates a new instance of the target semantic string type from a read-only character span.
	/// </summary>
	/// <param name="value">The read-only character span to convert.</param>
	/// <returns>A new instance of the target semantic string type.</returns>
	/// <exception cref="FormatException">The span does not meet the validation criteria for the target type.</exception>
	public T Create(ReadOnlySpan<char> value) => SemanticString<T>.Create(value);
#endif

	/// <summary>
	/// Attempts to create a new instance of the target semantic string type from a string value without throwing exceptions.
	/// </summary>
	/// <param name="value">The string value to convert.</param>
	/// <param name="result">
	/// When this method returns, contains the created semantic string instance if the conversion succeeded,
	/// or <see langword="null"/> if the conversion failed.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.
	/// </returns>
	public bool TryCreate(string? value, out T? result) => SemanticString<T>.TryCreate(value, out result);

	/// <summary>
	/// Attempts to create a new instance of the target semantic string type from a character array without throwing exceptions.
	/// </summary>
	/// <param name="value">The character array to convert.</param>
	/// <param name="result">
	/// When this method returns, contains the created semantic string instance if the conversion succeeded,
	/// or <see langword="null"/> if the conversion failed.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.
	/// </returns>
	public bool TryCreate(char[]? value, out T? result) => SemanticString<T>.TryCreate(value, out result);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
	/// <summary>
	/// Attempts to create a new instance of the target semantic string type from a read-only character span without throwing exceptions.
	/// </summary>
	/// <param name="value">The read-only character span to convert.</param>
	/// <param name="result">
	/// When this method returns, contains the created semantic string instance if the conversion succeeded,
	/// or <see langword="null"/> if the conversion failed.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.
	/// </returns>
	public bool TryCreate(ReadOnlySpan<char> value, out T? result) => SemanticString<T>.TryCreate(value, out result);
#endif
}
