# Path Handling

This guide demonstrates specialized path handling features for working with file system paths in a type-safe manner.

## Table of Contents

-   [Basic Path Types](#basic-path-types)
-   [Built-in Path Types](#built-in-path-types)
-   [Path Interface Hierarchy](#path-interface-hierarchy)
-   [Polymorphic Path Operations](#polymorphic-path-operations)
-   [Relative vs Absolute Paths](#relative-vs-absolute-paths)
-   [Path Existence Validation](#path-existence-validation)
-   [File Extension Validation](#file-extension-validation)
-   [Path Manipulation Operations](#path-manipulation-operations)
-   [Safe Path Construction](#safe-path-construction)

## Basic Path Types

The library provides built-in semantic path types that you can use directly without defining your own:

```csharp
using ktsu.Semantics;

// Create path instances using built-in types
var anyPath = Path.FromString<Path>(@"C:\Users\John\Documents\report.pdf");
var filePath = FilePath.FromString<FilePath>(@"C:\Users\John\Documents\report.pdf");
var directoryPath = DirectoryPath.FromString<DirectoryPath>(@"C:\Users\John\Documents");
var fileName = FileName.FromString<FileName>("report.pdf");
var extension = FileExtension.FromString<FileExtension>(".pdf");

Console.WriteLine($"Path: {anyPath}");
Console.WriteLine($"File: {filePath}");
Console.WriteLine($"Directory: {directoryPath}");
Console.WriteLine($"Filename: {fileName}");
Console.WriteLine($"Extension: {extension}");
```

## Built-in Path Types

The library includes pre-defined path types for common scenarios:

```csharp
using ktsu.Semantics;

// All built-in path types
Path generalPath = Path.FromString<Path>(@"C:\any\path");
AbsolutePath absolutePath = AbsolutePath.FromString<AbsolutePath>(@"C:\absolute\path");
RelativePath relativePath = RelativePath.FromString<RelativePath>(@"relative\path");
FilePath filePath = FilePath.FromString<FilePath>(@"path\to\file.txt");
DirectoryPath directoryPath = DirectoryPath.FromString<DirectoryPath>(@"path\to\directory");
AbsoluteFilePath absoluteFilePath = AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\absolute\file.txt");
RelativeFilePath relativeFilePath = RelativeFilePath.FromString<RelativeFilePath>(@"relative\file.txt");
AbsoluteDirectoryPath absoluteDirectoryPath = AbsoluteDirectoryPath.FromString<AbsoluteDirectoryPath>(@"C:\absolute\directory");
RelativeDirectoryPath relativeDirectoryPath = RelativeDirectoryPath.FromString<RelativeDirectoryPath>(@"relative\directory");
FileName fileName = FileName.FromString<FileName>("file.txt");
FileExtension fileExtension = FileExtension.FromString<FileExtension>(".txt");

// All types are strongly typed and validated at creation
Console.WriteLine($"Absolute file: {absoluteFilePath}");
Console.WriteLine($"Relative directory: {relativeDirectoryPath}");
```

## Path Interface Hierarchy

The path types implement a comprehensive interface hierarchy that enables polymorphism:

### Interface Structure

```csharp
// Base interface for all path types
public interface IPath { }

// Category interfaces
public interface IAbsolutePath : IPath { }
public interface IRelativePath : IPath { }
public interface IFilePath : IPath { }
public interface IDirectoryPath : IPath { }

// Composite interfaces
public interface IAbsoluteFilePath : IFilePath, IAbsolutePath { }
public interface IRelativeFilePath : IFilePath, IRelativePath { }
public interface IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath { }
public interface IRelativeDirectoryPath : IDirectoryPath, IRelativePath { }

// Separate interfaces for non-path components
public interface IFileName { }
public interface IFileExtension { }
```

### Interface Implementation Examples

```csharp
// Each path type implements appropriate interfaces
AbsoluteFilePath absoluteFile = AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\temp\file.txt");

// Can be treated as any of its implemented interfaces
IAbsoluteFilePath absoluteFileInterface = absoluteFile;
IFilePath fileInterface = absoluteFile;
IAbsolutePath absoluteInterface = absoluteFile;
IPath pathInterface = absoluteFile;

// All refer to the same object
Console.WriteLine(ReferenceEquals(absoluteFile, absoluteFileInterface)); // True
Console.WriteLine(ReferenceEquals(absoluteFile, fileInterface)); // True
Console.WriteLine(ReferenceEquals(absoluteFile, pathInterface)); // True
```

## Polymorphic Path Operations

### Type-Safe Collections

Store different path types in collections using common interfaces:

```csharp
// Collection of any path type
List<IPath> allPaths = [];
allPaths.Add(Path.FromString<Path>(@"general\path"));
allPaths.Add(AbsolutePath.FromString<AbsolutePath>(@"C:\absolute\path"));
allPaths.Add(RelativePath.FromString<RelativePath>(@"relative\path"));
allPaths.Add(FilePath.FromString<FilePath>(@"file.txt"));
allPaths.Add(DirectoryPath.FromString<DirectoryPath>(@"directory"));
allPaths.Add(AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\file.txt"));

Console.WriteLine($"Total paths: {allPaths.Count}");

// Collection of only file paths (any type)
List<IFilePath> filePaths = [];
filePaths.Add(FilePath.FromString<FilePath>(@"file.txt"));
filePaths.Add(AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\absolute\file.txt"));
filePaths.Add(RelativeFilePath.FromString<RelativeFilePath>(@"relative\file.txt"));

Console.WriteLine($"File paths: {filePaths.Count}");

// Collection of only absolute paths (any type)
List<IAbsolutePath> absolutePaths = [];
absolutePaths.Add(AbsolutePath.FromString<AbsolutePath>(@"C:\general\absolute"));
absolutePaths.Add(AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\absolute\file.txt"));
absolutePaths.Add(AbsoluteDirectoryPath.FromString<AbsoluteDirectoryPath>(@"C:\absolute\directory"));

Console.WriteLine($"Absolute paths: {absolutePaths.Count}");
```

### Polymorphic Methods

Write methods that accept interface parameters for maximum flexibility:

```csharp
// Method that works with any path type
static void ProcessAnyPath(IPath path)
{
    Console.WriteLine($"Processing path: {path}");

    // You can check the specific type if needed
    switch (path)
    {
        case IAbsolutePath absolutePath:
            Console.WriteLine("  This is an absolute path");
            break;
        case IRelativePath relativePath:
            Console.WriteLine("  This is a relative path");
            break;
    }
}

// Method that works with any file path
static void ProcessFilePath(IFilePath filePath)
{
    Console.WriteLine($"Processing file: {filePath}");

    // Can determine file vs directory context
    if (filePath is IAbsoluteFilePath)
        Console.WriteLine("  Absolute file path");
    else if (filePath is IRelativeFilePath)
        Console.WriteLine("  Relative file path");
}

// Method that works with any directory path
static void ProcessDirectoryPath(IDirectoryPath directoryPath)
{
    Console.WriteLine($"Processing directory: {directoryPath}");
}

// Method that works only with absolute paths
static void ProcessAbsolutePath(IAbsolutePath absolutePath)
{
    Console.WriteLine($"Processing absolute path: {absolutePath}");

    // Can differentiate between file and directory absolute paths
    switch (absolutePath)
    {
        case IAbsoluteFilePath absoluteFile:
            Console.WriteLine("  Absolute file");
            break;
        case IAbsoluteDirectoryPath absoluteDirectory:
            Console.WriteLine("  Absolute directory");
            break;
    }
}

// Usage examples
var absoluteFile = AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\temp\file.txt");
var relativeDirectory = RelativeDirectoryPath.FromString<RelativeDirectoryPath>(@"temp\directory");

ProcessAnyPath(absoluteFile);           // Works - IPath
ProcessFilePath(absoluteFile);          // Works - IFilePath
ProcessAbsolutePath(absoluteFile);      // Works - IAbsolutePath

ProcessAnyPath(relativeDirectory);      // Works - IPath
ProcessDirectoryPath(relativeDirectory); // Works - IDirectoryPath
// ProcessAbsolutePath(relativeDirectory); // Won't compile - not IAbsolutePath
```

### Type Filtering and Conversion

Use LINQ to filter collections by interface type:

```csharp
// Mixed collection of different path types
List<IPath> mixedPaths = [
    AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\file1.txt"),
    RelativeDirectoryPath.FromString<RelativeDirectoryPath>(@"relative\dir"),
    AbsoluteDirectoryPath.FromString<AbsoluteDirectoryPath>(@"C:\absolute\dir"),
    RelativeFilePath.FromString<RelativeFilePath>(@"relative\file.txt")
];

// Filter by specific interface types
List<IFilePath> onlyFiles = [.. mixedPaths.OfType<IFilePath>()];
List<IDirectoryPath> onlyDirectories = [.. mixedPaths.OfType<IDirectoryPath>()];
List<IAbsolutePath> onlyAbsolute = [.. mixedPaths.OfType<IAbsolutePath>()];
List<IRelativePath> onlyRelative = [.. mixedPaths.OfType<IRelativePath>()];

Console.WriteLine($"Original count: {mixedPaths.Count}");
Console.WriteLine($"Files: {onlyFiles.Count}");
Console.WriteLine($"Directories: {onlyDirectories.Count}");
Console.WriteLine($"Absolute: {onlyAbsolute.Count}");
Console.WriteLine($"Relative: {onlyRelative.Count}");

// Process different types differently
foreach (IPath path in mixedPaths)
{
    string pathType = path switch
    {
        IAbsoluteFilePath => "Absolute File",
        IRelativeFilePath => "Relative File",
        IAbsoluteDirectoryPath => "Absolute Directory",
        IRelativeDirectoryPath => "Relative Directory",
        IAbsolutePath => "Absolute Path",
        IRelativePath => "Relative Path",
        IFilePath => "File Path",
        IDirectoryPath => "Directory Path",
        _ => "Path"
    };

    Console.WriteLine($"{pathType}: {path}");
}
```

### Service Layer Integration

Use interfaces in service classes for dependency injection and testability:

```csharp
public interface IFileService
{
    Task<string> ReadFileAsync(IFilePath filePath);
    Task WriteFileAsync(IFilePath filePath, string content);
    bool FileExists(IFilePath filePath);
}

public interface IDirectoryService
{
    Task<IEnumerable<IFilePath>> GetFilesAsync(IDirectoryPath directoryPath);
    Task<IEnumerable<IDirectoryPath>> GetDirectoriesAsync(IDirectoryPath directoryPath);
    bool DirectoryExists(IDirectoryPath directoryPath);
}

public class FileSystemService : IFileService, IDirectoryService
{
    public async Task<string> ReadFileAsync(IFilePath filePath)
    {
        // Works with any file path type
        return await File.ReadAllTextAsync(filePath.ToString());
    }

    public async Task WriteFileAsync(IFilePath filePath, string content)
    {
        // Ensure directory exists for any file path type
        var directory = System.IO.Path.GetDirectoryName(filePath.ToString());
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(filePath.ToString(), content);
    }

    public bool FileExists(IFilePath filePath)
    {
        return File.Exists(filePath.ToString());
    }

    public async Task<IEnumerable<IFilePath>> GetFilesAsync(IDirectoryPath directoryPath)
    {
        var files = Directory.GetFiles(directoryPath.ToString());
        return files.Select(f => FilePath.FromString<FilePath>(f));
    }

    public async Task<IEnumerable<IDirectoryPath>> GetDirectoriesAsync(IDirectoryPath directoryPath)
    {
        var directories = Directory.GetDirectories(directoryPath.ToString());
        return directories.Select(d => DirectoryPath.FromString<DirectoryPath>(d));
    }

    public bool DirectoryExists(IDirectoryPath directoryPath)
    {
        return Directory.Exists(directoryPath.ToString());
    }
}

// Usage in application code
public class DocumentProcessor
{
    private readonly IFileService _fileService;
    private readonly IDirectoryService _directoryService;

    public DocumentProcessor(IFileService fileService, IDirectoryService directoryService)
    {
        _fileService = fileService;
        _directoryService = directoryService;
    }

    public async Task ProcessDocumentsAsync(IDirectoryPath inputDirectory, IDirectoryPath outputDirectory)
    {
        // Works with any directory path types
        var files = await _directoryService.GetFilesAsync(inputDirectory);

        foreach (var file in files)
        {
            // Works with any file path types
            if (_fileService.FileExists(file))
            {
                var content = await _fileService.ReadFileAsync(file);
                var processedContent = ProcessContent(content);

                // Combine paths polymorphically
                var outputFile = FilePath.FromString<FilePath>(
                    System.IO.Path.Combine(outputDirectory.ToString(), System.IO.Path.GetFileName(file.ToString())));

                await _fileService.WriteFileAsync(outputFile, processedContent);
            }
        }
    }

    private string ProcessContent(string content)
    {
        // Document processing logic
        return content.ToUpperInvariant();
    }
}
```

### Interface Benefits Summary

The path interface hierarchy provides several key advantages:

**1. Polymorphic Collections**

-   Store different path types in the same collection using shared interfaces
-   Filter collections by path characteristics (absolute vs relative, file vs directory)

**2. Flexible Method Parameters**

-   Write methods that accept the most general interface needed
-   Avoid code duplication across similar path operations

**3. Service Layer Design**

-   Create clean service interfaces that work with any appropriate path type
-   Enable better testing through interface mocking

**4. Type Safety with Flexibility**

-   Maintain strong typing while allowing polymorphic operations
-   Compiler prevents incorrect interface usage

**5. Future Extensibility**

-   New path types automatically work with existing polymorphic code
-   Interface hierarchy grows naturally with new requirements

## Relative vs Absolute Paths

Distinguish between relative and absolute paths:

```csharp
[IsRelativePath]
public sealed record RelativePath : SemanticString<RelativePath> { }

[IsAbsolutePath]
public sealed record AbsolutePath : SemanticString<AbsolutePath> { }

// Relative path examples
var relativePath1 = @"docs\readme.txt".As<RelativePath>();
var relativePath2 = @"../config/settings.json".As<RelativePath>();
var relativePath3 = @"./images/logo.png".As<RelativePath>();

// Absolute path examples (Windows)
var absolutePath1 = @"C:\Program Files\MyApp\config.ini".As<AbsolutePath>();
var absolutePath2 = @"D:\Data\exports\file.csv".As<AbsolutePath>();

// Absolute path examples (Unix-style)
var unixAbsolute = "/home/user/documents/file.txt".As<AbsolutePath>();

Console.WriteLine($"Relative: {relativePath1}");
Console.WriteLine($"Absolute: {absolutePath1}");
```

## Path Existence Validation

Validate that paths exist on the file system:

```csharp
[DoesExist]
public sealed record ExistingPath : SemanticString<ExistingPath> { }

// Create temporary files for demonstration
string tempDir = Path.GetTempPath();
string tempFile = Path.Combine(tempDir, "test_file.txt");
string tempSubDir = Path.Combine(tempDir, "test_directory");

// Create test files and directories
File.WriteAllText(tempFile, "Test content");
Directory.CreateDirectory(tempSubDir);

try
{
    // These will work because paths exist
    var existingFile = tempFile.As<ExistingPath>();
    var existingDir = tempSubDir.As<ExistingPath>();

    Console.WriteLine($"Existing file: {existingFile}");
    Console.WriteLine($"Existing directory: {existingDir}");

    // This would throw an exception
    // var nonExistent = @"C:\NonExistent\Path.txt".As<ExistingPath>();
}
finally
{
    // Cleanup
    File.Delete(tempFile);
    Directory.Delete(tempSubDir);
}
```

## File Extension Validation

Work with specific file types:

```csharp
// Image files only
[ValidateAny]
[EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".png", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".gif", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)]
public sealed record ImageFilePath : SemanticString<ImageFilePath> { }

// Document files only
[ValidateAny]
[EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".doc", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".docx", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".txt", StringComparison.OrdinalIgnoreCase)]
public sealed record DocumentFilePath : SemanticString<DocumentFilePath> { }

// Configuration files
[ValidateAny]
[EndsWith(".json", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".xml", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".yaml", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".yml", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".ini", StringComparison.OrdinalIgnoreCase)]
public sealed record ConfigFilePath : SemanticString<ConfigFilePath> { }

// Usage examples
var imagePath = @"C:\Photos\vacation.JPG".As<ImageFilePath>();     // Case insensitive
var documentPath = @"reports\annual_report.pdf".As<DocumentFilePath>();
var configPath = @"config\appsettings.json".As<ConfigFilePath>();

Console.WriteLine($"Image: {imagePath}");
Console.WriteLine($"Document: {documentPath}");
Console.WriteLine($"Config: {configPath}");
```

## Path Manipulation Operations

Combine path operations with semantic types:

```csharp
public sealed record BaseDirectory : SemanticString<BaseDirectory> { }
public sealed record SubDirectory : SemanticString<SubDirectory> { }
public sealed record FullPath : SemanticString<FullPath> { }

var baseDir = @"C:\MyApplication".As<BaseDirectory>();
var subDir = "logs".As<SubDirectory>();
var fileName = "app.log".As<FileName>();

// Combine paths
var fullPath = Path.Combine(baseDir, subDir, fileName).As<FullPath>();
Console.WriteLine($"Full path: {fullPath}");

// Path analysis
string directory = Path.GetDirectoryName(fullPath) ?? "";
string fileNameOnly = Path.GetFileName(fullPath);
string extension = Path.GetExtension(fullPath);
string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fullPath);

Console.WriteLine($"Directory: {directory}");
Console.WriteLine($"Filename: {fileNameOnly}");
Console.WriteLine($"Extension: {extension}");
Console.WriteLine($"Name without extension: {fileNameWithoutExt}");
```

## Safe Path Construction

Build paths safely with validation:

```csharp
public sealed record SafeFilePath : SemanticString<SafeFilePath>
{
    public override bool IsValid()
    {
        if (!base.IsValid()) return false;

        try
        {
            // Check for invalid characters
            var invalidChars = Path.GetInvalidPathChars();
            if (WeakString.Any(c => invalidChars.Contains(c)))
                return false;

            // Check if it's a valid path
            Path.GetFullPath(WeakString);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

// Safe path construction
public static class PathBuilder
{
    public static SafeFilePath CombineSafe(string basePath, params string[] paths)
    {
        try
        {
            string combined = basePath;
            foreach (string path in paths)
            {
                combined = Path.Combine(combined, path);
            }
            return combined.As<SafeFilePath>();
        }
        catch (Exception ex)
        {
            throw new FormatException($"Invalid path construction: {ex.Message}", ex);
        }
    }
}

// Usage
var safePath = PathBuilder.CombineSafe(@"C:\Data", "exports", "2024", "report.csv");
Console.WriteLine($"Safe path: {safePath}");
```

## Platform-Specific Paths

Handle different path formats:

```csharp
[RegexMatch(@"^[A-Z]:\\")]
public sealed record WindowsAbsolutePath : SemanticString<WindowsAbsolutePath> { }

[RegexMatch(@"^/")]
public sealed record UnixAbsolutePath : SemanticString<UnixAbsolutePath> { }

// Platform detection and conversion
public static class PathHelper
{
    public static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;

    public static string NormalizePath(string path)
    {
        if (IsWindows)
        {
            return path.Replace('/', '\\');
        }
        else
        {
            return path.Replace('\\', '/');
        }
    }
}

// Cross-platform path handling
string inputPath = "data/files/document.txt";
string normalizedPath = PathHelper.NormalizePath(inputPath);

if (PathHelper.IsWindows)
{
    // Would be "data\files\document.txt" on Windows
    Console.WriteLine($"Windows path: {normalizedPath}");
}
else
{
    // Stays "data/files/document.txt" on Unix
    Console.WriteLine($"Unix path: {normalizedPath}");
}
```

## Working with File Operations

Integrate with System.IO operations:

```csharp
public sealed record LogFilePath : SemanticString<LogFilePath> { }
public sealed record BackupPath : SemanticString<BackupPath> { }

public class FileManager
{
    public void CreateLogFile(LogFilePath logPath, string content)
    {
        // Ensure directory exists
        string? directory = Path.GetDirectoryName(logPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Write content
        File.WriteAllText(logPath, content);
        Console.WriteLine($"Log file created: {logPath}");
    }

    public void BackupFile(LogFilePath source, BackupPath destination)
    {
        if (File.Exists(source))
        {
            // Ensure backup directory exists
            string? backupDir = Path.GetDirectoryName(destination);
            if (!string.IsNullOrEmpty(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            File.Copy(source, destination, overwrite: true);
            Console.WriteLine($"File backed up: {source} -> {destination}");
        }
        else
        {
            throw new FileNotFoundException($"Source file not found: {source}");
        }
    }

    public IEnumerable<LogFilePath> FindLogFiles(DirectoryPath searchDirectory, string pattern = "*.log")
    {
        if (Directory.Exists(searchDirectory))
        {
            return Directory.GetFiles(searchDirectory, pattern)
                           .Select(path => path.As<LogFilePath>());
        }
        return Enumerable.Empty<LogFilePath>();
    }
}

// Usage example
var fileManager = new FileManager();
var logPath = @"logs\application.log".As<LogFilePath>();
var backupPath = @"backups\application_backup.log".As<BackupPath>();
var logDirectory = @"logs".As<DirectoryPath>();

// Create log file
fileManager.CreateLogFile(logPath, "Application started at " + DateTime.Now);

// Backup the file
fileManager.BackupFile(logPath, backupPath);

// Find all log files
var logFiles = fileManager.FindLogFiles(logDirectory);
Console.WriteLine("Found log files:");
foreach (var file in logFiles)
{
    Console.WriteLine($"  - {file}");
}
```

## Path Canonicalization

Normalize path formats automatically:

```csharp
public sealed record CanonicalPath : SemanticString<CanonicalPath>
{
    protected override string MakeCanonical(string input)
    {
        try
        {
            // Resolve to full path and normalize separators
            string fullPath = Path.GetFullPath(input);

            // Convert to platform-appropriate separators
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return fullPath.Replace('/', '\\');
            }
            else
            {
                return fullPath.Replace('\\', '/');
            }
        }
        catch
        {
            // If path resolution fails, just normalize separators
            char separator = Path.DirectorySeparatorChar;
            char altSeparator = Path.AltDirectorySeparatorChar;
            return input.Replace(altSeparator, separator);
        }
    }
}

// Various input formats become normalized
var path1 = @"C:\Users\John\..\John\Documents\file.txt".As<CanonicalPath>();
var path2 = "C:/Users/John/Documents/file.txt".As<CanonicalPath>();
var path3 = @".\docs\..\files\readme.txt".As<CanonicalPath>();

Console.WriteLine($"Path 1: {path1}"); // Resolves parent directory references
Console.WriteLine($"Path 2: {path2}"); // Converts forward slashes on Windows
Console.WriteLine($"Path 3: {path3}"); // Resolves relative references
```

## Configuration File Paths

Handle application configuration paths:

```csharp
[IsFilePath]
[EndsWith(".json", StringComparison.OrdinalIgnoreCase)]
public sealed record JsonConfigPath : SemanticString<JsonConfigPath> { }

[IsFilePath]
[EndsWith(".xml", StringComparison.OrdinalIgnoreCase)]
public sealed record XmlConfigPath : SemanticString<XmlConfigPath> { }

public class ConfigurationManager
{
    public T LoadJsonConfig<T>(JsonConfigPath configPath) where T : class
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configPath}");
        }

        string jsonContent = File.ReadAllText(configPath);
        // Would use JSON deserializer here
        Console.WriteLine($"Loading JSON config from: {configPath}");
        return default!; // Placeholder
    }

    public void SaveJsonConfig<T>(JsonConfigPath configPath, T config) where T : class
    {
        // Ensure directory exists
        string? directory = Path.GetDirectoryName(configPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Would use JSON serializer here
        Console.WriteLine($"Saving JSON config to: {configPath}");
    }
}

// Usage
var configManager = new ConfigurationManager();
var appConfigPath = @"config\appsettings.json".As<JsonConfigPath>();

// Load and save configurations with type safety
// configManager.LoadJsonConfig<AppSettings>(appConfigPath);
// configManager.SaveJsonConfig(appConfigPath, new AppSettings());
```

This path handling system provides type-safe file system operations while maintaining full compatibility with System.IO operations.
