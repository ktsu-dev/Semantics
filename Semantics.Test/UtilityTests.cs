// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System.Text;
using ktsu.Semantics.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for utility classes that handle internal string operations and path parsing.
/// </summary>
[TestClass]
public static class UtilityTests
{
	/// <summary>
	/// Tests for PooledStringBuilder utility class.
	/// </summary>
	[TestClass]
	public class PooledStringBuilderTests
	{
		[TestMethod]
		public void Get_ReturnsStringBuilder()
		{
			// Act
			StringBuilder sb = PooledStringBuilder.Get();

			// Assert
			Assert.IsNotNull(sb);
			Assert.AreEqual(0, sb.Length);
		}

		[TestMethod]
		public void Get_After_Return_ReusesInstance()
		{
			// Arrange
			StringBuilder sb1 = PooledStringBuilder.Get();
			sb1.Append("test");

			// Act
			PooledStringBuilder.Return(sb1);
			StringBuilder sb2 = PooledStringBuilder.Get();

			// Assert
			Assert.AreSame(sb1, sb2);
			Assert.AreEqual(0, sb2.Length); // Should be cleared
		}

		[TestMethod]
		public void Return_WithLargeCapacity_DoesNotPool()
		{
			// Arrange
			StringBuilder sb1 = PooledStringBuilder.Get();
			sb1.EnsureCapacity(500); // Larger than 360 limit
			PooledStringBuilder.Return(sb1);

			// Act
			StringBuilder sb2 = PooledStringBuilder.Get();

			// Assert
			Assert.AreNotSame(sb1, sb2);
		}

		[TestMethod]
		public void CombinePaths_EmptyComponents_ReturnsEmpty()
		{
			// Act
			string result = PooledStringBuilder.CombinePaths();

			// Assert
			Assert.AreEqual(string.Empty, result);
		}

		[TestMethod]
		public void CombinePaths_SingleComponent_ReturnsSameComponent()
		{
			// Arrange
			string component = "test";

			// Act
			string result = PooledStringBuilder.CombinePaths(component);

			// Assert
			Assert.AreEqual(component, result);
		}

		[TestMethod]
		public void CombinePaths_MultipleComponents_CombinesWithSeparator()
		{
			// Arrange
			string[] components = ["folder1", "folder2", "file.txt"];

			// Act
			string result = PooledStringBuilder.CombinePaths(components.AsSpan());

			// Assert
			string expected = $"folder1{Path.DirectorySeparatorChar}folder2{Path.DirectorySeparatorChar}file.txt";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void CombinePaths_ComponentsWithTrailingSeparator_DoesNotDuplicate()
		{
			// Arrange
			string[] components = [$"folder1{Path.DirectorySeparatorChar}", "folder2", "file.txt"];

			// Act
			string result = PooledStringBuilder.CombinePaths(components.AsSpan());

			// Assert
			string expected = $"folder1{Path.DirectorySeparatorChar}folder2{Path.DirectorySeparatorChar}file.txt";
			Assert.AreEqual(expected, result);
		}
	}

	/// <summary>
	/// Tests for SpanPathUtilities utility class.
	/// </summary>
	[TestClass]
	public class SpanPathUtilitiesTests
	{
		[TestMethod]
		public void GetDirectoryName_EmptyPath_ReturnsEmpty()
		{
			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetDirectoryName([]);

			// Assert
			Assert.IsTrue(result.IsEmpty);
		}

		[TestMethod]
		public void GetDirectoryName_NoSeparator_ReturnsEmpty()
		{
			// Arrange
			ReadOnlySpan<char> path = "filename.txt".AsSpan();

			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetDirectoryName(path);

			// Assert
			Assert.IsTrue(result.IsEmpty);
		}

		[TestMethod]
		public void GetDirectoryName_WithDirectory_ReturnsDirectoryPart()
		{
			// Arrange
			ReadOnlySpan<char> path = $"folder{Path.DirectorySeparatorChar}file.txt".AsSpan();

			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetDirectoryName(path);

			// Assert
			Assert.AreEqual("folder", result.ToString());
		}

		[TestMethod]
		public void GetDirectoryName_WithNestedDirectories_ReturnsFullDirectoryPath()
		{
			// Arrange
			ReadOnlySpan<char> path = $"folder1{Path.DirectorySeparatorChar}folder2{Path.DirectorySeparatorChar}file.txt".AsSpan();

			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetDirectoryName(path);

			// Assert
			string expected = $"folder1{Path.DirectorySeparatorChar}folder2";
			Assert.AreEqual(expected, result.ToString());
		}

		[TestMethod]
		public void GetDirectoryName_WithAltSeparator_Works()
		{
			// Arrange
			ReadOnlySpan<char> path = $"folder{Path.AltDirectorySeparatorChar}file.txt".AsSpan();

			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetDirectoryName(path);

			// Assert
			Assert.AreEqual("folder", result.ToString());
		}

		[TestMethod]
		public void GetFileName_EmptyPath_ReturnsEmpty()
		{
			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetFileName([]);

			// Assert
			Assert.IsTrue(result.IsEmpty);
		}

		[TestMethod]
		public void GetFileName_NoSeparator_ReturnsWholePath()
		{
			// Arrange
			ReadOnlySpan<char> path = "filename.txt".AsSpan();

			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetFileName(path);

			// Assert
			Assert.AreEqual("filename.txt", result.ToString());
		}

		[TestMethod]
		public void GetFileName_WithDirectory_ReturnsFilenamePart()
		{
			// Arrange
			ReadOnlySpan<char> path = $"folder{Path.DirectorySeparatorChar}file.txt".AsSpan();

			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetFileName(path);

			// Assert
			Assert.AreEqual("file.txt", result.ToString());
		}

		[TestMethod]
		public void GetFileName_WithNestedDirectories_ReturnsFilenamePart()
		{
			// Arrange
			ReadOnlySpan<char> path = $"folder1{Path.DirectorySeparatorChar}folder2{Path.DirectorySeparatorChar}file.txt".AsSpan();

			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetFileName(path);

			// Assert
			Assert.AreEqual("file.txt", result.ToString());
		}

		[TestMethod]
		public void GetFileName_WithAltSeparator_Works()
		{
			// Arrange
			ReadOnlySpan<char> path = $"folder{Path.AltDirectorySeparatorChar}file.txt".AsSpan();

			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetFileName(path);

			// Assert
			Assert.AreEqual("file.txt", result.ToString());
		}

		[TestMethod]
		public void GetFileName_TrailingSeparator_ReturnsEmpty()
		{
			// Arrange
			ReadOnlySpan<char> path = $"folder{Path.DirectorySeparatorChar}".AsSpan();

			// Act
			ReadOnlySpan<char> result = SpanPathUtilities.GetFileName(path);

			// Assert
			Assert.IsTrue(result.IsEmpty);
		}

		[TestMethod]
		public void EndsWithDirectorySeparator_EmptyPath_ReturnsFalse()
		{
			// Act
			bool result = SpanPathUtilities.EndsWithDirectorySeparator([]);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void EndsWithDirectorySeparator_WithSeparator_ReturnsTrue()
		{
			// Arrange
			ReadOnlySpan<char> path = $"folder{Path.DirectorySeparatorChar}".AsSpan();

			// Act
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void EndsWithDirectorySeparator_WithAltSeparator_ReturnsTrue()
		{
			// Arrange
			ReadOnlySpan<char> path = $"folder{Path.AltDirectorySeparatorChar}".AsSpan();

			// Act
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void EndsWithDirectorySeparator_WithoutSeparator_ReturnsFalse()
		{
			// Arrange
			ReadOnlySpan<char> path = "folder".AsSpan();

			// Act
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);

			// Assert
			Assert.IsFalse(result);
		}
	}

	/// <summary>
	/// Tests for InternedPathStrings utility class.
	/// </summary>
	[TestClass]
	public class InternedPathStringsTests
	{
		[TestMethod]
		public void DirectorySeparator_IsInterned()
		{
			// Assert
			Assert.IsTrue(ReferenceEquals(InternedPathStrings.DirectorySeparator, string.Intern(Path.DirectorySeparatorChar.ToString())));
		}

		[TestMethod]
		public void AltDirectorySeparator_IsInterned()
		{
			// Assert
			Assert.IsTrue(ReferenceEquals(InternedPathStrings.AltDirectorySeparator, string.Intern(Path.AltDirectorySeparatorChar.ToString())));
		}

		[TestMethod]
		public void Empty_IsInterned()
		{
			// Assert
			Assert.IsTrue(ReferenceEquals(InternedPathStrings.Empty, string.Intern(string.Empty)));
		}

		[TestMethod]
		public void WindowsRoot_IsInterned()
		{
			// Assert
			Assert.IsTrue(ReferenceEquals(InternedPathStrings.WindowsRoot, string.Intern(@"C:\")));
		}

		[TestMethod]
		public void UnixRoot_IsInterned()
		{
			// Assert
			Assert.IsTrue(ReferenceEquals(InternedPathStrings.UnixRoot, string.Intern("/")));
		}

		[TestMethod]
		public void WindowsUncRoot_IsInterned()
		{
			// Assert
			Assert.IsTrue(ReferenceEquals(InternedPathStrings.WindowsUncRoot, string.Intern(@"\\")));
		}

		[TestMethod]
		public void InternIfCommon_NullValue_ReturnsEmpty()
		{
			// Act
			string result = InternedPathStrings.InternIfCommon(null!);

			// Assert
			Assert.AreSame(InternedPathStrings.Empty, result);
		}

		[TestMethod]
		public void InternIfCommon_EmptyValue_ReturnsEmpty()
		{
			// Act
			string result = InternedPathStrings.InternIfCommon(string.Empty);

			// Assert
			Assert.AreSame(InternedPathStrings.Empty, result);
		}

		[TestMethod]
		public void InternIfCommon_DirectorySeparator_ReturnsInternedSeparator()
		{
			// Act
			string result = InternedPathStrings.InternIfCommon(Path.DirectorySeparatorChar.ToString());

			// Assert
			Assert.AreSame(InternedPathStrings.DirectorySeparator, result);
		}

		[TestMethod]
		public void InternIfCommon_AltDirectorySeparator_ReturnsInternedAltSeparator()
		{
			// Act
			string result = InternedPathStrings.InternIfCommon(Path.AltDirectorySeparatorChar.ToString());

			// Assert
			Assert.AreSame(InternedPathStrings.AltDirectorySeparator, result);
		}

		[TestMethod]
		public void InternIfCommon_WindowsRoot_ReturnsInternedWindowsRoot()
		{
			// Act
			string result = InternedPathStrings.InternIfCommon(@"C:\");

			// Assert
			Assert.AreSame(InternedPathStrings.WindowsRoot, result);
		}

		[TestMethod]
		public void InternIfCommon_UnixRoot_ReturnsInternedUnixRoot()
		{
			// Act
			string result = InternedPathStrings.InternIfCommon("/");

			// Assert
			Assert.AreSame(InternedPathStrings.UnixRoot, result);
		}

		[TestMethod]
		public void InternIfCommon_WindowsUncRoot_ReturnsInternedWindowsUncRoot()
		{
			// Act
			string result = InternedPathStrings.InternIfCommon(@"\\");

			// Assert
			Assert.AreSame(InternedPathStrings.WindowsUncRoot, result);
		}

		[TestMethod]
		public void InternIfCommon_UncommonValue_ReturnsSameInstance()
		{
			// Arrange
			string uncommonValue = "some/uncommon/path";

			// Act
			string result = InternedPathStrings.InternIfCommon(uncommonValue);

			// Assert
			Assert.AreSame(uncommonValue, result);
		}

		[TestMethod]
		public void InternIfCommon_LongPath_ReturnsSameInstance()
		{
			// Arrange
			string longPath = "very/long/path/that/exceeds/the/length/limit";

			// Act
			string result = InternedPathStrings.InternIfCommon(longPath);

			// Assert
			Assert.AreSame(longPath, result);
		}
	}
}
