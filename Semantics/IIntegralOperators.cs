// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Defines an interface for integral operations between physical quantities.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the left operand, which must inherit from <see cref="PhysicalQuantity{TSelf}"/>
/// and implement <see cref="IIntegralOperators{TSelf, TOther, TResult}"/>.
/// </typeparam>
/// <typeparam name="TOther">
/// The type of the right operand, which must inherit from <see cref="PhysicalQuantity{TOther}"/>.
/// </typeparam>
/// <typeparam name="TResult">
/// The type of the result, which must inherit from <see cref="PhysicalQuantity{TResult}"/>.
/// </typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "Required for type-safe physical quantity operations")]
public interface IIntegralOperators<TSelf, TOther, TResult>
	: IMultiplyOperators<TSelf, TOther, TResult>
	where TSelf : PhysicalQuantity<TSelf>, IIntegralOperators<TSelf, TOther, TResult>, new()
	where TOther : PhysicalQuantity<TOther>, new()
	where TResult : PhysicalQuantity<TResult>, new()
{
	/// <summary>
	/// Performs an integration operation between two physical quantities.
	/// </summary>
	/// <param name="left">The left operand of type <typeparamref name="TSelf"/>.</param>
	/// <param name="right">The right operand of type <typeparamref name="TOther"/>.</param>
	/// <returns>
	/// A new instance of <typeparamref name="TResult"/> representing the result of the integration.
	/// </returns>
	public static TResult Integrate(TSelf left, TOther right) =>
		PhysicalQuantity<TSelf>.Multiply<TResult>(left, right);
}
