# Physics Relationship Examples

This document demonstrates how to use the physics relationship operators and methods in the ktsu.Semantics library.

## Force × Length Ambiguity Resolution

In physics, `Force × Length` can represent two different quantities:
- **Work/Energy**: `W = F·d` (force in direction of displacement)
- **Torque**: `τ = F×r` (force perpendicular to moment arm)

### Work/Energy Calculations (Use `*` Operator)
```csharp
using ktsu.Semantics;

// Create quantities
var force = Force<double>.FromNewtons(10.0);        // 10 N
var displacement = Length<double>.FromMeters(5.0);   // 5 m

// Calculate work/energy using operator (force parallel to displacement)
var workDone = force * displacement;                 // 50 J
var energy = displacement * force;                   // 50 J (commutative)

Console.WriteLine($"Work done: {workDone.In(Units.Joule)} J");
```

### Torque Calculations (Use `CalculateTorque()` Method)
```csharp
// Create quantities
var force = Force<double>.FromNewtons(10.0);        // 10 N
var momentArm = Length<double>.FromMeters(0.5);     // 0.5 m

// Calculate torque using explicit method (force perpendicular to moment arm)
var torque = force.CalculateTorque(momentArm);       // 5 N·m
var torque2 = Force<double>.CalculateTorque(force, momentArm); // Static method

Console.WriteLine($"Torque: {torque.In(Units.NewtonMeter)} N·m");
```

## Advanced Physics Relationships with Constants

### Kinetic Energy (KE = ½mv²)
```csharp
var mass = Mass<double>.FromKilograms(2.0);          // 2 kg
var velocity = Velocity<double>.FromMetersPerSecond(10.0); // 10 m/s

// Calculate kinetic energy using fractional constant
var kineticEnergy = Energy<double>.FromKineticEnergy(mass, velocity); // 100 J

// Calculate velocity from kinetic energy
var calculatedVelocity = Energy<double>.GetVelocityFromKineticEnergy(kineticEnergy, mass); // 10 m/s
```

### Ideal Gas Law (PV = nRT)
```csharp
var pressure = Pressure<double>.FromPascals(101325.0);    // 1 atm
var volume = Volume<double>.FromCubicMeters(0.0224);      // 22.4 L
var temperature = Temperature<double>.FromKelvin(273.15);  // 0°C

// Calculate amount of substance using gas constant
var moles = AmountOfSubstance<double>.FromIdealGasLaw(pressure, volume, temperature); // ~1 mol

// Calculate pressure from other quantities
var calculatedPressure = AmountOfSubstance<double>.CalculatePressureFromIdealGas(moles, temperature, volume);
```

### Photon Energy (E = hf)
```csharp
var frequency = Frequency<double>.FromHertz(5.0e14);      // Green light frequency

// Calculate photon energy using Planck's constant
var photonEnergy = Frequency<double>.GetPhotonEnergy(frequency);

// Calculate frequency from photon energy
var calculatedFrequency = Frequency<double>.FromPhotonEnergy(photonEnergy);
```

### Reynolds Number (Re = ρvL/μ)
```csharp
var density = Density<double>.FromKilogramsPerCubicMeter(1.225);        // Air density
var velocity = Velocity<double>.FromMetersPerSecond(10.0);              // 10 m/s
var length = Length<double>.FromMeters(1.0);                           // 1 m characteristic length
var viscosity = DynamicViscosity<double>.FromPascalSeconds(1.825e-5);   // Air viscosity

// Calculate Reynolds number
var reynolds = ReynoldsNumber<double>.FromFluidProperties(density, velocity, length, viscosity);

// Simplified calculation for standard air
var airReynolds = ReynoldsNumber<double>.ForStandardAir(velocity, length);

// Determine flow regime
var regime = reynolds.GetPipeFlowRegime(); // "Laminar", "Transitional", or "Turbulent"
```

### Acoustic Impedance (Z = ρc)
```csharp
var density = Density<double>.FromKilogramsPerCubicMeter(1.225);        // Air density
var soundSpeed = SoundSpeed<double>.FromMetersPerSecond(343.0);         // Sound speed in air

// Calculate acoustic impedance
var impedance = AcousticImpedance<double>.FromDensityAndSoundSpeed(density, soundSpeed);

// Standard air impedance
var standardImpedance = AcousticImpedance<double>.ForStandardAir();
```

## Basic Physics Relationships

### Newton's Laws
```csharp
var mass = Mass<double>.FromKilograms(10.0);                    // 10 kg
var acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8); // 9.8 m/s²

// F = ma
var force = mass * acceleration;                                // 98 N

// a = F/m
var calculatedAcceleration = force / mass;                      // 9.8 m/s²

// m = F/a
var calculatedMass = force / acceleration;                      // 10 kg
```

### Power Relationships
```csharp
var force = Force<double>.FromNewtons(100.0);                   // 100 N
var velocity = Velocity<double>.FromMetersPerSecond(5.0);       // 5 m/s
var time = Time<double>.FromSeconds(10.0);                      // 10 s

// P = F·v
var mechanicalPower = force * velocity;                         // 500 W

// E = P·t
var energy = mechanicalPower * time;                            // 5000 J
```

### Electrical Relationships
```csharp
var current = ElectricCurrent<double>.FromAmperes(2.0);         // 2 A
var resistance = ElectricResistance<double>.FromOhms(10.0);     // 10 Ω
var time = Time<double>.FromSeconds(60.0);                      // 1 minute

// V = I·R (Ohm's law)
var voltage = current * resistance;                             // 20 V

// P = V·I
var power = voltage * current;                                  // 40 W

// Q = I·t
var charge = current * time;                                    // 120 C
```

This approach provides:
- **Clear semantic distinction** between work and torque
- **Type safety** at compile time
- **Physics accuracy** in documentation and usage
- **No operator ambiguity** in C# 
