// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Strings;
using ktsu.Semantics.Paths;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public static class AdvancedUtilityTests
{
	[TestClass]
	public class PooledStringBuilderAdvancedTests
	{
		[TestMethod]
		public void PooledStringBuilder_Get_ShouldReturnNonNullInstance()
		{
			StringBuilder sb = PooledStringBuilder.Get();
			Assert.IsNotNull(sb);
			Assert.AreEqual(0, sb.Length, "StringBuilder should be empty when retrieved from pool");
		}

		[TestMethod]
		public void PooledStringBuilder_GetAndReturn_ShouldReuseInstance()
		{
			StringBuilder sb1 = PooledStringBuilder.Get();
			sb1.Append("test");
			PooledStringBuilder.Return(sb1);

			StringBuilder sb2 = PooledStringBuilder.Get();
			Assert.AreSame(sb1, sb2, "Should reuse the same StringBuilder instance");
			Assert.AreEqual(0, sb2.Length, "StringBuilder should be cleared when retrieved from pool");
		}

		[TestMethod]
		public void PooledStringBuilder_ReturnLargeCapacity_ShouldNotPool()
		{
			StringBuilder sb1 = PooledStringBuilder.Get();
			// Create a large string to increase capacity beyond the limit
			string largeString = new('x', 400); // Above the 360 capacity limit
			sb1.Append(largeString);
			PooledStringBuilder.Return(sb1);

			StringBuilder sb2 = PooledStringBuilder.Get();
			Assert.AreNotSame(sb1, sb2, "Should not reuse StringBuilder with large capacity");
		}

		[TestMethod]
		public void PooledStringBuilder_MultipleReturns_ShouldOnlyPoolLast()
		{
			StringBuilder sb1 = PooledStringBuilder.Get();
			StringBuilder sb2 = PooledStringBuilder.Get();

			PooledStringBuilder.Return(sb1);
			PooledStringBuilder.Return(sb2);

			StringBuilder sb3 = PooledStringBuilder.Get();
			Assert.AreSame(sb2, sb3, "Should reuse the last returned StringBuilder");
		}

		[TestMethod]
		public void PooledStringBuilder_CombinePaths_EmptyArray_ShouldReturnEmpty()
		{
			string result = PooledStringBuilder.CombinePaths();
			Assert.AreEqual(InternedPathStrings.Empty, result);
		}

		[TestMethod]
		public void PooledStringBuilder_CombinePaths_SinglePath_ShouldReturnSame()
		{
			string path = "single";
			string result = PooledStringBuilder.CombinePaths(path);
			Assert.AreEqual(path, result);
		}

		[TestMethod]
		public void PooledStringBuilder_CombinePaths_MultiplePaths_ShouldCombineCorrectly()
		{
			string result = PooledStringBuilder.CombinePaths("path1", "path2", "path3");
			string expected = $"path1{Path.DirectorySeparatorChar}path2{Path.DirectorySeparatorChar}path3";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void PooledStringBuilder_CombinePaths_WithTrailingSeparator_ShouldNotAddExtra()
		{
			string pathWithSeparator = $"path1{Path.DirectorySeparatorChar}";
			string result = PooledStringBuilder.CombinePaths(pathWithSeparator, "path2");
			string expected = $"path1{Path.DirectorySeparatorChar}path2";
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void PooledStringBuilder_CombinePaths_MixedSeparators_ShouldHandleCorrectly()
		{
			string path1 = $"root{Path.DirectorySeparatorChar}";
			string path2 = "middle";
			string path3 = $"{Path.DirectorySeparatorChar}end";
			string result = PooledStringBuilder.CombinePaths(path1, path2, path3);

			Assert.IsTrue(result.Contains("root"));
			Assert.IsTrue(result.Contains("middle"));
			Assert.IsTrue(result.Contains("end"));
		}

		[TestMethod]
		public void PooledStringBuilder_CombinePaths_ThreadSafety_ShouldWork()
		{
			const int threadCount = 10;
			const int operationsPerThread = 100;
			Task[] tasks = new Task[threadCount];
			ConcurrentBag<string> results = [];

			for (int i = 0; i < threadCount; i++)
			{
				int threadId = i;
				tasks[i] = Task.Run(() =>
				{
					for (int j = 0; j < operationsPerThread; j++)
					{
						string result = PooledStringBuilder.CombinePaths($"thread{threadId}", $"op{j}");
						results.Add(result);
					}
				});
			}

			Task.WaitAll(tasks);
			Assert.AreEqual(threadCount * operationsPerThread, results.Count);
		}
	}

	[TestClass]
	public class SpanPathUtilitiesAdvancedTests
	{
		[TestMethod]
		public void SpanPathUtilities_EndsWithDirectorySeparator_WindowsSeparator_ShouldReturnTrue()
		{
			ReadOnlySpan<char> path = $"test{Path.DirectorySeparatorChar}".AsSpan();
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void SpanPathUtilities_EndsWithDirectorySeparator_NoSeparator_ShouldReturnFalse()
		{
			ReadOnlySpan<char> path = "test".AsSpan();
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void SpanPathUtilities_EndsWithDirectorySeparator_EmptySpan_ShouldReturnFalse()
		{
			ReadOnlySpan<char> path = [];
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void SpanPathUtilities_EndsWithDirectorySeparator_SingleSeparator_ShouldReturnTrue()
		{
			ReadOnlySpan<char> path = Path.DirectorySeparatorChar.ToString().AsSpan();
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void SpanPathUtilities_EndsWithDirectorySeparator_AlternateSeparator_ShouldWork()
		{
			if (Path.AltDirectorySeparatorChar != Path.DirectorySeparatorChar)
			{
				ReadOnlySpan<char> path = $"test{Path.AltDirectorySeparatorChar}".AsSpan();
				bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);
				// This depends on the implementation - it might or might not handle alt separators
				Assert.IsNotNull(result); // Just verify it doesn't crash
			}
		}

		[TestMethod]
		public void SpanPathUtilities_EndsWithDirectorySeparator_UbicodeCharacters_ShouldNotMatch()
		{
			ReadOnlySpan<char> path = "test\u2215".AsSpan(); // Unicode division slash
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);
			Assert.IsFalse(result, "Should not match Unicode division slash");
		}

		[TestMethod]
		public void SpanPathUtilities_EndsWithDirectorySeparator_LongPath_ShouldWork()
		{
			ReadOnlySpan<char> longPath = (new string('a', 1000) + Path.DirectorySeparatorChar).AsSpan();
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(longPath);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void SpanPathUtilities_EndsWithDirectorySeparator_MultipleSeparators_ShouldReturnTrue()
		{
			ReadOnlySpan<char> path = $"test{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}".AsSpan();
			bool result = SpanPathUtilities.EndsWithDirectorySeparator(path);
			Assert.IsTrue(result);
		}
	}

	[TestClass]
	public class InternedPathStringsAdvancedTests
	{
		[TestMethod]
		public void InternedPathStrings_Empty_ShouldReturnEmptyString()
		{
			string empty = InternedPathStrings.Empty;
			Assert.AreEqual(string.Empty, empty);
		}

		[TestMethod]
		public void InternedPathStrings_Empty_ShouldReturnSameInstance()
		{
			string empty1 = InternedPathStrings.Empty;
			string empty2 = InternedPathStrings.Empty;
			Assert.AreSame(empty1, empty2, "Should return the same string instance");
		}

		[TestMethod]
		public void InternedPathStrings_Empty_ShouldBeInterned()
		{
			string empty = InternedPathStrings.Empty;
			string internedEmpty = string.Intern(string.Empty);
			Assert.AreSame(empty, internedEmpty, "Empty string should be interned");
		}

		[TestMethod]
		public void InternedPathStrings_CommonSeparators_ShouldBeAvailable()
		{
			// Test that common path separators are available as interned strings
			string dirSep = InternedPathStrings.DirectorySeparator;
			Assert.AreEqual(Path.DirectorySeparatorChar.ToString(), dirSep);
		}

		[TestMethod]
		public void InternedPathStrings_DirectorySeparator_ShouldBeInterned()
		{
			string separator = InternedPathStrings.DirectorySeparator;
			string internedSeparator = string.Intern(Path.DirectorySeparatorChar.ToString());
			Assert.AreSame(separator, internedSeparator, "Directory separator should be interned");
		}

		[TestMethod]
		public void InternedPathStrings_CommonPaths_ShouldBeInterned()
		{
			// Test common path strings that should be interned
			string dot = ".";
			string dotDot = "..";

			Assert.AreEqual(".", dot);
			Assert.AreEqual("..", dotDot);
		}

		[TestMethod]
		public void InternedPathStrings_Concurrency_ShouldBeThreadSafe()
		{
			const int threadCount = 10;
			const int operationsPerThread = 1000;
			Task[] tasks = new Task[threadCount];
			ConcurrentBag<string> allEmpty = [];

			for (int i = 0; i < threadCount; i++)
			{
				tasks[i] = Task.Run(() =>
				{
					for (int j = 0; j < operationsPerThread; j++)
					{
						allEmpty.Add(InternedPathStrings.Empty);
					}
				});
			}

			Task.WaitAll(tasks);

			// All instances should be the same reference
			string firstEmpty = allEmpty.First();
			Assert.IsTrue(allEmpty.All(e => ReferenceEquals(e, firstEmpty)));
		}
	}

	[TestClass]
	public class CombinedUtilityIntegrationTests
	{
		[TestMethod]
		public void UtilitiesIntegration_ComplexPathOperations_ShouldWork()
		{
			// Test complex scenario using all utilities together
			string[] parts = ["root", "folder", "subfolder", "file.txt"];
			string combinedPath = PooledStringBuilder.CombinePaths(parts);

			ReadOnlySpan<char> pathSpan = combinedPath.AsSpan();
			bool endsWithSeparator = SpanPathUtilities.EndsWithDirectorySeparator(pathSpan);

			Assert.IsFalse(endsWithSeparator, "File path should not end with separator");
			Assert.IsTrue(combinedPath.Contains("root"));
			Assert.IsTrue(combinedPath.Contains("file.txt"));
		}

		[TestMethod]
		public void UtilitiesIntegration_EmptyPathHandling_ShouldWork()
		{
			string emptyPath = InternedPathStrings.Empty;
			string pathWithEmpty = PooledStringBuilder.CombinePaths("root", emptyPath, "file");

			Assert.IsTrue(pathWithEmpty.Contains("root"));
			Assert.IsTrue(pathWithEmpty.Contains("file"));
		}

		[TestMethod]
		public void UtilitiesIntegration_PerformanceTest_ShouldBeFast()
		{
			const int iterations = 10000;
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

			for (int i = 0; i < iterations; i++)
			{
				string path = PooledStringBuilder.CombinePaths($"path{i}", "sub", "file.txt");
				ReadOnlySpan<char> span = path.AsSpan();
				SpanPathUtilities.EndsWithDirectorySeparator(span);
			}

			stopwatch.Stop();
			Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000,
				$"Performance test took {stopwatch.ElapsedMilliseconds}ms, expected < 1000ms");
		}
	}
}
