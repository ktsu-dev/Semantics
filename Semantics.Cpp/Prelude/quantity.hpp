// @BANNER@
//
// A value tagged with a dimension, and the arithmetic that combines them.
//
// Shipped rather than generated, for the same reason as dimension.hpp: none of it is derived from
// the metadata. What is generated sits on top of this -- one class per dimension and per named
// overload, each wrapping one of these.
//
// Every operation here is constexpr and trivially inlinable, and that is load-bearing rather than
// stylistic. The whole claim is that this costs nothing at run time; it has been measured against
// bare floats at 0.9896 on MSVC, 0.999 on GCC and 0.953 on clang. An earlier formulation of the
// generated layer above it measured 1.40 on MSVC while both others folded it away entirely, so the
// rules the generator follows when it writes that layer are not negotiable -- see the header of
// CppQuantityGenerator.

#pragma once

#include <cmath>
#include <compare>
#include <type_traits>

#include "dimension.hpp"

namespace @NAMESPACE@
{

	// `Rep` is the underlying storage, so a component can use a narrower representation without
	// changing its meaning.
	template <typename D, typename Rep = float>
	class Quantity
	{
	public:
		using dimension = D;
		using rep = Rep;

		constexpr Quantity() noexcept = default;

		// Explicit: a bare number never becomes a quantity by accident. Crossing into the type
		// system is a decision, and it should be visible at the point where it happens.
		explicit constexpr Quantity(Rep value) noexcept
			: value_(value)
		{
		}

		// Widening the representation of the same dimension is safe and implicit.
		template <typename R2>
			requires(!std::is_same_v<R2, Rep> && std::is_convertible_v<R2, Rep>)
		constexpr Quantity(Quantity<D, R2> other) noexcept
			: value_(static_cast<Rep>(other.count()))
		{
		}

		// Leaving the type system is also explicit, by being a named call.
		[[nodiscard]] constexpr Rep count() const noexcept { return value_; }

		constexpr Quantity& operator+=(Quantity rhs) noexcept
		{
			value_ += rhs.value_;
			return *this;
		}

		constexpr Quantity& operator-=(Quantity rhs) noexcept
		{
			value_ -= rhs.value_;
			return *this;
		}

		constexpr Quantity& operator*=(Rep scale) noexcept
		{
			value_ *= scale;
			return *this;
		}

		constexpr Quantity& operator/=(Rep scale) noexcept
		{
			value_ /= scale;
			return *this;
		}

		[[nodiscard]] friend constexpr Quantity operator+(Quantity lhs, Quantity rhs) noexcept
		{
			return Quantity{ lhs.value_ + rhs.value_ };
		}

		[[nodiscard]] friend constexpr Quantity operator-(Quantity lhs, Quantity rhs) noexcept
		{
			return Quantity{ lhs.value_ - rhs.value_ };
		}

		[[nodiscard]] friend constexpr Quantity operator-(Quantity q) noexcept { return Quantity{ -q.value_ }; }

		// Scaling by a bare number preserves the dimension.
		[[nodiscard]] friend constexpr Quantity operator*(Quantity q, Rep scale) noexcept
		{
			return Quantity{ q.value_ * scale };
		}

		[[nodiscard]] friend constexpr Quantity operator*(Rep scale, Quantity q) noexcept
		{
			return Quantity{ scale * q.value_ };
		}

		[[nodiscard]] friend constexpr Quantity operator/(Quantity q, Rep scale) noexcept
		{
			return Quantity{ q.value_ / scale };
		}

		[[nodiscard]] friend constexpr bool operator==(Quantity, Quantity) noexcept = default;
		[[nodiscard]] friend constexpr auto operator<=>(Quantity, Quantity) noexcept = default;

	private:
		Rep value_{};
	};

	// Multiplying and dividing quantities combines their dimensions. These live outside the class
	// so both operands participate in deduction.
	template <typename DA, typename DB, typename Rep>
	[[nodiscard]] constexpr auto operator*(Quantity<DA, Rep> a, Quantity<DB, Rep> b) noexcept
	{
		return Quantity<DimensionProduct<DA, DB>, Rep>{ a.count() * b.count() };
	}

	template <typename DA, typename DB, typename Rep>
	[[nodiscard]] constexpr auto operator/(Quantity<DA, Rep> a, Quantity<DB, Rep> b) noexcept
	{
		return Quantity<DimensionQuotient<DA, DB>, Rep>{ a.count() / b.count() };
	}

	// A bare number over a quantity yields the inverse dimension: 1 / Seconds is Hz.
	template <typename D, typename Rep>
	[[nodiscard]] constexpr auto operator/(Rep scale, Quantity<D, Rep> q) noexcept
	{
		return Quantity<DimensionInverse<D>, Rep>{ scale / q.count() };
	}

	// The square root of a quantity halves its dimension, which only exists when every exponent is
	// even. `sqrt(SquareMetres)` is a length; `sqrt(Metres)` is rejected at compile time.
	template <typename D, typename Rep>
	[[nodiscard]] auto sqrt(Quantity<D, Rep> q) noexcept
	{
		static_assert(dimension_has_integer_root<D>,
			"sqrt() is undefined for this dimension: halving its exponents does not yield a whole "
			"combination of base units");
		return Quantity<DimensionSquareRoot<D>, Rep>{ static_cast<Rep>(std::sqrt(q.count())) };
	}

} // namespace @NAMESPACE@
