// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

/// <summary>
/// Base class for relative paths (not fully qualified)
/// </summary>
[IsRelativePath]
public abstract record SemanticRelativePath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticRelativePath<TDerived>
{
	/// <summary>
	/// Creates a relative path from an absolute path to another absolute path
	/// </summary>
	public static TRelativePath Make<TRelativePath, TFromPath, TToPath>(TFromPath from, TToPath to)
		where TRelativePath : SemanticRelativePath<TRelativePath>
		where TFromPath : SemanticPath<TFromPath>
		where TToPath : SemanticPath<TToPath>
	{
		Ensure.NotNull(from);
		Ensure.NotNull(to);

		string fromPath = Path.GetFullPath(from.WeakString);
		string toPath = Path.GetFullPath(to.WeakString);

		// A path that is not a directory is resolved from the directory that contains it.
		string baseDirectory = IsDirectoryPath(from)
			? fromPath
			: Path.GetDirectoryName(fromPath) ?? fromPath;

		// Compare paths, not URIs: a URI decodes a literal "%2E" in a file name, which can name a
		// different file or climb out of the base directory.
		string relativePath = PathPolyfill.GetRelativePath(baseDirectory, toPath);

		// Use unix-style separators because they work on windows too
		const string separator = "/";
		const string altSeparator = "\\";
#if NETSTANDARD2_0
		relativePath = StringPolyfill.Replace(relativePath, altSeparator, separator, StringComparison.Ordinal);
#else
		relativePath = relativePath.Replace(altSeparator, separator, StringComparison.Ordinal);
#endif

		return Create<TRelativePath>(relativePath);
	}

	/// <summary>
	/// Determines whether the specified path represents a directory path.
	/// </summary>
	/// <typeparam name="T">The type of semantic path to check.</typeparam>
	/// <param name="path">The path instance to check.</param>
	/// <returns><see langword="true"/> if the path implements <see cref="IDirectoryPath"/>; otherwise, <see langword="false"/>.</returns>
	private static bool IsDirectoryPath<T>(T path) where T : SemanticPath<T> => path is IDirectoryPath;
}
