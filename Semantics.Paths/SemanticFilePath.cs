// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

/// <summary>
/// Base class for file paths (paths that represent files)
/// </summary>
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
#if NETSTANDARD2_0
			string span = WeakString;

			// Find the last dot
			int lastDotIndex = span.LastIndexOf('.');
			if (lastDotIndex == -1 || lastDotIndex == span.Length - 1)
			{
				// No extension or trailing dot
				return FileExtension.Create<FileExtension>("");
			}

			// Return extension including the dot
			string extension = span.Substring(lastDotIndex);
			return FileExtension.Create<FileExtension>(extension);
#else
			ReadOnlySpan<char> span = WeakString.AsSpan();

			// Find the last dot
			int lastDotIndex = span.LastIndexOf('.');
			if (lastDotIndex == -1 || lastDotIndex == span.Length - 1)
			{
				// No extension or trailing dot
				return FileExtension.Create<FileExtension>("");
			}

			// Return extension including the dot
			ReadOnlySpan<char> extension = span[lastDotIndex..];
			return FileExtension.Create<FileExtension>(extension.ToString());
#endif
		}
	}

	/// <summary>
	/// Gets all trailing period-delimited segments including the leading period, or empty if no extensions
	/// </summary>
	public FileExtension FullFileExtension
	{
		get
		{
#if NETSTANDARD2_0
			string span = WeakString;

			// Find the first dot
			int firstDotIndex = span.IndexOf('.');
			if (firstDotIndex == -1)
			{
				// No extension
				return FileExtension.Create<FileExtension>("");
			}

			// Return everything from the first dot onward
			string fullExtension = span.Substring(firstDotIndex);
			return FileExtension.Create<FileExtension>(fullExtension);
#else
			ReadOnlySpan<char> span = WeakString.AsSpan();

			// Find the first dot
			int firstDotIndex = span.IndexOf('.');
			if (firstDotIndex == -1)
			{
				// No extension
				return FileExtension.Create<FileExtension>("");
			}

			// Return everything from the first dot onward
			ReadOnlySpan<char> fullExtension = span[firstDotIndex..];
			return FileExtension.Create<FileExtension>(fullExtension.ToString());
#endif
		}
	}

	/// <summary>
	/// Gets the filename portion of the path
	/// </summary>
	public FileName FileName => FileName.Create<FileName>(Path.GetFileName(WeakString));

	/// <summary>
	/// Gets the directory portion of the path
	/// </summary>
	public DirectoryPath DirectoryPath => DirectoryPath.Create<DirectoryPath>(Path.GetDirectoryName(WeakString) ?? "");
}
