// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

/// <summary>
/// Base class for file paths (paths that represent files)
/// </summary>
public abstract record SemanticFilePath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticFilePath<TDerived>
{
	/// <summary>
	/// Gets the file extension including the leading period, or empty if no extension.
	/// </summary>
	/// <remarks>
	/// Only the file-name segment is searched, so a dot in a directory name (<c>./notes.txt</c>,
	/// <c>/opt/app.v2/README</c>) is never mistaken for an extension. A leading dot in the file name
	/// marks a dotfile rather than an extension: <c>.bashrc</c> has no extension, and
	/// <c>.config.json</c> has <c>.json</c>.
	/// </remarks>
	public FileExtension FileExtension
	{
		get
		{
			string path = WeakString;
			int nameStart = FileNameStart(path);

			// A dot at the start of the name, or before it, is not an extension; neither is a trailing dot
			int lastDotIndex = path.LastIndexOf('.');
			if (lastDotIndex <= nameStart || lastDotIndex == path.Length - 1)
			{
				return FileExtension.Create<FileExtension>("");
			}

			return FileExtension.Create<FileExtension>(path.AsSpan(lastDotIndex));
		}
	}

	/// <summary>
	/// Gets all trailing period-delimited segments including the leading period, or empty if no extensions.
	/// </summary>
	/// <remarks>
	/// Follows the same rules as <see cref="FileExtension"/>: only the file-name segment is searched, and
	/// a leading dot in the file name is not an extension, so <c>.bashrc</c> has none.
	/// </remarks>
	public FileExtension FullFileExtension
	{
		get
		{
			string path = WeakString;
			int nameStart = FileNameStart(path);

			// Skip the name's first character, so a dotfile's leading dot is not taken as an extension
			int firstDotIndex = nameStart + 1 < path.Length ? path.IndexOf('.', nameStart + 1) : -1;
			if (firstDotIndex == -1)
			{
				return FileExtension.Create<FileExtension>("");
			}

			return FileExtension.Create<FileExtension>(path.AsSpan(firstDotIndex));
		}
	}

	/// <summary>
	/// Returns the index of the first character of the file-name segment of <paramref name="path"/>.
	/// </summary>
	/// <param name="path">The path to inspect.</param>
	/// <returns>The index just past the last directory separator, or 0 if there is none.</returns>
	private static int FileNameStart(string path) => path.LastIndexOfAny(['/', '\\']) + 1;

	/// <summary>
	/// Gets the filename portion of the path
	/// </summary>
	public FileName FileName => FileName.Create<FileName>(Path.GetFileName(WeakString));

	/// <summary>
	/// Gets the directory portion of the path
	/// </summary>
	public DirectoryPath DirectoryPath => DirectoryPath.Create<DirectoryPath>(Path.GetDirectoryName(WeakString) ?? "");
}
