// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;

/// <summary>
/// Metadata structure for domains generation.
/// </summary>
public class DomainsMetadata
{
	public List<Domain> Domains { get; set; } = [];
}

/// <summary>
/// Domain of physical constants.
/// </summary>
public class Domain
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public List<ConstantDefinition> Constants { get; set; } = [];
}

/// <summary>
/// Definition of a single physical constant for code generation.
/// </summary>
public class ConstantDefinition
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
}
