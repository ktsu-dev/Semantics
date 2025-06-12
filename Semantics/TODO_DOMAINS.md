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

## Mechanics Domain ✅
**Location:** `Quantities/Mechanics/`
**Status:** All quantities implemented

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
- [x] `Momentum.cs` - p = mv
- [x] `AngularVelocity.cs` - Rotational motion
- [x] `AngularAcceleration.cs` - Rate of angular velocity change
- [x] `Torque.cs` - Rotational force
- [x] `MomentOfInertia.cs` - Rotational mass
- [x] `Density.cs` - Mass per unit volume
- [x] `SpecificGravity.cs` - Density ratio

## Electrical Domain ✅  
**Location:** `Quantities/Electrical/`
**Status:** All electrical quantities implemented

### Implemented ✅
- [x] `ElectricCurrent.cs` - Flow of electric charge
- [x] `ElectricPotential.cs` - Voltage measurements
- [x] `ElectricResistance.cs` - Opposition to current flow
- [x] `ElectricCharge.cs` - Quantity of electricity
- [x] `ElectricCapacitance.cs` - Charge storage ability
- [x] `ElectricField.cs` - V/m, N/C units
- [x] `ElectricFlux.cs` - Gauss's law applications
- [x] `Permittivity.cs` - Electric field response
- [x] `ElectricConductivity.cs` - 1/resistance per length
- [x] `ElectricPowerDensity.cs` - Power per unit volume
- [x] `ImpedanceAC.cs` - Complex impedance for AC circuits

## Thermal Domain ✅
**Location:** `Quantities/Thermal/`
**Status:** All thermal quantities implemented
**Reference:** See `Thermal/README.md` for comprehensive implementation plan

### Implemented ✅
- [x] `Temperature.cs` - Complete K/°C/°F/°R conversions with constants
- [x] `Heat.cs` - Thermal energy (J, cal, BTU, kWh)
- [x] `ThermalConductivity.cs` - W/(m·K), Fourier's law implementation
- [x] `ThermalResistance.cs` - K/W, thermal circuits with series/parallel
- [x] `Entropy.cs` - J/K, thermodynamic entropy with ΔS = Q/T
- [x] `HeatCapacity.cs` - J/K, Q = C·ΔT relationships
- [x] `SpecificHeat.cs` - J/(kg·K), common material values included
- [x] `HeatTransferCoefficient.cs` - W/(m²·K), Newton's law of cooling
- [x] `ThermalExpansion.cs` - K⁻¹, linear/area/volume expansion
- [x] `ThermalDiffusivity.cs` - m²/s, α = k/(ρ·cp) relationships

## Chemical Domain ✅
**Location:** `Quantities/Chemical/`
**Status:** All 10 chemical quantities implemented (100% complete)
**Reference:** See `Chemical/README.md` for comprehensive implementation plan

### Implemented ✅
- [x] `AmountOfSubstance.cs` - Moles with Avogadro's number calculations
- [x] `Concentration.cs` - Molarity, ppm, weight/volume percent with dilution
- [x] `MolarMass.cs` - g/mol, molecular weights with common compounds
- [x] `PH.cs` - Logarithmic acidity scale with hydrogen ion conversions
- [x] `ReactionRate.cs` - Chemical kinetics with rate laws and half-life
- [x] `RateConstant.cs` - Arrhenius equation with temperature dependence
- [x] `ActivationEnergy.cs` - Energy barriers with Arrhenius plot analysis
- [x] `EnzymeActivity.cs` - Biochemical catalysis with Michaelis-Menten kinetics
- [x] `SurfaceTension.cs` - Surface forces with Young-Laplace equation
- [x] `DynamicViscosity.cs` - Fluid resistance with Reynolds number calculations

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

## Acoustic Domain ✅
**Location:** `Quantities/Acoustic/`
**Status:** All 20 acoustic quantities implemented (100% complete)
**Reference:** See `Acoustic/README.md` for comprehensive implementation plan

### Implemented ✅

**Basic Acoustic Quantities (9/9):**
- [x] `Frequency.cs` - Hz, kHz, MHz with musical note calculations
- [x] `Wavelength.cs` - Wave spatial period with frequency relationships
- [x] `SoundPressure.cs` - Pa, μPa, RMS measurements
- [x] `SoundIntensity.cs` - W/m² units with inverse square law
- [x] `SoundPower.cs` - Total acoustic power in watts
- [x] `AcousticImpedance.cs` - Pa·s/m, rayl with medium properties
- [x] `SoundSpeed.cs` - Velocity of sound waves with temperature effects
- [x] `SoundAbsorption.cs` - Energy absorption coefficient (0-1)
- [x] `ReverberationTime.cs` - Decay time with Sabine formula

**Decibel Scale Quantities (3/3):**
- [x] `SoundPressureLevel.cs` - dB SPL scale with 20 μPa reference
- [x] `SoundIntensityLevel.cs` - dB IL scale with 10⁻¹² W/m² reference
- [x] `SoundPowerLevel.cs` - dB PWL scale with 10⁻¹² W reference

**Room Acoustics (3/3):**
- [x] `ReflectionCoefficient.cs` - Sound reflection ratio with impedance relationships
- [x] `NoiseReductionCoefficient.cs` - NRC with standard frequency averaging
- [x] `SoundTransmissionClass.cs` - STC ratings with building code compliance

**Psychoacoustics (3/3):**
- [x] `Loudness.cs` - sones/phons with Stevens' power law implementation
- [x] `Pitch.cs` - Hz, mels, barks with musical note conversions  
- [x] `Sharpness.cs` - acums with perceptual quality assessment

**Electroacoustics (2/2):**
- [x] `Sensitivity.cs` - dB SPL/W, mV/Pa with efficiency calculations
- [x] `DirectionalityIndex.cs` - dB with directivity patterns and beamwidth

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

### Physical Constants Management 🔧

**IMPORTANT:** All hard-coded physical constants should be centralized in `PhysicalConstants.cs` to ensure:
- **Consistency** - Single source of truth for all constant values
- **Accuracy** - Based on 2019 SI redefinition and CODATA 2018 values
- **Maintainability** - Easy updates when standards change
- **Documentation** - Clear references and uncertainty information

#### Current Hard-Coded Constants Found:
- **Avogadro's Number**: `6.02214076e23` (found in `AmountOfSubstance.cs`)
- **Gas Constant**: `8.314` (found in `RateConstant.cs`, `ActivationEnergy.cs`)
- **Absolute Zero**: `273.15` (found in `Temperature.cs`, `Units.cs`)
- **Water Ion Product**: `14.0` (found in `pH.cs`)
- **Molar Volume STP**: `22.414` (found in `AmountOfSubstance.cs`)
- **Natural Log of 2**: `0.693147` (found in `ReactionRate.cs`)
- **Elementary Charge**: `1.602176634e-19` (found in `Units.cs`)

#### Implementation Rules:
1. **Use PhysicalConstants.Generic methods** for type-safe constant access:
   ```csharp
   // ✅ CORRECT
   T avogadroNumber = PhysicalConstants.Generic.AvogadroNumber<T>();
   
   // ❌ AVOID
   T avogadroNumber = T.CreateChecked(6.02214076e23);
   ```

2. **Add new constants to PhysicalConstants.cs** before using them:
   - Organize by category (Fundamental, Chemical, Temperature, etc.)
   - Include full documentation with SI status and references
   - Provide both `const double` and generic `T` accessor methods

3. **Refactor existing hard-coded values** when implementing new domains:
   - Search for `T.CreateChecked([0-9]+\.[0-9]+)` patterns
   - Evaluate if the value is a physical constant vs. calculation result
   - Replace with appropriate `PhysicalConstants.Generic` calls

4. **Document constant sources**:
   - CODATA 2018 for fundamental constants
   - NIST for derived constants
   - ISO/IEC standards for unit conversions
   - Mark exact vs. measured values

#### Priority Refactoring Tasks:
- [ ] Replace Avogadro's number in `AmountOfSubstance.cs`
- [ ] Replace gas constant in `RateConstant.cs` and `ActivationEnergy.cs`
- [ ] Replace absolute zero in `Temperature.cs`
- [ ] Replace water ion product in `pH.cs`
- [ ] Replace molar volume STP in `AmountOfSubstance.cs`
- [ ] Replace ln(2) in `ReactionRate.cs`
- [ ] Review thermal domain conversion factors
- [ ] Review acoustic domain reference values

### Next Steps (Recommended Order)
1. **Implement Optical** - Basic photometric quantities (wavelength, illuminance, luminance)
2. **Implement Electromagnetics** - Magnetic fields, inductance, flux
3. **Implement Materials Science** - Stress/strain, elastic modulus
4. **Implement Fluid Dynamics** - Flow rate, viscosity, Reynolds number
5. **Expand Nuclear & Radiation** - Radioactivity, absorbed dose
6. **Consider specialized domains** - Astronomy, geophysics, quantum mechanics

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

## ✅ COMPLETED DOMAINS

### Chemical Domain (10 quantities) - COMPLETE ✅
**Status**: Fully implemented with physical constants integration and comprehensive testing
- **Quantities**: AmountOfSubstance, Concentration, MolarMass, pH, ActivationEnergy, RateConstant, ReactionRate, EnzymeActivity, DynamicViscosity, SurfaceTension
- **Physical Constants Applied**: All hard-coded values replaced with PhysicalConstants.Generic calls
- **Testing**: 376 tests passing, including unit tests for physical constants mathematical consistency
- **Infrastructure**: PhysicalConstants.cs created with organized constant categories

## 🔄 IMPLEMENTATION GUIDELINES

### Physical Constants Management
**CRITICAL**: Before implementing any new domain, follow these physical constants guidelines:

1. **Search for Hard-coded Values**: 
   - Scan all quantities, units, and dimensions for hard-coded numerical values
   - Common patterns: `6.02214076e23`, `8.314`, `9.80665`, `273.15`, etc.
   - Check mathematical operations: `Math.Log`, `Math.Exp`, `Math.Pow`, trigonometric functions

2. **Add Constants to PhysicalConstants.cs**:
   - **Fundamental**: Avogadro, gas constant, elementary charge, speed of light, Planck, Boltzmann
   - **Temperature**: Absolute zero, water triple point, standard temperature, boiling point  
   - **Chemical**: Water ion product, molar volume STP, Ln2, neutral pH
   - **Conversion**: Temperature, energy, length, pressure conversions
   - **Acoustic**: Reference sound pressure, intensity, power, Sabine constant
   - **Mathematical**: Ln10, Log10E, common logarithms

3. **Use PhysicalConstants.Generic Methods**:
   ```csharp
   // Instead of: 6.02214076e23
   PhysicalConstants.Generic.AvogadroNumber<T>()
   
   // Instead of: 8.314
   PhysicalConstants.Generic.GasConstant<T>()
   
   // Instead of: Math.Log(2.0)
   PhysicalConstants.Generic.Ln2<T>()
   ```

4. **Accuracy Standards**:
   - Use 2019 SI redefinition values (CODATA 2018)
   - Maintain full precision: 8.31446261815324 (not 8.314)
   - Document sources in comments
   - Create unit tests comparing derived vs. calculated values

### Implementation Process

1. **Create Quantities**:
   - Follow existing patterns in Chemical domain
   - Add proper XML documentation
   - Include mathematical relationships between quantities
   - Use generic type constraints: `where T : struct, INumber<T>`

2. **Add Dimensions** (if needed):
   - Add to `PhysicalDimensions.cs`
   - Use bootstrap units during initialization to avoid circular dependencies
   - Specify correct SI base unit

3. **Add Units** (if needed):
   - Add to `Units.cs`
   - Include conversion factors using PhysicalConstants
   - Follow naming conventions

4. **Apply Physical Constants**:
   - Replace all hard-coded values
   - Use `PhysicalConstants.Generic` methods for type safety
   - Add derived constants if needed

5. **Create Tests**:
   - Unit tests for each quantity
   - Physical constants accuracy tests
   - Mathematical relationship verification
   - Integration tests for cross-quantity calculations

### Code Quality Standards

- **File Headers**: All files must include copyright header
- **Null Checks**: Use `ArgumentNullException.ThrowIfNull()` for reference parameters
- **Mathematical Operations**: Use `T.CreateChecked()` pattern for type-safe conversions
- **Documentation**: Comprehensive XML comments for all public members
- **Naming**: Follow existing conventions (e.g., `PH<T>` not `pH<T>`)

## 📋 PENDING DOMAINS

### Mechanical Domain (15+ quantities)
- Force, Pressure, Energy, Power, Torque, Momentum, AngularVelocity, etc.
- **Constants to define**: Gravitational acceleration, atmospheric pressure, etc.

### Electromagnetic Domain (12+ quantities)  
- ElectricCurrent, Voltage, Resistance, Capacitance, Inductance, etc.
- **Constants to define**: Elementary charge, magnetic permeability, etc.

### Thermal Domain (8+ quantities)
- Temperature, HeatCapacity, ThermalConductivity, Entropy, etc.
- **Constants to define**: Stefan-Boltzmann constant, Boltzmann constant, etc.

### Optical Domain (10+ quantities)
- LuminousIntensity, Illuminance, LuminousFlux, etc.
- **Constants to define**: Luminous efficacy, photopic vision constants, etc.

### Nuclear Domain (8+ quantities)
- RadioactiveDecay, HalfLife, DoseEquivalent, etc.
- **Constants to define**: Nuclear decay constants, radiation constants, etc.

### Acoustic Domain (8+ quantities) 
- SoundPressure, SoundIntensity, SoundPower, etc.
- **Constants to define**: Reference sound pressure (20 μPa), etc.

### Fluid Dynamics Domain (10+ quantities)
- FlowRate, ReynoldsNumber, Viscosity, etc.
- **Constants to define**: Fluid property constants, etc.

## 🏗️ INFRASTRUCTURE STATUS

### ✅ Completed Infrastructure
- **PhysicalConstants.cs**: Comprehensive constants with Generic helpers
- **Bootstrap System**: Circular dependency resolution for Units/Dimensions
- **Test Framework**: Physical constants accuracy verification
- **Documentation**: Implementation guidelines and standards

### 🔄 Infrastructure Improvements Needed
- **Integration Tests**: Cross-domain quantity relationships
- **Performance**: Benchmark physical constants vs. hard-coded values
- **Validation**: Dimensional analysis verification system
- **Documentation**: Complete library guide with examples

## 📊 CURRENT METRICS
- **Total Tests**: 376 (100% passing)
- **Domains Complete**: 1/8 (Chemical)
- **Physical Constants**: 20+ fundamental constants defined
- **Code Coverage**: High (all critical paths tested)

## 🎯 NEXT PRIORITIES
1. **Mechanical Domain**: Most fundamental, builds foundation for others
2. **Electromagnetic Domain**: Independent, well-defined constants
3. **Thermal Domain**: Integrates with Chemical and Mechanical
4. **Integration Testing**: Cross-domain relationships and conversions

---
*Last Updated*: Chemical Domain completion with physical constants integration
*Test Status*: 376/376 tests passing ✅
