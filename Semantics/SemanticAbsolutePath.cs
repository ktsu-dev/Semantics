// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Base class for absolute paths (fully qualified paths)
/// </summary>
[IsPath, IsAbsolutePath]
public abstract record SemanticAbsolutePath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticAbsolutePath<TDerived>
{
}
