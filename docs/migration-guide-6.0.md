# Migrating from Semantics 5.x to 6.0

## `PH<T>.ToPOH()` returns a `POH<T>`

`ToPOH()` used to return its pOH inside a `PH<T>`, so every pH member read it as a pH: the pOH
of an acid reported `IsBasic == true`, printed as `pH 11`, and `ToHydrogenConcentration()`
returned the hydroxide-ion concentration instead (#482).

It now returns the new `POH<T>` scale, generated from `logarithmic.json` like `PH<T>`. `POH<T>`
has no `IsAcidic`, `IsBasic` or pH formatting; it prints as `pOH {0}`, converts to and from the
hydroxide-ion `Concentration<T>`, and `ToPH()` gives the complementary pH at 25 °C.

```csharp
// 5.x
PH<double> poh = PH<double>.Create(3).ToPOH();          // "pH 11", IsBasic == true

// 6.0
POH<double> poh = PH<double>.Create(3).ToPOH();         // "pOH 11"
PH<double> ph = poh.ToPH();                             // pH 3
Concentration<double> oh = poh.ToHydroxideConcentration(); // 1e-11 mol/L
```

Code that only read `.Value` keeps compiling if it uses `var`, and the number is unchanged.
Code that declared the result as `PH<T>` gets `CS0029`; declare it as `POH<T>`, or call
`.ToPH()` if a pH was what it wanted.

## A directory's `Name` is a `DirectoryName`

`AbsoluteDirectoryPath.Name` and `DirectoryPath.Name` used to return a `FileName`, while
`RelativeDirectoryPath.Name` returned a `DirectoryName`. The `/` operator picks its result from
the right operand, so rebuilding a directory from its own parts turned it into a file path:
`dir.Parent / dir.Name` was an `AbsoluteFilePath` (#498).

All three now return a `DirectoryName`, and `RelativeDirectoryPath` gains the
`/ DirectoryName` operator the other two already had, so `dir.Parent / dir.Name` has the same
type as `dir` for every directory type. The name itself is unchanged: it is still the last
component as written, so the name of `/home/user/..` is `..`.

```csharp
AbsoluteDirectoryPath dir = AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>("/home/user/docs");

// 5.x
AbsoluteFilePath rebuilt = dir.Parent / dir.Name;      // a file path

// 6.0
AbsoluteDirectoryPath rebuilt = dir.Parent / dir.Name; // the same directory
```

Code that used `var`, or only read `.Name.WeakString` or `ToString()`, keeps compiling.
Code that declared the result as `FileName` gets `CS0029`; declare it as `DirectoryName`. If a
file name really was wanted, convert it with `dir.Name.As<FileName>()`.
