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
