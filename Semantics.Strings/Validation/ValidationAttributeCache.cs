// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;
using System.Collections.Concurrent;
using System.Linq;

/// <summary>
/// The validation attributes a semantic string type declares, read once per type.
/// </summary>
/// <remarks>
/// <see cref="Attribute.GetCustomAttributes(System.Reflection.MemberInfo, Type, bool)"/> builds new attribute
/// instances on every call, and each <see cref="NativeSemanticStringValidationAttribute"/> builds its validator
/// lazily, so reading them per value allocated a validator per value validated (#444). Attributes are
/// immutable once constructed and their validators are stateless, so one set per type serves every thread.
/// </remarks>
internal static class ValidationAttributeCache
{
	private static readonly ConcurrentDictionary<Type, Entry> Entries = new();

	/// <summary>Gets the validation attributes declared on a type and its bases, in declaration order.</summary>
	/// <param name="type">The semantic string type.</param>
	/// <returns>The attributes, shared between callers; do not modify the array.</returns>
	public static SemanticStringValidationAttribute[] Attributes(Type type) => Get(type).Attributes;

	/// <summary>Gets whether a type is marked <see cref="ValidateAnyAttribute"/>.</summary>
	/// <param name="type">The semantic string type.</param>
	/// <returns><see langword="true"/> when any one attribute passing is enough.</returns>
	public static bool ValidatesAny(Type type) => Get(type).ValidatesAny;

	private static Entry Get(Type type)
	{
		Ensure.NotNull(type);
		return Entries.GetOrAdd(type, static t => new Entry(
			[.. t.GetCustomAttributes(typeof(SemanticStringValidationAttribute), true).Cast<SemanticStringValidationAttribute>()],
			t.GetCustomAttributes(typeof(ValidateAnyAttribute), true).Length > 0));
	}

	private sealed record Entry(SemanticStringValidationAttribute[] Attributes, bool ValidatesAny);
}
