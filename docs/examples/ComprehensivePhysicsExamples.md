# Comprehensive Physics Examples

This document demonstrates real-world applications using quantities from all 8 physics domains in the ktsu.Semantics library.

## Table of Contents
- [1. Engineering Design Examples](#1-engineering-design-examples)
- [2. Scientific Research Applications](#2-scientific-research-applications)
- [3. Industrial Process Monitoring](#3-industrial-process-monitoring)
- [4. Environmental Monitoring](#4-environmental-monitoring)
- [5. Advanced Multi-Domain Calculations](#5-advanced-multi-domain-calculations)

## 1. Engineering Design Examples

### Electric Vehicle Motor Analysis
Combining **Mechanical**, **Electrical**, and **Thermal** domains:

```csharp
using ktsu.Semantics;

public class EVMotorAnalysis
{
    public static void AnalyzeMotorPerformance()
    {
        // Motor specifications (Electrical domain)
        var batteryVoltage = ElectricPotential<double>.FromVolts(400.0);    // 400V battery pack
        var motorCurrent = ElectricCurrent<double>.FromAmperes(300.0);      // 300A peak current
        var motorEfficiency = 0.95;                                        // 95% efficiency
        
        // Calculate electrical power
        var electricalPower = batteryVoltage * motorCurrent;                // P = V × I = 120 kW
        
        // Mechanical output (Mechanical domain)
        var mechanicalPower = Power<double>.Create(electricalPower.Value * motorEfficiency); // 114 kW
        var wheelRadius = Length<double>.FromMeters(0.35);                  // 35cm wheel radius
        var gearRatio = 10.0;                                              // 10:1 reduction
        
        // Calculate torque and speed
        var motorRPM = 8000.0;                                             // 8000 RPM
        var motorSpeed = AngularVelocity<double>.FromRPM(motorRPM);
        var motorTorque = Torque<double>.FromPowerAndAngularVelocity(mechanicalPower, motorSpeed);
        
        // Wheel characteristics
        var wheelTorque = Torque<double>.Create(motorTorque.Value * gearRatio);
        var wheelSpeed = AngularVelocity<double>.Create(motorSpeed.Value / gearRatio);
        var vehicleSpeed = Velocity<double>.Create(wheelSpeed.Value * wheelRadius.Value);
        
        // Thermal analysis (Thermal domain)
        var powerLoss = Power<double>.Create(electricalPower.Value * (1 - motorEfficiency)); // 6 kW loss
        var motorMass = Mass<double>.FromKilograms(50.0);                   // 50 kg motor
        var specificHeat = SpecificHeat<double>.FromJoulesPerKilogramKelvin(900.0); // Aluminum
        var operatingTime = Time<double>.FromMinutes(10.0);                 // 10 minutes
        
        // Temperature rise calculation
        var energyLoss = Energy<double>.Create(powerLoss.Value * operatingTime.Value);
        var tempRise = Temperature<double>.Create(energyLoss.Value / (motorMass.Value * specificHeat.Value));
        
        Console.WriteLine($"Motor Performance Analysis:");
        Console.WriteLine($"  Electrical Power: {electricalPower.In(Units.Kilowatt):F1} kW");
        Console.WriteLine($"  Mechanical Power: {mechanicalPower.In(Units.Kilowatt):F1} kW");
        Console.WriteLine($"  Motor Torque: {motorTorque.In(Units.NewtonMeter):F0} N⋅m");
        Console.WriteLine($"  Vehicle Speed: {vehicleSpeed.In(Units.KilometersPerHour):F1} km/h");
        Console.WriteLine($"  Temperature Rise: {tempRise.Value:F1} K");
    }
}
```

### Optical Fiber Communication System
Combining **Optical**, **Electrical**, and **Thermal** domains:

```csharp
public class OpticalFiberSystem
{
    public static void AnalyzeFiberTransmission()
    {
        // Optical domain - Laser diode characteristics
        var laserWavelength = 1550e-9; // 1550 nm (C-band)
        var frequency = Frequency<double>.FromSpeedAndWavelength(
            PhysicalConstants.Generic.SpeedOfLight<double>(), 
            laserWavelength);
            
        var laserPower = Power<double>.FromMilliwatts(10.0);                // 10 mW optical power
        var fiberLength = Length<double>.FromKilometers(100.0);             // 100 km fiber
        var attenuationCoeff = 0.2; // 0.2 dB/km at 1550 nm
        
        // Calculate optical power after fiber transmission
        var totalAttenuation = attenuationCoeff * fiberLength.In(Units.Kilometer);
        var receivedPowerDbm = 10 * Math.Log10(laserPower.In(Units.Milliwatt)) - totalAttenuation;
        var receivedPower = Power<double>.FromMilliwatts(Math.Pow(10, receivedPowerDbm / 10));
        
        // Electrical domain - Photodiode characteristics
        var responsivity = 1.0; // 1.0 A/W responsivity
        var photoCurrent = ElectricCurrent<double>.FromAmperes(receivedPower.In(Units.Watt) * responsivity);
        var loadResistance = ElectricResistance<double>.FromOhms(50.0);     // 50Ω load
        var signalVoltage = photoCurrent * loadResistance;                  // V = I × R
        
        // Thermal domain - Laser thermal management
        var laserEfficiency = 0.3; // 30% wall-plug efficiency
        var electricalInput = Power<double>.Create(laserPower.Value / laserEfficiency);
        var heatGenerated = Power<double>.Create(electricalInput.Value - laserPower.Value);
        
        var ambientTemp = Temperature<double>.FromCelsius(25.0);
        var thermalResistance = ThermalResistance<double>.FromKelvinPerWatt(10.0); // 10 K/W
        var junctionTemp = Temperature<double>.Create(
            ambientTemp.Value + heatGenerated.Value * thermalResistance.Value);
        
        Console.WriteLine($"Optical Fiber Analysis:");
        Console.WriteLine($"  Transmitted Power: {laserPower.In(Units.Milliwatt):F1} mW");
        Console.WriteLine($"  Received Power: {receivedPower.In(Units.Microwatt):F1} μW");
        Console.WriteLine($"  Photo Current: {photoCurrent.In(Units.Microampere):F1} μA");
        Console.WriteLine($"  Signal Voltage: {signalVoltage.In(Units.Millivolt):F1} mV");
        Console.WriteLine($"  Laser Junction Temp: {junctionTemp.InCelsius():F1} °C");
    }
}
```

## 2. Scientific Research Applications

### Nuclear Reactor Physics
Combining **Nuclear**, **Thermal**, and **Fluid Dynamics** domains:

```csharp
public class ReactorPhysics
{
    public static void AnalyzeReactorCore()
    {
        // Nuclear domain - Fission characteristics
        var fissionRate = RadioactiveActivity<double>.FromReactionsPerSecond(1e17); // 10^17 fissions/s
        var energyPerFission = Energy<double>.FromMeV(200.0);                       // 200 MeV per fission
        var thermalPower = Power<double>.Create(fissionRate.Value * energyPerFission.Value);
        
        // Neutron flux and cross-sections
        var neutronFlux = 1e14; // neutrons/(cm²·s)
        var fissionCrossSection = NuclearCrossSection<double>.FromBarns(582.0);     // U-235 thermal fission
        var absorptionCrossSection = NuclearCrossSection<double>.FromBarns(99.0);   // U-238 absorption
        
        // Thermal domain - Heat removal
        var coolantInletTemp = Temperature<double>.FromCelsius(290.0);              // PWR inlet temp
        var coolantOutletTemp = Temperature<double>.FromCelsius(325.0);             // PWR outlet temp
        var tempRise = coolantOutletTemp - coolantInletTemp;
        
        var waterSpecificHeat = SpecificHeat<double>.FromJoulesPerKilogramKelvin(4186.0);
        var requiredMassFlowRate = MassFlowRate<double>.Create(
            thermalPower.Value / (waterSpecificHeat.Value * tempRise.Value));
        
        // Fluid dynamics domain - Coolant flow
        var pipeRadius = Length<double>.FromCentimeters(15.0);                      // 30 cm diameter
        var pipeArea = Area<double>.Create(Math.PI * Math.Pow(pipeRadius.Value, 2));
        var waterDensity = Density<double>.FromKilogramsPerCubicMeter(750.0);       // Hot water density
        
        var volumetricFlowRate = VolumetricFlowRate<double>.Create(
            requiredMassFlowRate.Value / waterDensity.Value);
        var flowVelocity = Velocity<double>.Create(volumetricFlowRate.Value / pipeArea.Value);
        
        // Reynolds number for flow characterization
        var waterViscosity = DynamicViscosity<double>.FromPascalSeconds(0.0003);    // Hot water viscosity
        var reynolds = ReynoldsNumber<double>.FromFluidProperties(
            waterDensity, flowVelocity, Length<double>.Create(2 * pipeRadius.Value), waterViscosity);
        
        Console.WriteLine($"Nuclear Reactor Analysis:");
        Console.WriteLine($"  Thermal Power: {thermalPower.In(Units.Megawatt):F0} MW");
        Console.WriteLine($"  Required Flow Rate: {requiredMassFlowRate.Value:F0} kg/s");
        Console.WriteLine($"  Coolant Velocity: {flowVelocity.In(Units.MetersPerSecond):F1} m/s");
        Console.WriteLine($"  Reynolds Number: {reynolds.Value:E2}");
        Console.WriteLine($"  Flow Regime: {(reynolds.Value > 4000 ? "Turbulent" : "Laminar")}");
    }
}
```

### Acoustic Research Lab
Combining **Acoustic**, **Mechanical**, and **Electrical** domains:

```csharp
public class AcousticLab
{
    public static void AnalyzeSoundMeasurement()
    {
        // Acoustic domain - Sound measurement
        var soundPressure = SoundPressure<double>.FromPascals(0.1);                 // 0.1 Pa RMS
        var frequency = Frequency<double>.FromHertz(1000.0);                        // 1 kHz tone
        var measurementDistance = Length<double>.FromMeters(1.0);                   // 1 meter distance
        
        // Calculate sound pressure level
        var spl = SoundPressureLevel<double>.FromSoundPressure(soundPressure);      // dB SPL
        
        // Air properties for acoustic calculations
        var airDensity = Density<double>.FromKilogramsPerCubicMeter(1.225);         // Standard air
        var soundSpeed = SoundSpeed<double>.FromMetersPerSecond(343.0);             // Sound speed in air
        var acousticImpedance = AcousticImpedance<double>.FromDensityAndSoundSpeed(airDensity, soundSpeed);
        
        // Calculate acoustic intensity and power
        var soundIntensity = SoundIntensity<double>.FromPressureAndImpedance(soundPressure, acousticImpedance);
        var measurementArea = Area<double>.Create(4 * Math.PI * Math.Pow(measurementDistance.Value, 2)); // Sphere
        var soundPower = SoundPower<double>.FromIntensityAndArea(soundIntensity, measurementArea);
        
        // Wavelength and frequency analysis
        var wavelength = Wavelength<double>.FromSpeedAndFrequency(soundSpeed, frequency);
        
        // Mechanical domain - Vibration source
        var speakerMass = Mass<double>.FromGrams(50.0);                             // 50g speaker cone
        var displacement = Length<double>.FromMillimeters(1.0);                     // 1mm peak displacement
        var angularFreq = 2 * Math.PI * frequency.Value;
        var peakVelocity = Velocity<double>.Create(angularFreq * displacement.Value);
        var peakAcceleration = Acceleration<double>.Create(angularFreq * peakVelocity.Value);
        
        // Force and power requirements
        var mechanicalForce = Force<double>.Create(speakerMass.Value * peakAcceleration.Value);
        var mechanicalPower = Power<double>.Create(mechanicalForce.Value * peakVelocity.Value / 2); // RMS
        
        // Electrical domain - Amplifier requirements
        var speakerImpedance = ElectricResistance<double>.FromOhms(8.0);            // 8Ω speaker
        var efficiency = 0.05; // 5% electro-acoustic efficiency
        var electricalPower = Power<double>.Create(mechanicalPower.Value / efficiency);
        var driveCurrent = ElectricCurrent<double>.Create(Math.Sqrt(electricalPower.Value / speakerImpedance.Value));
        var driveVoltage = driveCurrent * speakerImpedance;
        
        Console.WriteLine($"Acoustic Laboratory Analysis:");
        Console.WriteLine($"  Sound Pressure Level: {spl.Value:F1} dB SPL");
        Console.WriteLine($"  Sound Intensity: {soundIntensity.Value:E2} W/m²");
        Console.WriteLine($"  Acoustic Power: {soundPower.In(Units.Milliwatt):F2} mW");
        Console.WriteLine($"  Wavelength: {wavelength.In(Units.Centimeter):F1} cm");
        Console.WriteLine($"  Peak Acceleration: {peakAcceleration.Value:F1} m/s²");
        Console.WriteLine($"  Drive Voltage: {driveVoltage.In(Units.Volt):F1} V RMS");
        Console.WriteLine($"  Drive Current: {driveCurrent.In(Units.Ampere):F2} A RMS");
    }
}
```

## 3. Industrial Process Monitoring

### Chemical Reactor Process Control
Combining **Chemical**, **Thermal**, and **Fluid Dynamics** domains:

```csharp
public class ChemicalProcess
{
    public static void MonitorReactorConditions()
    {
        // Chemical domain - Reaction kinetics
        var reactantConcentration = Concentration<double>.FromMolar(2.0);           // 2 M initial concentration
        var reactionTemp = Temperature<double>.FromCelsius(80.0);                   // 80°C reaction temperature
        var activationEnergy = ActivationEnergy<double>.FromJoulesPerMole(50000.0); // 50 kJ/mol
        
        // Calculate reaction rate constant using Arrhenius equation
        var preExponentialFactor = 1e12; // s⁻¹
        var rateConstant = RateConstant<double>.FromArrheniusEquation(
            preExponentialFactor, activationEnergy, reactionTemp);
        
        // First-order reaction rate
        var reactionRate = ReactionRate<double>.FromFirstOrderKinetics(rateConstant, reactantConcentration);
        
        // Enzyme activity for bioreactor
        var enzymeActivity = EnzymeActivity<double>.FromKatalPerLiter(0.5);          // 0.5 kat/L
        var substrateAmount = AmountOfSubstance<double>.FromMoles(10.0);             // 10 mol substrate
        
        // Thermal domain - Heat management
        var reactionHeat = Energy<double>.FromKilojoules(150.0);                     // 150 kJ reaction enthalpy
        var reactorMass = Mass<double>.FromKilograms(500.0);                        // 500 kg reactor contents
        var specificHeat = SpecificHeat<double>.FromJoulesPerKilogramKelvin(3500.0); // Solution specific heat
        
        var adiabaticTempRise = Temperature<double>.Create(
            reactionHeat.Value / (reactorMass.Value * specificHeat.Value));
        
        // Heat transfer coefficient for cooling
        var heatTransferCoeff = HeatTransferCoefficient<double>.FromWattsPerSquareMeterKelvin(1000.0);
        var coolingArea = Area<double>.FromSquareMeters(20.0);                      // 20 m² cooling surface
        var coolantTemp = Temperature<double>.FromCelsius(20.0);                    // 20°C coolant
        var tempDifference = reactionTemp - coolantTemp;
        
        var coolingPower = Power<double>.Create(
            heatTransferCoeff.Value * coolingArea.Value * tempDifference.Value);
        
        // Fluid dynamics domain - Mixing and flow
        var stirrerSpeed = AngularVelocity<double>.FromRPM(200.0);                  // 200 RPM stirrer
        var impellerDiameter = Length<double>.FromCentimeters(30.0);                // 30 cm impeller
        var fluidViscosity = DynamicViscosity<double>.FromPascalSeconds(0.001);     // Water-like viscosity
        var fluidDensity = Density<double>.FromKilogramsPerCubicMeter(1100.0);      // Solution density
        
        // Power number calculation for mixing
        var tipSpeed = Velocity<double>.Create(stirrerSpeed.Value * impellerDiameter.Value);
        var reynoldsNumber = ReynoldsNumber<double>.FromFluidProperties(
            fluidDensity, tipSpeed, impellerDiameter, fluidViscosity);
        
        // Estimate mixing power (simplified)
        var powerNumber = 1.5; // Typical for turbulent mixing
        var mixingPower = Power<double>.Create(
            powerNumber * fluidDensity.Value * Math.Pow(stirrerSpeed.Value, 3) * Math.Pow(impellerDiameter.Value, 5));
        
        Console.WriteLine($"Chemical Process Monitoring:");
        Console.WriteLine($"  Reaction Rate: {reactionRate.Value:E2} mol/(L⋅s)");
        Console.WriteLine($"  Rate Constant: {rateConstant.Value:E2} s⁻¹");
        Console.WriteLine($"  Adiabatic Temp Rise: {adiabaticTempRise.Value:F1} K");
        Console.WriteLine($"  Cooling Power Required: {coolingPower.In(Units.Kilowatt):F1} kW");
        Console.WriteLine($"  Mixing Reynolds Number: {reynoldsNumber.Value:E2}");
        Console.WriteLine($"  Mixing Power: {mixingPower.In(Units.Kilowatt):F2} kW");
    }
}
```

## 4. Environmental Monitoring

### Atmospheric Pollution Analysis
Combining **Chemical**, **Fluid Dynamics**, and **Thermal** domains:

```csharp
public class EnvironmentalMonitoring
{
    public static void AnalyzeAirQuality()
    {
        // Chemical domain - Pollutant concentrations
        var no2Concentration = Concentration<double>.FromPartsPerMillion(0.05);     // 50 ppb NO₂
        var so2Concentration = Concentration<double>.FromMilligramsPerCubicMeter(10.0); // 10 mg/m³ SO₂
        var coConcentration = Concentration<double>.FromPartsPerMillion(1.0);       // 1 ppm CO
        
        // Convert PPM to molar concentrations at STP
        var airDensity = Density<double>.FromKilogramsPerCubicMeter(1.225);         // Standard air density
        var standardTemp = Temperature<double>.FromCelsius(25.0);
        var standardPressure = Pressure<double>.FromPascals(101325.0);
        
        // Molar volume at standard conditions
        var gasConstant = PhysicalConstants.Generic.GasConstant<double>();
        var molarVolume = Volume<double>.Create(gasConstant * standardTemp.Value / standardPressure.Value);
        
        // Fluid dynamics domain - Atmospheric dispersion
        var windSpeed = Velocity<double>.FromMetersPerSecond(5.0);                  // 5 m/s wind
        var mixingHeight = Length<double>.FromMeters(1000.0);                       // 1 km mixing layer
        var roughnessLength = Length<double>.FromCentimeters(10.0);                 // Urban roughness
        
        // Calculate atmospheric stability parameters
        var airViscosity = DynamicViscosity<double>.FromPascalSeconds(1.825e-5);    // Air kinematic viscosity
        var charakteristicLength = Length<double>.FromKilometers(1.0);              // 1 km scale
        
        var atmosphericReynolds = ReynoldsNumber<double>.FromFluidProperties(
            airDensity, windSpeed, charakteristicLength, airViscosity);
        
        // Dispersion coefficients (simplified Gaussian model)
        var horizontalDispersion = Length<double>.FromMeters(100.0);                // σy
        var verticalDispersion = Length<double>.FromMeters(50.0);                   // σz
        
        // Thermal domain - Temperature effects
        var surfaceTemp = Temperature<double>.FromCelsius(25.0);                    // Surface temperature
        var ambientTemp = Temperature<double>.FromCelsius(20.0);                    // Ambient at height
        var tempGradient = (ambientTemp - surfaceTemp).Value / mixingHeight.Value;  // K/m
        
        // Buoyancy effects on dispersion
        var thermalExpansion = ThermalExpansion<double>.FromPerKelvin(1.0 / 273.15); // Ideal gas
        var densityVariation = airDensity.Value * thermalExpansion.Value * (surfaceTemp - ambientTemp).Value;
        
        // Convective heat flux
        var convectiveHeatFlux = HeatTransferCoefficient<double>.FromWattsPerSquareMeterKelvin(10.0);
        var surfaceArea = Area<double>.FromSquareKilometers(1.0);                   // 1 km² area
        var heatFlux = Power<double>.Create(
            convectiveHeatFlux.Value * surfaceArea.Value * Math.Abs(surfaceTemp - ambientTemp).Value);
        
        Console.WriteLine($"Environmental Monitoring Analysis:");
        Console.WriteLine($"  NO₂ Concentration: {no2Concentration.InPartsPerMillion():F2} ppm");
        Console.WriteLine($"  Wind Speed: {windSpeed.In(Units.MetersPerSecond):F1} m/s");
        Console.WriteLine($"  Atmospheric Reynolds: {atmosphericReynolds.Value:E2}");
        Console.WriteLine($"  Temperature Gradient: {tempGradient * 1000:F2} K/km");
        Console.WriteLine($"  Surface Heat Flux: {heatFlux.In(Units.Megawatt):F1} MW");
        Console.WriteLine($"  Mixing Layer Height: {mixingHeight.In(Units.Meter):F0} m");
    }
}
```

## 5. Advanced Multi-Domain Calculations

### Plasma Physics Laboratory
Combining **Nuclear**, **Electrical**, **Thermal**, and **Optical** domains:

```csharp
public class PlasmaPhysics
{
    public static void AnalyzePlasmaConditions()
    {
        // Electrical domain - Plasma electrical properties
        var electronDensity = 1e20; // electrons/m³
        var electronTemp = Temperature<double>.FromElectronVolts(10.0);              // 10 eV electron temperature
        var ionTemp = Temperature<double>.FromElectronVolts(5.0);                    // 5 eV ion temperature
        
        var plasmaConductivity = ElectricConductivity<double>.FromSiemensPerMeter(1e6); // High conductivity
        var magneticField = 2.0; // 2 Tesla magnetic field
        
        // Calculate plasma frequency and cyclotron frequency
        var electronMass = PhysicalConstants.Generic.ElectronMass<double>();
        var elementaryCharge = PhysicalConstants.Generic.ElementaryCharge<double>();
        var permittivity = PhysicalConstants.Generic.VacuumPermittivity<double>();
        
        var plasmaFrequency = Math.Sqrt(electronDensity * Math.Pow(elementaryCharge, 2) / 
                                      (electronMass * permittivity));
        var plasmaFreq = Frequency<double>.FromRadiansPerSecond(plasmaFrequency);
        
        // Optical domain - Plasma radiation
        var bremsstrahlung = 1e13; // W/m³ volume emission rate
        var plasmaVolume = Volume<double>.FromCubicMeters(1.0);                      // 1 m³ plasma volume
        var radiatedPower = Power<double>.Create(bremsstrahlung * plasmaVolume.Value);
        
        // Characteristic radiation wavelength
        var kBoltzmann = PhysicalConstants.Generic.BoltzmannConstant<double>();
        var planckConstant = PhysicalConstants.Generic.PlanckConstant<double>();
        var lightSpeed = PhysicalConstants.Generic.SpeedOfLight<double>();
        
        var characteristicEnergy = kBoltzmann * electronTemp.InKelvin();
        var characteristicFrequency = characteristicEnergy / planckConstant;
        var characteristicWavelength = lightSpeed / characteristicFrequency;
        
        // Nuclear domain - Fusion reactions
        var deuteriumDensity = 5e19; // D nuclei/m³
        var tritiumDensity = 5e19; // T nuclei/m³
        var fusionCrossSection = NuclearCrossSection<double>.FromBarns(5.0);         // D-T fusion cross section
        var averageVelocity = Math.Sqrt(3 * kBoltzmann * ionTemp.InKelvin() / 
                                      (2.5 * PhysicalConstants.Generic.AtomicMassUnit<double>())); // D-T average
        
        var reactionRate = deuteriumDensity * tritiumDensity * fusionCrossSection.Value * averageVelocity;
        var fusionPower = Power<double>.Create(reactionRate * 17.6e6 * elementaryCharge); // 17.6 MeV per reaction
        
        // Thermal domain - Heat balance
        var specificHeat = SpecificHeat<double>.FromJoulesPerKilogramKelvin(5200.0); // Plasma specific heat
        var plasmaDensity = Density<double>.FromKilogramsPerCubicMeter(1e-6);        // Low density plasma
        var confinementTime = Time<double>.FromSeconds(1.0);                        // 1 second confinement
        
        var thermalEnergy = Energy<double>.Create(
            plasmaDensity.Value * plasmaVolume.Value * specificHeat.Value * electronTemp.InKelvin());
        
        var powerBalance = fusionPower.Value - radiatedPower.Value; // Net power
        var temperatureRise = Temperature<double>.Create(
            powerBalance * confinementTime.Value / 
            (plasmaDensity.Value * plasmaVolume.Value * specificHeat.Value));
        
        Console.WriteLine($"Plasma Physics Analysis:");
        Console.WriteLine($"  Plasma Frequency: {plasmaFreq.In(Units.Gigahertz):F1} GHz");
        Console.WriteLine($"  Fusion Power: {fusionPower.In(Units.Megawatt):F2} MW");
        Console.WriteLine($"  Radiated Power: {radiatedPower.In(Units.Megawatt):F2} MW");
        Console.WriteLine($"  Net Power: {powerBalance / 1e6:F2} MW");
        Console.WriteLine($"  Characteristic Wavelength: {characteristicWavelength * 1e9:F1} nm");
        Console.WriteLine($"  Temperature Rise Rate: {temperatureRise.Value:F1} K/s");
    }
}
```

## Best Practices Summary

### 1. Domain Integration
- **Always use appropriate units** for each domain
- **Leverage physical constants** from `PhysicalConstants.Generic`
- **Combine domains logically** based on real physics relationships

### 2. Error Handling
- **Check for physical validity** using `IsPhysicallyValid` property
- **Handle unit conversions safely** with try-catch blocks
- **Validate input ranges** for physical reasonableness

### 3. Performance Optimization
- **Cache frequently used constants** to avoid repeated calculations
- **Use appropriate numeric types** (double, float, decimal) based on precision needs
- **Minimize unit conversions** in tight loops

### 4. Code Clarity
- **Use descriptive variable names** that indicate physical meaning
- **Add comments explaining** the physics relationships
- **Group related calculations** by physical domain

This comprehensive example set demonstrates the power and flexibility of the ktsu.Semantics library for real-world engineering and scientific applications across all physics domains. 
