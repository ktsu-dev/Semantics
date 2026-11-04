// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

/// <summary>
/// Base class for directory paths (paths that represent directories)
/// </summary>
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
	/// An entry whose path this library cannot represent, for example one that fails path validation because its
	/// name holds a character such as <c>|</c> or its full path is too long, is skipped rather than failing the
	/// whole listing, so every entry that can be represented is still returned.
	/// <para>
	/// The returned types depend on the current directory type:
	/// <list type="bullet">
	/// <item><description><see cref="AbsoluteDirectoryPath"/> returns <see cref="AbsoluteFilePath"/> and <see cref="AbsoluteDirectoryPath"/> objects</description></item>
	/// <item><description><see cref="RelativeDirectoryPath"/> returns <see cref="RelativeFilePath"/> and <see cref="RelativeDirectoryPath"/> objects</description></item>
	/// <item><description><see cref="DirectoryPath"/> returns <see cref="FilePath"/> and <see cref="DirectoryPath"/> objects</description></item>
	/// </list>
	/// </para>
	/// </remarks>
	public virtual IEnumerable<IPath> GetContents()
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
				IPath? item = TryCreateEntry(entry);
				if (item is not null)
				{
					contents.Add(item);
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

	/// <summary>
	/// Wraps one directory entry as the appropriate path type, or returns <see langword="null"/> when the entry is
	/// neither a file nor a directory any more, or its path fails validation.
	/// </summary>
	/// <param name="entry">The entry's path, as listed.</param>
	/// <returns>The wrapped entry, or <see langword="null"/> to skip it.</returns>
	private IPath? TryCreateEntry(string entry)
	{
		try
		{
			if (File.Exists(entry))
			{
				return CreateFilePath(entry);
			}

			if (Directory.Exists(entry))
			{
				return CreateDirectoryPath(entry);
			}
		}
		catch (ArgumentException)
		{
			// The entry's path fails validation, which says nothing about its siblings.
		}

		return null;
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
