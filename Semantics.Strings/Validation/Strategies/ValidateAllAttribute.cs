// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;

/// <summary>
/// Specifies that all validation attributes should pass (logical AND)
/// This is the default behavior, but can be used for clarity
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ValidateAllAttribute : Attribute
{
}
