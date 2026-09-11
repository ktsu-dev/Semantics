// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// As much of <c>dimensions.json</c> as the C++ projection reads.
/// </summary>
/// <remarks>
/// A second reader of the same file rather than a share of
/// <c>Semantics.SourceGenerators</c>'s model, and deliberately.
/// <para>
/// The shared thing is the JSON. That project's model is a Roslyn component's internal shape: it
/// carries every field the C# generators need, validates for their diagnostics, and lives in a
/// netstandard2.0 assembly that brings Microsoft.CodeAnalysis with it. Referencing it drags all of
/// that into a project that wants none of it, and compiling its source in here subjects a file
/// another project owns to this one's analysis rules. Reading the file directly costs the few
/// declarations below and leaves the two consumers independent -- which is what they are.
/// </para>
/// <para>
/// The cost of the duplication is a field added to the metadata and read by only one side. That is
/// caught rather than hoped for: this reader ignores what it does not know, and
/// <see cref="QuantityVocabulary"/> refuses anything it cannot make sense of by name.
/// </para>
/// </remarks>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public sealed class QuantityMetadata
{
	/// <summary>Gets the dimensions the file declares, in the order it declares them.</summary>
	[JsonPropertyName("physicalDimensions")]
	public Collection<MetadataDimension> PhysicalDimensions { get; } = [];

	/// <summary>
	/// Reads the metadata.
	/// </summary>
	/// <param name="json">The contents of <c>dimensions.json</c>.</param>
	/// <returns>The dimensions it declares.</returns>
	/// <exception cref="JsonException">The document is not the shape this expects.</exception>
	public static QuantityMetadata Parse(string json) =>
		JsonSerializer.Deserialize<QuantityMetadata>(json, Options)
			?? throw new JsonException("dimensions.json is empty.");

	private static readonly JsonSerializerOptions Options = new()
	{
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling = JsonCommentHandling.Skip,
	};
}

/// <summary>One physical dimension.</summary>
/// <remarks>
/// The collection properties are get-only and populated in place, which is what
/// <see cref="JsonObjectCreationHandling.Populate"/> asks for: a settable
/// <c>List&lt;T&gt;</c> would deserialise without it and is what the analyzers object to.
/// </remarks>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public sealed class MetadataDimension
{
	/// <summary>Gets or sets what the dimension is called.</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets the exponents, keyed by axis, naming only the axes that are not zero.
	/// </summary>
	[JsonPropertyName("dimensionalFormula")]
	public Dictionary<string, int> DimensionalFormula { get; } = [];

	/// <summary>Gets or sets the vector forms this dimension has.</summary>
	public MetadataForms Quantities { get; set; } = new();

	/// <summary>Gets what this dimension multiplied by another produces.</summary>
	public Collection<MetadataRelationship> Integrals { get; } = [];

	/// <summary>Gets what this dimension divided by another produces.</summary>
	public Collection<MetadataRelationship> Derivatives { get; } = [];

	/// <summary>Gets what this dimension dotted with another produces.</summary>
	public Collection<MetadataRelationship> DotProducts { get; } = [];

	/// <summary>Gets what this dimension crossed with another produces.</summary>
	public Collection<MetadataRelationship> CrossProducts { get; } = [];
}

/// <summary>The vector forms a dimension declares, from the magnitude up to four components.</summary>
/// <remarks>
/// Indexed rather than named individually by everything that walks them, because every rule about
/// a form -- what it is called, how many components it has, whether a relationship reaches it --
/// is the same rule with a different number in it.
/// </remarks>
public sealed class MetadataForms
{
	/// <summary>Gets or sets the magnitude form, or null when the dimension has none.</summary>
	[JsonPropertyName("vector0")]
	public MetadataForm? Vector0 { get; set; }

	/// <summary>Gets or sets the signed one-dimensional form.</summary>
	[JsonPropertyName("vector1")]
	public MetadataForm? Vector1 { get; set; }

	/// <summary>Gets or sets the two-component form.</summary>
	[JsonPropertyName("vector2")]
	public MetadataForm? Vector2 { get; set; }

	/// <summary>Gets or sets the three-component form.</summary>
	[JsonPropertyName("vector3")]
	public MetadataForm? Vector3 { get; set; }

	/// <summary>Gets or sets the four-component form.</summary>
	[JsonPropertyName("vector4")]
	public MetadataForm? Vector4 { get; set; }

	/// <summary>The number of forms a dimension can declare, counting the magnitude.</summary>
	internal const int Count = 5;

	/// <summary>
	/// Gets one form by how many components it has.
	/// </summary>
	/// <param name="form">The component count, from zero to four.</param>
	/// <returns>The form, or null when the dimension does not declare it.</returns>
	internal MetadataForm? this[int form] => form switch
	{
		0 => Vector0,
		1 => Vector1,
		2 => Vector2,
		3 => Vector3,
		4 => Vector4,
		_ => null,
	};
}

/// <summary>One vector form: a base type and the names that refine it.</summary>
/// <remarks>
/// The collection properties are get-only and populated in place, which is what
/// <see cref="JsonObjectCreationHandling.Populate"/> asks for: a settable
/// <c>List&lt;T&gt;</c> would deserialise without it and is what the analyzers object to.
/// </remarks>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public sealed class MetadataForm
{
	/// <summary>Gets or sets what the base type is called.</summary>
	public string Base { get; set; } = string.Empty;

	/// <summary>Gets the named refinements of that base.</summary>
	public Collection<MetadataOverload> Overloads { get; } = [];
}

/// <summary>A named refinement of a base quantity.</summary>
public sealed class MetadataOverload
{
	/// <summary>Gets or sets what it is called.</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Gets or sets what it is.</summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the stricter bound this refinement opts into, when it has one.
	/// </summary>
	/// <remarks>
	/// Only a strict-positive floor is modelled, because it is the only one declared: a wavelength,
	/// a period and a half-life are quantities for which zero is unphysical rather than merely
	/// small.
	/// </remarks>
	public MetadataConstraints? PhysicalConstraints { get; set; }
}

/// <summary>A bound tighter than the magnitude form's own.</summary>
public sealed class MetadataConstraints
{
	/// <summary>Gets or sets the value the quantity must exceed.</summary>
	public string MinExclusive { get; set; } = string.Empty;
}

/// <summary>One declared relationship between dimensions.</summary>
/// <remarks>
/// The collection property is get-only and populated in place, which is what
/// <see cref="JsonObjectCreationHandling.Populate"/> asks for.
/// </remarks>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public sealed class MetadataRelationship
{
	/// <summary>Gets or sets the dimension on the other side of the operator.</summary>
	public string Other { get; set; } = string.Empty;

	/// <summary>Gets or sets the dimension the operator produces.</summary>
	public string Result { get; set; } = string.Empty;

	/// <summary>
	/// Gets the vector forms this relationship is declared at, or nothing to mean every form the
	/// participants share.
	/// </summary>
	/// <remarks>
	/// A constraint rather than a request: a cross product is declared at <c>[3]</c> because it is
	/// only defined in three dimensions, not because three is the form someone happened to want.
	/// </remarks>
	public Collection<int> Forms { get; } = [];
}
