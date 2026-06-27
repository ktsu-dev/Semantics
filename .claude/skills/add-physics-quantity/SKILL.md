---
name: add-physics-quantity
description: Add a new physics quantity type with units, dimensions, operators, and tests to the source-generated system
disable-model-invocation: true
---

# Add Physics Quantity

Add a new physics quantity to the source-generated physics system. This is a fully metadata-driven process - no C# code needs to be written manually.

## Required Information

Before starting, gather from the user:
1. **Quantity name** (e.g., "Viscosity", "MagneticFlux")
2. **Physical dimension symbol** (e.g., "M L⁻¹ T⁻¹")
3. **Dimensional formula** (exponents for mass, length, time, temperature, current, amount, luminosity)
4. **SI unit name and symbol** (e.g., "Pascal" / "Pa")
5. **Which vector forms** are needed (magnitude only, or 1D/2D/3D/4D)
6. **Physics relationships** - what other quantities multiply/divide to produce this one
7. **Any semantic overloads** (named aliases like Weight for ForceMagnitude)

## Steps

### Step 1: Add Dimension to dimensions.json

**File**: `Semantics.SourceGenerators/Metadata/dimensions.json`

Add a new entry to the `physicalDimensions` array following the existing pattern. Each dimension needs:
- `name`: PascalCase dimension name
- `symbol`: Unicode dimension symbol
- `dimensionalFormula`: object with exponents for base dimensions
- `availableUnits`: array of unit names (must match units.json entries)
- `quantities`: object defining vector forms (vector0 = magnitude, vector1-4 = directional)
- `integrals`: array of `{other, result}` pairs where `Self * Other = Result`
- `derivatives`: array of `{other, result}` pairs where `Self / Other = Result`
- `dotProducts`: array for dot product relationships (vector forms only)
- `crossProducts`: array for cross product relationships (vector3 forms only)

**Important**: The generator automatically creates inverse operators. If you define `Force * Length = Energy` on Force, the generator also creates `Energy / Length = Force` and `Energy / Force = Length` on Energy.

### Step 2: Add Units to units.json (if new units needed)

**File**: `Semantics.SourceGenerators/Metadata/units.json`

Add unit entries to the appropriate `unitCategories` entry, or create a new category. Each unit needs:
- `name`: PascalCase unit name
- `symbol`: unit symbol string
- `description`: brief description
- `system`: one of "SIBase", "SIDerived", "Imperial", "USCustomary", "CGS", "Other"

### Step 3: Add Conversions to conversions.json (if non-SI units exist)

**File**: `Semantics.SourceGenerators/Metadata/conversions.json`

Add conversion factor entries for converting between unit systems. Each factor needs:
- `name`: descriptive PascalCase name (e.g., "CalorieToJoules")
- `description`: includes the exact numeric value
- `value`: string representation of the conversion factor

### Step 4: Add Physical Constants to domains.json (if applicable)

**File**: `Semantics.SourceGenerators/Metadata/domains.json`

If the quantity involves physical constants, add them to the relevant domain entry.

### Step 5: Build and Verify

```bash
cd Semantics.Quantities && dotnet build
```

The source generator will create:
- Quantity classes for each vector form in `Generated/`
- Updated `PhysicalDimensions.g.cs` with dimension metadata
- Updated `Units.g.cs` with unit definitions
- Operator overloads for all defined physics relationships

### Step 6: Run Tests

```bash
dotnet test
```

Verify all existing tests still pass and the new quantity types are correctly generated.

## Validation Checklist

- [ ] Dimensional formula exponents sum correctly for the physical dimension
- [ ] Physics relationships are dimensionally consistent (check both sides of equations)
- [ ] Inverse operators are NOT manually added (generator handles them automatically)
- [ ] Unit symbols follow standard conventions
- [ ] `availableUnits` entries match `units.json` unit names exactly
- [ ] No circular dependencies in physics relationships
