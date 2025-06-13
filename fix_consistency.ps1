#!/usr/bin/env pwsh

# Script to fix documentation consistency issues in ktsu.Semantics physics quantities

Write-Host "Fixing documentation consistency issues in physics quantities..." -ForegroundColor Green

# Define the mapping of dimension names to their symbols from PhysicalDimensions.cs
$dimensionSymbols = @{
    "Length" = "[L]"
    "Mass" = "[M]"
    "Time" = "[T]"
    "ElectricCurrent" = "[I]"
    "Temperature" = "[Θ]"
    "AmountOfSubstance" = "[N]"
    "LuminousIntensity" = "[J]"
    "Dimensionless" = "[1]"
    "Area" = "[L²]"
    "Volume" = "[L³]"
    "Velocity" = "[L T⁻¹]"
    "Acceleration" = "[L T⁻²]"
    "Force" = "[M L T⁻²]"
    "Pressure" = "[M L⁻¹ T⁻²]"
    "Energy" = "[M L² T⁻²]"
    "Power" = "[M L² T⁻³]"
    "ElectricPotential" = "[M L² T⁻³ I⁻¹]"
    "ElectricResistance" = "[M L² T⁻³ I⁻²]"
    "ElectricCharge" = "[I T]"
    "ElectricCapacitance" = "[M⁻¹ L⁻² T⁴ I²]"
    "Frequency" = "[T⁻¹]"
    "Momentum" = "[M L T⁻¹]"
    "AngularVelocity" = "[T⁻¹]"
    "AngularAcceleration" = "[T⁻²]"
    "Torque" = "[M L² T⁻²]"
    "MomentOfInertia" = "[M L²]"
    "Density" = "[M L⁻³]"
    "ElectricField" = "[M L T⁻³ I⁻¹]"
    "ElectricFlux" = "[M L³ T⁻³ I⁻¹]"
    "Permittivity" = "[M⁻¹ L⁻³ T⁴ I²]"
    "ElectricConductivity" = "[M⁻¹ L⁻³ T³ I²]"
    "ElectricPowerDensity" = "[M L⁻¹ T⁻³]"
    "ImpedanceAC" = "[M L² T⁻³ I⁻²]"
    "SoundPressure" = "[M L⁻¹ T⁻²]"
    "SoundIntensity" = "[M T⁻³]"
    "SoundPower" = "[M L² T⁻³]"
    "AcousticImpedance" = "[M L⁻² T⁻¹]"
    "SoundSpeed" = "[L T⁻¹]"
    "SoundAbsorption" = "[1]"
    "ReverberationTime" = "[T]"
    "Wavelength" = "[L]"
    "SoundPressureLevel" = "[1]"
    "SoundIntensityLevel" = "[1]"
    "SoundPowerLevel" = "[1]"
    "ReflectionCoefficient" = "[1]"
    "NoiseReductionCoefficient" = "[1]"
    "SoundTransmissionClass" = "[1]"
    "Loudness" = "[1]"
    "Pitch" = "[T⁻¹]"
    "Sharpness" = "[1]"
    "Sensitivity" = "[1]"
    "DirectionalityIndex" = "[1]"
    "Heat" = "[M L² T⁻²]"
    "Entropy" = "[M L² T⁻² Θ⁻¹]"
    "ThermalConductivity" = "[M L T⁻³ Θ⁻¹]"
    "HeatCapacity" = "[M L² T⁻² Θ⁻¹]"
    "SpecificHeat" = "[L² T⁻² Θ⁻¹]"
    "HeatTransferCoefficient" = "[M T⁻³ Θ⁻¹]"
    "ThermalExpansion" = "[Θ⁻¹]"
    "ThermalDiffusivity" = "[L² T⁻¹]"
    "ThermalResistance" = "[Θ T³ M⁻¹ L⁻²]"
}

# Function to fix dimension property documentation
function Fix-DimensionProperty {
    param([string]$filePath, [string]$className, [string]$dimensionName)

    $symbol = $dimensionSymbols[$dimensionName]
    if (-not $symbol) {
        Write-Warning "No symbol found for dimension: $dimensionName in file: $filePath"
        return
    }

    $content = Get-Content $filePath -Raw

    # Replace inheritdoc with explicit documentation
    $oldPattern = "/// <inheritdoc/>`r?`n\s*public override PhysicalDimension Dimension => PhysicalDimensions\.$dimensionName;"
    $newPattern = "/// <summary>Gets the physical dimension of $($className.ToLower()) $symbol.</summary>`n	public override PhysicalDimension Dimension => PhysicalDimensions.$dimensionName;"

    $newContent = $content -replace $oldPattern, $newPattern

    if ($newContent -ne $content) {
        Set-Content $filePath $newContent -NoNewline
        Write-Host "Fixed dimension property in: $filePath" -ForegroundColor Yellow
    }
}

# Function to fix constructor documentation
function Fix-ConstructorDocumentation {
    param([string]$filePath, [string]$className)

    $content = Get-Content $filePath -Raw

    # Pattern 1: Old style without summary tags
    $oldPattern1 = "/// Initializes a new instance of the $className class\."
    $newPattern1 = "/// <summary>Initializes a new instance of the <see cref=`"$className{T}`"/> class.</summary>"

    $newContent = $content -replace $oldPattern1, $newPattern1

    # Pattern 2: Old style with summary tags but no cref
    $oldPattern2 = "/// <summary>`r?`n\s*/// Initializes a new instance of the $className class\.`r?`n\s*/// </summary>"
    $newPattern2 = "/// <summary>`n	/// Initializes a new instance of the <see cref=`"$className{T}`"/> class.`n	/// </summary>"

    $newContent = $newContent -replace $oldPattern2, $newPattern2

    if ($newContent -ne $content) {
        Set-Content $filePath $newContent -NoNewline
        Write-Host "Fixed constructor documentation in: $filePath" -ForegroundColor Yellow
    }
}

# Get all physics quantity files that need fixing
$quantityFiles = Get-ChildItem -Path "Semantics/Quantities" -Recurse -Include "*.cs" |
    Where-Object { $_.Name -notmatch "^(PhysicalDimensions|Units|PhysicalConstants|README)" -and $_.Directory.Name -ne "Core" }

Write-Host "Found $($quantityFiles.Count) quantity files to process" -ForegroundColor Cyan

foreach ($file in $quantityFiles) {
    $className = $file.BaseName
    Write-Host "Processing: $($file.Name)" -ForegroundColor White

    # Read the content to find the dimension
    $content = Get-Content $file.FullName -Raw

    # Extract dimension name from the file
    if ($content -match "PhysicalDimensions\.(\w+)") {
        $dimensionName = $matches[1]
        Fix-DimensionProperty -filePath $file.FullName -className $className -dimensionName $dimensionName
        Fix-ConstructorDocumentation -filePath $file.FullName -className $className
    } else {
        Write-Warning "Could not find dimension in file: $($file.Name)"
    }
}

Write-Host "Documentation consistency fixes completed!" -ForegroundColor Green
