# Chemical Domain

This domain contains chemistry and chemical engineering related physical quantities.

## TODO: Physical Quantities to Implement

### Core Chemical Quantities
- [ ] `AmountOfSubstance.cs` - Already have base Mole unit (needs expansion)
  - Moles, kilomoles, millimoles
  - Avogadro's number relationships
- [ ] `Concentration.cs` - Solution concentration
  - Molarity (mol/L), molality (mol/kg), normality
  - Parts per million (ppm), parts per billion (ppb)
  - Weight/volume percent, volume/volume percent
- [ ] `MolarMass.cs` - Mass per mole of substance
  - g/mol, kg/mol, lb/mol units
  - Molecular weight calculations

### Chemical Properties
- [ ] `pH.cs` - Acidity/alkalinity scale
  - Logarithmic scale (dimensionless)
  - pOH, pKa, pKb relationships
  - Buffer capacity calculations
- [ ] `Solubility.cs` - Maximum dissolution amount
  - g/L, mol/L, mg/mL units
  - Temperature dependence
- [ ] `DissociationConstant.cs` - Equilibrium constant
  - Ka, Kb, Kw values (dimensionless or M)
  - Acid-base equilibrium

### Reaction Kinetics
- [ ] `ReactionRate.cs` - Rate of chemical reactions
  - mol/(L·s), M/s, concentration/time units
  - Rate law calculations
- [ ] `ActivationEnergy.cs` - Energy barrier for reactions
  - J/mol, kJ/mol, cal/mol, eV units
  - Arrhenius equation relationships
- [ ] `RateConstant.cs` - Reaction rate coefficient
  - Units depend on reaction order (s⁻¹, L/(mol·s), etc.)
  - Temperature dependence (Arrhenius)

### Enzyme Kinetics  
- [ ] `EnzymeActivity.cs` - Catalytic activity
  - Katal (mol/s), enzyme units (μmol/min)
  - Specific activity (activity/mass)
- [ ] `MichaelisMenten.cs` - Enzyme kinetics parameters
  - Km (substrate concentration), Vmax (maximum rate)
  - Turnover number (kcat)

### Electrochemistry
- [ ] `ElectrochemicalPotential.cs` - Redox potential
  - Volts vs standard hydrogen electrode
  - Nernst equation calculations
- [ ] `ElectricConductivity.cs` - Ionic conductivity
  - S/m, mS/cm units
  - Concentration dependence

### Physical Chemistry
- [ ] `SurfaceTension.cs` - Interfacial tension
  - N/m, dyn/cm units  
  - Capillary action calculations
- [ ] `VaporPressure.cs` - Equilibrium vapor pressure
  - Pa, mmHg, torr, atm units
  - Clausius-Clapeyron relationship
- [ ] `Viscosity.cs` - Fluid resistance to flow
  - Pa·s, cP, P units
  - Temperature and concentration dependence

### Dimensional Relationships
```csharp
// Molarity: C = n/V
var molarity = amountOfSubstance / volume;

// Dilution: C1V1 = C2V2  
var finalConcentration = (initialConcentration * initialVolume) / finalVolume;

// pH: pH = -log10[H+]
var pH = -Math.Log10(hydrogenConcentration.Value);

// Rate law: rate = k[A]^m[B]^n
var reactionRate = rateConstant * Math.Pow(concentrationA.Value, orderA) * Math.Pow(concentrationB.Value, orderB);
```

### Applications
- Analytical chemistry
- Process engineering  
- Pharmaceutical development
- Environmental monitoring
- Quality control
- Biochemical research 
