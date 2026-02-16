// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;
nusing ktsu.Semantics.Strings;

/// <summary>
/// Base class for directory paths (paths that represent directories)
/// </summary>
[IsPath, IsDirectoryPath]
public abstract record SemanticDirectoryPath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticDirectoryPath<TDerived>
{
	/// <summary>
	/// Gets the files and directories contained in this directory as semantic path types.
	/// Files are returned as the appropriate file path type, and directories as the appropriate directory path type.
	/// </summary>
	/// <value>
	/// A collection of <see cref="IPath"/> objects representing the contents of the directory.
	/// Returns an empty collection if the directory doesn't exist or cannot be accessed.
	/// </value>
	/// <remarks>
	/// The returned types depend on the current directory type:
	/// <list type="bullet">
	/// <item><description><see cref="AbsoluteDirectoryPath"/> returns <see cref="AbsoluteFilePath"/> and <see cref="AbsoluteDirectoryPath"/> objects</description></item>
	/// <item><description><see cref="RelativeDirectoryPath"/> returns <see cref="RelativeFilePath"/> and <see cref="RelativeDirectoryPath"/> objects</description></item>
	/// <item><description><see cref="DirectoryPath"/> returns <see cref="FilePath"/> and <see cref="DirectoryPath"/> objects</description></item>
	/// </list>
	/// </remarks>
	public virtual IEnumerable<IPath> Contents
	{
		get
		{
			string directoryPath = WeakString;
			if (!Directory.Exists(directoryPath))
			{
				return [];
			}

			try
			{
				List<IPath> contents = [];

				// Get all files and directories
				string[] entries = Directory.GetFileSystemEntries(directoryPath);

				foreach (string entry in entries)
				{
					if (File.Exists(entry))
					{
						// It's a file - create appropriate file path type
						contents.Add(CreateFilePath(entry));
					}
					else if (Directory.Exists(entry))
					{
						// It's a directory - create appropriate directory path type
						contents.Add(CreateDirectoryPath(entry));
					}
				}

				return contents;
			}
			catch (UnauthorizedAccessException)
			{
				// Return empty collection if access denied
				return [];
			}
			catch (DirectoryNotFoundException)
			{
				// Return empty collection if directory not found
				return [];
			}
		}
	}

	/// <summary>
	/// Creates an appropriate file path type based on the current directory path type.
	/// </summary>
	/// <param name="filePath">The file path to wrap.</param>
	/// <returns>An <see cref="IFilePath"/> of the appropriate type.</returns>
	protected virtual IFilePath CreateFilePath(string filePath) =>
		FilePath.Create<FilePath>(filePath);

	/// <summary>
	/// Creates an appropriate directory path type based on the current directory path type.
	/// </summary>
	/// <param name="directoryPath">The directory path to wrap.</param>
	/// <returns>An <see cref="IDirectoryPath"/> of the appropriate type.</returns>
	protected virtual IDirectoryPath CreateDirectoryPath(string directoryPath) =>
		DirectoryPath.Create<DirectoryPath>(directoryPath);
}
