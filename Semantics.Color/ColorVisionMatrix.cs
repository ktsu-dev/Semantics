// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Color;

/// <summary>
/// A 3x3 matrix taking linear RGB to the linear RGB a viewer with a color vision deficiency
/// perceives, as returned by <see cref="ColorVision.Matrix"/>. Rows produce red, green and blue.
/// </summary>
/// <param name="M11">Red output from red input.</param>
/// <param name="M12">Red output from green input.</param>
/// <param name="M13">Red output from blue input.</param>
/// <param name="M21">Green output from red input.</param>
/// <param name="M22">Green output from green input.</param>
/// <param name="M23">Green output from blue input.</param>
/// <param name="M31">Blue output from red input.</param>
/// <param name="M32">Blue output from green input.</param>
/// <param name="M33">Blue output from blue input.</param>
public readonly record struct ColorVisionMatrix(
	double M11, double M12, double M13,
	double M21, double M22, double M23,
	double M31, double M32, double M33)
{
	/// <summary>Gets the identity matrix, which is normal color vision.</summary>
	public static ColorVisionMatrix Identity { get; } = new(1, 0, 0, 0, 1, 0, 0, 0, 1);

	/// <summary>
	/// Applies this matrix to a color's linear channels, clamping each result to 0..1. Alpha is kept.
	/// </summary>
	/// <param name="color">The color to transform.</param>
	/// <returns>The color as the simulated viewer sees it.</returns>
	public Color Apply(Color color) => new(
		Color.Clamp01((M11 * color.R) + (M12 * color.G) + (M13 * color.B)),
		Color.Clamp01((M21 * color.R) + (M22 * color.G) + (M23 * color.B)),
		Color.Clamp01((M31 * color.R) + (M32 * color.G) + (M33 * color.B)),
		color.A);

	/// <summary>Interpolates linearly, entry by entry, between two matrices.</summary>
	/// <param name="from">The matrix at <paramref name="t"/> = 0.</param>
	/// <param name="to">The matrix at <paramref name="t"/> = 1.</param>
	/// <param name="t">The interpolation fraction.</param>
	/// <returns>The interpolated matrix.</returns>
	internal static ColorVisionMatrix Lerp(ColorVisionMatrix from, ColorVisionMatrix to, double t)
	{
		double s = 1.0 - t;
		return new(
			(s * from.M11) + (t * to.M11), (s * from.M12) + (t * to.M12), (s * from.M13) + (t * to.M13),
			(s * from.M21) + (t * to.M21), (s * from.M22) + (t * to.M22), (s * from.M23) + (t * to.M23),
			(s * from.M31) + (t * to.M31), (s * from.M32) + (t * to.M32), (s * from.M33) + (t * to.M33));
	}
}
