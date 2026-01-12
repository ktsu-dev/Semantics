## v1.0.26-pre.1 (prerelease)

Changes since v1.0.25:

- Bump the microsoft group with 3 updates ([@dependabot[bot]](https://github.com/dependabot[bot]))
- Bump the microsoft group with 1 update ([@dependabot[bot]](https://github.com/dependabot[bot]))
- Bump Polyfill from 8.8.0 to 9.7.0 ([@dependabot[bot]](https://github.com/dependabot[bot]))
- Bump MSTest.Sdk from 3.10.2 to 4.0.2 ([@dependabot[bot]](https://github.com/dependabot[bot]))
## v1.0.25 (patch)

Changes since v1.0.24:

- Add DirectoryName type and improve path validation semantics ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.24 (patch)

Changes since v1.0.23:

- Enhance project detection logic to support multi-project solutions ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.23 (patch)

Changes since v1.0.22:

- Add compatibility supression files ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.22 (patch)

Changes since v1.0.21:

- Refactor testing and coverage configuration ([@matt-edmondson](https://github.com/matt-edmondson))
- Modernize codebase and simplify multi-framework support ([@matt-edmondson](https://github.com/matt-edmondson))
- Update project configurations and SDK versions ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.21 (patch)

Changes since v1.0.20:

- Refactor validation attributes to enhance initialization and validation logic ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor SemanticString methods to utilize WeakString for span-based operations ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement polyfill for ArgumentNullException in path classes for compatibility ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor directory path implementations to support synchronous enumeration for older .NET versions ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance validation error messages in ContainsAttribute, EndsWithAttribute, and StartsWithAttribute classes ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance null argument validation in RelativeFilePath for .NET compatibility ([@matt-edmondson](https://github.com/matt-edmondson))
- Remove obsolete cursor ignore files to streamline project structure ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor Exposure calculations to use CoulombPerKilogram unit ([@matt-edmondson](https://github.com/matt-edmondson))
- Remove unnecessary using directives from test files for improved clarity ([@matt-edmondson](https://github.com/matt-edmondson))
- Add unit tests for various SemanticString validators ([@matt-edmondson](https://github.com/matt-edmondson))
- Add unit tests for AcousticOperator and PhysicalDimension extensions ([@matt-edmondson](https://github.com/matt-edmondson))
- Add unit tests for AcousticImpedance, ReflectionCoefficient, and SoundSpeed classes ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor validation attributes to remove FluentValidation dependency ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix argument exception message assertion in SemanticStringAdditionalTests for improved clarity ([@matt-edmondson](https://github.com/matt-edmondson))
- Remove obsolete files and clean up project structure ([@matt-edmondson](https://github.com/matt-edmondson))
- Add System.Memory package and implement path polyfills for .NET compatibility ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance semantic string handling with read-only span support ([@matt-edmondson](https://github.com/matt-edmondson))
- Add path-related polyfills and refactor namespaces for clarity ([@matt-edmondson](https://github.com/matt-edmondson))
- Update project configuration and refine string handling methods ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix missing usings ([@matt-edmondson](https://github.com/matt-edmondson))
- Add unit tests for Casing and Contracts validation in SemanticString ([@matt-edmondson](https://github.com/matt-edmondson))
- Add unit tests for AcousticDirectionalityIndex functionality ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor SemanticDirectoryPath class to improve content retrieval method naming and enhance error handling. The `Contents` property has been renamed to `GetContents` for clarity, and synchronous handling has been implemented to ensure compatibility with older .NET versions. ([@matt-edmondson](https://github.com/matt-edmondson))
- Update Invoke-DotNetTest function to remove unnecessary `--no-build` flag from dotnet test command for improved test execution. ([@matt-edmondson](https://github.com/matt-edmondson))
- Update RegexMatchAttribute to include a timeout in Regex matching for improved performance and reliability. This change ensures that the regex operation does not hang indefinitely by setting a one-second timeout, enhancing the overall validation process. ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor directory path content retrieval methods for consistency and clarity ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor PhysicalDimensions class to use IReadOnlySet for standard physical dimensions and update test classes to static for consistency. This improves clarity and aligns with coding standards across the project. ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance GitHub Actions workflow and testing configuration ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor validation framework and introduce new path semantics ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor path handling code for improved readability and consistency ([@matt-edmondson](https://github.com/matt-edmondson))
- Add unit tests for casing and line count validators in SemanticString ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.20 (patch)

Changes since v1.0.19:

- Refactor project structure and update SDK versions ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.20-pre.1 (prerelease)

Incremental prerelease update.
## v1.0.19 (patch)

Changes since v1.0.18:

- Refactor exception assertions in unit tests for consistency ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor performance regression tests to set CI-friendly targets ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.18 (patch)

Changes since v1.0.17:

- Enhance path handling and testing for directory and file combinations ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.17 (patch)

Changes since v1.0.16:

- Enhance semantic string type conversions and path handling ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.17-pre.1 (prerelease)

Incremental prerelease update.
## v1.0.16 (patch)

Changes since v1.0.15:

- Enhance performance regression tests with updated targets and optimizations ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.15 (patch)

Changes since v1.0.14:

- Remove BenchmarkMemoryAllocation Test Due to Incompatibility ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.14 (patch)

Changes since v1.0.13:

- Enhance Derived Cursor Rules with Additional Validation Guidelines ([@matt-edmondson](https://github.com/matt-edmondson))
- Update TAGS.md to Use Spaces for Multi-Word Tags ([@matt-edmondson](https://github.com/matt-edmondson))
- Standardize documentation styles for physics quantities and enhance integration tests. This update introduces explicit XML documentation formats for dimension properties and constructors, improving clarity and consistency. Additionally, it refines advanced integration tests to ensure accurate cross-domain calculations, further solidifying the library's robustness for scientific applications. ([@matt-edmondson](https://github.com/matt-edmondson))
- Add derived constants validation tests for PhysicalConstants ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance DESCRIPTION and README for Improved Clarity and Detail ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance the Semantics library by completing the implementation of the Physics Quantities System, which now includes 80+ quantities across 8 scientific domains. Update the README.md and documentation to reflect the comprehensive capabilities, including type-safe arithmetic, automatic unit conversions, and centralized physical constants. Introduce integration and performance benchmarks to validate cross-domain calculations and ensure efficient operations. This update significantly improves the library's usability and functionality for scientific computing and engineering applications. ([@matt-edmondson](https://github.com/matt-edmondson))
- Standardize documentation styles and enhance performance benchmarks in the Semantics library. This update includes the migration of hardcoded constants to the PhysicalConstants class, ensuring all constants are accessed through generic getters. Additionally, it refines the performance benchmarks to utilize these constants, improving code clarity and maintainability. The integration tests have also been updated to reflect these changes, ensuring accurate calculations across various domains. ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor Bootstrap Units into Separate Class for Improved Organization ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance Performance Benchmarks and Derived Cursor Rules ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement comprehensive enhancements to the ktsu.Semantics library, including standardized documentation for all physics quantities, improved testing strategies with advanced integration and performance regression tests, and the addition of real-world physics examples. This update significantly enhances code consistency, documentation clarity, and testing robustness, establishing a solid foundation for future development and ensuring a professional-grade solution for type-safe physics calculations in .NET applications. ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor Units and PhysicalConstants for Consistency and Maintainability ([@matt-edmondson](https://github.com/matt-edmondson))
- Remove outdated TODO comments for various scientific domains in Units.cs to streamline the codebase and improve maintainability. ([@matt-edmondson](https://github.com/matt-edmondson))
- Optimize Performance Benchmarks to Reduce Memory Allocation ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.13 (patch)

Changes since v1.0.12:

- Add validation tests for derived physical constants in PhysicalConstantsTests.cs ([@matt-edmondson](https://github.com/matt-edmondson))
- Add comprehensive tests for physical constants validation in PhysicalConstantsTests.cs ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.12 (patch)

Changes since v1.0.11:

- Enhance the Semantics library by implementing key physical relationships across various domains, including Mechanics, Electrical, Thermal, and Optical. Introduce operator overloads for quantities such as Force, Mass, Energy, ElectricCharge, and LuminousFlux, enabling intuitive calculations. Ensure all new implementations adhere to coding standards and include comprehensive XML documentation. This update significantly improves the usability and functionality of the library, providing a robust framework for physical quantity calculations in .NET applications. ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement additional physical relationships in the Semantics library, enhancing the Acoustic, Electrical, and Mechanical domains. Introduce operator overloads for calculating sound power from intensity and area, charge from capacitance and voltage, and torque from force and distance. Update existing quantities to support intuitive calculations, ensuring adherence to coding standards and comprehensive XML documentation. This update further solidifies the library's framework for physical quantity calculations in .NET applications. ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor operator overloads in the Semantics library to resolve ambiguities between work/energy and torque calculations. Update the Force and Length operators, replacing the removed method with an explicit CalculateTorque method. Enhance documentation and ensure compliance with coding standards. This update improves clarity and usability for physical quantity calculations in .NET applications. ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement the Chemical Domain in the Semantics library, completing 10 quantities: ActivationEnergy, AmountOfSubstance, Concentration, DynamicViscosity, EnzymeActivity, MolarMass, PH, RateConstant, ReactionRate, and SurfaceTension. Introduce PhysicalConstants.cs for centralized management of physical constants, ensuring accuracy and maintainability. Update PhysicalDimensions and Units to incorporate new chemical dimensions and units. Achieve 376 passing tests, marking the Chemical domain as fully implemented in TODO_DOMAINS.md. ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement additional physical relationships in the Semantics library, focusing on the Acoustic, Chemical, and Fluid Dynamics domains. Introduce operator overloads for calculating acoustic impedance from density and sound speed, photon energy from frequency, and apply the ideal gas law for amount of substance calculations. Update existing quantities to enhance usability and ensure adherence to coding standards with comprehensive XML documentation. This update further strengthens the library's framework for physical quantity calculations in .NET applications. ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor physics relationship calculations in the Semantics library to resolve operator ambiguities and enhance clarity. Update the Force and Length operators to distinguish between work/energy and torque calculations, introducing explicit methods for torque. Clean up unused variables and improve documentation for better usability. This update strengthens the library's framework for accurate physical quantity calculations in .NET applications. ([@matt-edmondson](https://github.com/matt-edmondson))
- Complete the Fluid Dynamics domain in the Semantics library by implementing key quantities including KinematicViscosity, BulkModulus, VolumetricFlowRate, MassFlowRate, and ReynoldsNumber. Update the tracker to reflect the successful implementation of all 8 domains, achieving a total of 85 quantities. Ensure all new quantities adhere to coding standards and include comprehensive XML documentation. This marks a significant milestone in the library's development, providing a robust and professional-grade physical quantities system for .NET applications. ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement the Mechanical Domain in the Semantics library, introducing quantities such as Force, Energy, Pressure, and their associated calculations. Centralize physical constants management in PhysicalConstants.cs, ensuring type-safe access to constants like standard gravity and atmospheric pressure. Update PhysicalDimensions and Units to incorporate new mechanical dimensions and units. Achieve comprehensive testing with 100% passing tests, marking the Mechanical domain as fully implemented in the updated implementation plan and progress tracker. ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance the Semantics library by implementing the Optical, Nuclear, and Fluid Dynamics domains. Introduce new quantities such as Illuminance, Luminance, RefractiveIndex, AbsorbedDose, EquivalentDose, and various fluid dynamics properties including BulkModulus and KinematicViscosity. Centralize physical constants in PhysicalConstants.cs for type-safe access. Update PhysicalDimensions and Units to reflect new dimensions and units. Achieve comprehensive testing with all new quantities passing, marking significant progress in the implementation plan. ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.11 (patch)

Changes since v1.0.10:

- Implement comprehensive chemical quantities including ActivationEnergy, AmountOfSubstance, Concentration, DynamicViscosity, EnzymeActivity, MolarMass, pH, RateConstant, ReactionRate, and SurfaceTension. Update PhysicalDimensions and Units to include new chemical dimensions and units, enhancing the completeness of the quantities implementation. ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement chemical quantities including ActivationEnergy, AmountOfSubstance, Concentration, DynamicViscosity, EnzymeActivity, MolarMass, PH, RateConstant, ReactionRate, and SurfaceTension. Update PhysicalDimensions and Units to include new chemical dimensions and units. Mark the Chemical domain as fully implemented in TODO_DOMAINS.md, enhancing the completeness of the quantities implementation. ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.10 (patch)

Changes since v1.0.9:

- Implement comprehensive acoustic quantities including SoundPressureLevel, SoundIntensityLevel, SoundPowerLevel, ReflectionCoefficient, NoiseReductionCoefficient, SoundTransmissionClass, Loudness, Pitch, Sharpness, Sensitivity, and DirectionalityIndex. Update PhysicalDimensions to include new acoustic dimensions and mark the Acoustic domain as fully implemented in TODO_DOMAINS.md, enhancing the completeness of the quantities implementation. ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement thermal quantities including Heat, HeatCapacity, SpecificHeat, ThermalConductivity, ThermalExpansion, ThermalDiffusivity, HeatTransferCoefficient, and Entropy. Update TODO_DOMAINS.md to mark the Thermal domain as fully implemented, enhancing the completeness of the quantities implementation. ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.9 (patch)

Changes since v1.0.8:

- Implement core acoustic quantities including Frequency, Wavelength, SoundPressure, SoundIntensity, SoundPower, AcousticImpedance, SoundSpeed, SoundAbsorption, and ReverberationTime. Update PhysicalDimensions to include new acoustic dimensions and mark the Acoustic domain as significantly progressed in TODO_DOMAINS.md, enhancing the completeness of the quantities implementation. ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.8 (patch)

Changes since v1.0.7:

- Refactor conversions ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor PhysicalQuantity and PhysicalDimension relationship by moving BaseUnit property to PhysicalDimension. Update related classes to access BaseUnit through Dimension, enhancing clarity and encapsulation of dimensional properties. ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement comprehensive quantities for Mechanics and Electrical domains, including new classes for AngularAcceleration, AngularVelocity, Density, ElectricConductivity, ElectricField, and others. Update PhysicalDimensions to include new dimensions and mark all quantities as implemented in TODO_DOMAINS.md, enhancing the overall structure and completeness of the quantities implementation. ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor conversions ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor conversions ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance PhysicalDimension constructor to require IUnit parameter, ensuring proper initialization in operator overloads. Update PhysicalDimensions to reflect new constructor signature, improving clarity and preventing type conversion errors. ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor PhysicalDimension to accept BaseUnit in constructor, removing runtime lookup. Update PhysicalDimensions to initialize dimensions with their respective base units, enhancing performance and clarity in dimensional properties. ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor conversions ([@matt-edmondson](https://github.com/matt-edmondson))
- Organize quantities ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor quantities ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor quantities ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor quantities ([@matt-edmondson](https://github.com/matt-edmondson))
- Add tests ([@matt-edmondson](https://github.com/matt-edmondson))
- Add tests ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.7 (patch)

Changes since v1.0.6:

- Update README and documentation to provide a comprehensive overview of the ktsu.Semantics library ([@matt-edmondson](https://github.com/matt-edmondson))
- Add more physical quantities ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.6 (patch)

Changes since v1.0.5:

- Update README and architecture documentation for examples directory structure ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.5 (patch)

Changes since v1.0.4:

- Refactor semantic validation attributes and introduce new path validation strategies ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor validation attributes and enhance documentation ([@matt-edmondson](https://github.com/matt-edmondson))
- Implement semantic path operators and enhance path interfaces ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor casing validation attributes to utilize FluentValidation ([@matt-edmondson](https://github.com/matt-edmondson))
-  Add new semantic path types and validation strategies ([@matt-edmondson](https://github.com/matt-edmondson))
- Add more teats ([@matt-edmondson](https://github.com/matt-edmondson))
- Integrate FluentValidation into semantic validation attributes ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance documentation and suppress CA1812 warning ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor validation attributes to utilize FluentValidation ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.4 (patch)

Changes since v1.0.3:

- Enhance semantic path documentation and interface functionality ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.3 (patch)

Changes since v1.0.2:

- Refactor GitHub Actions workflow to reposition .NET SDK setup step for improved clarity and maintainability. The setup step is now placed after the JDK setup, ensuring a more logical flow in the CI process. ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.2 (patch)

Changes since v1.0.1:

- Enhance documentation for path interface hierarchy and examples ([@matt-edmondson](https://github.com/matt-edmondson))
- Add interfaces for path type hierarchy to enable polymorphism ([@matt-edmondson](https://github.com/matt-edmondson))
- Add comprehensive interface tests for semantic path types ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.1 (patch)

Changes since v1.0.0:

- Enhance GitHub Actions workflow by adding .NET SDK setup step with caching for improved build performance. ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.0 (patch)

Changes since v1.0.0-pre.1:

- Remove DebugConsole project and associated test files ([@matt-edmondson](https://github.com/matt-edmondson))
- Add DebugConsole project and initial tests for SemanticString functionality ([@matt-edmondson](https://github.com/matt-edmondson))
## v1.0.0-pre.1 (prerelease)

- initial version ([@matt-edmondson](https://github.com/matt-edmondson))
