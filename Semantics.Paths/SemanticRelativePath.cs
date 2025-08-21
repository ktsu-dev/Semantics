// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

/// <summary>
/// Base class for relative paths (not fully qualified)
/// </summary>
[IsPath, IsRelativePath]
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
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(from);
		ArgumentNullException.ThrowIfNull(to);
#else
		ArgumentNullExceptionPolyfill.ThrowIfNull(from);
		ArgumentNullExceptionPolyfill.ThrowIfNull(to);
#endif

		FileInfo fromInfo = new(Path.GetFullPath(from.WeakString));
		FileInfo toInfo = new(Path.GetFullPath(to.WeakString));

		// Use unix-style separators because they work on windows too
		const string separator = "/";
		const string altSeparator = "\\";

		string fromPath = Path.GetFullPath(fromInfo.FullName)
			.Replace(altSeparator, separator, StringComparison.Ordinal);
		string toPath = Path.GetFullPath(toInfo.FullName)
			.Replace(altSeparator, separator, StringComparison.Ordinal);

		// Handle directory paths - ensure they end with separator
		bool fromIsDirectory = IsDirectoryPath(from);
		bool toIsDirectory = IsDirectoryPath(to);

		if (fromIsDirectory && !fromPath.EndsWith(separator, StringComparison.Ordinal))
		{
			fromPath += separator;
		}

		if (toIsDirectory && !toPath.EndsWith(separator, StringComparison.Ordinal))
		{
			toPath += separator;
		}

		Uri fromUri = new(fromPath);
		Uri toUri = new(toPath);

		Uri relativeUri = fromUri.MakeRelativeUri(toUri);
		string relativePath = Uri.UnescapeDataString(relativeUri.ToString());
		relativePath = relativePath.Replace(altSeparator, separator, StringComparison.Ordinal);

		return Create<TRelativePath>(relativePath);
	}

	/// <summary>
	/// Determines whether the specified path type represents a directory path based on its validation attributes.
	/// </summary>
	/// <typeparam name="T">The type of semantic path to check.</typeparam>
	/// <param name="path">The path instance to check.</param>
	/// <returns><see langword="true"/> if the path type has the <see cref="IsDirectoryPathAttribute"/>; otherwise, <see langword="false"/>.</returns>
	private static bool IsDirectoryPath<T>(T path) where T : SemanticPath<T>
	{
		// Check if it's a directory-specific type based on validation attributes
		Type type = path.GetType();
		return type.GetCustomAttributes(typeof(IsDirectoryPathAttribute), true).Length > 0;
	}
}
