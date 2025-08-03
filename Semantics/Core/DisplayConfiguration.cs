// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Threading;

/// <summary>
/// Configuration for display-related conversions between pixels and physical units.
/// </summary>
public static class DisplayConfiguration
{
	/// <summary>
	/// Standard DPI (dots per inch) values commonly used in displays.
	/// </summary>
	public static class StandardDPI
	{
		/// <summary>Traditional print and Mac standard DPI.</summary>
		public const float Print = 72.0f;

		/// <summary>Standard Windows default DPI.</summary>
		public const float Windows = 96.0f;

		/// <summary>High-DPI Windows (125% scaling).</summary>
		public const float WindowsHighDPI = 120.0f;

		/// <summary>Higher DPI (150% scaling).</summary>
		public const float HighDPI = 144.0f;

		/// <summary>Very high DPI (200% scaling).</summary>
		public const float VeryHighDPI = 192.0f;

		/// <summary>Modern mobile/retina displays.</summary>
		public const float Retina = 300.0f;
	}

	private static float currentDPI = StandardDPI.Windows; // Default to Windows standard
	private static readonly Lock dpiLock = new();

	/// <summary>
	/// Gets or sets the current display DPI used for pixel-to-physical unit conversions.
	/// </summary>
	/// <remarks>
	/// This value is used globally for all pixel-to-meter conversions in vector quantities.
	/// Changes take effect immediately for new conversions.
	/// </remarks>
	public static float CurrentDPI
	{
		get
		{
			lock (dpiLock)
			{
				return currentDPI;
			}
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value), "DPI must be positive");
			}

			lock (dpiLock)
			{
				currentDPI = value;
			}
		}
	}

	/// <summary>
	/// Converts pixels to meters using the current DPI setting.
	/// </summary>
	/// <param name="pixels">The pixel value to convert.</param>
	/// <returns>The equivalent value in meters.</returns>
	/// <remarks>
	/// Conversion formula: meters = pixels / DPI * inches_to_meters
	/// Uses PhysicalConstants.Conversion.InchesToMeters for accurate conversion.
	/// </remarks>
	public static float PixelsToMeters(float pixels) => pixels / CurrentDPI * (float)PhysicalConstants.Conversion.InchesToMeters;

	/// <summary>
	/// Converts meters to pixels using the current DPI setting.
	/// </summary>
	/// <param name="meters">The meter value to convert.</param>
	/// <returns>The equivalent value in pixels.</returns>
	public static float MetersToPixels(float meters) => meters / (float)PhysicalConstants.Conversion.InchesToMeters * CurrentDPI;

	/// <summary>
	/// Gets the current pixel-to-meter conversion factor.
	/// </summary>
	/// <value>The conversion factor (multiply pixels by this to get meters).</value>
	public static float PixelToMeterFactor => (float)PhysicalConstants.Conversion.InchesToMeters / CurrentDPI;

	/// <summary>
	/// Gets the current meter-to-pixel conversion factor.
	/// </summary>
	/// <value>The conversion factor (multiply meters by this to get pixels).</value>
	public static float MeterToPixelFactor => CurrentDPI / (float)PhysicalConstants.Conversion.InchesToMeters;

	/// <summary>
	/// Sets the DPI to a standard value and returns the previous DPI.
	/// </summary>
	/// <param name="standardDPI">One of the standard DPI values.</param>
	/// <returns>The previous DPI value.</returns>
	public static float SetStandardDPI(float standardDPI)
	{
		float previousDPI = CurrentDPI;
		CurrentDPI = standardDPI;
		return previousDPI;
	}

	/// <summary>
	/// Auto-detects the system DPI if possible, falls back to Windows standard.
	/// </summary>
	/// <returns>The detected or default DPI value.</returns>
	/// <remarks>
	/// This is a placeholder for platform-specific DPI detection.
	/// In a real implementation, you'd use platform APIs to get the actual screen DPI.
	/// </remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "DPI detection is a fallback - should not crash application")]
	public static float DetectSystemDPI()
	{
		// Platform-specific DPI detection with better fallbacks
		try
		{
			// Attempt to detect based on environment
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				// Windows: Use environment variables or registry if available
				// This is still a simplified implementation
				string? dpiAwareness = Environment.GetEnvironmentVariable("DPI_AWARENESS");
				if (!string.IsNullOrEmpty(dpiAwareness) && float.TryParse(dpiAwareness, out float envDpi))
				{
					return envDpi;
				}
				return StandardDPI.Windows;
			}
			else if (Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX)
			{
				// Unix/macOS: Check environment variables
				string? xftDpi = Environment.GetEnvironmentVariable("Xft.dpi");
				if (!string.IsNullOrEmpty(xftDpi) && float.TryParse(xftDpi, out float unixDpi))
				{
					return unixDpi;
				}
				return StandardDPI.Print; // Unix typically uses 72 DPI as base
			}
		}
		catch (Exception)
		{
			// Ignore any exceptions in detection
		}

		// Default fallback based on platform
		return Environment.OSVersion.Platform == PlatformID.Win32NT
			? StandardDPI.Windows
			: StandardDPI.Print;
	}
}
