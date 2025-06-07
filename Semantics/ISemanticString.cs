// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Globalization;
using System.Text;

/// <summary>
/// Defines the contract for semantic string types that provide type-safe string operations.
/// Semantic strings prevent accidental mixing of different string types by providing
/// compile-time type safety while maintaining compatibility with System.String operations.
/// </summary>
/// <remarks>
/// This interface wraps the System.String interface to provide semantic typing capabilities.
/// All operations maintain the same behavior as their System.String counterparts but operate
/// on semantic string types, ensuring type safety at compile time.
/// </remarks>
public interface ISemanticString : IComparable, IComparable<ISemanticString>, IEnumerable<char>
{
	/// <summary>
	/// Gets the underlying string value for interoperability with non-semantic string operations.
	/// </summary>
	/// <value>
	/// The raw string value contained within this semantic string.
	/// </value>
	/// <remarks>
	/// Use this property when you need to interoperate with APIs that expect regular strings.
	/// The name "WeakString" emphasizes that this breaks the type safety guarantees.
	/// </remarks>
	public string WeakString { get; init; }

	// System.String interface

	/// <summary>
	/// Gets the character at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index of the character to get.</param>
	/// <returns>The character at the specified index.</returns>
	/// <exception cref="IndexOutOfRangeException">
	/// <paramref name="index"/> is less than zero or greater than or equal to the length of this string.
	/// </exception>
	public char this[int index] { get; }

	/// <summary>
	/// Gets the number of characters in this string.
	/// </summary>
	/// <value>The number of characters in this string.</value>
	public int Length { get; }

	/// <summary>
	/// Compares this instance with a specified object and returns an integer that indicates
	/// whether this instance precedes, follows, or appears in the same position in the sort
	/// order as the specified object.
	/// </summary>
	/// <param name="value">An object that evaluates to a string.</param>
	/// <returns>
	/// A 32-bit signed integer that indicates whether this instance precedes, follows, or
	/// appears in the same position in the sort order as the <paramref name="value"/> parameter.
	/// </returns>
	public new int CompareTo(object value);

	/// <summary>
	/// Returns a value indicating whether a specified substring occurs within this string.
	/// </summary>
	/// <param name="value">The string to seek.</param>
	/// <returns>
	/// <see langword="true"/> if the <paramref name="value"/> parameter occurs within this string,
	/// or if <paramref name="value"/> is the empty string (""); otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	public bool Contains(string value);

	/// <summary>
	/// Returns a value indicating whether a specified substring occurs within this string,
	/// using the specified comparison rules.
	/// </summary>
	/// <param name="value">The string to seek.</param>
	/// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
	/// <returns>
	/// <see langword="true"/> if the <paramref name="value"/> parameter occurs within this string,
	/// or if <paramref name="value"/> is the empty string (""); otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a valid <see cref="StringComparison"/> value.</exception>
	public bool Contains(string value, StringComparison comparisonType);

	/// <summary>
	/// Copies a specified number of characters from a specified position in this instance
	/// to a specified position in an array of Unicode characters.
	/// </summary>
	/// <param name="sourceIndex">The index of the first character in this instance to copy.</param>
	/// <param name="destination">An array of Unicode characters.</param>
	/// <param name="destinationIndex">The index in <paramref name="destination"/> at which the copy operation begins.</param>
	/// <param name="count">The number of characters in this instance to copy to <paramref name="destination"/>.</param>
	/// <exception cref="ArgumentNullException"><paramref name="destination"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="sourceIndex"/>, <paramref name="destinationIndex"/>, or <paramref name="count"/> is negative.
	/// -or- <paramref name="sourceIndex"/> does not identify a position in this instance.
	/// -or- <paramref name="destinationIndex"/> does not identify a valid index in the <paramref name="destination"/> array.
	/// -or- <paramref name="count"/> is greater than the length of the substring from <paramref name="sourceIndex"/> to the end of this instance.
	/// -or- <paramref name="count"/> is greater than the length of the subarray from <paramref name="destinationIndex"/> to the end of the <paramref name="destination"/> array.
	/// </exception>
	public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

	/// <summary>
	/// Determines whether the end of this string instance matches the specified string.
	/// </summary>
	/// <param name="value">The string to compare to the substring at the end of this instance.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="value"/> matches the end of this instance; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	public bool EndsWith(string value);

	/// <summary>
	/// Determines whether the end of this string instance matches the specified string
	/// when compared using the specified culture.
	/// </summary>
	/// <param name="value">The string to compare to the substring at the end of this instance.</param>
	/// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
	/// <param name="culture">Cultural information that determines how this instance and <paramref name="value"/> are compared.</param>
	/// <returns>
	/// <see langword="true"/> if the <paramref name="value"/> parameter matches the end of this string; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	public bool EndsWith(string value, bool ignoreCase, CultureInfo culture);

	/// <summary>
	/// Determines whether the end of this string instance matches the specified string
	/// when compared using the specified comparison option.
	/// </summary>
	/// <param name="value">The string to compare to the substring at the end of this instance.</param>
	/// <param name="comparisonType">One of the enumeration values that determines how this string and <paramref name="value"/> are compared.</param>
	/// <returns>
	/// <see langword="true"/> if the <paramref name="value"/> parameter matches the end of this string; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
	public bool EndsWith(string value, StringComparison comparisonType);

	/// <summary>
	/// Determines whether this instance and a specified object, which must also be a <see cref="string"/> object,
	/// have the same value.
	/// </summary>
	/// <param name="obj">The string to compare to this instance.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="obj"/> is a <see cref="string"/> and its value is the same as this instance;
	/// otherwise, <see langword="false"/>. If <paramref name="obj"/> is <see langword="null"/>, the method returns <see langword="false"/>.
	/// </returns>
	public bool Equals(object obj);

	/// <summary>
	/// Determines whether this string and a specified <see cref="string"/> object have the same value.
	/// </summary>
	/// <param name="value">The string to compare to this instance.</param>
	/// <returns>
	/// <see langword="true"/> if the value of the <paramref name="value"/> parameter is the same as the value of this instance;
	/// otherwise, <see langword="false"/>. If <paramref name="value"/> is <see langword="null"/>, the method returns <see langword="false"/>.
	/// </returns>
	public bool Equals(string value);

	/// <summary>
	/// Determines whether this string and a specified <see cref="string"/> object have the same value.
	/// A parameter specifies the culture, case, and sort rules used in the comparison.
	/// </summary>
	/// <param name="value">The string to compare to this instance.</param>
	/// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
	/// <returns>
	/// <see langword="true"/> if the value of the <paramref name="value"/> parameter is the same as this string;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
	public bool Equals(string value, StringComparison comparisonType);

	/// <summary>
	/// Retrieves an object that can iterate through the individual characters in this string.
	/// </summary>
	/// <returns>An enumerator object.</returns>
	public new CharEnumerator GetEnumerator();

	/// <summary>
	/// Returns the hash code for this string.
	/// </summary>
	/// <returns>A 32-bit signed integer hash code.</returns>
	public int GetHashCode();

	/// <summary>
	/// Returns the <see cref="TypeCode"/> for class <see cref="string"/>.
	/// </summary>
	/// <returns>The enumerated constant <see cref="TypeCode.String"/>.</returns>
	public TypeCode GetTypeCode();

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified character in this string.
	/// </summary>
	/// <param name="value">A Unicode character to seek.</param>
	/// <returns>
	/// The zero-based index position of <paramref name="value"/> if that character is found, or -1 if it is not.
	/// </returns>
	public int IndexOf(char value);

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified character in this string.
	/// The search starts at a specified character position.
	/// </summary>
	/// <param name="value">A Unicode character to seek.</param>
	/// <param name="startIndex">The search starting position.</param>
	/// <returns>
	/// The zero-based index position of <paramref name="value"/> from the start of the string if that character is found, or -1 if it is not.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="startIndex"/> is less than 0 (zero) or greater than the length of the string.
	/// </exception>
	public int IndexOf(char value, int startIndex);

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified character in this string.
	/// The search starts at a specified character position and examines a specified number of character positions.
	/// </summary>
	/// <param name="value">A Unicode character to seek.</param>
	/// <param name="startIndex">The search starting position.</param>
	/// <param name="count">The number of character positions to examine.</param>
	/// <returns>
	/// The zero-based index position of <paramref name="value"/> from the start of the string if that character is found, or -1 if it is not.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="startIndex"/> or <paramref name="count"/> is negative.
	/// -or- <paramref name="startIndex"/> plus <paramref name="count"/> specify a position not within this instance.
	/// </exception>
	public int IndexOf(char value, int startIndex, int count);

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified string in this instance.
	/// </summary>
	/// <param name="value">The string to seek.</param>
	/// <returns>
	/// The zero-based index position of <paramref name="value"/> if that string is found, or -1 if it is not.
	/// If <paramref name="value"/> is empty, the return value is 0.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	public int IndexOf(string value);

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified string in this instance.
	/// The search starts at a specified character position.
	/// </summary>
	/// <param name="value">The string to seek.</param>
	/// <param name="startIndex">The search starting position.</param>
	/// <returns>
	/// The zero-based index position of <paramref name="value"/> from the start of the current instance if that string is found, or -1 if it is not.
	/// If <paramref name="value"/> is empty, the return value is <paramref name="startIndex"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="startIndex"/> is less than 0 (zero) or greater than the length of this string.
	/// </exception>
	public int IndexOf(string value, int startIndex);

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified string in this instance.
	/// The search starts at a specified character position and examines a specified number of character positions.
	/// </summary>
	/// <param name="value">The string to seek.</param>
	/// <param name="startIndex">The search starting position.</param>
	/// <param name="count">The number of character positions to examine.</param>
	/// <returns>
	/// The zero-based index position of <paramref name="value"/> from the start of the current instance if that string is found, or -1 if it is not.
	/// If <paramref name="value"/> is empty, the return value is <paramref name="startIndex"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="count"/> or <paramref name="startIndex"/> is negative.
	/// -or- <paramref name="startIndex"/> is greater than the length of this string.
	/// -or- <paramref name="count"/> is greater than the length of this string minus <paramref name="startIndex"/>.
	/// </exception>
	public int IndexOf(string value, int startIndex, int count);

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified string in the current string object.
	/// Parameters specify the starting search position in the current string, the number of characters in the current string to search,
	/// and the type of search to use for the specified string.
	/// </summary>
	/// <param name="value">The string to seek.</param>
	/// <param name="startIndex">The search starting position.</param>
	/// <param name="count">The number of character positions to examine.</param>
	/// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
	/// <returns>
	/// The zero-based index position of the <paramref name="value"/> parameter from the start of the current instance if that string is found, or -1 if it is not.
	/// If <paramref name="value"/> is empty, the return value is <paramref name="startIndex"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="count"/> or <paramref name="startIndex"/> is negative.
	/// -or- <paramref name="startIndex"/> is greater than the length of this instance.
	/// -or- <paramref name="count"/> is greater than the length of this string minus <paramref name="startIndex"/>.
	/// </exception>
	/// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a valid <see cref="StringComparison"/> value.</exception>
	public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType);

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified string in the current string object.
	/// A parameter specifies the starting search position in the current string and the type of comparison to use.
	/// </summary>
	/// <param name="value">The string to seek.</param>
	/// <param name="startIndex">The search starting position.</param>
	/// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
	/// <returns>
	/// The zero-based index position of the <paramref name="value"/> parameter from the start of the current instance if that string is found, or -1 if it is not.
	/// If <paramref name="value"/> is empty, the return value is <paramref name="startIndex"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="startIndex"/> is less than 0 (zero) or greater than the length of this string.
	/// </exception>
	/// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a valid <see cref="StringComparison"/> value.</exception>
	public int IndexOf(string value, int startIndex, StringComparison comparisonType);

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified string in this instance.
	/// A parameter specifies the type of search to use for the specified string.
	/// </summary>
	/// <param name="value">The string to seek.</param>
	/// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
	/// <returns>
	/// The zero-based index position of the <paramref name="value"/> parameter if that string is found, or -1 if it is not.
	/// If <paramref name="value"/> is empty, the return value is 0.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a valid <see cref="StringComparison"/> value.</exception>
	public int IndexOf(string value, StringComparison comparisonType);
	/// <summary>
	/// Reports the zero-based index of the first occurrence in this instance of any character in a specified array of Unicode characters.
	/// </summary>
	/// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
	/// <returns>
	/// The zero-based index position of the first occurrence in this instance where any character in <paramref name="anyOf"/> was found;
	/// -1 if no character in <paramref name="anyOf"/> was found.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="anyOf"/> is <see langword="null"/>.</exception>
	public int IndexOfAny(char[] anyOf);

	/// <summary>
	/// Reports the zero-based index of the first occurrence in this instance of any character in a specified array of Unicode characters.
	/// The search starts at a specified character position.
	/// </summary>
	/// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
	/// <param name="startIndex">The search starting position.</param>
	/// <returns>
	/// The zero-based index position of the first occurrence in this instance where any character in <paramref name="anyOf"/> was found;
	/// -1 if no character in <paramref name="anyOf"/> was found.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="anyOf"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="startIndex"/> is negative.
	/// -or- <paramref name="startIndex"/> is greater than the number of characters in this instance.
	/// </exception>
	public int IndexOfAny(char[] anyOf, int startIndex);

	/// <summary>
	/// Reports the zero-based index of the first occurrence in this instance of any character in a specified array of Unicode characters.
	/// The search starts at a specified character position and examines a specified number of character positions.
	/// </summary>
	/// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
	/// <param name="startIndex">The search starting position.</param>
	/// <param name="count">The number of character positions to examine.</param>
	/// <returns>
	/// The zero-based index position of the first occurrence in this instance where any character in <paramref name="anyOf"/> was found;
	/// -1 if no character in <paramref name="anyOf"/> was found.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="anyOf"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="count"/> or <paramref name="startIndex"/> is negative.
	/// -or- <paramref name="count"/> + <paramref name="startIndex"/> is greater than the number of characters in this instance.
	/// </exception>
	public int IndexOfAny(char[] anyOf, int startIndex, int count);

	/// <summary>
	/// Returns a new string in which a specified string is inserted at a specified index position in this instance.
	/// </summary>
	/// <param name="startIndex">The zero-based index position of the insertion.</param>
	/// <param name="value">The string to insert.</param>
	/// <returns>
	/// A new string that is equivalent to this instance, but with <paramref name="value"/> inserted at position <paramref name="startIndex"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="startIndex"/> is negative or greater than the length of this instance.
	/// </exception>
	public string Insert(int startIndex, string value);

	/// <summary>
	/// Indicates whether this string is in Unicode normalization form C.
	/// </summary>
	/// <returns>
	/// <see langword="true"/> if this string is in normalization form C; otherwise, <see langword="false"/>.
	/// </returns>
	public bool IsNormalized();

	/// <summary>
	/// Indicates whether this string is in the specified Unicode normalization form.
	/// </summary>
	/// <param name="normalizationForm">A Unicode normalization form.</param>
	/// <returns>
	/// <see langword="true"/> if this string is in the normalization form specified by the <paramref name="normalizationForm"/> parameter;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// The current instance contains invalid Unicode characters.
	/// </exception>
	public bool IsNormalized(NormalizationForm normalizationForm);
	/// <inheritdoc/>
	public int LastIndexOf(char value);
	/// <inheritdoc/>
	public int LastIndexOf(char value, int startIndex);
	/// <inheritdoc/>
	public int LastIndexOf(char value, int startIndex, int count);
	/// <inheritdoc/>
	public int LastIndexOf(string value);
	/// <inheritdoc/>
	public int LastIndexOf(string value, int startIndex);
	/// <inheritdoc/>
	public int LastIndexOf(string value, int startIndex, int count);
	/// <inheritdoc/>
	public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType);
	/// <inheritdoc/>
	public int LastIndexOf(string value, int startIndex, StringComparison comparisonType);
	/// <inheritdoc/>
	public int LastIndexOf(string value, StringComparison comparisonType);
	/// <inheritdoc/>
	public int LastIndexOfAny(char[] anyOf);
	/// <inheritdoc/>
	public int LastIndexOfAny(char[] anyOf, int startIndex);
	/// <inheritdoc/>
	public int LastIndexOfAny(char[] anyOf, int startIndex, int count);
	/// <inheritdoc/>
	public string Normalize();
	/// <inheritdoc/>
	public string Normalize(NormalizationForm normalizationForm);
	/// <inheritdoc/>
	public string PadLeft(int totalWidth);
	/// <inheritdoc/>
	public string PadLeft(int totalWidth, char paddingChar);
	/// <inheritdoc/>
	public string PadRight(int totalWidth);
	/// <inheritdoc/>
	public string PadRight(int totalWidth, char paddingChar);
	/// <inheritdoc/>
	public string Remove(int startIndex);
	/// <inheritdoc/>
	public string Remove(int startIndex, int count);
	/// <inheritdoc/>
	public string Replace(char oldChar, char newChar);
	/// <inheritdoc/>
	public string Replace(string oldValue, string newValue);
	/// <inheritdoc/>
	public string[] Split(char[] separator, int count);
	/// <inheritdoc/>
	public string[] Split(char[] separator, int count, StringSplitOptions options);
	/// <inheritdoc/>
	public string[] Split(char[] separator, StringSplitOptions options);
	/// <inheritdoc/>
	public string[] Split(params char[] separator);
	/// <inheritdoc/>
	public string[] Split(string[] separator, int count, StringSplitOptions options);
	/// <inheritdoc/>
	public string[] Split(string[] separator, StringSplitOptions options);
	/// <summary>
	/// Determines whether the beginning of this string instance matches the specified string.
	/// </summary>
	/// <param name="value">The string to compare.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="value"/> matches the beginning of this string; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	public bool StartsWith(string value);

	/// <summary>
	/// Determines whether the beginning of this string instance matches the specified string
	/// when compared using the specified culture.
	/// </summary>
	/// <param name="value">The string to compare.</param>
	/// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
	/// <param name="culture">Cultural information that determines how this instance and <paramref name="value"/> are compared.</param>
	/// <returns>
	/// <see langword="true"/> if the <paramref name="value"/> parameter matches the beginning of this string; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	public bool StartsWith(string value, bool ignoreCase, CultureInfo culture);

	/// <summary>
	/// Determines whether the beginning of this string instance matches the specified string
	/// when compared using the specified comparison option.
	/// </summary>
	/// <param name="value">The string to compare.</param>
	/// <param name="comparisonType">One of the enumeration values that determines how this string and <paramref name="value"/> are compared.</param>
	/// <returns>
	/// <see langword="true"/> if this instance begins with <paramref name="value"/>; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
	public bool StartsWith(string value, StringComparison comparisonType);

	/// <summary>
	/// Retrieves a substring from this instance. The substring starts at a specified character position and continues to the end of the string.
	/// </summary>
	/// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
	/// <returns>
	/// A string that is equivalent to the substring that begins at <paramref name="startIndex"/> in this instance,
	/// or <see cref="string.Empty"/> if <paramref name="startIndex"/> is equal to the length of this instance.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="startIndex"/> is less than zero or greater than the length of this instance.
	/// </exception>
	public string Substring(int startIndex);

	/// <summary>
	/// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
	/// </summary>
	/// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
	/// <param name="length">The number of characters in the substring.</param>
	/// <returns>
	/// A string that is equivalent to the substring of length <paramref name="length"/> that begins at <paramref name="startIndex"/> in this instance,
	/// or <see cref="string.Empty"/> if <paramref name="startIndex"/> is equal to the length of this instance and <paramref name="length"/> is zero.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
	/// -or- <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
	/// </exception>
	public string Substring(int startIndex, int length);

	/// <summary>
	/// Copies the characters in this instance to a Unicode character array.
	/// </summary>
	/// <returns>A Unicode character array whose elements are the individual characters of this instance.</returns>
	public char[] ToCharArray();

	/// <summary>
	/// Copies the characters in a specified substring in this instance to a Unicode character array.
	/// </summary>
	/// <param name="startIndex">The starting position of a substring in this instance.</param>
	/// <param name="length">The length of the substring in this instance.</param>
	/// <returns>
	/// A Unicode character array whose elements are the <paramref name="length"/> number of characters in this instance
	/// starting from character position <paramref name="startIndex"/>.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
	/// -or- <paramref name="startIndex"/> plus <paramref name="length"/> is greater than the length of this instance.
	/// </exception>
	public char[] ToCharArray(int startIndex, int length);

	/// <summary>
	/// Returns a copy of this string converted to lowercase.
	/// </summary>
	/// <returns>A string in lowercase.</returns>
	public string ToLower();

	/// <summary>
	/// Returns a copy of this string converted to lowercase, using the casing rules of the specified culture.
	/// </summary>
	/// <param name="culture">An object that supplies culture-specific casing rules.</param>
	/// <returns>A string in lowercase.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="culture"/> is <see langword="null"/>.</exception>
	public string ToLower(CultureInfo culture);

	/// <summary>
	/// Returns a copy of this string converted to lowercase using the casing rules of the invariant culture.
	/// </summary>
	/// <returns>A string in lowercase.</returns>
	public string ToLowerInvariant();

	/// <summary>
	/// Returns this instance of <see cref="string"/>; no actual conversion is performed.
	/// </summary>
	/// <returns>The current string.</returns>
	public string ToString();

	/// <summary>
	/// Returns this instance of <see cref="string"/>; no actual conversion is performed.
	/// </summary>
	/// <param name="provider">An object that supplies culture-specific formatting information (ignored).</param>
	/// <returns>The current string.</returns>
	public string ToString(IFormatProvider provider);

	/// <summary>
	/// Returns a copy of this string converted to uppercase.
	/// </summary>
	/// <returns>A string in uppercase.</returns>
	public string ToUpper();

	/// <summary>
	/// Returns a copy of this string converted to uppercase, using the casing rules of the specified culture.
	/// </summary>
	/// <param name="culture">An object that supplies culture-specific casing rules.</param>
	/// <returns>A string in uppercase.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="culture"/> is <see langword="null"/>.</exception>
	public string ToUpper(CultureInfo culture);

	/// <summary>
	/// Returns a copy of this string converted to uppercase using the casing rules of the invariant culture.
	/// </summary>
	/// <returns>A string in uppercase.</returns>
	public string ToUpperInvariant();

	/// <summary>
	/// Removes all leading and trailing white-space characters from the current string.
	/// </summary>
	/// <returns>
	/// The string that remains after all white-space characters are removed from the start and end of the current string.
	/// If no characters can be trimmed from the current instance, the method returns the current instance unchanged.
	/// </returns>
	public string Trim();

	/// <summary>
	/// Removes all leading and trailing occurrences of a set of characters specified in an array from the current string.
	/// </summary>
	/// <param name="trimChars">
	/// An array of Unicode characters to remove, or <see langword="null"/>.
	/// </param>
	/// <returns>
	/// The string that remains after all occurrences of the characters in the <paramref name="trimChars"/> parameter
	/// are removed from the start and end of the current string. If <paramref name="trimChars"/> is <see langword="null"/>
	/// or an empty array, white-space characters are removed instead.
	/// </returns>
	public string Trim(params char[] trimChars);

	/// <summary>
	/// Removes all trailing occurrences of a set of characters specified in an array from the current string.
	/// </summary>
	/// <param name="trimChars">
	/// An array of Unicode characters to remove, or <see langword="null"/>.
	/// </param>
	/// <returns>
	/// The string that remains after all occurrences of the characters in the <paramref name="trimChars"/> parameter
	/// are removed from the end of the current string. If <paramref name="trimChars"/> is <see langword="null"/>
	/// or an empty array, white-space characters are removed instead.
	/// </returns>
	public string TrimEnd(params char[] trimChars);

	/// <summary>
	/// Removes all leading occurrences of a set of characters specified in an array from the current string.
	/// </summary>
	/// <param name="trimChars">
	/// An array of Unicode characters to remove, or <see langword="null"/>.
	/// </param>
	/// <returns>
	/// The string that remains after all occurrences of the characters in the <paramref name="trimChars"/> parameter
	/// are removed from the start of the current string. If <paramref name="trimChars"/> is <see langword="null"/>
	/// or an empty array, white-space characters are removed instead.
	/// </returns>
	public string TrimStart(params char[] trimChars);
}
