// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp;

/// <summary>
/// What a target says about how it wants the quantity vocabulary spelled.
/// </summary>
/// <remarks>
/// Deliberately short. Almost nothing here is a choice: the names come from the metadata, the
/// arithmetic comes from the prelude, and the shape of a generated class is settled by what it has
/// to compile to rather than by taste. What is left is where the types live and what the file says
/// at the top.
/// </remarks>
public sealed record CppQuantityOptions
{
	/// <summary>
	/// Gets the namespace the vocabulary is generated into.
	/// </summary>
	/// <remarks>
	/// A target puts these types in its own namespace rather than one this library picks, because
	/// a program reading <c>Length</c> should see its own engine's name in front of it.
	/// </remarks>
	public string Namespace { get; init; } = "ktsu";

	/// <summary>
	/// Gets the extension a generated header is given, including its leading dot.
	/// </summary>
	public string HeaderExtension { get; init; } = ".hpp";

	/// <summary>
	/// Gets the line a generated file's banner opens with.
	/// </summary>
	/// <remarks>
	/// The reader of a generated file wants the name of the thing they would run again, which is
	/// not necessarily this library: a target that drives it from its own build step says so.
	/// </remarks>
	public string GeneratedBy { get; init; } = "ktsu.Semantics.Cpp";
}
