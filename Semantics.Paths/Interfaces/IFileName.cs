// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

/// <summary>
/// Interface for filenames (without directory path)
/// </summary>
public interface IFileName
{
	/// <summary>
	/// Gets the underlying string value for interoperability with non-semantic string operations.
	/// </summary>
	/// <value>The raw string value of this filename.</value>
	/// <remarks>
	/// Exposed on the interface so that code holding an <see cref="IFileName"/> — for example the results of
	/// a polymorphic filename API — can read the value without downcasting to a concrete type.
	/// </remarks>
	public string WeakString { get; }
}
