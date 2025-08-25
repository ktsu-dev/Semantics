// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

/// <summary>
/// Defines contracts and invariants that all semantic string implementations must satisfy.
/// Ensures Liskov Substitution Principle compliance by establishing clear behavioral contracts.
/// </summary>
public static class SemanticStringContracts
{
	/// <summary>
	/// Validates that a semantic string implementation satisfies the basic contracts.
	/// </summary>
	/// <typeparam name="T">The semantic string type to validate</typeparam>
	/// <param name="instance">The instance to validate</param>
	/// <returns>True if all contracts are satisfied</returns>
	public static bool ValidateContracts<T>(T instance) where T : SemanticString<T>
	{
		if (instance is null)
		{
			return false;
		}

		// Contract 1: WeakString property must never be null if instance is valid
		if (instance.IsValid() && instance.WeakString is null)
		{
			return false;
		}

		// Contract 2: ToString() must return the same value as WeakString
		if (instance.ToString() != instance.WeakString)
		{
			return false;
		}

		// Contract 3: Length property must match WeakString.Length
		if (instance.Length != instance.WeakString.Length)
		{
			return false;
		}

		// Contract 4: Equality must be consistent
		if (!instance.Equals(instance))
		{
			return false;
		}

		// Contract 5: Hash code must be consistent
		int hash1 = instance.GetHashCode();
		int hash2 = instance.GetHashCode();
		return hash1 == hash2 && instance.CompareTo(instance) == 0;
	}

	/// <summary>
	/// Validates that two semantic string instances follow proper equality contracts.
	/// </summary>
	/// <typeparam name="T">The semantic string type</typeparam>
	/// <param name="first">First instance</param>
	/// <param name="second">Second instance</param>
	/// <returns>True if equality contracts are satisfied</returns>
	public static bool ValidateEqualityContracts<T>(T? first, T? second) where T : SemanticString<T>
		=> (first is null && second is null) || (first is not null && second is not null && ValidateEqualityContractsInternal(first, second));

	/// <summary>
	/// Internal helper method for validating equality contracts between non-null instances.
	/// </summary>
	/// <typeparam name="T">The semantic string type</typeparam>
	/// <param name="first">First non-null instance</param>
	/// <param name="second">Second non-null instance</param>
	/// <returns>True if equality contracts are satisfied</returns>
	private static bool ValidateEqualityContractsInternal<T>(T first, T second) where T : SemanticString<T>
	{
		// Reflexivity: x.Equals(x) must be true
		if (!first.Equals(first) || !second.Equals(second))
		{
			return false;
		}

		// Symmetry: x.Equals(y) must equal y.Equals(x)
		bool firstEqualsSecond = first.Equals(second);
		bool secondEqualsFirst = second.Equals(first);
		if (firstEqualsSecond != secondEqualsFirst)
		{
			return false;
		}

		// Hash code consistency: if x.Equals(y), then x.GetHashCode() == y.GetHashCode()
		return !firstEqualsSecond || first.GetHashCode() == second.GetHashCode();
	}

	/// <summary>
	/// Validates that comparison operations follow proper ordering contracts.
	/// </summary>
	/// <typeparam name="T">The semantic string type</typeparam>
	/// <param name="first">First instance</param>
	/// <param name="second">Second instance</param>
	/// <param name="third">Third instance for transitivity testing</param>
	/// <returns>True if comparison contracts are satisfied</returns>
	public static bool ValidateComparisonContracts<T>(T? first, T? second, T? third) where T : SemanticString<T>
		=> first is null || second is null || third is null || ValidateComparisonContractsInternal(first, second, third);

	/// <summary>
	/// Internal helper method for validating comparison contracts between non-null instances.
	/// </summary>
	/// <typeparam name="T">The semantic string type</typeparam>
	/// <param name="first">First non-null instance</param>
	/// <param name="second">Second non-null instance</param>
	/// <param name="third">Third non-null instance</param>
	/// <returns>True if comparison contracts are satisfied</returns>
	private static bool ValidateComparisonContractsInternal<T>(T first, T second, T third) where T : SemanticString<T>
	{
		// Reflexivity: x.CompareTo(x) must be 0
		if (first.CompareTo(first) != 0 || second.CompareTo(second) != 0 || third.CompareTo(third) != 0)
		{
			return false;
		}

		// Antisymmetry: if x.CompareTo(y) > 0, then y.CompareTo(x) < 0
		int firstToSecond = first.CompareTo(second.WeakString);
		int secondToFirst = second.CompareTo(first.WeakString);
		if ((firstToSecond > 0 && secondToFirst >= 0) || (firstToSecond < 0 && secondToFirst <= 0))
		{
			return false;
		}

		// Transitivity: if x.CompareTo(y) <= 0 and y.CompareTo(z) <= 0, then x.CompareTo(z) <= 0
		int secondToThird = second.CompareTo(third.WeakString);
		int firstToThird = first.CompareTo(third.WeakString);
		return !(firstToSecond <= 0 && secondToThird <= 0 && firstToThird > 0);
	}
}
