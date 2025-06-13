---
status: draft
---

# Complete Physics Domains Guide

This guide provides comprehensive coverage of all physics domains in the ktsu.Semantics library, with real-world examples and advanced usage patterns.

## Overview

The ktsu.Semantics library includes **80+ physics quantities** across **8 scientific domains**, each with:

- Type-safe arithmetic with dimensional analysis
- Automatic unit conversions 
- Physics relationships as operators
- Centralized physical constants
- Generic numeric type support (double, float, decimal)

## Domain Reference

### 🔧 Mechanics Domain (15 Quantities)

**Core Quantities**: Position, Velocity, Acceleration, Force, Pressure, Energy, Power, Momentum, Torque, Angular Velocity, Angular Acceleration, Moment of Inertia, Density, Mass, Time

#### Fundamental Relationships

```csharp
// Newton's Laws
var mass = Mass<double>.FromKilograms(10.0);
var acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8);
var force = mass * acceleration;  // F = ma (98 N)

// Work and Energy
var distance = Length<double>.FromMeters(5.0);
var work = force * distance;      // W = F·d (490 J)
var time = Time<double>.FromSeconds(2.0);
var power = work / time;          // P = W/t (245 W)

// Rotational Mechanics  
var torque = Torque<double>.FromNewtonMeters(50.0);
var angularVel = AngularVelocity<double>.FromRadiansPerSecond(10.0);
var rotationalPower = torque * angularVel;  // P = τω (500 W)
```

#### Real-World Examples

**Automotive Engine Analysis**
```csharp
// Engine specifications
var engineTorque = Torque<double>.FromNewtonMeters(300.0);    // 300 N·m peak torque
var rpm = 3000.0; // RPM
var angularVelocity = AngularVelocity<double>.FromRPM(rpm);

// Calculate power output
var enginePower = engineTorque * angularVelocity;            // ~94 kW
var horsePower = enginePower.ToHorsepower();                 // ~126 HP

// Vehicle dynamics
var vehicleMass = Mass<double>.FromKilograms(1500.0);        // 1.5 ton car
var acceleration = Acceleration<double>.FromMetersPerSecondSquared(3.0); // 0-100 km/h in ~9s
var drivingForce = vehicleMass * acceleration;               // 4.5 kN
```

**Structural Engineering**
```csharp
// Beam loading analysis
var beamLength = Length<double>.FromMeters(6.0);
var distributedLoad = Force<double>.FromNewtonsPerMeter(5000.0); // 5 kN/m
var totalLoad = distributedLoad * beamLength;                // 30 kN

// Material properties
var steelDensity = Density<double>.FromKilogramsPerCubicMeter(7850.0);
var beamVolume = Volume<double>.FromCubicMeters(0.02);       // 20L beam volume
var beamWeight = steelDensity * beamVolume * PhysicalConstants.Generic.StandardGravity<double>();
```

### ⚡ Electrical Domain (11 Quantities)

**Core Quantities**: Electric Potential, Electric Current, Electric Resistance, Electric Charge, Electric Power Density, Electric Field, Electric Flux, Electric Capacitance, Permittivity, Electric Conductivity, Impedance AC

#### Fundamental Relationships

```csharp
// Ohm's Law
var voltage = ElectricPotential<double>.FromVolts(12.0);
var resistance = ElectricResistance<double>.FromOhms(6.0);
var current = voltage / resistance;                          // I = V/R (2 A)

// Power relationships
var electricalPower = voltage * current;                     // P = VI (24 W)
var resistivePower = current.Squared() * resistance;         // P = I²R (24 W)

// Energy and charge
var time = Time<double>.FromHours(1.0);
var energy = electricalPower * time;                         // E = Pt (24 Wh)
var charge = current * time;                                 // Q = It (2 Ah)
```

#### Real-World Examples

**Solar Panel System Design**
```csharp
// Solar panel specifications
var panelVoltage = ElectricPotential<double>.FromVolts(36.0);     // Open circuit voltage
var panelCurrent = ElectricCurrent<double>.FromAmperes(8.5);      // Short circuit current
var maxPowerPoint = panelVoltage * panelCurrent * 0.78;          // ~238W (fill factor)

// System sizing
var dailyEnergyNeed = Energy<double>.FromKilowattHours(25.0);     // 25 kWh/day
var sunHours = Time<double>.FromHours(5.0);                      // 5 hours peak sun
var requiredPower = dailyEnergyNeed / sunHours;                  // 5 kW needed
var panelsNeeded = Math.Ceiling(requiredPower.Value / maxPowerPoint.Value); // 21 panels
```

**Motor Control Analysis**  
```csharp
// 3-phase motor analysis
var lineVoltage = ElectricPotential<double>.FromVolts(400.0);     // 400V 3-phase
var motorPower = Power<double>.FromKilowatts(15.0);              // 15 kW motor
var efficiency = 0.92;                                           // 92% efficient
var powerFactor = 0.85;                                          // 0.85 PF

var inputPower = Power<double>.Create(motorPower.Value / efficiency);      // 16.3 kW
var lineCurrent = inputPower / (lineVoltage * Math.Sqrt(3) * powerFactor); // ~28A per phase
```

### 🌡️ Thermal Domain (10 Quantities)

**Core Quantities**: Temperature, Heat, Entropy, Thermal Conductivity, Heat Capacity, Specific Heat, Heat Transfer Coefficient, Thermal Expansion, Thermal Diffusivity, Thermal Resistance

#### Fundamental Relationships

```csharp
// Heat transfer
var deltaT = Temperature<double>.FromKelvin(50.0);               // Temperature difference
var heatCapacity = HeatCapacity<double>.FromJoulesPerKelvin(1000.0); // Thermal mass
var heatRequired = Heat<double>.Create(heatCapacity.Value * deltaT.Value); // Q = CΔT

// Conduction
var thermalConductivity = ThermalConductivity<double>.FromWattsPerMeterKelvin(50.0);
var thickness = Length<double>.FromMeters(0.1);
var area = Area<double>.FromSquareMeters(2.0);
var heatTransferRate = (thermalConductivity * area * deltaT) / thickness; // Fourier's law
```

#### Real-World Examples

**HVAC System Design**
```csharp
// Building thermal analysis
var roomVolume = Volume<double>.FromCubicMeters(150.0);          // 150 m³ room
var airDensity = Density<double>.FromKilogramsPerCubicMeter(1.2); // Air density
var specificHeatAir = SpecificHeat<double>.FromJoulesPerKilogramKelvin(1005.0); // Cp air

var roomMass = roomVolume * airDensity;                          // Air mass in room
var indoorTemp = Temperature<double>.FromCelsius(22.0);          // Target temperature
var outdoorTemp = Temperature<double>.FromCelsius(35.0);         // Hot day
var tempDifference = outdoorTemp - indoorTemp;                   // 13°C difference

// Cooling load calculation
var coolingPower = Power<double>.Create(
    roomMass.Value * specificHeatAir.Value * tempDifference.Value / 3600.0); // W
```

**Industrial Heat Exchanger**
```csharp
// Heat exchanger effectiveness
var hotFluidTemp = Temperature<double>.FromCelsius(150.0);       // Hot fluid in
var coldFluidTemp = Temperature<double>.FromCelsius(20.0);       // Cold fluid in
var massFlowRate = MassFlowRate<double>.FromKilogramsPerSecond(2.0); // Fluid flow

var waterSpecificHeat = SpecificHeat<double>.FromJoulesPerKilogramKelvin(4186.0);
var maxHeatTransfer = massFlowRate * waterSpecificHeat * (hotFluidTemp - coldFluidTemp);

// With 80% effectiveness
var actualHeatTransfer = Heat<double>.Create(maxHeatTransfer.Value * 0.8);
var hotFluidOutTemp = Temperature<double>.Create(
    hotFluidTemp.Value - (actualHeatTransfer.Value / (massFlowRate.Value * waterSpecificHeat.Value)));
```

### 🧪 Chemical Domain (10 Quantities)

**Core Quantities**: Amount of Substance, Molarity, Molality, Molar Mass, Reaction Rate, Rate Constant, Activation Energy, pH, Solubility, Enzyme Activity

#### Fundamental Relationships

```csharp
// Concentration calculations
var moles = AmountOfSubstance<double>.FromMoles(0.5);
var solutionVolume = Volume<double>.FromLiters(2.0);
var molarity = moles / solutionVolume;                           // M = n/V (0.25 M)

// Reaction kinetics
var rateConstant = RateConstant<double>.FromPerSecond(0.1);      // 1st order reaction
var concentration = Molarity<double>.FromMolar(0.5);            // Initial concentration
var reactionRate = rateConstant * concentration;                // Rate = k[A]
```

#### Real-World Examples

**Pharmaceutical Manufacturing**
```csharp
// Drug synthesis calculation
var molecularWeight = MolarMass<double>.FromGramsPerMole(180.16); // Glucose MW
var targetYield = Mass<double>.FromGrams(50.0);                  // 50g target
var requiredMoles = AmountOfSubstance<double>.Create(targetYield.Value / molecularWeight.Value);

// Reaction stoichiometry (2:1 ratio)
var startingMaterial = AmountOfSubstance<double>.Create(requiredMoles.Value * 2.0);
var startingMass = Mass<double>.Create(startingMaterial.Value * 150.0); // SM MW = 150

// Process conditions
var reactionTemp = Temperature<double>.FromCelsius(80.0);
var activationEnergy = ActivationEnergy<double>.Create(75000.0); // 75 kJ/mol
var reactionRate = RateConstant<double>.FromArrheniusEquation(1e10, activationEnergy, reactionTemp);
```

**Water Treatment Analysis**
```csharp
// pH calculation and adjustment
var waterPH = pH<double>.FrompH(8.2);                           // Alkaline water
var targetPH = pH<double>.FrompH(7.0);                          // Neutral target
var pHDifference = waterPH - targetPH;                          // 1.2 pH units

// Buffer capacity
var waterVolume = Volume<double>.FromLiters(1000.0);            // 1000L tank
var bufferCapacity = HeatCapacity<double>.FromJoulesPerKelvin(50.0); // Buffer strength
var acidNeeded = AmountOfSubstance<double>.Create(
    Math.Pow(10, -targetPH.Value) * waterVolume.Value);         // Simplified calculation
```

### 🔊 Acoustic Domain (20 Quantities)

**Core Quantities**: Frequency, Wavelength, Sound Pressure, Sound Intensity, Sound Power, Acoustic Impedance, Sound Speed, Sound Absorption, Reverberation Time, plus 11 specialized acoustic metrics

#### Fundamental Relationships

```csharp
// Wave relationships
var frequency = Frequency<double>.FromHertz(1000.0);             // 1 kHz tone
var soundSpeed = SoundSpeed<double>.FromMetersPerSecond(343.0);  // Speed in air
var wavelength = soundSpeed / frequency;                         // λ = v/f (0.343 m)

// Acoustic power and intensity
var soundPower = SoundPower<double>.FromWatts(0.001);           // 1 mW source
var sphericalArea = Area<double>.FromSquareMeters(4 * Math.PI * 100); // 10m radius sphere
var intensity = soundPower / sphericalArea;                      // I = P/A
```

#### Real-World Examples

**Audio System Design**
```csharp
// Speaker system analysis
var speakerPower = Power<double>.FromWatts(100.0);              // 100W speaker
var efficiency = 0.02;                                          // 2% efficiency (typical)
var acousticPower = SoundPower<double>.Create(speakerPower.Value * efficiency); // 2W acoustic

// Room acoustics
var roomVolume = Volume<double>.FromCubicMeters(200.0);         // 200 m³ room
var absorptionCoeff = SoundAbsorption<double>.Create(0.3);      // 30% absorption
var revTime = ReverbertainTime<double>.FromSabineEquation(roomVolume, absorptionCoeff);

// Sound level at distance
var distance = Length<double>.FromMeters(3.0);                 // 3m from speaker
var sphericalSurface = Area<double>.Create(4 * Math.PI * Math.Pow(distance.Value, 2));
var soundIntensity = acousticPower / sphericalSurface;
var soundPressureLevel = SoundPressureLevel<double>.FromIntensity(soundIntensity);
```

**Noise Control Engineering**
```csharp
// Industrial noise assessment
var machinePower = SoundPower<double>.FromWatts(0.01);          // 10 mW industrial machine
var workerDistance = Length<double>.FromMeters(2.0);            // 2m from machine
var roomConstant = 50.0; // Room acoustic constant

// Near field vs far field
var nearFieldIntensity = machinePower / (Area<double>.Create(4 * Math.PI * Math.Pow(workerDistance.Value, 2)));
var reverberantIntensity = SoundIntensity<double>.Create(4 * machinePower.Value / roomConstant);
var totalIntensity = SoundIntensity<double>.Create(nearFieldIntensity.Value + reverberantIntensity.Value);

var exposureLevel = SoundPressureLevel<double>.FromIntensity(totalIntensity);
var safetyMargin = 85.0 - exposureLevel.Value; // OSHA 85 dB limit
```

### ☢️ Nuclear Domain (5 Quantities)

**Core Quantities**: Radioactive Activity, Absorbed Dose, Equivalent Dose, Exposure, Nuclear Cross Section

#### Real-World Examples

**Medical Radiotherapy**
```csharp
// Radiation treatment planning
var sourceActivity = RadioactiveActivity<double>.FromBecquerels(3.7e10); // 1 Ci Cs-137 source
var treatmentTime = Time<double>.FromMinutes(10.0);             // 10 minute treatment
var patientDistance = Length<double>.FromMeters(1.0);           // 1m from source

// Dose calculation (simplified)
var exposureRate = Exposure<double>.Create(sourceActivity.Value / (4 * Math.PI * Math.Pow(patientDistance.Value, 2)));
var totalExposure = Exposure<double>.Create(exposureRate.Value * treatmentTime.Value);
var absorbedDose = AbsorbedDose<double>.FromExposure(totalExposure);

// Safety verification
var doseLimit = EquivalentDose<double>.FromSieverts(0.02);      // 20 mSv annual limit
var equivalentDose = EquivalentDose<double>.Create(absorbedDose.Value * 1.0); // Quality factor = 1 for gamma
var safetyFactor = doseLimit.Value / equivalentDose.Value;
```

### 💡 Optical Domain (6 Quantities)

**Core Quantities**: Luminous Intensity, Luminous Flux, Illuminance, Luminance, Refractive Index, Optical Power

#### Real-World Examples

**Lighting Design**
```csharp
// LED lighting calculation
var ledFlux = LuminousFlux<double>.FromLumens(3000.0);          // 3000 lm LED array
var roomArea = Area<double>.FromSquareMeters(25.0);             // 25 m² room
var averageIlluminance = ledFlux / roomArea;                    // 120 lux average

// Task lighting requirements
var requiredIlluminance = Illuminance<double>.FromLux(500.0);   // 500 lux for reading
var additionalFlux = LuminousFlux<double>.Create(
    (requiredIlluminance.Value - averageIlluminance.Value) * roomArea.Value); // Additional 9500 lm needed
```

### 🌊 Fluid Dynamics Domain (5 Quantities)

**Core Quantities**: Kinematic Viscosity, Bulk Modulus, Volumetric Flow Rate, Mass Flow Rate, Reynolds Number

#### Real-World Examples

**Pipeline Design**
```csharp
// Water pipeline analysis
var pipeFlow = VolumetricFlowRate<double>.FromLitersPerSecond(50.0); // 50 L/s flow
var pipeDiameter = Length<double>.FromMeters(0.2);               // 20 cm diameter
var waterViscosity = KinematicViscosity<double>.ForWater();      // Water at 20°C

var velocity = Velocity<double>.Create(pipeFlow.Value / (Math.PI * Math.Pow(pipeDiameter.Value/2, 2)));
var reynolds = ReynoldsNumber<double>.Calculate(velocity, pipeDiameter, waterViscosity);
var flowRegime = reynolds.GetPipeFlowRegime();                   // "Turbulent" expected

// Pressure drop calculation
var frictionFactor = reynolds.CalculateDarcyFriction();
var pipeLength = Length<double>.FromMeters(1000.0);             // 1 km pipeline
var pressureDrop = Pressure<double>.Create(
    frictionFactor * (pipeLength.Value / pipeDiameter.Value) * 
    (waterDensity.Value * Math.Pow(velocity.Value, 2) / 2));
```

## Advanced Integration Patterns

### Multi-Domain Calculations

**Complete System Analysis**
```csharp
// Hydroelectric power plant analysis
public class HydroPlantAnalysis
{
    public Power<double> CalculatePowerOutput(
        VolumetricFlowRate<double> waterFlow,
        Length<double> headHeight,
        double turbineEfficiency = 0.9,
        double generatorEfficiency = 0.95)
    {
        // Mechanical power from water
        var waterDensity = Density<double>.FromKilogramsPerCubicMeter(1000.0);
        var gravity = PhysicalConstants.Generic.StandardGravity<double>();
        
        var massFlow = waterDensity * waterFlow;
        var hydraulicPower = Power<double>.Create(
            massFlow.Value * gravity * headHeight.Value);
        
        // Electrical power output
        var mechanicalPower = Power<double>.Create(hydraulicPower.Value * turbineEfficiency);
        var electricalPower = Power<double>.Create(mechanicalPower.Value * generatorEfficiency);
        
        return electricalPower;
    }
}

// Usage
var plant = new HydroPlantAnalysis();
var flow = VolumetricFlowRate<double>.FromCubicMetersPerSecond(100.0);  // 100 m³/s
var head = Length<double>.FromMeters(50.0);                            // 50 m head
var output = plant.CalculatePowerOutput(flow, head);                    // ~41.6 MW
```

### Performance Optimization Patterns

**Batch Processing**
```csharp
// Process multiple quantities efficiently
public static List<Energy<double>> CalculateKineticEnergies(
    List<(double mass, double velocity)> objects)
{
    return objects.Select(obj => {
        var mass = Mass<double>.FromKilograms(obj.mass);
        var velocity = Velocity<double>.FromMetersPerSecond(obj.velocity);
        return Energy<double>.FromKineticEnergy(mass, velocity);
    }).ToList();
}
```

### Unit Testing Patterns

**Physics Relationship Verification**
```csharp
[TestMethod]
public void VerifyEnergyConservation()
{
    // Initial kinetic energy
    var mass = Mass<double>.FromKilograms(10.0);
    var velocity = Velocity<double>.FromMetersPerSecond(20.0);
    var kineticEnergy = Energy<double>.FromKineticEnergy(mass, velocity);
    
    // Work done by friction
    var frictionForce = Force<double>.FromNewtons(50.0);
    var distance = Length<double>.FromMeters(30.0);
    var workDone = frictionForce * distance;
    
    // Final energy should equal initial minus work
    var finalEnergy = kineticEnergy - workDone;
    var expectedFinalVelocity = Math.Sqrt(2 * finalEnergy.Value / mass.Value);
    
    Assert.AreEqual(10.0, expectedFinalVelocity, 0.01, "Energy conservation violated");
}
```

## Best Practices

### 1. Use Type-Safe Constants
```csharp
// Good: Type-safe constant access
var gravity = PhysicalConstants.Generic.StandardGravity<double>();

// Bad: Hard-coded values
var gravity = 9.80665; // No type safety, source unclear
```

### 2. Leverage Dimensional Analysis
```csharp
// The compiler prevents dimensional errors
var force = mass * acceleration;        // ✅ Dimensionally correct
// var invalid = mass + acceleration;   // ❌ Compiler error
```

### 3. Use Appropriate Numeric Types
```csharp
// High precision calculations
var precise = Temperature<decimal>.FromCelsius(25.000001m);

// Performance-critical code
var fast = Temperature<float>.FromCelsius(25.0f);

// General purpose
var general = Temperature<double>.FromCelsius(25.0);
```

### 4. Handle Unit Conversions Explicitly
```csharp
// Clear intent with explicit conversions
var tempF = temperature.ToFahrenheit();
var energyKwh = energy.ToKilowattHours();

// Use In() method for custom units
var pressureBar = pressure.In(Units.Bar);
```

## Migration Guide

### From Other Libraries

**From UnitsNet**
```csharp
// UnitsNet
var length = Length.FromMeters(5.0);
var area = length * length;

// ktsu.Semantics
var length = Length<double>.FromMeters(5.0);
var area = length * length;  // Returns Area<double>
```

**From Quantities**
```csharp
// Quantities
var force = new Force(100, Unit.Newton);

// ktsu.Semantics  
var force = Force<double>.FromNewtons(100.0);
```

This comprehensive guide demonstrates the full capabilities of the ktsu.Semantics physics domains, providing the foundation for scientific computing, engineering applications, and educational tools. 
