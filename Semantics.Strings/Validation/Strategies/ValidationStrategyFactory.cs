// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

/// <summary>
/// Factory for creating appropriate validation strategies based on type attributes.
/// </summary>
public static class ValidationStrategyFactory
{
	private static readonly ValidateAllStrategy AllStrategy = new();
	private static readonly ValidateAnyStrategy AnyStrategy = new();

	/// <summary>
	/// Creates the appropriate validation strategy based on the type's validation attributes.
	/// </summary>
	/// <param name="type">The type to examine for validation strategy attributes</param>
	/// <returns>The appropriate validation strategy instance</returns>
	public static IValidationStrategy CreateStrategy(Type type)
	{
		Ensure.NotNull(type);

		// Check for ValidateAny attribute
		object[] validateAnyAttributes = type.GetCustomAttributes(typeof(ValidateAnyAttribute), true);
		return validateAnyAttributes.Length > 0 ? AnyStrategy : AllStrategy;
	}
}
