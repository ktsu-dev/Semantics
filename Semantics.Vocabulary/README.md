# Semantics.Vocabulary

The physics, once. `dimensions.json` says which quantities exist, which vector forms each has, and
which relationships hold between them; this resolves all of that into the classes and operators a
generator should emit, and separates out everything the metadata asks for that cannot be honoured.

Two generators consume it:

- **`Semantics.SourceGenerators`** emits the C# quantity types.
- **`Semantics.Cpp`** emits the C++ projection.

Before this existed, only the C++ side checked whether a declared relationship was dimensionally
true. The C# side checked that the names resolved (SEM001) and that the forms existed (SEM003), and
then emitted the operator — so `Sensitivity * Pressure -> ElectricPotential`, which is off by
`M L⁴ T⁻⁵ I⁻²`, shipped as a working C# operator that computes the wrong physics. Sharing the
resolution is what makes one check serve both.

## Why this is shared source rather than a project

These files are compiled into both consumers via `Compile Include`, not built into an assembly of
their own. Three reasons, in order of weight:

1. **`Semantics.SourceGenerators` is a Roslyn component.** It bundles its dependencies into the
   analyzer so they load at analysis time, and its `.csproj` carries a long comment about the
   netstandard2.0 facades in that bundle colliding with in-box types in any consumer that
   references it as a plain library. Adding an assembly to that bundle is exactly the kind of thing
   that breaks subtly and late. Compiling the source in adds nothing to load.
2. **It would otherwise be a published package.** `Semantics.Cpp` is packed, so a project reference
   from it becomes a NuGet dependency that has to exist on the feed. That is a public surface
   commitment for what is, so far, an implementation detail two projects share.
3. **Nothing passes these types across the boundary.** The two generators never exchange objects —
   they are separate builds emitting different languages — so two compiled copies cost nothing.
   (No project references both, so there is no `CS0433` ambiguity either. Keep it that way.)

What it costs: the files are compiled twice, under two analysis configurations. That is the reason
everything here stays inside netstandard2.0's API surface — no `System.HashCode`, no range
expressions, no `string.Contains(char)` — since the generator has the older one. There are comments
at each of those spots saying so.

If a third consumer appears, or if the types ever need to cross an assembly boundary, this should
become a real project and take the packaging decision on its merits.

## What is not here

`DimensionVector.ToCpp` stayed in `Semantics.Cpp`, as an extension method. Writing a dimension as a
C++ template argument list is that target's business, not the vocabulary's.

Each consumer keeps its own metadata reader and adapts it to `DimensionDeclaration`. That is
deliberate and is explained on `QuantityMetadata` in `Semantics.Cpp`: the readers differ in what
they carry beyond the physics — units, symbols, and the validation the C# diagnostics need — and a
reader is allowed to be its own business. The physics is not.
