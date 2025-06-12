# Thermal Domain

This domain contains thermodynamics and heat transfer related physical quantities.

## TODO: Physical Quantities to Implement

### Core Thermal Quantities
- [ ] `Temperature.cs` - Already partially implemented in Mechanics (needs expansion)
  - Celsius, Fahrenheit, Rankine, Kelvin conversions
  - Absolute vs relative temperature handling
- [ ] `Heat.cs` - Thermal energy quantity
  - Joules, calories, BTU conversions
  - Heat capacity calculations
- [ ] `Entropy.cs` - Thermodynamic entropy
  - J/K, cal/K units
  - Statistical mechanics relationships

### Heat Transfer Quantities  
- [ ] `ThermalConductivity.cs` - Heat conduction coefficient
  - W/(m·K), cal/(s·cm·K) units
  - Fourier's law relationships
- [ ] `HeatCapacity.cs` - Thermal energy storage capacity
  - J/K, cal/K, BTU/°F units
  - Specific vs total heat capacity
- [ ] `SpecificHeat.cs` - Heat capacity per unit mass
  - J/(kg·K), cal/(g·K), BTU/(lb·°F) units
- [ ] `HeatTransferCoefficient.cs` - Convective heat transfer
  - W/(m²·K), BTU/(h·ft²·°F) units
  - Newton's law of cooling

### Thermal Properties
- [ ] `ThermalExpansion.cs` - Material expansion coefficient  
  - K⁻¹, °F⁻¹ units
  - Linear, area, and volumetric expansion
- [ ] `ThermalDiffusivity.cs` - Heat diffusion rate
  - m²/s, ft²/h units
  - α = k/(ρ·cp) relationship
- [ ] `ThermalResistance.cs` - Resistance to heat flow
  - K/W, °F·h/BTU units
  - Thermal circuit analysis

### Dimensional Relationships
```csharp
// Heat flow: Q̇ = k·A·ΔT/L (Fourier's law)
var heatFlow = thermalConductivity * area * temperatureDifference / length;

// Heat capacity: Q = C·ΔT
var heat = heatCapacity * temperatureChange;

// Thermal resistance: R = L/(k·A)
var thermalResistance = length / (thermalConductivity * area);
```

### Engineering Applications
- HVAC system design
- Heat exchanger analysis  
- Thermal insulation calculations
- Engine thermal management
- Electronic cooling systems 
