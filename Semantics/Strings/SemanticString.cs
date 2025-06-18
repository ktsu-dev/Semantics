// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

/// <summary>
/// Base class for all semantic string types using CRTP (Curiously Recurring Template Pattern).
/// Provides type safety and validation for string values that have specific meaning or format requirements.
/// </summary>
[DebuggerDisplay(value: $"{{{nameof(GetDebuggerDisplay)}(),nq}}")]

public abstract record SemanticString<TDerived> : ISemanticString
	where TDerived : SemanticString<TDerived>
{
	/// <summary>
	/// Converts this semantic string to another semantic string type while preserving the underlying string value.
	/// </summary>
	/// <typeparam name="TDest">The target semantic string type to convert to.</typeparam>
	/// <returns>A new instance of the target semantic string type containing the same string value.</returns>
	/// <exception cref="ArgumentException">
	/// The current string value does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This method enables type conversions between compatible semantic string types:
	/// <code>
	/// var filePath = absolutePath.As&lt;FilePath&gt;();
	/// var url = uriString.As&lt;UrlString&gt;();
	/// </code>
	/// The conversion includes validation according to the target type's attributes and requirements.
	/// </remarks>
	public TDest As<TDest>()
		where TDest : SemanticString<TDest>
		=> Create<TDest>(WeakString);

	/// <summary>
	/// Provides a hook for derived types to normalize or canonicalize the input string before storage.
	/// </summary>
	/// <param name="input">The input string to be canonicalized.</param>
	/// <returns>The canonicalized version of the input string.</returns>
	/// <remarks>
	/// Override this method in derived types to implement custom normalization logic such as:
	/// <list type="bullet">
	/// <item><description>Converting to a standard case (upper/lower)</description></item>
	/// <item><description>Normalizing path separators</description></item>
	/// <item><description>Removing or standardizing whitespace</description></item>
	/// <item><description>Applying cultural-specific transformations</description></item>
	/// </list>
	/// The canonicalization occurs before validation, so the normalized value must still pass all validation rules.
	/// The base implementation returns the input unchanged.
	/// </remarks>
	protected virtual string MakeCanonical(string input) => input;

	/// <inheritdoc/>
	public char[] ToCharArray() => ToCharArray(semanticString: this);
	/// <inheritdoc/>
	public char[] ToCharArray(int startIndex, int length) => WeakString.ToCharArray(startIndex: startIndex, length: length);

	// ISemanticString implementation
	/// <summary>
	/// Gets or initializes the underlying string value for interoperability with non-semantic string operations.
	/// </summary>
	/// <value>
	/// The raw string value contained within this semantic string. Defaults to <see cref="string.Empty"/>.
	/// </value>
	/// <remarks>
	/// Use this property when you need to interoperate with APIs that expect regular strings.
	/// The name "WeakString" emphasizes that accessing this property breaks the type safety guarantees
	/// that semantic strings provide. Consider using implicit conversion to string instead when possible.
	/// This property is init-only to support record initialization while maintaining immutability.
	/// </remarks>
	public string WeakString { get; init; } = string.Empty;

	/// <inheritdoc/>
	public int Length => WeakString.Length;

	/// <inheritdoc/>
	public char this[int index] => WeakString[index: index];

	/// <inheritdoc/>
	public int CompareTo(object? value) => WeakString.CompareTo(value: value);
	/// <inheritdoc/>
	public int CompareTo(ISemanticString? other) => WeakString.CompareTo(strB: other?.WeakString);

	/// <inheritdoc/>
	public bool Contains(string value) => WeakString.Contains(value: value);
	/// <inheritdoc/>
	public bool Contains(string value, StringComparison comparisonType) => WeakString.Contains(value: value, comparisonType: comparisonType);

	/// <inheritdoc/>
	public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) => WeakString.CopyTo(sourceIndex: sourceIndex, destination: destination, destinationIndex: destinationIndex, count: count);

	/// <inheritdoc/>
	public bool EndsWith(string value) => WeakString.EndsWith(value: value);
	/// <inheritdoc/>
	public bool EndsWith(string value, bool ignoreCase, CultureInfo culture) => WeakString.EndsWith(value: value, ignoreCase: ignoreCase, culture: culture);
	/// <inheritdoc/>
	public bool EndsWith(string value, StringComparison comparisonType) => WeakString.EndsWith(value: value, comparisonType: comparisonType);

	/// <inheritdoc/>
	public bool Equals(string value) => WeakString.Equals(value: value);
	/// <inheritdoc/>
	public bool Equals(string value, StringComparison comparisonType) => WeakString.Equals(value: value, comparisonType: comparisonType);

	/// <inheritdoc/>
	public CharEnumerator GetEnumerator() => WeakString.GetEnumerator();

	/// <inheritdoc/>
	public TypeCode GetTypeCode() => WeakString.GetTypeCode();

	/// <inheritdoc/>
	public int IndexOf(char value) => WeakString.IndexOf(value: value);
	/// <inheritdoc/>
	public int IndexOf(char value, int startIndex) => WeakString.IndexOf(value: value, startIndex: startIndex);
	/// <inheritdoc/>
	public int IndexOf(char value, int startIndex, int count) => WeakString.IndexOf(value: value, startIndex: startIndex, count: count);
	/// <inheritdoc/>
	public int IndexOf(string value) => WeakString.IndexOf(value: value);
	/// <inheritdoc/>
	public int IndexOf(string value, int startIndex) => WeakString.IndexOf(value: value, startIndex: startIndex);
	/// <inheritdoc/>
	public int IndexOf(string value, int startIndex, int count) => WeakString.IndexOf(value: value, startIndex: startIndex, count: count);
	/// <inheritdoc/>
	public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType) => WeakString.IndexOf(value: value, startIndex: startIndex, count: count, comparisonType: comparisonType);
	/// <inheritdoc/>
	public int IndexOf(string value, int startIndex, StringComparison comparisonType) => WeakString.IndexOf(value: value, startIndex: startIndex, comparisonType: comparisonType);
	/// <inheritdoc/>
	public int IndexOf(string value, StringComparison comparisonType) => WeakString.IndexOf(value: value, comparisonType: comparisonType);

	/// <inheritdoc/>
	public int IndexOfAny(char[] anyOf) => WeakString.IndexOfAny(anyOf: anyOf);
	/// <inheritdoc/>
	public int IndexOfAny(char[] anyOf, int startIndex) => WeakString.IndexOfAny(anyOf: anyOf, startIndex: startIndex);
	/// <inheritdoc/>
	public int IndexOfAny(char[] anyOf, int startIndex, int count) => WeakString.IndexOfAny(anyOf: anyOf, startIndex: startIndex, count: count);

	/// <inheritdoc/>
	public string Insert(int startIndex, string value) => WeakString.Insert(startIndex: startIndex, value: value);

	/// <inheritdoc/>
	public bool IsNormalized() => WeakString.IsNormalized();
	/// <inheritdoc/>
	public bool IsNormalized(NormalizationForm normalizationForm) => WeakString.IsNormalized(normalizationForm: normalizationForm);

	/// <inheritdoc/>
	public int LastIndexOf(char value) => WeakString.LastIndexOf(value: value);
	/// <inheritdoc/>
	public int LastIndexOf(char value, int startIndex) => WeakString.LastIndexOf(value: value, startIndex: startIndex);
	/// <inheritdoc/>
	public int LastIndexOf(char value, int startIndex, int count) => WeakString.LastIndexOf(value: value, startIndex: startIndex, count: count);
	/// <inheritdoc/>
	public int LastIndexOf(string value) => WeakString.LastIndexOf(value: value);
	/// <inheritdoc/>
	public int LastIndexOf(string value, int startIndex) => WeakString.LastIndexOf(value: value, startIndex: startIndex);
	/// <inheritdoc/>
	public int LastIndexOf(string value, int startIndex, int count) => WeakString.LastIndexOf(value: value, startIndex: startIndex, count: count);
	/// <inheritdoc/>
	public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType) => WeakString.LastIndexOf(value: value, startIndex: startIndex, count: count, comparisonType: comparisonType);
	/// <inheritdoc/>
	public int LastIndexOf(string value, int startIndex, StringComparison comparisonType) => WeakString.LastIndexOf(value: value, startIndex: startIndex, comparisonType: comparisonType);
	/// <inheritdoc/>
	public int LastIndexOf(string value, StringComparison comparisonType) => WeakString.LastIndexOf(value: value, comparisonType: comparisonType);

	/// <inheritdoc/>
	public int LastIndexOfAny(char[] anyOf) => WeakString.LastIndexOfAny(anyOf: anyOf);
	/// <inheritdoc/>
	public int LastIndexOfAny(char[] anyOf, int startIndex) => WeakString.LastIndexOfAny(anyOf: anyOf, startIndex: startIndex);
	/// <inheritdoc/>
	public int LastIndexOfAny(char[] anyOf, int startIndex, int count) => WeakString.LastIndexOfAny(anyOf: anyOf, startIndex: startIndex, count: count);

	/// <inheritdoc/>
	public string Normalize() => WeakString.Normalize();
	/// <inheritdoc/>
	public string Normalize(NormalizationForm normalizationForm) => WeakString.Normalize(normalizationForm: normalizationForm);

	/// <inheritdoc/>
	public string PadLeft(int totalWidth) => WeakString.PadLeft(totalWidth: totalWidth);
	/// <inheritdoc/>
	public string PadLeft(int totalWidth, char paddingChar) => WeakString.PadLeft(totalWidth: totalWidth, paddingChar: paddingChar);

	/// <inheritdoc/>
	public string PadRight(int totalWidth) => WeakString.PadRight(totalWidth: totalWidth);
	/// <inheritdoc/>
	public string PadRight(int totalWidth, char paddingChar) => WeakString.PadRight(totalWidth: totalWidth, paddingChar: paddingChar);

	/// <inheritdoc/>
	[SuppressMessage("Style", "IDE0057:Use range operator", Justification = "I'd rather wrap the class 1:1 than reimplement it")]
	public string Remove(int startIndex) => WeakString.Remove(startIndex: startIndex);
	/// <inheritdoc/>
	public string Remove(int startIndex, int count) => WeakString.Remove(startIndex: startIndex, count: count);

	/// <inheritdoc/>
	public string Replace(char oldChar, char newChar) => WeakString.Replace(oldChar: oldChar, newChar: newChar);
	/// <inheritdoc/>
	public string Replace(string oldValue, string newValue) => WeakString.Replace(oldValue: oldValue, newValue: newValue);

	/// <inheritdoc/>
	public string[] Split(char[] separator, int count) => WeakString.Split(separator: separator, count: count);
	/// <inheritdoc/>
	public string[] Split(char[] separator, int count, StringSplitOptions options) => WeakString.Split(separator: separator, count: count, options: options);
	/// <inheritdoc/>
	public string[] Split(char[] separator, StringSplitOptions options) => WeakString.Split(separator: separator, options: options);
	/// <inheritdoc/>
	public string[] Split(params char[] separator) => WeakString.Split(separator: separator);
	/// <inheritdoc/>
	public string[] Split(string[] separator, int count, StringSplitOptions options) => WeakString.Split(separator: separator, count: count, options: options);
	/// <inheritdoc/>
	public string[] Split(string[] separator, StringSplitOptions options) => WeakString.Split(separator: separator, options: options);

	/// <inheritdoc/>
	public bool StartsWith(string value) => WeakString.StartsWith(value: value);
	/// <inheritdoc/>
	public bool StartsWith(string value, bool ignoreCase, CultureInfo culture) => WeakString.StartsWith(value: value, ignoreCase: ignoreCase, culture: culture);
	/// <inheritdoc/>
	public bool StartsWith(string value, StringComparison comparisonType) => WeakString.StartsWith(value: value, comparisonType: comparisonType);

	/// <inheritdoc/>
	public string Substring(int startIndex) => WeakString[startIndex..];
	/// <inheritdoc/>
	public string Substring(int startIndex, int length) => WeakString.Substring(startIndex: startIndex, length: length);

	/// <inheritdoc/>
	public string ToLower() => WeakString.ToLower();
	/// <inheritdoc/>
	public string ToLower(CultureInfo culture) => WeakString.ToLower(culture: culture);

	/// <inheritdoc/>
	public string ToLowerInvariant() => WeakString.ToLowerInvariant();

	/// <inheritdoc/>
	public sealed override string ToString() => WeakString;
	/// <inheritdoc/>
	public string ToString(IFormatProvider provider) => WeakString.ToString(provider: provider);

	/// <inheritdoc/>
	public string ToUpper() => WeakString.ToUpper();
	/// <inheritdoc/>
	public string ToUpper(CultureInfo culture) => WeakString.ToUpper(culture: culture);

	/// <inheritdoc/>
	public string ToUpperInvariant() => WeakString.ToUpperInvariant();

	/// <inheritdoc/>
	public string Trim() => WeakString.Trim();
	/// <inheritdoc/>
	public string Trim(params char[] trimChars) => WeakString.Trim(trimChars: trimChars);

	/// <inheritdoc/>
	public string TrimEnd(params char[] trimChars) => WeakString.TrimEnd(trimChars: trimChars);
	/// <inheritdoc/>
	public string TrimStart(params char[] trimChars) => WeakString.TrimStart(trimChars: trimChars);

	/// <summary>
	/// Returns an enumerator that iterates through the characters in the semantic string.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the characters in the string.</returns>
	IEnumerator<char> IEnumerable<char>.GetEnumerator() => ((IEnumerable<char>)WeakString).GetEnumerator();

	/// <summary>
	/// Returns an enumerator that iterates through the characters in the semantic string.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the characters in the string.</returns>
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)WeakString).GetEnumerator();

	// SemanticString implementation
	/// <inheritdoc/>
	[ExcludeFromCodeCoverage(Justification = "DebuggerDisplay")]
	protected string GetDebuggerDisplay() => $"({GetType().Name})\"{WeakString}\"";

	/// <inheritdoc/>
	public static string ToString(ISemanticString? semanticString) => semanticString?.WeakString ?? string.Empty;
	/// <inheritdoc/>
	public static char[] ToCharArray(ISemanticString? semanticString) => semanticString?.WeakString.ToCharArray() ?? [];
	/// <inheritdoc/>
	public static ReadOnlySpan<char> ToReadOnlySpan(ISemanticString? semanticString) => semanticString is null ? [] : semanticString.WeakString.AsSpan();

	/// <inheritdoc/>
	public bool IsEmpty() => IsEmpty(semanticString: this);

	/// <summary>
	/// Determines whether the specified semantic string is empty.
	/// </summary>
	/// <param name="semanticString">The semantic string to check.</param>
	/// <returns><see langword="true"/> if the semantic string is null or has zero length; otherwise, <see langword="false"/>.</returns>
	private static bool IsEmpty(SemanticString<TDerived>? semanticString) => semanticString?.Length == 0;

	/// <summary>
	/// Determines whether this semantic string instance is valid according to both basic requirements and attribute validations.
	/// </summary>
	/// <returns>
	/// <see langword="true"/> if the string is valid (non-null and passes all validation attributes); otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method performs two levels of validation:
	/// <list type="number">
	/// <item><description>Basic validation: ensures the underlying string is not null</description></item>
	/// <item><description>Attribute validation: validates against all validation attributes applied to the type</description></item>
	/// </list>
	/// Override this method in derived types to add custom validation logic beyond attribute-based validation.
	/// </remarks>
	public virtual bool IsValid() => IsValid(semanticString: this) && ValidateAttributes();

	/// <summary>
	/// Determines whether the specified semantic string is valid (not null).
	/// </summary>
	/// <param name="semanticString">The semantic string to check.</param>
	/// <returns><see langword="true"/> if the semantic string is not null and has a non-null WeakString; otherwise, <see langword="false"/>.</returns>
	private static bool IsValid(SemanticString<TDerived>? semanticString) => semanticString?.WeakString is not null;

	/// <summary>
	/// Validates that this SemanticString instance meets the criteria defined by
	/// the validation attributes applied to its type.
	/// </summary>
	/// <returns><see langword="true"/> if the string passes all attribute validations; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method uses reflection to find all validation attributes (classes inheriting from <see cref="SemanticStringValidationAttribute"/>)
	/// applied to the current type and validates the string value against each one.
	/// The validation logic respects <see cref="ValidateAllAttribute"/> (default) and <see cref="ValidateAnyAttribute"/> decorations
	/// to determine whether all attributes must pass or if any single attribute passing is sufficient.
	/// </remarks>
	public virtual bool ValidateAttributes() => AttributeValidation.ValidateAttributes(this, GetType());

	// Note: Explicit conversion operators were removed in favor of the .As<T>() extension method pattern
	// for better discoverability, consistency, and clearer intent at call sites.

	// Type-safe operations returning TDerived
	/// <summary>
	/// Creates a new semantic string with the specified prefix prepended to the current value.
	/// </summary>
	/// <param name="prefix">The prefix to prepend to the current string value.</param>
	/// <returns>A new semantic string instance with the prefix prepended.</returns>
	/// <exception cref="ArgumentException">The resulting string does not meet the validation criteria for this semantic string type.</exception>
	public TDerived WithPrefix(string prefix) => Create<TDerived>($"{prefix}{this}");

	/// <summary>
	/// Creates a new semantic string with the specified suffix appended to the current value.
	/// </summary>
	/// <param name="suffix">The suffix to append to the current string value.</param>
	/// <returns>A new semantic string instance with the suffix appended.</returns>
	/// <exception cref="ArgumentException">The resulting string does not meet the validation criteria for this semantic string type.</exception>
	public TDerived WithSuffix(string suffix) => Create<TDerived>($"{this}{suffix}");

	// IComparable implementation with proper generic constraints
	/// <summary>
	/// Determines whether one semantic string is less than another.
	/// </summary>
	/// <param name="left">The first semantic string to compare.</param>
	/// <param name="right">The second semantic string to compare.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
	public static bool operator <(SemanticString<TDerived>? left, SemanticString<TDerived>? right) => left is null ? right is not null : left.CompareTo(value: right?.WeakString) < 0;

	/// <summary>
	/// Determines whether one semantic string is less than or equal to another.
	/// </summary>
	/// <param name="left">The first semantic string to compare.</param>
	/// <param name="right">The second semantic string to compare.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
	public static bool operator <=(SemanticString<TDerived>? left, SemanticString<TDerived>? right) => left is null || left.CompareTo(value: right?.WeakString) <= 0;

	/// <summary>
	/// Determines whether one semantic string is greater than another.
	/// </summary>
	/// <param name="left">The first semantic string to compare.</param>
	/// <param name="right">The second semantic string to compare.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
	public static bool operator >(SemanticString<TDerived>? left, SemanticString<TDerived>? right) => left is not null && left.CompareTo(value: right?.WeakString) > 0;

	/// <summary>
	/// Determines whether one semantic string is greater than or equal to another.
	/// </summary>
	/// <param name="left">The first semantic string to compare.</param>
	/// <param name="right">The second semantic string to compare.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
	public static bool operator >=(SemanticString<TDerived>? left, SemanticString<TDerived>? right) => left is null ? right is null : left.CompareTo(value: right?.WeakString) >= 0;

	// Implicit conversions
	/// <summary>
	/// Implicitly converts a semantic string to a character array.
	/// </summary>
	/// <param name="value">The semantic string to convert.</param>
	/// <returns>A character array containing the characters of the semantic string, or an empty array if the value is <see langword="null"/>.</returns>
	public static implicit operator char[](SemanticString<TDerived>? value) => value?.ToCharArray() ?? [];

	/// <summary>
	/// Implicitly converts a semantic string to a read-only character span.
	/// </summary>
	/// <param name="value">The semantic string to convert.</param>
	/// <returns>A read-only span containing the characters of the semantic string, or an empty span if the value is <see langword="null"/>.</returns>
	public static implicit operator ReadOnlySpan<char>(SemanticString<TDerived>? value) => value is null ? default : value.AsSpan();

	/// <summary>
	/// Implicitly converts a semantic string to a regular string.
	/// </summary>
	/// <param name="value">The semantic string to convert.</param>
	/// <returns>The string representation of the semantic string, or <see cref="string.Empty"/> if the value is <see langword="null"/>.</returns>
	public static implicit operator string(SemanticString<TDerived>? value) => value?.WeakString ?? string.Empty;

	// Factory methods
	/// <summary>
	/// Creates a new instance of the specified semantic string type from a string value.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The string value to convert.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">
	/// The <paramref name="value"/> does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This is the primary factory method for creating semantic string instances. It performs the following steps:
	/// <list type="number">
	/// <item><description>Creates an instance of the target type</description></item>
	/// <item><description>Applies canonicalization through the type's <see cref="MakeCanonical"/> method</description></item>
	/// <item><description>Validates the result using <see cref="IsValid()"/> method</description></item>
	/// <item><description>Throws <see cref="ArgumentException"/> if validation fails</description></item>
	/// </list>
	/// </remarks>
	public static TDest Create<TDest>(string? value)
		where TDest : SemanticString<TDest>
	{
		TDest newInstance = FromStringInternal<TDest>(value: value);
		return PerformValidation(value: newInstance);
	}

	/// <summary>
	/// Creates a new instance of the specified semantic string type from a character array.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The character array to convert.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">
	/// The string representation of <paramref name="value"/> does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This factory method converts the character array to a string and then creates a semantic string instance,
	/// applying canonicalization and validation in the process.
	/// </remarks>
	public static TDest Create<TDest>(char[]? value)
		where TDest : SemanticString<TDest>
	{
		ArgumentNullException.ThrowIfNull(value);
		return Create<TDest>(value: new string(value: value));
	}

	/// <summary>
	/// Creates a new instance of the specified semantic string type from a read-only character span.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The read-only character span to convert.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="ArgumentException">
	/// The string representation of <paramref name="value"/> does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This factory method is optimized for performance when working with character spans,
	/// avoiding unnecessary allocations until the final string creation.
	/// The span is converted to a string and then processed through canonicalization and validation.
	/// </remarks>
	public static TDest Create<TDest>(ReadOnlySpan<char> value)
		where TDest : SemanticString<TDest>
		=> Create<TDest>(value: value.ToString());

	/// <summary>
	/// Creates a new instance of the specified semantic string type from a string value.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The string value to convert.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">
	/// The <paramref name="value"/> does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This is an internal factory method for creating semantic string instances. It performs the following steps:
	/// <list type="number">
	/// <item><description>Creates an instance of the target type</description></item>
	/// <item><description>Applies canonicalization through the type's <see cref="MakeCanonical"/> method</description></item>
	/// <item><description>Validates the result using <see cref="IsValid()"/> method</description></item>
	/// <item><description>Throws <see cref="ArgumentException"/> if validation fails</description></item>
	/// </list>
	/// </remarks>
	private static TDest FromString<TDest>(string? value)
		where TDest : SemanticString<TDest>
	{
		TDest newInstance = FromStringInternal<TDest>(value: value);
		return PerformValidation(value: newInstance);
	}

	/// <summary>
	/// Creates a new instance of the specified semantic string type from a character array.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The character array to convert.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">
	/// The string representation of <paramref name="value"/> does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This internal factory method converts the character array to a string and then creates a semantic string instance,
	/// applying canonicalization and validation in the process.
	/// </remarks>
	private static TDest FromCharArray<TDest>(char[]? value)
		where TDest : SemanticString<TDest>
	{
		ArgumentNullException.ThrowIfNull(value);
		return FromString<TDest>(value: new string(value: value));
	}

	/// <summary>
	/// Creates a new instance of the specified semantic string type from a read-only character span.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The read-only character span to convert.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="ArgumentException">
	/// The string representation of <paramref name="value"/> does not meet the validation criteria defined by the target semantic string type.
	/// </exception>
	/// <remarks>
	/// This internal factory method is optimized for performance when working with character spans,
	/// avoiding unnecessary allocations until the final string creation.
	/// The span is converted to a string and then processed through canonicalization and validation.
	/// </remarks>
	private static TDest FromReadOnlySpan<TDest>(ReadOnlySpan<char> value)
		where TDest : SemanticString<TDest>
		=> FromString<TDest>(value: value.ToString());

	/// <summary>
	/// Internal factory method that creates a new semantic string instance without validation.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The string value to convert.</param>
	/// <returns>A new instance of the specified semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <remarks>
	/// This method creates the instance and applies canonicalization but does not perform validation.
	/// It is used internally by the public factory methods which then call validation separately.
	/// </remarks>
	private static TDest FromStringInternal<TDest>(string? value)
		where TDest : SemanticString<TDest>
	{
		ArgumentNullException.ThrowIfNull(value);

		Type typeOfTDest = typeof(TDest);
		TDest newInstance = (TDest)Activator.CreateInstance(type: typeOfTDest)!;
		typeOfTDest.GetProperty(name: nameof(WeakString))!.SetValue(obj: newInstance, value: newInstance.MakeCanonical(value));
		return newInstance;
	}

	/// <summary>
	/// Validates a semantic string instance and throws an exception if validation fails.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type being validated.</typeparam>
	/// <param name="value">The semantic string instance to validate.</param>
	/// <returns>The validated semantic string instance.</returns>
	/// <exception cref="ArgumentException">The semantic string instance is null or fails validation.</exception>
	private static TDest PerformValidation<TDest>(TDest? value)
		where TDest : SemanticString<TDest>
	{
		if (value == null || !value.IsValid())
		{
			throw new ArgumentException(message: $"Cannot convert \"{value}\" to {typeof(TDest).Name}");
		}
		return value;
	}

	/// <summary>
	/// Creates a new instance of this semantic string type from a string value.
	/// </summary>
	/// <param name="value">The string value to convert.</param>
	/// <returns>A new instance of this semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">The string does not meet the validation criteria for this type.</exception>
	/// <remarks>
	/// This method provides type inference when called on concrete semantic string types,
	/// eliminating the need for explicit generic type parameters.
	/// </remarks>
	public static TDerived Create(string? value) => Create<TDerived>(value);

	/// <summary>
	/// Creates a new instance of this semantic string type from a character array.
	/// </summary>
	/// <param name="value">The character array to convert.</param>
	/// <returns>A new instance of this semantic string type.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">The string does not meet the validation criteria for this type.</exception>
	/// <remarks>
	/// This method provides type inference when called on concrete semantic string types,
	/// eliminating the need for explicit generic type parameters.
	/// </remarks>
	public static TDerived Create(char[]? value) => Create<TDerived>(value);

	/// <summary>
	/// Creates a new instance of this semantic string type from a read-only character span.
	/// </summary>
	/// <param name="value">The read-only character span to convert.</param>
	/// <returns>A new instance of this semantic string type.</returns>
	/// <exception cref="ArgumentException">The string does not meet the validation criteria for this type.</exception>
	/// <remarks>
	/// This method provides type inference when called on concrete semantic string types,
	/// eliminating the need for explicit generic type parameters.
	/// </remarks>
	public static TDerived Create(ReadOnlySpan<char> value) => Create<TDerived>(value);

	/// <summary>
	/// Attempts to create a new instance of this semantic string type from a string value without throwing exceptions.
	/// </summary>
	/// <param name="value">The string value to convert.</param>
	/// <param name="result">
	/// When this method returns, contains the created semantic string instance if the conversion succeeded,
	/// or <see langword="null"/> if the conversion failed.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method provides exception-free creation of semantic string instances with type inference.
	/// It eliminates the need for explicit generic type parameters when called on concrete types.
	/// </remarks>
	public static bool TryCreate(string? value, out TDerived? result) => TryFromString(value, out result);

	/// <summary>
	/// Attempts to create a new instance of this semantic string type from a character array without throwing exceptions.
	/// </summary>
	/// <param name="value">The character array to convert.</param>
	/// <param name="result">
	/// When this method returns, contains the created semantic string instance if the conversion succeeded,
	/// or <see langword="null"/> if the conversion failed.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method provides exception-free creation of semantic string instances with type inference.
	/// It eliminates the need for explicit generic type parameters when called on concrete types.
	/// </remarks>
	public static bool TryCreate(char[]? value, out TDerived? result) => TryFromCharArray(value, out result);

	/// <summary>
	/// Attempts to create a new instance of this semantic string type from a read-only character span without throwing exceptions.
	/// </summary>
	/// <param name="value">The read-only character span to convert.</param>
	/// <param name="result">
	/// When this method returns, contains the created semantic string instance if the conversion succeeded,
	/// or <see langword="null"/> if the conversion failed.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method provides exception-free creation of semantic string instances with type inference.
	/// It eliminates the need for explicit generic type parameters when called on concrete types.
	/// </remarks>
	public static bool TryCreate(ReadOnlySpan<char> value, out TDerived? result) => TryFromReadOnlySpan(value, out result);

	/// <summary>
	/// Attempts to create a new instance of the specified semantic string type from a string value without throwing exceptions.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The string value to convert.</param>
	/// <param name="result">
	/// When this method returns, contains the created semantic string instance if the conversion succeeded,
	/// or <see langword="null"/> if the conversion failed.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method provides exception-free creation of semantic string instances.
	/// It performs the same validation as <see cref="FromString{TDest}(string)"/> but returns false instead of throwing exceptions when validation fails.
	/// </remarks>
	public static bool TryCreate<TDest>(string? value, out TDest? result)
		where TDest : SemanticString<TDest>
		=> TryFromString(value, out result);

	/// <summary>
	/// Attempts to create a new instance of the specified semantic string type from a character array without throwing exceptions.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The character array to convert.</param>
	/// <param name="result">
	/// When this method returns, contains the created semantic string instance if the conversion succeeded,
	/// or <see langword="null"/> if the conversion failed.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method provides exception-free creation of semantic string instances.
	/// It performs the same validation as <see cref="FromCharArray{TDest}(char[])"/> but returns false instead of throwing exceptions when validation fails.
	/// </remarks>
	public static bool TryCreate<TDest>(char[]? value, out TDest? result)
		where TDest : SemanticString<TDest>
		=> TryFromCharArray(value, out result);

	/// <summary>
	/// Attempts to create a new instance of the specified semantic string type from a read-only character span without throwing exceptions.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The read-only character span to convert.</param>
	/// <param name="result">
	/// When this method returns, contains the created semantic string instance if the conversion succeeded,
	/// or <see langword="null"/> if the conversion failed.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method provides exception-free creation of semantic string instances.
	/// It performs the same validation as <see cref="FromReadOnlySpan{TDest}(ReadOnlySpan{char})"/> but returns false instead of throwing exceptions when validation fails.
	/// </remarks>
	public static bool TryCreate<TDest>(ReadOnlySpan<char> value, out TDest? result)
		where TDest : SemanticString<TDest>
		=> TryFromReadOnlySpan(value, out result);

	/// <summary>
	/// Attempts to create a new instance of the specified semantic string type from a string value without throwing exceptions.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The string value to convert.</param>
	/// <param name="result">When this method returns, contains the created semantic string if the conversion succeeded, or null if the conversion failed.</param>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This internal method provides exception-free creation of semantic string instances. It performs the same validation
	/// as <see cref="FromString{TDest}(string?)"/> but returns false instead of throwing exceptions when validation fails.
	/// This is useful for scenarios where you want to attempt creation without handling exceptions.
	/// </remarks>
	private static bool TryFromString<TDest>(string? value, out TDest? result)
		where TDest : SemanticString<TDest>
	{
		result = null;
		try
		{
			if (value is null)
			{
				return false;
			}

			result = FromString<TDest>(value);
			return true;
		}
		catch (ArgumentException)
		{
			return false;
		}
	}

	/// <summary>
	/// Attempts to create a new instance of the specified semantic string type from a character array without throwing exceptions.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The character array to convert.</param>
	/// <param name="result">When this method returns, contains the created semantic string if the conversion succeeded, or null if the conversion failed.</param>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This internal method provides exception-free creation of semantic string instances from character arrays.
	/// It performs the same validation as <see cref="FromCharArray{TDest}(char[])"/> but returns false instead of throwing exceptions when validation fails.
	/// </remarks>
	private static bool TryFromCharArray<TDest>(char[]? value, out TDest? result)
		where TDest : SemanticString<TDest>
	{
		result = null;
		try
		{
			if (value is null)
			{
				return false;
			}

			result = FromCharArray<TDest>(value);
			return true;
		}
		catch (ArgumentException)
		{
			return false;
		}
	}

	/// <summary>
	/// Attempts to create a new instance of the specified semantic string type from a read-only character span without throwing exceptions.
	/// </summary>
	/// <typeparam name="TDest">The semantic string type to create.</typeparam>
	/// <param name="value">The read-only character span to convert.</param>
	/// <param name="result">When this method returns, contains the created semantic string if the conversion succeeded, or null if the conversion failed.</param>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This internal method provides exception-free creation of semantic string instances from character spans.
	/// It performs the same validation as <see cref="FromReadOnlySpan{TDest}(ReadOnlySpan{char})"/> but returns false instead of throwing exceptions when validation fails.
	/// This is particularly useful for performance-critical scenarios where you want to avoid both exceptions and unnecessary string allocations.
	/// </remarks>
	private static bool TryFromReadOnlySpan<TDest>(ReadOnlySpan<char> value, out TDest? result)
		where TDest : SemanticString<TDest>
	{
		result = null;
		try
		{
			result = FromReadOnlySpan<TDest>(value);
			return true;
		}
		catch (ArgumentException)
		{
			return false;
		}
	}

	/// <summary>
	/// Gets this semantic string as a read-only span without allocation.
	/// </summary>
	/// <value>A read-only span over the characters of this semantic string.</value>
	/// <remarks>
	/// This property provides zero-allocation access to the underlying string characters
	/// and is more efficient than ToCharArray() or implicit char[] conversion.
	/// </remarks>
	public ReadOnlySpan<char> AsSpan() => WeakString.AsSpan();

	/// <summary>
	/// Gets a portion of this semantic string as a read-only span without allocation.
	/// </summary>
	/// <param name="start">The starting index of the span.</param>
	/// <returns>A read-only span over the specified portion of the string.</returns>
	/// <remarks>
	/// This method is more efficient than Substring(int) as it doesn't allocate a new string.
	/// Use this when you need to work with a portion of the string without creating a new string instance.
	/// </remarks>
	public ReadOnlySpan<char> AsSpan(int start) => WeakString.AsSpan(start);

	/// <summary>
	/// Gets a portion of this semantic string as a read-only span without allocation.
	/// </summary>
	/// <param name="start">The starting index of the span.</param>
	/// <param name="length">The length of the span.</param>
	/// <returns>A read-only span over the specified portion of the string.</returns>
	/// <remarks>
	/// This method is more efficient than Substring(int, int) as it doesn't allocate a new string.
	/// Use this when you need to work with a portion of the string without creating a new string instance.
	/// </remarks>
	public ReadOnlySpan<char> AsSpan(int start, int length) => WeakString.AsSpan(start, length);

	/// <summary>
	/// Finds the first occurrence of a character sequence in the semantic string using span semantics.
	/// </summary>
	/// <param name="value">The character sequence to search for.</param>
	/// <param name="comparisonType">The type of comparison to perform.</param>
	/// <returns>The index of the first occurrence of the sequence, or -1 if not found.</returns>
	/// <remarks>
	/// This overload uses span-based search which can be more efficient than string-based IndexOf for certain scenarios.
	/// </remarks>
	public int IndexOf(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal) => AsSpan().IndexOf(value, comparisonType);

	/// <summary>
	/// Finds the last occurrence of a character sequence in the semantic string using span semantics.
	/// </summary>
	/// <param name="value">The character sequence to search for.</param>
	/// <param name="comparisonType">The type of comparison to perform.</param>
	/// <returns>The index of the last occurrence of the sequence, or -1 if not found.</returns>
	/// <remarks>
	/// This overload uses span-based search which can be more efficient than string-based LastIndexOf for certain scenarios.
	/// </remarks>
	public int LastIndexOf(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal) => AsSpan().LastIndexOf(value, comparisonType);

	/// <summary>
	/// Determines whether the semantic string starts with the specified span using efficient span comparison.
	/// </summary>
	/// <param name="value">The span to compare with the beginning of this string.</param>
	/// <param name="comparisonType">The type of comparison to perform.</param>
	/// <returns>true if this string starts with the specified span; otherwise, false.</returns>
	/// <remarks>
	/// This overload uses span-based comparison which avoids string allocations when working with substrings or spans.
	/// </remarks>
	public bool StartsWith(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal) => AsSpan().StartsWith(value, comparisonType);

	/// <summary>
	/// Determines whether the semantic string ends with the specified span using efficient span comparison.
	/// </summary>
	/// <param name="value">The span to compare with the end of this string.</param>
	/// <param name="comparisonType">The type of comparison to perform.</param>
	/// <returns>true if this string ends with the specified span; otherwise, false.</returns>
	/// <remarks>
	/// This overload uses span-based comparison which avoids string allocations when working with substrings or spans.
	/// </remarks>
	public bool EndsWith(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal) => AsSpan().EndsWith(value, comparisonType);

	/// <summary>
	/// Determines whether the semantic string contains the specified span using efficient span comparison.
	/// </summary>
	/// <param name="value">The span to search for within this string.</param>
	/// <param name="comparisonType">The type of comparison to perform.</param>
	/// <returns>true if this string contains the specified span; otherwise, false.</returns>
	/// <remarks>
	/// This overload uses span-based search which can be more efficient than string-based Contains for certain scenarios.
	/// </remarks>
	public bool Contains(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.Ordinal) => AsSpan().Contains(value, comparisonType);

	/// <summary>
	/// Counts the number of characters that match the specified predicate using span semantics.
	/// </summary>
	/// <param name="predicate">A function to test each character.</param>
	/// <returns>The number of characters that match the predicate.</returns>
	/// <remarks>
	/// This method iterates over the span without allocating additional memory,
	/// making it efficient for character counting operations.
	/// </remarks>
	public int Count(Func<char, bool> predicate)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		ReadOnlySpan<char> span = AsSpan();
		int count = 0;
		for (int i = 0; i < span.Length; i++)
		{
			if (predicate(span[i]))
			{
				count++;
			}
		}
		return count;
	}

	/// <summary>
	/// Splits the semantic string into spans based on the specified separator without allocating strings.
	/// </summary>
	/// <param name="separator">The character that delimits the spans in this string.</param>
	/// <param name="options">Options to control the splitting behavior.</param>
	/// <returns>An enumerable of spans that represent the segments of this string separated by the separator.</returns>
	/// <remarks>
	/// This method provides a zero-allocation alternative to Split() when you only need to enumerate
	/// the parts without creating string objects. Use this for performance-critical scenarios.
	/// </remarks>
	public SpanSplitEnumerator Split(char separator, StringSplitOptions options = StringSplitOptions.None) => new(AsSpan(), separator, options);

	/// <summary>
	/// Trims whitespace from both ends of the semantic string using span semantics.
	/// </summary>
	/// <returns>A span representing the trimmed portion of the string.</returns>
	/// <remarks>
	/// This overload returns a span over the original string without allocating a new string.
	/// Convert to string only if you need to store the result.
	/// </remarks>
	public ReadOnlySpan<char> TrimAsSpan() => AsSpan().Trim();

	/// <summary>
	/// Trims specified characters from both ends of the semantic string using span semantics.
	/// </summary>
	/// <param name="trimChars">The characters to remove.</param>
	/// <returns>A span representing the trimmed portion of the string.</returns>
	/// <remarks>
	/// This overload returns a span over the original string without allocating a new string.
	/// Convert to string only if you need to store the result.
	/// </remarks>
	public ReadOnlySpan<char> TrimAsSpan(ReadOnlySpan<char> trimChars) => AsSpan().Trim(trimChars);

	/// <summary>
	/// Trims whitespace from the start of the semantic string using span semantics.
	/// </summary>
	/// <returns>A span representing the trimmed portion of the string.</returns>
	/// <remarks>
	/// This overload returns a span over the original string without allocating a new string.
	/// Convert to string only if you need to store the result.
	/// </remarks>
	public ReadOnlySpan<char> TrimStartAsSpan() => AsSpan().TrimStart();

	/// <summary>
	/// Trims specified characters from the start of the semantic string using span semantics.
	/// </summary>
	/// <param name="trimChars">The characters to remove.</param>
	/// <returns>A span representing the trimmed portion of the string.</returns>
	/// <remarks>
	/// This overload returns a span over the original string without allocating a new string.
	/// Convert to string only if you need to store the result.
	/// </remarks>
	public ReadOnlySpan<char> TrimStartAsSpan(ReadOnlySpan<char> trimChars) => AsSpan().TrimStart(trimChars);

	/// <summary>
	/// Trims whitespace from the end of the semantic string using span semantics.
	/// </summary>
	/// <returns>A span representing the trimmed portion of the string.</returns>
	/// <remarks>
	/// This overload returns a span over the original string without allocating a new string.
	/// Convert to string only if you need to store the result.
	/// </remarks>
	public ReadOnlySpan<char> TrimEndAsSpan() => AsSpan().TrimEnd();

	/// <summary>
	/// Trims specified characters from the end of the semantic string using span semantics.
	/// </summary>
	/// <param name="trimChars">The characters to remove.</param>
	/// <returns>A span representing the trimmed portion of the string.</returns>
	/// <remarks>
	/// This overload returns a span over the original string without allocating a new string.
	/// Convert to string only if you need to store the result.
	/// </remarks>
	public ReadOnlySpan<char> TrimEndAsSpan(ReadOnlySpan<char> trimChars) => AsSpan().TrimEnd(trimChars);

	/// <summary>
	/// Provides efficient enumeration over string segments split by a character separator.
	/// </summary>
	/// <remarks>
	/// This struct avoids allocations by working directly with spans and implements
	/// the enumerable pattern for use in foreach loops.
	/// </remarks>
	public ref struct SpanSplitEnumerator
	{
		private ReadOnlySpan<char> _remaining;
		private readonly char _separator;
		private readonly StringSplitOptions _options;

		internal SpanSplitEnumerator(ReadOnlySpan<char> span, char separator, StringSplitOptions options)
		{
			_remaining = span;
			_separator = separator;
			_options = options;
			Current = default;
		}

		/// <summary>
		/// Gets the current span segment.
		/// </summary>
		public ReadOnlySpan<char> Current { get; private set; }

		/// <summary>
		/// Returns this enumerator.
		/// </summary>
		/// <returns>This enumerator instance.</returns>
		public readonly SpanSplitEnumerator GetEnumerator() => this;

		/// <summary>
		/// Advances to the next segment.
		/// </summary>
		/// <returns>true if there is a next segment; otherwise, false.</returns>
		public bool MoveNext()
		{
			if (_remaining.IsEmpty)
			{
				return false;
			}

			int separatorIndex = _remaining.IndexOf(_separator);
			if (separatorIndex >= 0)
			{
				Current = _remaining[..separatorIndex];
				_remaining = _remaining[(separatorIndex + 1)..];
			}
			else
			{
				Current = _remaining;
				_remaining = default;
			}

			// Handle StringSplitOptions.RemoveEmptyEntries
			if (_options == StringSplitOptions.RemoveEmptyEntries && Current.IsEmpty)
			{
				return MoveNext(); // Recursively skip empty entries
			}

			return true;
		}
	}
}
