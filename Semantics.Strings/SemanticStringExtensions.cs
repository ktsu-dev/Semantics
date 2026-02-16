// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

/// <summary>
/// Provides extension methods for converting standard types to semantic string types.
/// These extensions enable fluent syntax for creating type-safe semantic strings from
/// regular strings, character arrays, and spans.
/// </summary>
/// <remarks>
/// The extension methods use the "As" naming convention to indicate type conversion
/// while maintaining clarity that a semantic type is being created.
/// All methods leverage the factory methods in <see cref="SemanticString{TDerived}"/>
/// to ensure proper validation and canonicalization.
/// </remarks>
public static class SemanticStringExtensions
{
	/// <summary>
	/// Converts a string to the specified semantic string type.
	/// </summary>
	/// <typeparam name="TDerived">The target semantic string type to convert to.</typeparam>
	/// <param name="value">The string value to convert. Can be <see langword="null"/>.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="FormatException">
	/// The <paramref name="value"/> does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This extension method provides a fluent syntax for converting strings to semantic types:
	/// <code>
	/// var emailAddress = "user@example.com".As&lt;EmailAddress&gt;();
	/// var filePath = "/path/to/file.txt".As&lt;FilePath&gt;();
	/// </code>
	/// The conversion process includes validation according to any attributes applied to the target type.
	/// </remarks>
	public static TDerived As<TDerived>(this string? value)
		where TDerived : SemanticString<TDerived>
		=> SemanticString<TDerived>.Create<TDerived>(value: value);

	/// <summary>
	/// Converts a character array to the specified semantic string type.
	/// </summary>
	/// <typeparam name="TDerived">The target semantic string type to convert to.</typeparam>
	/// <param name="value">The character array to convert. Can be <see langword="null"/>.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="FormatException">
	/// The string representation of <paramref name="value"/> does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This extension method enables conversion from character arrays to semantic string types:
	/// <code>
	/// char[] chars = {'t', 'e', 's', 't'};
	/// var semanticString = chars.As&lt;MySemanticString&gt;();
	/// </code>
	/// The character array is first converted to a string, then validated according to the target type's requirements.
	/// </remarks>
	public static TDerived As<TDerived>(this char[]? value)
		where TDerived : SemanticString<TDerived>
		=> SemanticString<TDerived>.Create<TDerived>(value: value);

	/// <summary>
	/// Converts a read-only character span to the specified semantic string type.
	/// </summary>
	/// <typeparam name="TDerived">The target semantic string type to convert to.</typeparam>
	/// <param name="value">The read-only character span to convert.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="FormatException">
	/// The string representation of <paramref name="value"/> does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This extension method enables efficient conversion from character spans to semantic string types
	/// without additional allocations until the final string creation:
	/// <code>
	/// ReadOnlySpan&lt;char&gt; span = "example".AsSpan();
	/// var semanticString = span.As&lt;MySemanticString&gt;();
	/// </code>
	/// This is particularly useful for performance-critical scenarios where span-based operations are preferred.
	/// </remarks>
	public static TDerived As<TDerived>(this ReadOnlySpan<char> value)
		where TDerived : SemanticString<TDerived>
		=> SemanticString<TDerived>.Create<TDerived>(value: value);

	/// <summary>
	/// Converts one semantic string type to another semantic string type.
	/// </summary>
	/// <typeparam name="TSource">The source semantic string type to convert from.</typeparam>
	/// <typeparam name="TTarget">The target semantic string type to convert to.</typeparam>
	/// <param name="value">The semantic string value to convert. Can be <see langword="null"/>.</param>
	/// <returns>A new instance of the target semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="FormatException">
	/// The underlying string value does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This extension method provides a fluent syntax for converting between semantic string types:
	/// <code>
	/// var emailAddress = "user@example.com".As&lt;EmailAddress&gt;();
	/// var userName = emailAddress.As&lt;UserName&gt;();
	/// var filePath = "/path/to/file.txt".As&lt;FilePath&gt;();
	/// var directoryPath = filePath.As&lt;DirectoryPath&gt;();
	/// </code>
	/// The conversion uses the underlying string value and validates it according to the target type's requirements.
	/// </remarks>
	public static TTarget As<TSource, TTarget>(this SemanticString<TSource>? value)
		where TSource : SemanticString<TSource>
		where TTarget : SemanticString<TTarget>
		=> SemanticString<TTarget>.Create<TTarget>(value: value?.WeakString);
}
