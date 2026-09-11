// @BANNER@
//
// The exponent vector every quantity is tagged with, and the arithmetic over it.
//
// Nothing here is derived from the metadata, which is why it is shipped rather than generated:
// eight integers and the four ways to combine them are the same whatever dimensions.json says.
// `Semantics.Cpp` writes this file out beside the vocabulary it does generate.
//
// Eight axes, not seven. SI has seven bases and files the radian under dimensionless, and this
// system keeps angle as an eighth because the alternative is that an angle is the same type as a
// ratio and an angular velocity the same type as a frequency -- so adding a heading to a ratio
// would compile. Measured against the real metadata, the eighth axis is what separates
// AngularDisplacement from Dimensionless and AngularVelocity from Frequency, and nothing else in
// the vocabulary changes.
//
// It does not separate everything, and it is not meant to: 72 dimensions share 63 exponent
// vectors, so Area and NuclearCrossSection, Torque and Energy, AbsorbedDose and EquivalentDose are
// each one vector between two names. Telling those apart is the generated classes' job. This layer
// exists for the other direction -- so that a product nobody declared still has a type.

#pragma once

namespace @NAMESPACE@
{

	// Every exponent defaults to zero, so a dimension is written as its non-zero prefix --
	// Dimension<1, 0, -2> is an acceleration -- rather than as eight numbers of which five are
	// noise.
	template <int Length = 0, int Mass = 0, int Time = 0, int Angle = 0, int Current = 0, int Temperature = 0,
		int Amount = 0, int Luminous = 0>
	struct Dimension
	{
		static constexpr int length = Length;
		static constexpr int mass = Mass;
		static constexpr int time = Time;
		static constexpr int angle = Angle;
		static constexpr int current = Current;
		static constexpr int temperature = Temperature;
		static constexpr int amount = Amount;
		static constexpr int luminous = Luminous;
	};

	template <typename A, typename B>
	using DimensionProduct =
		Dimension<A::length + B::length, A::mass + B::mass, A::time + B::time, A::angle + B::angle,
			A::current + B::current, A::temperature + B::temperature, A::amount + B::amount,
			A::luminous + B::luminous>;

	template <typename A, typename B>
	using DimensionQuotient =
		Dimension<A::length - B::length, A::mass - B::mass, A::time - B::time, A::angle - B::angle,
			A::current - B::current, A::temperature - B::temperature, A::amount - B::amount,
			A::luminous - B::luminous>;

	template <typename A>
	using DimensionInverse = Dimension<-A::length, -A::mass, -A::time, -A::angle, -A::current, -A::temperature,
		-A::amount, -A::luminous>;

	// Halving exponents is only meaningful when every one of them is even. The square root of an
	// area is a length; the square root of a length is not anything these base units can name.
	template <typename A>
	inline constexpr bool dimension_has_integer_root =
		(A::length % 2 == 0) && (A::mass % 2 == 0) && (A::time % 2 == 0) && (A::angle % 2 == 0) &&
		(A::current % 2 == 0) && (A::temperature % 2 == 0) && (A::amount % 2 == 0) && (A::luminous % 2 == 0);

	template <typename A>
	using DimensionSquareRoot = Dimension<A::length / 2, A::mass / 2, A::time / 2, A::angle / 2, A::current / 2,
		A::temperature / 2, A::amount / 2, A::luminous / 2>;

	using Dimensionless = Dimension<0>;

} // namespace @NAMESPACE@
