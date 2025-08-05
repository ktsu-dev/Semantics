// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;

public class UnitsMetadata
{
	public List<UnitCategory> UnitCategories { get; set; } = [];
}

public class UnitCategory
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public List<UnitDefinition> Units { get; set; } = [];
}

public class UnitDefinition
{
	public string Name { get; set; } = string.Empty;
	public string Symbol { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string System { get; set; } = string.Empty;
	public string Magnitude { get; set; } = string.Empty;
	public string ConversionFactor { get; set; } = string.Empty;
	public string Offset { get; set; } = string.Empty;
}
