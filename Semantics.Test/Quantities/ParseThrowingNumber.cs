// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using System.Globalization;
using System.Numerics;

/// <summary>
/// A storage type backed by <see cref="double"/> whose parse methods throw <typeparamref name="TException"/>
/// instead of returning <see langword="false"/>, as a custom numeric type may for a
/// <see cref="NumberStyles"/> value it does not support.
/// </summary>
/// <typeparam name="TException">The exception every parse method throws.</typeparam>
/// <param name="Inner">The value.</param>
internal readonly record struct ParseThrowingNumber<TException>(double Inner) : INumber<ParseThrowingNumber<TException>>
	where TException : Exception, new()
{
	public static ParseThrowingNumber<TException> One => new(1d);

	public static int Radix => 2;

	public static ParseThrowingNumber<TException> Zero => new(0d);

	public static ParseThrowingNumber<TException> AdditiveIdentity => Zero;

	public static ParseThrowingNumber<TException> MultiplicativeIdentity => One;

	public static ParseThrowingNumber<TException> Abs(ParseThrowingNumber<TException> value) => new(Math.Abs(value.Inner));

	public static bool IsCanonical(ParseThrowingNumber<TException> value) => true;

	public static bool IsComplexNumber(ParseThrowingNumber<TException> value) => false;

	public static bool IsEvenInteger(ParseThrowingNumber<TException> value) => double.IsEvenInteger(value.Inner);

	public static bool IsFinite(ParseThrowingNumber<TException> value) => double.IsFinite(value.Inner);

	public static bool IsImaginaryNumber(ParseThrowingNumber<TException> value) => false;

	public static bool IsInfinity(ParseThrowingNumber<TException> value) => double.IsInfinity(value.Inner);

	public static bool IsInteger(ParseThrowingNumber<TException> value) => double.IsInteger(value.Inner);

	public static bool IsNaN(ParseThrowingNumber<TException> value) => double.IsNaN(value.Inner);

	public static bool IsNegative(ParseThrowingNumber<TException> value) => double.IsNegative(value.Inner);

	public static bool IsNegativeInfinity(ParseThrowingNumber<TException> value) => double.IsNegativeInfinity(value.Inner);

	public static bool IsNormal(ParseThrowingNumber<TException> value) => double.IsNormal(value.Inner);

	public static bool IsOddInteger(ParseThrowingNumber<TException> value) => double.IsOddInteger(value.Inner);

	public static bool IsPositive(ParseThrowingNumber<TException> value) => double.IsPositive(value.Inner);

	public static bool IsPositiveInfinity(ParseThrowingNumber<TException> value) => double.IsPositiveInfinity(value.Inner);

	public static bool IsRealNumber(ParseThrowingNumber<TException> value) => double.IsRealNumber(value.Inner);

	public static bool IsSubnormal(ParseThrowingNumber<TException> value) => double.IsSubnormal(value.Inner);

	public static bool IsZero(ParseThrowingNumber<TException> value) => value.Inner == 0d;

	public static ParseThrowingNumber<TException> MaxMagnitude(ParseThrowingNumber<TException> x, ParseThrowingNumber<TException> y) => new(double.MaxMagnitude(x.Inner, y.Inner));

	public static ParseThrowingNumber<TException> MaxMagnitudeNumber(ParseThrowingNumber<TException> x, ParseThrowingNumber<TException> y) => new(double.MaxMagnitudeNumber(x.Inner, y.Inner));

	public static ParseThrowingNumber<TException> MinMagnitude(ParseThrowingNumber<TException> x, ParseThrowingNumber<TException> y) => new(double.MinMagnitude(x.Inner, y.Inner));

	public static ParseThrowingNumber<TException> MinMagnitudeNumber(ParseThrowingNumber<TException> x, ParseThrowingNumber<TException> y) => new(double.MinMagnitudeNumber(x.Inner, y.Inner));

	public static ParseThrowingNumber<TException> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new TException();

	public static ParseThrowingNumber<TException> Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new TException();

	public static ParseThrowingNumber<TException> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new TException();

	public static ParseThrowingNumber<TException> Parse(string s, IFormatProvider? provider) => throw new TException();

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out ParseThrowingNumber<TException> result) => throw new TException();

	public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out ParseThrowingNumber<TException> result) => throw new TException();

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ParseThrowingNumber<TException> result) => throw new TException();

	public static bool TryParse(string? s, IFormatProvider? provider, out ParseThrowingNumber<TException> result) => throw new TException();

	public static bool TryConvertFromChecked<TOther>(TOther value, out ParseThrowingNumber<TException> result)
		where TOther : INumberBase<TOther>
	{
		result = new(double.CreateChecked(value));
		return true;
	}

	public static bool TryConvertFromSaturating<TOther>(TOther value, out ParseThrowingNumber<TException> result)
		where TOther : INumberBase<TOther>
	{
		result = new(double.CreateSaturating(value));
		return true;
	}

	public static bool TryConvertFromTruncating<TOther>(TOther value, out ParseThrowingNumber<TException> result)
		where TOther : INumberBase<TOther>
	{
		result = new(double.CreateTruncating(value));
		return true;
	}

	public static bool TryConvertToChecked<TOther>(ParseThrowingNumber<TException> value, out TOther result)
		where TOther : INumberBase<TOther>
	{
		result = TOther.CreateChecked(value.Inner);
		return true;
	}

	public static bool TryConvertToSaturating<TOther>(ParseThrowingNumber<TException> value, out TOther result)
		where TOther : INumberBase<TOther>
	{
		result = TOther.CreateSaturating(value.Inner);
		return true;
	}

	public static bool TryConvertToTruncating<TOther>(ParseThrowingNumber<TException> value, out TOther result)
		where TOther : INumberBase<TOther>
	{
		result = TOther.CreateTruncating(value.Inner);
		return true;
	}

	public static ParseThrowingNumber<TException> operator +(ParseThrowingNumber<TException> value) => value;

	public static ParseThrowingNumber<TException> operator -(ParseThrowingNumber<TException> value) => new(-value.Inner);

	public static ParseThrowingNumber<TException> operator ++(ParseThrowingNumber<TException> value) => new(value.Inner + 1d);

	public static ParseThrowingNumber<TException> operator --(ParseThrowingNumber<TException> value) => new(value.Inner - 1d);

	public static ParseThrowingNumber<TException> operator +(ParseThrowingNumber<TException> left, ParseThrowingNumber<TException> right) => new(left.Inner + right.Inner);

	public static ParseThrowingNumber<TException> operator -(ParseThrowingNumber<TException> left, ParseThrowingNumber<TException> right) => new(left.Inner - right.Inner);

	public static ParseThrowingNumber<TException> operator *(ParseThrowingNumber<TException> left, ParseThrowingNumber<TException> right) => new(left.Inner * right.Inner);

	public static ParseThrowingNumber<TException> operator /(ParseThrowingNumber<TException> left, ParseThrowingNumber<TException> right) => new(left.Inner / right.Inner);

	public static ParseThrowingNumber<TException> operator %(ParseThrowingNumber<TException> left, ParseThrowingNumber<TException> right) => new(left.Inner % right.Inner);

	public static bool operator <(ParseThrowingNumber<TException> left, ParseThrowingNumber<TException> right) => left.Inner < right.Inner;

	public static bool operator >(ParseThrowingNumber<TException> left, ParseThrowingNumber<TException> right) => left.Inner > right.Inner;

	public static bool operator <=(ParseThrowingNumber<TException> left, ParseThrowingNumber<TException> right) => left.Inner <= right.Inner;

	public static bool operator >=(ParseThrowingNumber<TException> left, ParseThrowingNumber<TException> right) => left.Inner >= right.Inner;

	public int CompareTo(object? obj) => obj switch
	{
		null => 1,
		ParseThrowingNumber<TException> other => CompareTo(other),
		_ => throw new ArgumentException("The object is not a ParseThrowingNumber of the same exception type.", nameof(obj)),
	};

	public int CompareTo(ParseThrowingNumber<TException> other) => Inner.CompareTo(other.Inner);

	public string ToString(string? format, IFormatProvider? formatProvider) => Inner.ToString(format, formatProvider);

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
		=> Inner.TryFormat(destination, out charsWritten, format, provider);
}
