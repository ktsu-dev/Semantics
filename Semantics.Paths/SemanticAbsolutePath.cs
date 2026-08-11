// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

/// <summary>
/// Base class for absolute paths (fully qualified paths)
/// </summary>
[IsAbsolutePath]
public abstract record SemanticAbsolutePath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticAbsolutePath<TDerived>
{
}
