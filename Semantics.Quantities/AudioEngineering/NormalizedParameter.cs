// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Maps a host-normalized parameter position in <c>[0, 1]</c> onto a real value range, and back, using
/// a configurable <see cref="ParameterTaper"/>.
/// </summary>
/// <remarks>
/// Plugin hosts automate parameters as a normalized <c>[0, 1]</c> value, but a control's useful range is
/// rarely linear in that domain — frequency and gain feel right on a logarithmic taper, while many
/// controls want extra resolution at one end. <see cref="NormalizedParameter{T}"/> centralises that
/// mapping so an effect parameter can declare its range and taper once and convert in both directions:
/// <see cref="Denormalize"/> turns the host's <c>[0, 1]</c> into the real value, and
/// <see cref="Normalize"/> turns a real value back into <c>[0, 1]</c> for the host. Construct instances
/// with the factory methods rather than <see langword="default"/>.
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
public readonly record struct NormalizedParameter<T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the value at normalized position <c>0</c>.</summary>
	public T Min { get; }

	/// <summary>Gets the value at normalized position <c>1</c>.</summary>
	public T Max { get; }

	/// <summary>Gets the taper that shapes the mapping.</summary>
	public ParameterTaper Taper { get; }

	/// <summary>Gets the power-curve skew exponent (1 = unskewed).</summary>
	public T Skew { get; }

	private NormalizedParameter(T min, T max, ParameterTaper taper, T skew)
	{
		Min = min;
		Max = max;
		Taper = taper;
		Skew = skew;
	}

	/// <summary>
	/// Creates a linear parameter range.
	/// </summary>
	/// <param name="min">The value at normalized position <c>0</c>.</param>
	/// <param name="max">The value at normalized position <c>1</c>.</param>
	/// <returns>A new linear <see cref="NormalizedParameter{T}"/>.</returns>
	public static NormalizedParameter<T> Linear(T min, T max) => new(min, max, ParameterTaper.Linear, T.One);

	/// <summary>
	/// Creates a linear parameter range bent by a power-curve skew.
	/// </summary>
	/// <param name="min">The value at normalized position <c>0</c>.</param>
	/// <param name="max">The value at normalized position <c>1</c>.</param>
	/// <param name="skew">The skew exponent: greater than one biases resolution toward <paramref name="min"/>, less than one toward <paramref name="max"/>.</param>
	/// <returns>A new skewed <see cref="NormalizedParameter{T}"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="skew"/> is not positive.</exception>
	public static NormalizedParameter<T> Skewed(T min, T max, T skew)
	{
		if (T.IsNaN(skew) || skew <= T.Zero)
		{
			throw new ArgumentOutOfRangeException(nameof(skew), skew, "Skew must be a positive value.");
		}

		return new(min, max, ParameterTaper.Linear, skew);
	}

	/// <summary>
	/// Creates a logarithmic (constant-ratio) parameter range, suitable for frequency and gain controls.
	/// </summary>
	/// <param name="min">The value at normalized position <c>0</c>.</param>
	/// <param name="max">The value at normalized position <c>1</c>.</param>
	/// <returns>A new logarithmic <see cref="NormalizedParameter{T}"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="min"/> and <paramref name="max"/> are not both non-zero and of the same sign.</exception>
	public static NormalizedParameter<T> Logarithmic(T min, T max)
	{
		double lo = double.CreateChecked(min);
		double hi = double.CreateChecked(max);
		if (lo == 0.0 || hi == 0.0 || Math.Sign(lo) != Math.Sign(hi))
		{
			throw new ArgumentOutOfRangeException(nameof(min), "A logarithmic range requires min and max to be non-zero and of the same sign.");
		}

		return new(min, max, ParameterTaper.Logarithmic, T.One);
	}

	/// <summary>
	/// Creates a linear parameter range whose skew is chosen so that <paramref name="center"/> sits at the
	/// midpoint (normalized position <c>0.5</c>).
	/// </summary>
	/// <param name="min">The value at normalized position <c>0</c>.</param>
	/// <param name="max">The value at normalized position <c>1</c>.</param>
	/// <param name="center">The value that should map to the midpoint of the control; must lie strictly between <paramref name="min"/> and <paramref name="max"/>.</param>
	/// <returns>A new skewed <see cref="NormalizedParameter{T}"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="center"/> does not lie strictly between <paramref name="min"/> and <paramref name="max"/>.</exception>
	public static NormalizedParameter<T> WithCenter(T min, T max, T center)
	{
		double lo = double.CreateChecked(min);
		double hi = double.CreateChecked(max);
		double mid = double.CreateChecked(center);
		double proportion = (mid - lo) / (hi - lo);
		if (double.IsNaN(proportion) || proportion <= 0.0 || proportion >= 1.0)
		{
			throw new ArgumentOutOfRangeException(nameof(center), center, "Center must lie strictly between min and max.");
		}

		double skew = Math.Log(proportion) / Math.Log(0.5);
		return new(min, max, ParameterTaper.Linear, T.CreateChecked(skew));
	}

	/// <summary>
	/// Maps a host-normalized position in <c>[0, 1]</c> to the corresponding value in the range.
	/// </summary>
	/// <param name="normalized">The normalized position; values outside <c>[0, 1]</c> are clamped.</param>
	/// <returns>The corresponding value between <see cref="Min"/> and <see cref="Max"/>.</returns>
	public T Denormalize(T normalized)
	{
		double x = Math.Clamp(double.CreateChecked(normalized), 0.0, 1.0);
		double skew = double.CreateChecked(Skew);
		double lo = double.CreateChecked(Min);
		double hi = double.CreateChecked(Max);

		// Apply the power-curve skew first; a skew of one leaves x unchanged.
		double shaped = skew == 1.0 ? x : Math.Pow(x, skew);

		double value = Taper == ParameterTaper.Logarithmic
			? lo * Math.Pow(hi / lo, shaped)
			: lo + ((hi - lo) * shaped);

		return T.CreateChecked(value);
	}

	/// <summary>
	/// Maps a value in the range back to its host-normalized position in <c>[0, 1]</c>.
	/// </summary>
	/// <param name="value">The value; results outside <c>[0, 1]</c> are clamped.</param>
	/// <returns>The normalized position between <c>0</c> and <c>1</c>.</returns>
	public T Normalize(T value)
	{
		double v = double.CreateChecked(value);
		double skew = double.CreateChecked(Skew);
		double lo = double.CreateChecked(Min);
		double hi = double.CreateChecked(Max);

		double shaped = Taper == ParameterTaper.Logarithmic
			? Math.Log(v / lo) / Math.Log(hi / lo)
			: (v - lo) / (hi - lo);

		shaped = Math.Clamp(shaped, 0.0, 1.0);

		// Invert the power-curve skew.
		double x = skew == 1.0 ? shaped : Math.Pow(shaped, 1.0 / skew);

		return T.CreateChecked(Math.Clamp(x, 0.0, 1.0));
	}

	/// <summary>
	/// Clamps a value to the inclusive range spanned by <see cref="Min"/> and <see cref="Max"/>.
	/// </summary>
	/// <param name="value">The value to clamp.</param>
	/// <returns>The clamped value.</returns>
	public T Clamp(T value)
	{
		T low = T.Min(Min, Max);
		T high = T.Max(Min, Max);
		return T.Clamp(value, low, high);
	}
}
