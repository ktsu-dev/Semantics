# Optical Domain

This domain contains optics, photonics, and light-related physical quantities.

## TODO: Physical Quantities to Implement

### Core Optical Quantities
- [ ] `Wavelength.cs` - Electromagnetic wavelength
  - Meters, nanometers, micrometers, angstroms
  - Visible, UV, IR spectrum ranges
- [ ] `Frequency.cs` - Already partially in Acoustics (expand for optics)
  - Hertz, terahertz, petahertz
  - c = λν relationship with wavelength
- [ ] `Wavenumber.cs` - Spatial frequency
  - m⁻¹, cm⁻¹ units
  - Spectroscopy applications

### Photometric Quantities (Human Vision)
- [ ] `LuminousIntensity.cs` - Already have base Candela (needs expansion)
  - Candela, millicandela
  - Point source brightness
- [ ] `LuminousFlux.cs` - Total light output
  - Lumen (lm), candela-steradian
  - Light source characterization
- [ ] `Illuminance.cs` - Light incident on surface
  - Lux (lm/m²), foot-candles
  - Lighting design calculations
- [ ] `Luminance.cs` - Surface brightness
  - cd/m², nits, foot-lamberts
  - Display technology

### Radiometric Quantities (Physical Energy)
- [ ] `RadiantIntensity.cs` - Radiant power per solid angle
  - W/sr units
  - Laser beam characterization
- [ ] `RadiantFlux.cs` - Radiant power
  - Watts, milliwatts
  - Optical power measurements
- [ ] `Irradiance.cs` - Radiant flux per area
  - W/m², mW/cm²
  - Solar irradiance, laser intensity
- [ ] `Radiance.cs` - Radiant intensity per area
  - W/(m²·sr), W/(cm²·sr)
  - Surface emission properties

### Optical Properties
- [ ] `RefractiveIndex.cs` - Light bending property
  - Dimensionless ratio
  - Snell's law calculations
- [ ] `OpticalPower.cs` - Lens focusing strength
  - Diopters (D), m⁻¹
  - Vision correction, lens design
- [ ] `Transmittance.cs` - Light transmission fraction
  - Dimensionless (0-1) or percentage
  - Filter and material characterization
- [ ] `Reflectance.cs` - Light reflection fraction
  - Dimensionless (0-1) or percentage  
  - Mirror and surface properties
- [ ] `Absorbance.cs` - Light absorption logarithmic scale
  - Dimensionless (optical density)
  - Beer-Lambert law applications

### Laser & Coherent Light
- [ ] `BeamDivergence.cs` - Angular spread of laser beam
  - Radians, milliradians, degrees
  - Laser beam quality
- [ ] `CoherenceLength.cs` - Spatial coherence distance
  - Meters, millimeters, micrometers
  - Interferometry applications
- [ ] `OpticalPathLength.cs` - Effective distance in medium
  - Meters (accounting for refractive index)
  - Phase calculations

### Spectroscopy
- [ ] `SpectralPower.cs` - Power per wavelength
  - W/nm, W/cm⁻¹ units
  - Light source characterization
- [ ] `SpectralResponse.cs` - Detector sensitivity vs wavelength
  - A/W, V/W per wavelength
  - Photodetector specifications
- [ ] `OpticalDensity.cs` - Logarithmic absorption
  - Dimensionless (-log₁₀T)
  - Filter specifications

### Fiber Optics
- [ ] `NumericalAperture.cs` - Light-gathering ability
  - Dimensionless (sin θ)
  - Fiber coupling efficiency
- [ ] `OpticalLoss.cs` - Signal attenuation
  - dB/km, dB/m units
  - Fiber link budget calculations

### Dimensional Relationships
```csharp
// Speed of light: c = λν
var speedOfLight = wavelength * frequency;

// Snell's law: n₁sin(θ₁) = n₂sin(θ₂)
var refractedAngle = Math.Asin((refractiveIndex1 / refractiveIndex2) * Math.Sin(incidentAngle));

// Beer-Lambert law: A = εcl
var absorbance = extinctionCoefficient * concentration * pathLength;

// Lens equation: 1/f = 1/s + 1/s'
var focalLength = 1.0 / (1.0/objectDistance + 1.0/imageDistance);
```

### Applications
- Optical design (lenses, mirrors)
- Lighting engineering
- Display technology
- Laser systems
- Fiber optic communications
- Spectroscopy and analysis
- Machine vision
- Photography and imaging 
