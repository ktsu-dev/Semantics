// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

/// <summary>
/// Interface for file extensions (starts with a period)
/// </summary>
public interface IFileExtension
{
	/// <summary>
	/// Gets the underlying string value for interoperability with non-semantic string operations.
	/// </summary>
	/// <value>The raw string value of this file extension.</value>
	/// <remarks>
	/// Exposed on the interface so that code holding an <see cref="IFileExtension"/> — for example the results of
	/// a polymorphic extension API — can read the value without downcasting to a concrete type.
	/// </remarks>
	public string WeakString { get; }
}
