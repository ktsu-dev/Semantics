// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using System;
using ktsu.Semantics.Strings;

/// <summary>
/// Validates that a file path's final segment can be a file name: it is not <c>.</c> or <c>..</c>.
/// </summary>
/// <remarks>
/// A path ending in <c>.</c> or <c>..</c> names a directory, so typing it as a file would let
/// <c>dir / FileName.Create("..")</c> escape <c>dir</c>, and the extension members would read the
/// dots as an extension separator. Applied to every file path type through
/// <see cref="SemanticFilePath{TDerived}"/>, so a valid file path always has a valid <see cref="FileName"/>.
/// Directory paths are unaffected: a directory may legitimately be named <c>..</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
internal sealed class HasFileNameSegmentAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for a file path's final segment.
	/// </summary>
	/// <returns>A validation adapter for file paths</returns>
	protected override ValidationAdapter CreateValidator() => new FileNameSegmentValidator();

	/// <summary>
	/// Validation adapter for a file path's final segment.
	/// </summary>
	private sealed class FileNameSegmentValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that the segment after the last directory separator is not <c>.</c> or <c>..</c>.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			string segment = value.Substring(value.LastIndexOfAny(['/', '\\']) + 1);
			return FileNameSegment.IsDotSegment(segment)
				? ValidationResult.Failure(FileNameSegment.DotSegmentError)
				: ValidationResult.Success();
		}
	}
}

/// <summary>
/// The rule shared by <see cref="IsFileNameAttribute"/> and <see cref="HasFileNameSegmentAttribute"/>.
/// </summary>
internal static class FileNameSegment
{
	/// <summary>
	/// The error reported for a file name of <c>.</c> or <c>..</c>.
	/// </summary>
	internal const string DotSegmentError = "A file name cannot be \".\" or \"..\", which name a directory.";

	/// <summary>
	/// Checks whether a segment is <c>.</c> or <c>..</c>.
	/// </summary>
	/// <param name="segment">The segment to check.</param>
	/// <returns><see langword="true"/> if the segment is <c>.</c> or <c>..</c>; otherwise, <see langword="false"/>.</returns>
	internal static bool IsDotSegment(string segment) => segment is "." or "..";
}
