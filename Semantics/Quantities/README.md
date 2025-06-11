# Physical Quantities Organization

This directory contains physical quantities organized by physics domains for better maintainability and logical grouping.

## Directory Structure

### 🔧 **Mechanics** (14 files)
Fundamental mechanics and motion-related quantities:
- `Acceleration.cs` - Linear acceleration (m/s²)
- `Area.cs` - Two-dimensional space (m²)
- `Density.cs` - Mass per unit volume (kg/m³)
- `Energy.cs` - Work and energy (J)
- `Force.cs` - Push or pull interactions (N)
- `Jerk.cs` - Rate of change of acceleration (m/s³)
- `Length.cs` - One-dimensional distance (m)
- `Mass.cs` - Amount of matter (kg)
- `Momentum.cs` - Mass times velocity (kg⋅m/s)
- `Power.cs` - Rate of energy transfer (W)
- `Pressure.cs` - Force per unit area (Pa)
- `Time.cs` - Duration (s)
- `Velocity.cs` - Rate of change of position (m/s)
- `Volume.cs` - Three-dimensional space (m³)

### ⚡ **Electromagnetism** (4 files)
Electrical and magnetic quantities:
- `Charge.cs` - Electric charge (C)
- `ElectricCurrent.cs` - Flow of electric charge (A)
- `ElectricPotential.cs` - Electric potential difference (V)
- `Resistance.cs` - Opposition to current flow (Ω)

### 🧪 **Chemistry** (2 files)
Chemical quantities and stoichiometry:
- `AmountOfSubstance.cs` - Number of particles (mol)
- `MolarMass.cs` - Mass per mole (kg/mol)

### 💡 **Photometry** (4 files)
Light measurement quantities:
- `Illuminance.cs` - Luminous flux per unit area (lx)
- `LuminousFlux.cs` - Luminous power (lm)
- `LuminousIntensity.cs` - Luminous flux per solid angle (cd)
- `SolidAngle.cs` - Three-dimensional angle (sr)

### 🔄 **RotationalMechanics** (6 files)
Angular and rotational quantities:
- `Angle.cs` - Angular displacement (rad)
- `AngularAcceleration.cs` - Rate of change of angular velocity (rad/s²)
- `AngularMomentum.cs` - Rotational momentum (kg⋅m²/s)
- `AngularVelocity.cs` - Rate of change of angle (rad/s)
- `MomentOfInertia.cs` - Rotational inertia (kg⋅m²)
- `Torque.cs` - Rotational force (N⋅m)

### 🌡️ **Thermodynamics** (1 file)
Heat and temperature quantities:
- `Temperature.cs` - Thermal energy measure (K)

### 📐 **Base Class** (1 file)
- `SemanticQuantity.cs` - Base class for all physical quantities

## Relationships Between Domains

The quantities across domains are interconnected through dimensional relationships:

- **Mechanics ↔ Electromagnetism**: Power = Voltage × Current
- **Mechanics ↔ Chemistry**: Mass ↔ MolarMass × AmountOfSubstance  
- **Mechanics ↔ RotationalMechanics**: Energy ↔ AngularMomentum × AngularVelocity
- **Mechanics ↔ Photometry**: Power ↔ LuminousFlux (in radiometry)
- **RotationalMechanics**: Complete angular motion calculus chain (θ ↔ ω ↔ α)

## Design Principles

1. **Domain Separation**: Each physics domain is isolated for better code organization
2. **Relationship Preservation**: Cross-domain operations maintain dimensional correctness
3. **SI Units**: All base units follow the International System of Units
4. **Type Safety**: Strong typing prevents dimensional analysis errors
5. **Extensibility**: New quantities can be added to appropriate domains

## Usage

All quantities inherit from the base `SemanticQuantity` class and implement appropriate operators for dimensional analysis and unit conversions.

```csharp
// Mechanics domain
Length distance = 100.Meters();
Time time = 10.Seconds();
Velocity speed = distance / time; // 10 m/s

// Electromagnetism domain  
ElectricCurrent current = 2.Amperes();
Resistance resistance = 10.Ohms();
ElectricPotential voltage = current * resistance; // 20 V

// Cross-domain operations
Power electricalPower = voltage * current; // 40 W
``` 
