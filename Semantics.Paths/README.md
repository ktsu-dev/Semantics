# ktsu.Semantics.Paths

> Polymorphic, strongly-typed file system path types that keep files, directories, absolute, and relative paths distinct at compile time.

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](../LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.Paths?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Paths)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics.Paths?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Paths)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.Paths?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Paths)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

`ktsu.Semantics.Paths` is one package in the [ktsu.Semantics](../README.md) family. It builds on [`ktsu.Semantics.Strings`](../Semantics.Strings/README.md), which supplies the underlying `SemanticString<T>` machinery.

## Introduction

`ktsu.Semantics.Paths` models a file system path as a type, not a bare string. The type encodes two independent facts: whether the path names a file or a directory, and whether it is absolute or relative. That gives concrete types like `AbsoluteFilePath`, `RelativeDirectoryPath`, and orientation-agnostic `FilePath` / `DirectoryPath`, all implementing a shared `IPath` interface hierarchy. You can hold mixed paths in a `List<IPath>` and filter by capability with `OfType<IFilePath>()`.

Paths are canonicalized on creation (separators normalized, trailing separators trimmed), convert implicitly to `string` so they drop into any `System.IO` API, and compose with a `/` operator that returns the correctly-typed result.

## Features

- **Interface hierarchy**: `IPath` with `IFilePath`, `IDirectoryPath`, `IAbsolutePath`, `IRelativePath`, and the four combinations (`IAbsoluteFilePath`, `IAbsoluteDirectoryPath`, `IRelativeFilePath`, `IRelativeDirectoryPath`).
- **Eight concrete path types**: `AbsolutePath`, `RelativePath`, `FilePath`, `DirectoryPath`, `AbsoluteFilePath`, `AbsoluteDirectoryPath`, `RelativeFilePath`, `RelativeDirectoryPath`.
- **Path decomposition**: `FileName`, `FileExtension`, `FullFileExtension` (for `.tar.gz`), `DirectoryPath`, `FileNameWithoutExtension`, plus the `FileName` / `FileExtension` / `DirectoryName` primitive value types.
- **Composition with `/`**: `directory / relativePath`, `directory / FileName`, and similar, each returning the right result type.
- **Absolute/relative conversions**: `AsAbsolute()`, `AsAbsolute(baseDirectory)`, and `AsRelative(baseDirectory)`.
- **Directory navigation**: `Parent`, `Depth`, `IsRoot`, `GetAncestors()`, `IsChildOf`, `IsParentOf`, and strongly-typed `GetContents()` enumeration.
- **Filesystem checks**: `Exists`, `IsFile`, `IsDirectory`.
- **Validation and canonicalization** inherited from the semantic string framework, driven by path attributes such as `[IsAbsolutePath]`, `[IsFileName]`, and `[IsFileExtension]`.

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.Semantics.Paths
```

### .NET CLI

```bash
dotnet add package ktsu.Semantics.Paths
```

### Package Reference

```xml
<PackageReference Include="ktsu.Semantics.Paths" Version="x.y.z" />
```

## Usage Examples

### Basic Example

```csharp
using ktsu.Semantics.Paths;

// Build paths with the typed factory, then compose with the '/' operator
AbsoluteDirectoryPath projectDir = AbsoluteDirectoryPath.Create(@"C:\repos\app");
RelativeFilePath rel = RelativeFilePath.Create(@"src\Program.cs");

AbsoluteFilePath source = projectDir / rel;   // C:\repos\app\src\Program.cs
FileName name = source.FileName;              // Program.cs
FileExtension ext = source.FileExtension;     // .cs
AbsoluteDirectoryPath dir = source.AbsoluteDirectoryPath;

if (source.Exists)
{
    // implicit conversion to string drops straight into System.IO
    string text = System.IO.File.ReadAllText(source);
}
```

### Absolute and relative conversions

```csharp
using ktsu.Semantics.Paths;

AbsoluteDirectoryPath root = AbsoluteDirectoryPath.Create(@"C:\data");
AbsoluteFilePath file = AbsoluteFilePath.Create(@"C:\data\logs\app.log");

RelativeFilePath relative = file.AsRelative(root);        // logs\app.log
AbsoluteFilePath backAgain = relative.AsAbsolute(root);   // C:\data\logs\app.log

AbsoluteFilePath renamed = file.ChangeExtension(FileExtension.Create(".bak"));
bool nested = file.IsChildOf(root);                       // true
```

`AsAbsolute()` with no argument resolves a relative path against the current working directory. The `baseDirectory` overload of `AsAbsolute` exists on the relative path types only.

### Polymorphic collections and typed enumeration

```csharp
using ktsu.Semantics.Paths;

List<IPath> all =
[
    AbsoluteFilePath.Create(@"C:\data.txt"),
    RelativeDirectoryPath.Create(@"logs\app"),
    FilePath.Create(@"document.pdf"),
];

List<IFilePath> files = all.OfType<IFilePath>().ToList();
List<IAbsolutePath> absolutes = all.OfType<IAbsolutePath>().ToList();

// GetContents() yields correctly-typed children
AbsoluteDirectoryPath project = AbsoluteDirectoryPath.Create(@"C:\project");
foreach (IPath entry in project.GetContents())
{
    switch (entry)
    {
        case AbsoluteFilePath f:
            Console.WriteLine($"file: {f.FileName} ({f.FileExtension})");
            break;
        case AbsoluteDirectoryPath d:
            Console.WriteLine($"dir:  {d.Name} at depth {d.Depth}");
            break;
    }
}
```

`GetContents()` returns an empty sequence rather than throwing when the directory is missing or access is denied.

## API Reference

### Interface hierarchy

| Interface | Extends | Notable member |
|-----------|---------|----------------|
| `IPath` | (none) | marker |
| `IFilePath` | `IPath` | `AbsoluteFilePath AsAbsolute()` |
| `IDirectoryPath` | `IPath` | `AsAbsolute()`, `IEnumerable<IPath> GetContents()` |
| `IAbsolutePath` | `IPath` | `AbsolutePath AsAbsolute()` |
| `IRelativePath` | `IPath` | `AbsolutePath AsAbsolute()` |
| `IAbsoluteFilePath` | `IFilePath, IAbsolutePath` | typed `AsAbsolute()` |
| `IAbsoluteDirectoryPath` | `IDirectoryPath, IAbsolutePath` | typed `AsAbsolute()` |
| `IRelativeFilePath` | `IFilePath, IRelativePath` | typed `AsAbsolute()` |
| `IRelativeDirectoryPath` | `IDirectoryPath, IRelativePath` | typed `AsAbsolute()` |

### Concrete types and creation

All concrete types are `sealed record`s created through the inherited static factory `Create(...)` (accepting `string`, `char[]`, or `ReadOnlySpan<char>`). `Create` throws `ArgumentException` on invalid input and `ArgumentNullException` on null.

| Type | Kind | Interface |
|------|------|-----------|
| `AbsolutePath` | untyped absolute | `IAbsolutePath` |
| `RelativePath` | untyped relative | `IRelativePath` |
| `FilePath` | orientation-agnostic file | `IFilePath` |
| `DirectoryPath` | orientation-agnostic directory | `IDirectoryPath` |
| `AbsoluteFilePath` | absolute file | `IAbsoluteFilePath` |
| `AbsoluteDirectoryPath` | absolute directory | `IAbsoluteDirectoryPath` |
| `RelativeFilePath` | relative file | `IRelativeFilePath` |
| `RelativeDirectoryPath` | relative directory | `IRelativeDirectoryPath` |

The primitive component types `FileName`, `FileExtension`, and `DirectoryName` are semantic strings in their own right.

### Common members

| Name | Type | Description |
|------|------|-------------|
| `Exists` | `bool` | True if the path is an existing file or directory. |
| `IsFile` / `IsDirectory` | `bool` | Filesystem-backed checks. |
| `FileName` | `FileName` | Filename portion (file paths). |
| `FileExtension` / `FullFileExtension` | `FileExtension` | Last extension / everything from the first dot. |
| `DirectoryPath` | `DirectoryPath` | Directory portion of a file path. |
| `Parent` / `Name` / `Depth` / `IsRoot` | directory members | Directory navigation. |
| `AsAbsolute()` | typed absolute path | Resolve against the current working directory. |
| `AsAbsolute(AbsoluteDirectoryPath)` | typed absolute path | Resolve a relative path against a base (relative types only). |
| `AsRelative(AbsoluteDirectoryPath)` | typed relative path | Make relative to a base directory. |
| `GetContents()` | `IEnumerable<IPath>` | Strongly-typed directory children. |
| `operator /` | typed result | Combine a directory with a relative path, `FileName`, or `DirectoryName`. |

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE.md](../LICENSE.md) file for details.
