# Migrating from Semantics 5.x to 6.0

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
