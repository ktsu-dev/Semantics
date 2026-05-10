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
	public string Magnitude { get; set; } = "1";
	public string ConversionFactor { get; set; } = "1";
	public string Offset { get; set; } = "0";

	/// <summary>
	/// Plural-form identifier used when emitting <c>From{FactoryName}</c> factories per #49.
	/// Empty string means "fall back to the rule built into the generator" — currently
	/// <c>Name + "s"</c>, which is correct for regular units (Meter→Meters, Newton→Newtons)
	/// but wrong for irregulars (Foot, Inch), already-plural compounds (MetersPerSecond),
	/// and mass nouns (Hertz, Lux, Siemens). Set this explicitly for those cases.
	/// </summary>
	public string FactoryName { get; set; } = string.Empty;
}
