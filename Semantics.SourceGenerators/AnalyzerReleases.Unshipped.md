; Unshipped analyzer release.
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------
SEM001  | Semantics.SourceGenerators | Warning | Reports relationships in dimensions.json that reference unknown dimension names.
SEM002  | Semantics.SourceGenerators | Warning | Reports schema-level validation issues in dimensions.json (missing fields, duplicate type names, etc).
SEM003  | Semantics.SourceGenerators | Warning | Reports a relationship whose explicit `forms` list references a vector form not declared on a participating dimension.
SEM004  | Semantics.SourceGenerators | Warning | Reports a `dimensions.json` `availableUnits` entry that doesn't match any unit declared in `units.json`.
