// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

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
