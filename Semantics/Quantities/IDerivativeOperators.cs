// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Defines an interface for derivative operations between physical quantities.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the left operand, which must inherit from <see cref="PhysicalQuantity{TSelf}"/>
/// and implement <see cref="IDerivativeOperators{TSelf, TOther, TResult}"/>.
/// </typeparam>
/// <typeparam name="TOther">
/// The type of the right operand, which must inherit from <see cref="PhysicalQuantity{TOther}"/>.
/// </typeparam>
/// <typeparam name="TResult">
/// The type of the result, which must inherit from <see cref="PhysicalQuantity{TResult}"/>.
/// </typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "Required for type-safe physical quantity operations")]
public interface IDerivativeOperators<TSelf, TOther, TResult>
	: IDivisionOperators<TSelf, TOther, TResult>
	where TSelf : PhysicalQuantity<TSelf>, IDerivativeOperators<TSelf, TOther, TResult>, new()
	where TOther : PhysicalQuantity<TOther>, new()
	where TResult : PhysicalQuantity<TResult>, new()
{
	/// <summary>
	/// Computes the derivative of two physical quantities.
	/// </summary>
	/// <param name="left">The left operand of type <typeparamref name="TSelf"/>.</param>
	/// <param name="right">The right operand of type <typeparamref name="TOther"/>.</param>
	/// <returns>
	/// A new instance of <typeparamref name="TResult"/> representing the result of the derivative operation.
	/// </returns>
	public static TResult Derive(TSelf left, TOther right) =>
		PhysicalQuantity<TSelf>.Divide<TResult>(left, right);
}
