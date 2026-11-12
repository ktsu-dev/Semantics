// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

/// <summary>
/// Base class for file paths (paths that represent files)
/// </summary>
[HasFileNameSegment]
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
	/// Follows the same rules as <see cref="FileExtension"/>: only the file-name segment is searched,
	/// a leading dot in the file name is not an extension, so <c>.bashrc</c> has none, and a trailing dot
	/// is not part of one, so <c>report.</c> has none and <c>archive.tar.</c> has <c>.tar</c>.
	/// </remarks>
	public FileExtension FullFileExtension
	{
		get
		{
			string path = WeakString;
			int nameStart = FileNameStart(path);

			// A trailing dot is not an extension, as FileExtension treats it
			int end = path.Length;
			if (end - 1 > nameStart && path[end - 1] == '.')
			{
				end--;
			}

			// Skip the name's first character, so a dotfile's leading dot is not taken as an extension
			int searchStart = nameStart + 1;
			int firstDotIndex = searchStart < end ? path.IndexOf('.', searchStart, end - searchStart) : -1;
			if (firstDotIndex == -1)
			{
				return FileExtension.Create<FileExtension>("");
			}

			return FileExtension.Create<FileExtension>(path.AsSpan(firstDotIndex, end - firstDotIndex));
		}
	}

	/// <summary>
	/// Returns the index of the first character of the file-name segment of <paramref name="path"/>.
	/// </summary>
	/// <param name="path">The path to inspect.</param>
	/// <returns>The index just past the last directory separator, or 0 if there is none.</returns>
	private static int FileNameStart(string path) => path.LastIndexOfAny(['/', '\\']) + 1;

	/// <summary>
	/// Gets the path with its <see cref="FileExtension"/> removed, so a dotfile such as <c>.bashrc</c>,
	/// which has no extension, keeps its whole name. A trailing dot after the name (<c>file.</c>) is
	/// removed too, as <see cref="Path.ChangeExtension(string, string)"/> removes it.
	/// </summary>
	private protected string WeakStringWithoutExtension
	{
		get
		{
			string path = WeakString;
			int length = path.Length - FileExtension.WeakString.Length;
			if (length == path.Length && length - 1 > FileNameStart(path) && path[length - 1] == '.')
			{
				length--;
			}

			return path.Substring(0, length);
		}
	}

	/// <summary>
	/// Gets the path with its <see cref="FileExtension"/> replaced by <paramref name="newExtension"/>, or
	/// with <paramref name="newExtension"/> appended when the path has none, as a dotfile does.
	/// </summary>
	/// <param name="newExtension">The new extension, with or without its leading period.</param>
	/// <returns>The path with the new extension.</returns>
	private protected string WeakStringWithExtension(FileExtension newExtension)
	{
		Ensure.NotNull(newExtension);

		string extension = newExtension.WeakString;
		return extension.Length > 0 && extension[0] == '.'
			? WeakStringWithoutExtension + extension
			: $"{WeakStringWithoutExtension}.{extension}";
	}

	/// <summary>
	/// Gets the filename portion of the path
	/// </summary>
	public FileName FileName => FileName.Create<FileName>(Path.GetFileName(WeakString));

	/// <summary>
	/// Gets the directory portion of the path
	/// </summary>
	/// <remarks>A bare file name's directory is the starting directory, written <c>.</c>.</remarks>
	public DirectoryPath DirectoryPath => DirectoryPath.Create<DirectoryPath>(Path.GetDirectoryName(WeakString) switch
	{
		"" => ".",
		var directory => directory ?? "",
	});
}
