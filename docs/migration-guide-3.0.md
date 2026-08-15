# Migrating from Semantics 2.x to 3.0

Semantics 3.0 is a small release with two breaking changes, both in
`ktsu.Semantics.Strings` and `ktsu.Semantics.Music`. Quantities, Paths and
Color are unaffected.

## Quick checklist

1. Replace the ten removed first-class .NET type validation attributes
   (`[IsGuid]`, `[IsUri]`, …) — see below.
2. Rename `ChordOmission` → `ChordOmissions` and `ChordTension` →
   `ChordTensions`.

## 1. First-class .NET type attributes removed

The ten attributes under `Semantics.Strings/Validation/Attributes/FirstClassTypes/`
were marked `[Obsolete]` throughout 2.x and have now been deleted:

`[IsBoolean]`, `[IsDateTime]`, `[IsDecimal]`, `[IsDouble]`, `[IsGuid]`,
`[IsInt32]`, `[IsIpAddress]`, `[IsTimeSpan]`, `[IsUri]`, `[IsVersion]`

Each of these validated that a string *parses as* some .NET type while still
storing it as a string — paying string overhead and re-parsing on every use.
Wrap the .NET type directly instead:

```csharp
// Before
[IsGuid]
public sealed record TransactionId : SemanticString<TransactionId> { }

// After
public sealed record TransactionId(Guid Value)
{
    public static TransactionId New() => new(Guid.NewGuid());
}
```

If a value genuinely has to remain a string inside a wider validation pipeline
(for example when it is one of several `[ValidateAny]` alternatives), write a
custom validation attribute wrapping the corresponding `TryParse`:

```csharp
public sealed class IsGuidAttribute : SemanticStringValidationAttribute
{
    public override bool Validate(ISemanticString semanticString) =>
        Guid.TryParse(semanticString.ToString(), out _);
}
```

Note the behavioural detail if you do: the removed attributes treated an empty
string as **valid**. Replicate that with `string.IsNullOrEmpty(value) || TryParse(...)`
if your types relied on it.

## 2. Chord flag enums renamed to plural

`[Flags]` enums are named in the plural, matching `ChordModifiers` and the
`Chord.Omissions` / `Chord.Tensions` properties that carry them:

| 2.x | 3.0 |
|---|---|
| `ChordOmission` | `ChordOmissions` |
| `ChordTension` | `ChordTensions` |

The members are unchanged, so this is a mechanical rename:

```csharp
// Before
if (chord.Omissions.HasFlag(ChordOmission.Fifth)) { … }

// After
if (chord.Omissions.HasFlag(ChordOmissions.Fifth)) { … }
```
