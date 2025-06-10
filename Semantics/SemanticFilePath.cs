// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Base class for file paths (paths that represent files)
/// </summary>
[IsPath, IsFilePath]
public abstract record SemanticFilePath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticFilePath<TDerived>
{
	/// <summary>
	/// Gets the file extension including the leading period, or empty if no extension
	/// </summary>
	public FileExtension FileExtension
	{
		get
		{
			ReadOnlySpan<char> span = WeakString.AsSpan();

			// Find the last dot
			int lastDotIndex = span.LastIndexOf('.');
			if (lastDotIndex == -1 || lastDotIndex == span.Length - 1)
			{
				// No extension or trailing dot
				return FileExtension.FromString<FileExtension>("");
			}

			// Return extension including the dot
			ReadOnlySpan<char> extension = span[lastDotIndex..];
			return FileExtension.FromString<FileExtension>(extension.ToString());
		}
	}

	/// <summary>
	/// Gets all trailing period-delimited segments including the leading period, or empty if no extensions
	/// </summary>
	public FileExtension FullFileExtension
	{
		get
		{
			ReadOnlySpan<char> span = WeakString.AsSpan();

			// Find the first dot
			int firstDotIndex = span.IndexOf('.');
			if (firstDotIndex == -1)
			{
				// No extension
				return FileExtension.FromString<FileExtension>("");
			}

			// Return everything from the first dot onward
			ReadOnlySpan<char> fullExtension = span[firstDotIndex..];
			return FileExtension.FromString<FileExtension>(fullExtension.ToString());
		}
	}

	/// <summary>
	/// Gets the filename portion of the path
	/// </summary>
	public FileName FileName => FileName.FromString<FileName>(Path.GetFileName(WeakString));

	/// <summary>
	/// Gets the directory portion of the path
	/// </summary>
	public DirectoryPath DirectoryPath => DirectoryPath.FromString<DirectoryPath>(Path.GetDirectoryName(WeakString) ?? "");
}
