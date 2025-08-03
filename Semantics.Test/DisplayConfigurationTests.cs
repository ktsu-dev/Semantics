// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for DisplayConfiguration system that handles DPI and pixel-to-physical unit conversions.
/// </summary>
[TestClass]
public class DisplayConfigurationTests
{
	private const float Tolerance = 1e-6f;

	[TestInitialize]
	public void TestInitialize()
	{
		// Reset to default DPI before each test
		DisplayConfiguration.CurrentDPI = DisplayConfiguration.StandardDPI.Windows;
	}

	[TestClass]
	public class StandardDPITests
	{
		[TestMethod]
		public void StandardDPI_Constants_ShouldHaveCorrectValues()
		{
			// Assert
			Assert.AreEqual(72.0f, DisplayConfiguration.StandardDPI.Print);
			Assert.AreEqual(96.0f, DisplayConfiguration.StandardDPI.Windows);
			Assert.AreEqual(120.0f, DisplayConfiguration.StandardDPI.WindowsHighDPI);
			Assert.AreEqual(144.0f, DisplayConfiguration.StandardDPI.HighDPI);
			Assert.AreEqual(192.0f, DisplayConfiguration.StandardDPI.VeryHighDPI);
			Assert.AreEqual(300.0f, DisplayConfiguration.StandardDPI.Retina);
		}
	}

	[TestClass]
	public class CurrentDPITests
	{
		[TestMethod]
		public void CurrentDPI_DefaultValue_ShouldBeWindowsStandard()
		{
			// Assert
			Assert.AreEqual(DisplayConfiguration.StandardDPI.Windows, DisplayConfiguration.CurrentDPI);
		}

		[TestMethod]
		public void CurrentDPI_SetValue_ShouldUpdate()
		{
			// Arrange & Act
			DisplayConfiguration.CurrentDPI = DisplayConfiguration.StandardDPI.Retina;

			// Assert
			Assert.AreEqual(DisplayConfiguration.StandardDPI.Retina, DisplayConfiguration.CurrentDPI);
		}

		[TestMethod]
		public void CurrentDPI_SetNegativeValue_ShouldThrowArgumentException()
		{
			// Act & Assert
			Assert.ThrowsException<ArgumentException>(() => 
			{
				DisplayConfiguration.CurrentDPI = -96.0f;
			});
		}

		[TestMethod]
		public void CurrentDPI_SetZeroValue_ShouldThrowArgumentException()
		{
			// Act & Assert
			Assert.ThrowsException<ArgumentException>(() => 
			{
				DisplayConfiguration.CurrentDPI = 0.0f;
			});
		}
	}

	[TestClass]
	public class ConversionTests
	{
		[TestMethod]
		public void PixelsToMeters_WindowsDPI_ShouldConvertCorrectly()
		{
			// Arrange
			DisplayConfiguration.CurrentDPI = 96.0f;
			float pixels = 96.0f; // Should equal 1 inch = 0.0254 meters

			// Act
			float meters = DisplayConfiguration.PixelsToMeters(pixels);

			// Assert
			Assert.AreEqual(0.0254f, meters, Tolerance);
		}

		[TestMethod]
		public void MetersToPixels_WindowsDPI_ShouldConvertCorrectly()
		{
			// Arrange
			DisplayConfiguration.CurrentDPI = 96.0f;
			float meters = 0.0254f; // 1 inch

			// Act
			float pixels = DisplayConfiguration.MetersToPixels(meters);

			// Assert
			Assert.AreEqual(96.0f, pixels, Tolerance);
		}

		[TestMethod]
		public void PixelsToMeters_RetinaDPI_ShouldConvertCorrectly()
		{
			// Arrange
			DisplayConfiguration.CurrentDPI = 300.0f;
			float pixels = 300.0f; // Should equal 1 inch = 0.0254 meters

			// Act
			float meters = DisplayConfiguration.PixelsToMeters(pixels);

			// Assert
			Assert.AreEqual(0.0254f, meters, Tolerance);
		}

		[TestMethod]
		public void MetersToPixels_RetinaDPI_ShouldConvertCorrectly()
		{
			// Arrange
			DisplayConfiguration.CurrentDPI = 300.0f;
			float meters = 0.0254f; // 1 inch

			// Act
			float pixels = DisplayConfiguration.MetersToPixels(meters);

			// Assert
			Assert.AreEqual(300.0f, pixels, Tolerance);
		}

		[TestMethod]
		public void RoundTripConversion_ShouldMaintainValue()
		{
			// Arrange
			DisplayConfiguration.CurrentDPI = 144.0f;
			float originalPixels = 288.0f;

			// Act
			float meters = DisplayConfiguration.PixelsToMeters(originalPixels);
			float backToPixels = DisplayConfiguration.MetersToPixels(meters);

			// Assert
			Assert.AreEqual(originalPixels, backToPixels, Tolerance);
		}

		[TestMethod]
		public void ConversionFactors_ShouldBeConsistent()
		{
			// Arrange
			float[] dpiValues = { 72.0f, 96.0f, 120.0f, 144.0f, 192.0f, 300.0f };

			foreach (float dpi in dpiValues)
			{
				// Arrange
				DisplayConfiguration.CurrentDPI = dpi;
				float testPixels = 100.0f;

				// Act
				float meters = DisplayConfiguration.PixelsToMeters(testPixels);
				float backToPixels = DisplayConfiguration.MetersToPixels(meters);

				// Assert
				Assert.AreEqual(testPixels, backToPixels, Tolerance, $"Failed for DPI: {dpi}");
			}
		}
	}

	[TestClass]
	public class ThreadSafetyTests
	{
		[TestMethod]
		public void CurrentDPI_ConcurrentAccess_ShouldBeThreadSafe()
		{
			// Arrange
			const int numThreads = 10;
			const int numIterations = 100;
			var tasks = new Task[numThreads];
			var exceptions = new List<Exception>();

			// Act
			for (int i = 0; i < numThreads; i++)
			{
				int threadId = i;
				tasks[i] = Task.Run(() =>
				{
					try
					{
						for (int j = 0; j < numIterations; j++)
						{
							// Alternate between different DPI values
							float dpi = threadId % 2 == 0 ? 96.0f : 300.0f;
							DisplayConfiguration.CurrentDPI = dpi;
							
							// Verify we can read the value
							float currentDpi = DisplayConfiguration.CurrentDPI;
							Assert.IsTrue(currentDpi > 0);
						}
					}
					catch (Exception ex)
					{
						lock (exceptions)
						{
							exceptions.Add(ex);
						}
					}
				});
			}

			Task.WaitAll(tasks);

			// Assert
			Assert.AreEqual(0, exceptions.Count, $"Thread safety test failed with {exceptions.Count} exceptions");
		}
	}

	[TestClass]
	public class PhysicalUnitConversionTests
	{
		[TestMethod]
		public void CommonScreenSizes_ShouldConvertToReasonablePhysicalSizes()
		{
			// Arrange - Common screen resolutions and DPI values
			var testCases = new[]
			{
				new { Width = 1920, Height = 1080, DPI = 96.0f, Name = "1080p at 96 DPI" },
				new { Width = 2560, Height = 1440, DPI = 144.0f, Name = "1440p at 144 DPI" },
				new { Width = 3840, Height = 2160, DPI = 192.0f, Name = "4K at 192 DPI" }
			};

			foreach (var testCase in testCases)
			{
				// Arrange
				DisplayConfiguration.CurrentDPI = testCase.DPI;

				// Act
				float widthMeters = DisplayConfiguration.PixelsToMeters(testCase.Width);
				float heightMeters = DisplayConfiguration.PixelsToMeters(testCase.Height);

				// Convert to more familiar units for verification
				float widthInches = widthMeters / 0.0254f;
				float heightInches = heightMeters / 0.0254f;

				// Assert - Screen sizes should be reasonable (between 10 and 50 inches)
				Assert.IsTrue(widthInches > 10 && widthInches < 50, 
					$"{testCase.Name}: Width {widthInches:F1} inches seems unreasonable");
				Assert.IsTrue(heightInches > 5 && heightInches < 30, 
					$"{testCase.Name}: Height {heightInches:F1} inches seems unreasonable");
			}
		}

		[TestMethod]
		public void OneInch_ShouldConvertTo254Millimeters()
		{
			// Arrange
			DisplayConfiguration.CurrentDPI = 96.0f;
			float oneInchInPixels = 96.0f;

			// Act
			float meters = DisplayConfiguration.PixelsToMeters(oneInchInPixels);
			float millimeters = meters * 1000.0f;

			// Assert
			Assert.AreEqual(25.4f, millimeters, Tolerance);
		}
	}

	[TestClass]
	public class EdgeCaseTests
	{
		[TestMethod]
		public void ZeroPixels_ShouldConvertToZeroMeters()
		{
			// Arrange
			DisplayConfiguration.CurrentDPI = 96.0f;

			// Act
			float meters = DisplayConfiguration.PixelsToMeters(0.0f);

			// Assert
			Assert.AreEqual(0.0f, meters, Tolerance);
		}

		[TestMethod]
		public void ZeroMeters_ShouldConvertToZeroPixels()
		{
			// Arrange
			DisplayConfiguration.CurrentDPI = 96.0f;

			// Act
			float pixels = DisplayConfiguration.MetersToPixels(0.0f);

			// Assert
			Assert.AreEqual(0.0f, pixels, Tolerance);
		}

		[TestMethod]
		public void VerySmallValues_ShouldMaintainPrecision()
		{
			// Arrange
			DisplayConfiguration.CurrentDPI = 300.0f;
			float verySmallPixels = 0.001f;

			// Act
			float meters = DisplayConfiguration.PixelsToMeters(verySmallPixels);
			float backToPixels = DisplayConfiguration.MetersToPixels(meters);

			// Assert
			Assert.AreEqual(verySmallPixels, backToPixels, 1e-9f);
		}
	}
}