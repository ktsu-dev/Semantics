# Path Handling

This guide demonstrates specialized path handling features for working with file system paths in a type-safe manner.

## Basic Path Types

The library provides several path-specific semantic string types:

```csharp
using ktsu.Semantics;

// General path validation
[IsPath]
public sealed record AnyPath : SemanticString<AnyPath> { }

// Specific path types
[IsFilePath]
public sealed record FilePath : SemanticString<FilePath> { }

[IsDirectoryPath]
public sealed record DirectoryPath : SemanticString<DirectoryPath> { }

[IsFileName]
public sealed record FileName : SemanticString<FileName> { }

[IsExtension]
public sealed record FileExtension : SemanticString<FileExtension> { }

// Usage examples
var anyPath = @"C:\Users\John\Documents\report.pdf".As<AnyPath>();
var filePath = @"C:\Users\John\Documents\report.pdf".As<FilePath>();
var directoryPath = @"C:\Users\John\Documents".As<DirectoryPath>();
var fileName = "report.pdf".As<FileName>();
var extension = ".pdf".As<FileExtension>();
```

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
