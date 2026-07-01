// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;
using System.Text.Json;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is a structurally well-formed JWT: exactly three '.'-separated segments,
/// with non-empty header and payload segments that base64url-decode to JSON objects. The signature
/// segment may be empty (e.g. <c>alg=none</c>) and is neither decoded nor verified.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsJwtTokenAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for JWT validation.</summary>
	/// <returns>A validation adapter for JWTs.</returns>
	protected override ValidationAdapter CreateValidator() => new JwtTokenValidator();

	private sealed class JwtTokenValidator : ValidationAdapter
	{
		protected override ValidationResult ValidateValue(string value)
		{
			string[] parts = value.Split('.');
			if (parts.Length != 3)
			{
				return ValidationResult.Failure("A JWT must have exactly three '.'-separated segments.");
			}

			if (parts[0].Length == 0 || parts[1].Length == 0)
			{
				return ValidationResult.Failure("A JWT must have non-empty header and payload segments.");
			}

			if (!DecodesToJsonObject(parts[0]))
			{
				return ValidationResult.Failure("The JWT header must be base64url-encoded JSON object.");
			}

			return DecodesToJsonObject(parts[1])
				? ValidationResult.Success()
				: ValidationResult.Failure("The JWT payload must be base64url-encoded JSON object.");
		}

		private static bool DecodesToJsonObject(string segment)
		{
			string base64 = segment.Replace('-', '+').Replace('_', '/');
			switch (base64.Length % 4)
			{
				case 2:
					base64 += "==";
					break;
				case 3:
					base64 += "=";
					break;
				case 1:
					return false;
				default:
					break;
			}

			byte[] bytes;
			try
			{
				bytes = Convert.FromBase64String(base64);
			}
			catch (FormatException)
			{
				return false;
			}

			try
			{
				using JsonDocument document = JsonDocument.Parse(bytes);
				return document.RootElement.ValueKind == JsonValueKind.Object;
			}
			catch (JsonException)
			{
				return false;
			}
		}
	}
}
