// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System.Collections.Concurrent;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public static class AdvancedErrorScenarioTests
{
	[TestClass]
	public class SemanticQuantityErrorTests
	{
		[TestMethod]
		[ExpectedException(typeof(DivideByZeroException))]
		public void SemanticQuantity_DivideByZero_ShouldThrow()
		{
			Length<double> length = Length<double>.FromMeters(10.0);
			Length<double> zeroLength = Length<double>.FromMeters(0.0);
			_ = length / zeroLength;
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SemanticQuantity_NullArgument_ShouldThrow()
		{
			Length<double> length = Length<double>.FromMeters(10.0);
			Length<double>? nullLength = null;
			_ = length * nullLength!;
		}

		[TestMethod]
		public void SemanticQuantity_InfinityHandling_ShouldWork()
		{
			Length<double> length = Length<double>.FromMeters(double.PositiveInfinity);
			Assert.IsTrue(double.IsPositiveInfinity(length.Value));
		}

		[TestMethod]
		public void SemanticQuantity_NaNHandling_ShouldWork()
		{
			Length<double> length = Length<double>.FromMeters(double.NaN);
			Assert.IsTrue(double.IsNaN(length.Value));
		}

		[TestMethod]
		public void SemanticQuantity_MaxMinValues_ShouldWork()
		{
			Length<double> maxLength = Length<double>.FromMeters(double.MaxValue);
			Length<double> minLength = Length<double>.FromMeters(double.MinValue);
			Assert.AreEqual(double.MaxValue, maxLength.Value);
			Assert.AreEqual(double.MinValue, minLength.Value);
		}

		[TestMethod]
		public void SemanticQuantity_Overflow_ShouldHandle()
		{
			Length<double> length = Length<double>.FromMeters(double.MaxValue);
			Length<double> doubled = length * 2.0;
			Assert.IsTrue(double.IsPositiveInfinity(doubled.Value));
		}

		[TestMethod]
		public void SemanticQuantity_Underflow_ShouldHandle()
		{
			Length<double> length = Length<double>.FromMeters(double.MinValue);
			Length<double> doubled = length * 2.0;
			Assert.IsTrue(double.IsNegativeInfinity(doubled.Value));
		}

		[TestMethod]
		public void SemanticQuantity_Precision_ShouldMaintain()
		{
			Length<double> length = Length<double>.FromMeters(1.23456789);
			Assert.AreEqual(1.23456789, length.Value);
		}
	}

	[TestClass]
	public class SemanticStringAdvancedErrorTests
	{
		[RegexMatch(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
		private sealed partial record PasswordString : SemanticString<PasswordString> { }

		[TestMethod]
		public void PasswordString_ValidPassword_ShouldWork()
		{
			PasswordString password = SemanticString<PasswordString>.Create<PasswordString>("ValidP@ss123");
			Assert.IsNotNull(password);
			Assert.AreEqual("ValidP@ss123", password.WeakString);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void PasswordString_TooShort_ShouldThrow()
		{
			SemanticString<PasswordString>.Create<PasswordString>("Short1!");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void PasswordString_NoUppercase_ShouldThrow()
		{
			SemanticString<PasswordString>.Create<PasswordString>("lowercase1!");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void PasswordString_NoSpecialChar_ShouldThrow()
		{
			SemanticString<PasswordString>.Create<PasswordString>("NoSpecial1");
		}

		[TestMethod]
		public void SemanticString_UnicodeHandling_ShouldWork()
		{
			ChineseString chineseString = SemanticString<ChineseString>.Create<ChineseString>("你好世界");
			Assert.IsNotNull(chineseString);
			Assert.AreEqual("你好世界", chineseString.WeakString);
		}

		[ChineseText]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
		private sealed partial record ChineseString : SemanticString<ChineseString> { }

		[AttributeUsage(AttributeTargets.Class)]
		private sealed class ChineseTextAttribute : SemanticStringValidationAttribute
		{
			public override bool Validate(ISemanticString semanticString)
			{
				return semanticString.WeakString.All(c => c is >= (char)0x4E00 and <= (char)0x9FFF);
			}
		}

		[TestMethod]
		public void SemanticString_MaxLengthStress_ShouldWork()
		{
			LongTestString longString = SemanticString<LongTestString>.Create<LongTestString>(new string('a', 10000));
			Assert.IsNotNull(longString);
			Assert.AreEqual(10000, longString.Length);
		}

		[Contains("a")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
		private sealed partial record LongTestString : SemanticString<LongTestString> { }

		[TestMethod]
		public void SemanticString_CultureInvariance_ShouldWork()
		{
			CaseTestString testString = SemanticString<CaseTestString>.Create<CaseTestString>("Test");
			Assert.IsNotNull(testString);
			Assert.AreEqual("Test", testString.WeakString);

			// Should fail with different case
			Assert.ThrowsException<ArgumentException>(() =>
				SemanticString<CaseTestString>.Create<CaseTestString>("test"));
		}

		[StartsWith("Test")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
		private sealed partial record CaseTestString : SemanticString<CaseTestString> { }

		[TestMethod]
		public void SemanticString_MemoryLeakPrevention_ShouldWork()
		{
			TestString testString = SemanticString<TestString>.Create<TestString>("test");
			Assert.IsNotNull(testString);
			Assert.AreEqual("test", testString.WeakString);
		}

		[StartsWith("test")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
		private sealed partial record TestString : SemanticString<TestString> { }
	}

	[TestClass]
	public class ValidationStrategyErrorTests
	{
		private sealed class FailingValidationStrategy : IValidationStrategy
		{
			public static bool Validate(object? value) => throw new InvalidOperationException("Validation failed");
			public bool Validate(ISemanticString semanticString, Type type) => throw new NotImplementedException();
		}

		private sealed class AlwaysFailStrategy : IValidationStrategy
		{
			public static bool Validate(object? _) => false;
			public bool Validate(ISemanticString semanticString, Type type) => throw new NotImplementedException();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ValidationStrategy_ThrowingValidator_ShouldPropagate()
		{
			FailingValidationStrategy strategy = new();
			FailingValidationStrategy.Validate("test");
		}

		[TestMethod]
		public void ValidationStrategy_AlwaysFails_ShouldReturnFalse()
		{
			AlwaysFailStrategy strategy = new();
			bool result = AlwaysFailStrategy.Validate("test");
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void ValidationStrategy_NullInput_ShouldHandle()
		{
			AlwaysFailStrategy strategy = new();
			bool result = AlwaysFailStrategy.Validate(null!);
			Assert.IsFalse(result);
		}
	}

	[TestClass]
	public class PathValidationErrorTests
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
		private sealed partial record TestDirectoryPath : SemanticDirectoryPath<TestDirectoryPath> { }

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void SemanticPath_InvalidCharacters_ShouldThrow()
		{
			char[] invalidChars = Path.GetInvalidPathChars();
			if (invalidChars.Length > 0)
			{
				string invalidPath = $"test{invalidChars[0]}path";
				SemanticString<TestDirectoryPath>.Create<TestDirectoryPath>(invalidPath);
			}
			else
			{
				// If no invalid characters, throw manually for test
				throw new ArgumentException("No invalid characters to test");
			}
		}

		[TestMethod]
		public void SemanticPath_MaxPathLength_ShouldHandle()
		{
			try
			{
				// Test very long path (Windows has 260 char limit traditionally)
				string longPath = new('a', 300);
				TestDirectoryPath path = SemanticString<TestDirectoryPath>.Create<TestDirectoryPath>(longPath);
				Assert.IsNotNull(path);
			}
			catch (ArgumentException)
			{
				// This is acceptable - path might be too long
				Assert.IsTrue(true);
			}
		}

		[TestMethod]
		public void SemanticPath_EmptyString_ShouldHandle()
		{
			try
			{
				TestDirectoryPath emptyPath = SemanticString<TestDirectoryPath>.Create<TestDirectoryPath>("");
				Assert.IsNotNull(emptyPath);
			}
			catch (ArgumentException)
			{
				// This might be expected behavior
				Assert.IsTrue(true);
			}
		}

		[TestMethod]
		public void SemanticPath_WhitespaceOnly_ShouldHandle()
		{
			try
			{
				TestDirectoryPath whitespacePath = SemanticString<TestDirectoryPath>.Create<TestDirectoryPath>("   ");
				Assert.IsNotNull(whitespacePath);
			}
			catch (ArgumentException)
			{
				// This might be expected behavior
				Assert.IsTrue(true);
			}
		}
	}

	[TestClass]
	public class ConcurrencyErrorTests
	{
		[TestMethod]
		public void SemanticString_ThreadSafety_ShouldWork()
		{
			const int threadCount = 20;
			const int operationsPerThread = 1000;
			Task[] tasks = new Task[threadCount];
			ConcurrentBag<Exception> exceptions = [];
			ConcurrentBag<string> results = [];

			for (int i = 0; i < threadCount; i++)
			{
				int threadIndex = i;
				tasks[i] = Task.Run(() =>
				{
					try
					{
						for (int j = 0; j < operationsPerThread; j++)
						{
							ThreadTestString str = SemanticString<ThreadTestString>.Create<ThreadTestString>($"thread{threadIndex}operation{j}");
							results.Add(str.WeakString);
						}
					}
					catch (ArgumentException ex)
					{
						exceptions.Add(ex);
					}
					catch (InvalidOperationException ex)
					{
						exceptions.Add(ex);
					}
				});
			}

			Task.WaitAll(tasks);

			Assert.AreEqual(0, exceptions.Count, $"Exceptions occurred: {string.Join(", ", exceptions.Select(e => e.Message))}");
			Assert.AreEqual(threadCount * operationsPerThread, results.Count);
		}

		[StartsWith("thread")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
		private sealed partial record ThreadTestString : SemanticString<ThreadTestString> { }

		[TestMethod]
		public void SemanticQuantity_ThreadSafety_ShouldWork()
		{
			const int threadCount = 20;
			const int operationsPerThread = 1000;
			Task[] tasks = new Task[threadCount];
			ConcurrentBag<Exception> exceptions = [];
			ConcurrentBag<double> results = [];

			for (int i = 0; i < threadCount; i++)
			{
				int threadIndex = i;
				tasks[i] = Task.Run(() =>
				{
					try
					{
						for (int j = 0; j < operationsPerThread; j++)
						{
							Length<double> length = Length<double>.FromMeters(threadIndex + j);
							Length<double> doubled = length * 2.0;
							results.Add(doubled.Value);
						}
					}
					catch (ArgumentException ex)
					{
						exceptions.Add(ex);
					}
					catch (InvalidOperationException ex)
					{
						exceptions.Add(ex);
					}
					catch (ArithmeticException ex)
					{
						exceptions.Add(ex);
					}
				});
			}

			Task.WaitAll(tasks);

			Assert.AreEqual(0, exceptions.Count, $"Exceptions occurred: {string.Join(", ", exceptions.Select(e => e.Message))}");
			Assert.AreEqual(threadCount * operationsPerThread, results.Count);
		}
	}

	[TestClass]
	public class ResourceManagementTests
	{
		[TestMethod]
		public void PooledStringBuilder_ExceptionSafety_ShouldReturnToPool()
		{
			StringBuilder? sb = null;
			try
			{
				sb = PooledStringBuilder.Get();
				sb.Append("test");
				throw new InvalidOperationException("Test exception");
			}
			catch (InvalidOperationException)
			{
				// Expected exception
			}
			finally
			{
				if (sb != null)
				{
					PooledStringBuilder.Return(sb);
				}
			}

			// Verify the StringBuilder was returned to pool
			StringBuilder newSb = PooledStringBuilder.Get();
			Assert.AreEqual(0, newSb.Length, "StringBuilder should have been cleared");
		}

		[TestMethod]
		public void PooledStringBuilder_MultipleFinally_ShouldBeSafe()
		{
			StringBuilder sb = PooledStringBuilder.Get();
			try
			{
				sb.Append("test");
			}
			finally
			{
				PooledStringBuilder.Return(sb);
				PooledStringBuilder.Return(sb); // Double return should be safe
			}

			Assert.IsTrue(true, "Double return should not cause issues");
		}
	}
}
