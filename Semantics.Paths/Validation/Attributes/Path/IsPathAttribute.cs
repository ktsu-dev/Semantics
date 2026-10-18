// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using System;
using System.IO;
#if !NET5_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a string represents a valid path for the host platform.
/// </summary>
/// <remarks>
/// This attribute enforces the following rules:
/// <list type="bullet">
/// <item><description>Path must not contain any characters returned by <see cref="Path.GetInvalidPathChars()"/></description></item>
/// <item><description>On Windows only, path must not contain the reserved characters &lt;, &gt; and |</description></item>
/// <item><description>On Windows only, path length must not exceed 256 characters</description></item>
/// <item><description>Empty or null strings are considered valid</description></item>
/// </list>
/// The reserved characters and the length limit are Windows rules. Linux and macOS allow all three
/// characters in a file name and paths far longer than 256 characters, so applying them there would
/// refuse real files the file system or git reports. On those platforms only NUL is refused.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsPathAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for path validation.
	/// </summary>
	/// <returns>A validation adapter for path strings</returns>
	protected override ValidationAdapter CreateValidator() => new PathValidator();

	/// <summary>
	/// Validation adapter for path strings using efficient span-based character validation.
	/// </summary>
	private sealed class PathValidator : ValidationAdapter
	{
		private const int WindowsMaxPathLength = 256;

		private static readonly bool IsWindows =
#if NET5_0_OR_GREATER
			OperatingSystem.IsWindows();
#else
			RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif

		// On Windows, add the reserved characters (<, >, |) to the system's invalid path chars, since
		// GetInvalidPathChars does not include them on every runtime. On other platforms they are legal
		// file name characters, so only the system's own invalid chars apply.
		private static readonly char[] InvalidPathChars = BuildInvalidCharArray();

		private static char[] BuildInvalidCharArray()
		{
			char[] systemInvalid = Path.GetInvalidPathChars();
			char[] reservedChars = IsWindows ? ['<', '>', '|'] : [];

			// Combine arrays, using HashSet to deduplicate in case system already includes reserved chars
			System.Collections.Generic.HashSet<char> charSet = [.. systemInvalid, .. reservedChars];

			char[] result = new char[charSet.Count];
			charSet.CopyTo(result);
			return result;
		}

		/// <summary>
		/// Validates that a string represents a valid path.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			if (IsWindows && value.Length > WindowsMaxPathLength)
			{
				return ValidationResult.Failure($"Path length cannot exceed {WindowsMaxPathLength} characters.");
			}

			// Use efficient span-based search for invalid characters
#if NETSTANDARD2_0
			bool hasInvalidChars = value.IndexOfAny(InvalidPathChars) != -1;
#else
			ReadOnlySpan<char> valueSpan = value.AsSpan();
			bool hasInvalidChars = valueSpan.IndexOfAny(InvalidPathChars) != -1;
#endif
			return hasInvalidChars
				? ValidationResult.Failure("Path contains invalid characters.")
				: ValidationResult.Success();
		}
	}
}
