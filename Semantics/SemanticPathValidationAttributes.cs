// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.IO;

/// <summary>
/// Validates that a string represents a valid path with no invalid path characters and a reasonable length.
/// </summary>
/// <remarks>
/// This attribute enforces the following rules:
/// <list type="bullet">
/// <item><description>Path length must not exceed 256 characters</description></item>
/// <item><description>Path must not contain any characters returned by <see cref="Path.GetInvalidPathChars()"/></description></item>
/// <item><description>Empty or null strings are considered valid</description></item>
/// </list>
/// The 256-character limit provides a reasonable balance between compatibility and practical usage,
/// while being more restrictive than the maximum path lengths supported by most file systems.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsPathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a valid path.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is a valid path; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		if (value.Length > 256)
		{
			return false;
		}

		// Check for characters from GetInvalidPathChars() and additional problematic characters
		// In .NET Core+, GetInvalidPathChars() doesn't include all characters that can cause issues in paths
		char[] invalidChars = [.. Path.GetInvalidPathChars(), '<', '>', '|'];
		return !value.Intersect(invalidChars).Any();
	}
}

/// <summary>
/// Validates that a path is relative (not fully qualified), meaning it does not start from a root directory.
/// </summary>
/// <remarks>
/// A relative path is one that specifies a location relative to the current working directory or another specified directory.
/// Examples of relative paths:
/// <list type="bullet">
/// <item><description><c>file.txt</c> - file in current directory</description></item>
/// <item><description><c>folder/file.txt</c> - file in subdirectory</description></item>
/// <item><description><c>../file.txt</c> - file in parent directory</description></item>
/// <item><description><c>./folder/file.txt</c> - file in subdirectory (explicit current directory)</description></item>
/// </list>
/// This validation uses <see cref="Path.IsPathFullyQualified(string)"/> to determine if a path is absolute.
/// Empty or null strings are considered valid relative paths.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsRelativePathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a relative path.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is a relative path; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || !Path.IsPathFullyQualified(value);
	}
}

/// <summary>
/// Validates that a path is absolute (fully qualified), meaning it specifies a complete path from the root of the file system.
/// </summary>
/// <remarks>
/// An absolute path provides the complete location of a file or directory from the root directory.
/// Examples of absolute paths:
/// <list type="bullet">
/// <item><description><c>C:\Windows\System32</c> - Windows absolute path</description></item>
/// <item><description><c>/usr/local/bin</c> - Unix/Linux absolute path</description></item>
/// <item><description><c>\\server\share\file.txt</c> - UNC path</description></item>
/// </list>
/// This validation uses <see cref="Path.IsPathFullyQualified(string)"/> with a directory separator appended
/// to handle edge cases where the path might be interpreted differently.
/// Empty or null strings are considered valid for flexibility in initialization scenarios.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsAbsolutePathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents an absolute path.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is an absolute path; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || Path.IsPathFullyQualified(value + Path.DirectorySeparatorChar);
	}
}

/// <summary>
/// Validates that a path represents a directory (not an existing file)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsDirectoryPathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a directory path by ensuring it's not an existing file.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is empty, null, or not an existing file; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This validation passes if the path doesn't exist as a file, allowing for non-existent directories
	/// and existing directories. It only fails if the path exists and is specifically a file.
	/// </remarks>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || !File.Exists(value);
	}
}

/// <summary>
/// Validates that a path represents a file (not an existing directory)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsFilePathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a file path by ensuring it's not an existing directory.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is empty, null, or not an existing directory; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This validation passes if the path doesn't exist as a directory, allowing for non-existent files
	/// and existing files. It only fails if the path exists and is specifically a directory.
	/// </remarks>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || !Directory.Exists(value);
	}
}

/// <summary>
/// Validates that a string represents a valid filename (no invalid filename characters, not a directory)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsFileNameAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a valid filename.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is a valid filename; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// A valid filename must meet the following criteria:
	/// <list type="bullet">
	/// <item><description>Must not contain any characters from <see cref="Path.GetInvalidFileNameChars()"/></description></item>
	/// <item><description>Must not be an existing directory path</description></item>
	/// <item><description>Empty or null strings are considered valid</description></item>
	/// </list>
	/// </remarks>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || (!Directory.Exists(value) && !value.Intersect(Path.GetInvalidFileNameChars()).Any());
	}
}

/// <summary>
/// Validates that a path exists on the filesystem
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DoesExistAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a path that exists on the filesystem.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the path exists as either a file or directory; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This validation requires the path to actually exist on the filesystem as either a file or directory.
	/// Empty or null strings are considered invalid and will fail validation.
	/// The validation uses both <see cref="File.Exists(string)"/> and <see cref="Directory.Exists(string)"/>
	/// to check for existence.
	/// </remarks>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return !string.IsNullOrEmpty(value) && (File.Exists(value) || Directory.Exists(value));
	}
}

/// <summary>
/// Validates that a string represents a valid file extension (starts with a period)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsExtensionAttribute : SemanticStringValidationAttribute
{
	/// <inheritdoc/>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || value.StartsWith('.');
	}
}

/// <summary>
/// Validates that a path string contains valid filename characters using span semantics.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsValidFileNameAttribute : SemanticStringValidationAttribute
{
	private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

	/// <inheritdoc/>
	public override bool Validate(ISemanticString semanticString)
	{
		ReadOnlySpan<char> value = semanticString.WeakString.AsSpan();

		// Use span-based search for invalid characters
		return value.IndexOfAny(InvalidFileNameChars) == -1;
	}
}

/// <summary>
/// Validates that a path string contains valid path characters using span semantics.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsValidPathAttribute : SemanticStringValidationAttribute
{
	private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

	/// <inheritdoc/>
	public override bool Validate(ISemanticString semanticString)
	{
		ReadOnlySpan<char> value = semanticString.WeakString.AsSpan();

		// Use span-based search for invalid characters
		return value.IndexOfAny(InvalidPathChars) == -1;
	}
}
