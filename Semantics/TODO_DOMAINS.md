# Semantic Quantities Library - Domain Implementation TODOs

This document tracks the implementation progress of physical quantity domains in the Semantics library.

## Implementation Status Legend
- ✅ **Completed** - Fully implemented and tested
- 🚧 **In Progress** - Partially implemented  
- 📋 **Planned** - Detailed TODO lists created
- 🔍 **Research** - Needs domain analysis
- ❓ **Uncertain** - May not be needed

## Core Infrastructure ✅
- [x] `PhysicalQuantity<T>` abstract base class
- [x] `IUnit`, `ICompoundUnit` interfaces
- [x] `Units` static class with unit definitions
- [x] `Extensions` class for fluent syntax
- [x] Unit system classification (10 categories)
- [x] Automatic base unit detection
- [x] Domain-based file organization

## Mechanics Domain 🚧
**Location:** `Quantities/Mechanics/`
**Status:** Core quantities implemented, some advanced quantities needed

### Implemented ✅
- [x] `Length.cs` - Distance measurements
- [x] `Mass.cs` - Matter quantity  
- [x] `Time.cs` - Temporal measurements
- [x] `Area.cs` - 2D spatial measurements
- [x] `Volume.cs` - 3D spatial measurements
- [x] `Velocity.cs` - Rate of position change
- [x] `Acceleration.cs` - Rate of velocity change
- [x] `Force.cs` - Newton's second law (F=ma)
- [x] `Pressure.cs` - Force per unit area
- [x] `Energy.cs` - Work and energy quantities
- [x] `Power.cs` - Rate of energy transfer

### TODO 📋
- [ ] `Momentum.cs` - p = mv
- [ ] `AngularVelocity.cs` - Rotational motion
- [ ] `AngularAcceleration.cs` - Rate of angular velocity change
- [ ] `Torque.cs` - Rotational force
- [ ] `MomentOfInertia.cs` - Rotational mass
- [ ] `Density.cs` - Mass per unit volume
- [ ] `SpecificGravity.cs` - Density ratio

## Electrical Domain 🚧  
**Location:** `Quantities/Electrical/`
**Status:** Basic electrical quantities implemented

### Implemented ✅
- [x] `ElectricCurrent.cs` - Flow of electric charge
- [x] `ElectricPotential.cs` - Voltage measurements
- [x] `ElectricResistance.cs` - Opposition to current flow
- [x] `ElectricCharge.cs` - Quantity of electricity
- [x] `ElectricCapacitance.cs` - Charge storage ability

### TODO 📋
- [ ] `ElectricField.cs` - V/m, N/C units
- [ ] `ElectricFlux.cs` - Gauss's law applications
- [ ] `Permittivity.cs` - Electric field response
- [ ] `ElectricConductivity.cs` - 1/resistance per length
- [ ] `ElectricPowerDensity.cs` - Power per unit volume
- [ ] `ImpedanceAC.cs` - Complex impedance for AC circuits

## Thermal Domain 📋
**Location:** `Quantities/Thermal/`
**Status:** Detailed TODO created, ready for implementation
**Reference:** See `Thermal/README.md` for comprehensive implementation plan

### Priority Items
- [ ] `Temperature.cs` - Expand existing with Celsius/Fahrenheit
- [ ] `Heat.cs` - Thermal energy (J, cal, BTU)
- [ ] `ThermalConductivity.cs` - W/(m·K) units
- [ ] `HeatCapacity.cs` - J/K, cal/K units
- [ ] `SpecificHeat.cs` - J/(kg·K) units

## Chemical Domain 📋  
**Location:** `Quantities/Chemical/`
**Status:** Detailed TODO created, ready for implementation
**Reference:** See `Chemical/README.md` for comprehensive implementation plan

### Priority Items
- [ ] `Concentration.cs` - Molarity, molality, ppm
- [ ] `pH.cs` - Logarithmic acidity scale
- [ ] `ReactionRate.cs` - Chemical kinetics
- [ ] `MolarMass.cs` - g/mol, molecular weight
- [ ] `EnzymeActivity.cs` - Biochemical catalysis

## Optical Domain 📋
**Location:** `Quantities/Optical/`  
**Status:** Detailed TODO created, ready for implementation
**Reference:** See `Optical/README.md` for comprehensive implementation plan

### Priority Items
- [ ] `Wavelength.cs` - λ in nm, μm, Å
- [ ] `RefractiveIndex.cs` - Dimensionless n
- [ ] `OpticalPower.cs` - Diopters for lenses
- [ ] `Illuminance.cs` - Lux, foot-candles
- [ ] `Luminance.cs` - cd/m², nits

## Acoustic Domain 📋
**Location:** `Quantities/Acoustic/`
**Status:** Frequency implemented, detailed TODO created  
**Reference:** See `Acoustic/README.md` for comprehensive implementation plan

### Implemented ✅
- [x] `Frequency.cs` - Hz, kHz, MHz

### Priority Items  
- [ ] `SoundPressure.cs` - Pa, μPa, RMS measurements
- [ ] `SoundPressureLevel.cs` - dB SPL scale
- [ ] `SoundIntensity.cs` - W/m² units
- [ ] `AcousticImpedance.cs` - Pa·s/m, rayl

## Future Domains 🔍

### Electromagnetics 🔍
**Priority:** High for engineering applications
- Magnetic fields (Tesla, Gauss)
- Inductance (Henry, mH, μH)  
- Magnetic flux (Weber, Maxwell)
- Electromagnetic waves

### Materials Science 🔍
**Priority:** Medium for structural engineering
- Stress/strain relationships
- Elastic modulus (GPa)
- Yield strength (MPa)
- Hardness scales (Brinell, Rockwell)

### Nuclear & Radiation 🔍  
**Priority:** Specialized applications
- Radioactivity (Becquerel, Curie)
- Absorbed dose (Gray, Rad)
- Dose equivalent (Sievert, Rem)

### Fluid Dynamics 🔍
**Priority:** Medium for engineering
- Flow rate (m³/s, L/min, GPM)
- Viscosity (Pa·s, cP)
- Reynolds number (dimensionless)

### Astronomy & Cosmology ❓
**Priority:** Low, specialized domain
- Astronomical units (AU, ly, pc)
- Stellar magnitudes
- Redshift measurements

### Geophysics ❓  
**Priority:** Low, specialized domain
- Seismic velocities
- Earthquake magnitudes
- Gravity variations

### Quantum Mechanics ❓
**Priority:** Very low, highly specialized
- Action (J·s, ℏ)
- Wave numbers (m⁻¹, cm⁻¹)
- Cross sections (barn)

## Implementation Guidelines

### Next Steps (Recommended Order)
1. **Complete Mechanics** - Add missing rotational quantities
2. **Complete Electrical** - Add electromagnetic field quantities  
3. **Implement Thermal** - Start with Temperature expansion
4. **Implement Chemical** - Focus on concentration and pH
5. **Implement Optical** - Basic photometric quantities
6. **Expand Acoustic** - Sound pressure and levels

### Code Standards
- Follow existing patterns from Length.cs, Force.cs examples
- Include comprehensive unit conversions
- Add dimensional analysis relationships  
- Implement proper ToString() and equality
- Add XML documentation for all public members
- Include extension methods in Extensions.cs
- Add corresponding units to Units.cs

### Testing Requirements
- Unit conversion accuracy tests
- Dimensional analysis verification
- Cross-domain relationship validation
- Performance benchmarks for common operations

## Architecture TODOs

### High Priority
- [ ] **PhysicalDimension objects** instead of string dimensions
- [ ] **Generic INumber support** instead of specific numeric types
- [ ] **Compilation error fixes** for missing abstract members
- [ ] **Unit test expansion** for all implemented quantities

### Medium Priority  
- [ ] **Performance optimization** for common operations
- [ ] **Serialization support** (JSON, XML)
- [ ] **Culture-specific formatting** for international use
- [ ] **Range validation** for physical constraints

### Low Priority
- [ ] **Code generation** for repetitive quantity classes
- [ ] **Plugin architecture** for custom domains
- [ ] **Integration with measurement APIs** 
- [ ] **CAD/engineering tool integration** 
