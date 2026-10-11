// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Color;

/// <summary>
/// The three kinds of color vision deficiency simulated by <see cref="ColorVision"/>, named for the
/// cone type that is anomalous (at partial severity) or missing (at severity 1).
/// </summary>
public enum ColorVisionDeficiency
{
	/// <summary>Anomalous or missing long-wavelength (red) cones: protanomaly, and protanopia at severity 1.</summary>
	Protan = 0,

	/// <summary>Anomalous or missing medium-wavelength (green) cones: deuteranomaly, and deuteranopia at severity 1.</summary>
	Deutan = 1,

	/// <summary>Anomalous or missing short-wavelength (blue) cones: tritanomaly, and tritanopia at severity 1.</summary>
	Tritan = 2,
}
