// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using System;

/// <summary>
/// Base interface for all path types
/// </summary>
public interface IPath : IComparable<IPath>
{
	/// <summary>
	/// Gets the underlying string value for interoperability with non-semantic string operations.
	/// </summary>
	/// <value>The raw string value of this path.</value>
	/// <remarks>
	/// Exposed on the interface so that code holding an <see cref="IPath"/> — for example the results of
	/// <see cref="SemanticDirectoryPath{TDerived}.GetContents"/> — can read the value without downcasting to a concrete type.
	/// </remarks>
	public string WeakString { get; }
}
