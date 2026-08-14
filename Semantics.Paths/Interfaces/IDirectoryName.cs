// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

/// <summary>
/// Interface for directory names
/// </summary>
public interface IDirectoryName
{
	/// <summary>
	/// Gets the underlying string value for interoperability with non-semantic string operations.
	/// </summary>
	/// <value>The raw string value of this directory name.</value>
	/// <remarks>
	/// Exposed on the interface so that code holding an <see cref="IDirectoryName"/> — for example the results of
	/// a polymorphic directory name API — can read the value without downcasting to a concrete type.
	/// </remarks>
	public string WeakString { get; }
}
