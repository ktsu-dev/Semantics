# Acoustic Domain

This domain contains acoustics, sound, and vibration related physical quantities.

## TODO: Physical Quantities to Implement

### Core Acoustic Quantities
- [ ] `Frequency.cs` - **IMPLEMENTED** Sound wave frequency
  - Hertz, kilohertz, megahertz
  - Audio, ultrasonic, infrasonic ranges
- [ ] `SoundPressure.cs` - Pressure variations in sound waves
  - Pascals, micropascals, bars
  - RMS vs peak measurements
- [ ] `SoundIntensity.cs` - Sound power per unit area
  - W/m², μW/cm² units
  - Energy flow calculations
- [ ] `SoundPower.cs` - Total acoustic power
  - Watts, milliwatts, acoustic watts
  - Source characterization

### Decibel Scale Quantities
- [ ] `SoundPressureLevel.cs` - Logarithmic pressure scale
  - dB SPL (re 20 μPa)
  - Human hearing perception
- [ ] `SoundIntensityLevel.cs` - Logarithmic intensity scale
  - dB IL (re 10⁻¹² W/m²)
  - Acoustic measurements
- [ ] `SoundPowerLevel.cs` - Logarithmic power scale
  - dB PWL (re 10⁻¹² W)
  - Source comparison

### Acoustic Properties
- [ ] `AcousticImpedance.cs` - Resistance to sound flow
  - Pa·s/m, rayl units
  - Material characterization
- [ ] `SoundSpeed.cs` - Velocity of sound waves
  - m/s, ft/s units
  - Medium-dependent propagation
- [ ] `SoundAbsorption.cs` - Energy absorption coefficient
  - Dimensionless (0-1) or percentage
  - Room acoustics design
- [ ] `ReflectionCoefficient.cs` - Sound reflection ratio
  - Dimensionless (0-1)
  - Interface analysis

### Room Acoustics
- [ ] `ReverberationTime.cs` - Decay time of sound
  - Seconds (T60, T30)
  - Architectural acoustics
- [ ] `NoiseReductionCoefficient.cs` - Average absorption
  - Dimensionless (NRC)
  - Material specifications
- [ ] `SoundTransmissionClass.cs` - Isolation rating
  - Dimensionless (STC)
  - Building acoustics

### Vibration Quantities  
- [ ] `Acceleration.cs` - Already in Mechanics (expand for vibration)
  - m/s², g units
  - Vibration analysis
- [ ] `Velocity.cs` - Already in Mechanics (expand for vibration)
  - m/s, mm/s, in/s units
  - RMS vs peak measurements
- [ ] `Displacement.cs` - Vibration amplitude
  - Millimeters, micrometers, mils
  - Peak-to-peak measurements

### Psychoacoustics
- [ ] `Loudness.cs` - Perceived sound strength
  - Sones, phons units
  - Equal-loudness contours
- [ ] `Pitch.cs` - Perceived frequency
  - Hertz, mels, barks
  - Psychoacoustic scales
- [ ] `Sharpness.cs` - High-frequency content perception
  - Acum units
  - Sound quality metrics

### Electroacoustics
- [ ] `Sensitivity.cs` - Microphone/speaker efficiency
  - mV/Pa, dB SPL/W units
  - Transducer specifications
- [ ] `DirectionalityIndex.cs` - Beam pattern measure
  - Decibels (dB)
  - Antenna/speaker patterns

### Dimensional Relationships
```csharp
// Sound speed: c = λf
var soundSpeed = wavelength * frequency;

// Sound intensity: I = p²/(ρc)
var soundIntensity = soundPressure.Squared() / (density * soundSpeed);

// Sound pressure level: SPL = 20·log₁₀(p/p₀)
var soundPressureLevel = 20 * Math.Log10(soundPressure.Value / referencePressure.Value);

// Reverberation time: T60 = 0.161V/A (Sabine)
var reverberationTime = 0.161 * volume / totalAbsorption;
```

### Applications
- Audio engineering
- Noise control
- Architectural acoustics  
- Underwater acoustics
- Medical ultrasound
- Non-destructive testing
- Musical acoustics
- Environmental noise assessment 
