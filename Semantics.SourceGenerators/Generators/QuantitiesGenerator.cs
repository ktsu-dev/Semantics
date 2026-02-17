// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using System;
using System.Collections.Generic;
using System.Linq;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.Templates;

/// <summary>
/// Source generator that creates quantity record types from dimensions.json metadata.
/// Generates scalar quantity types with factory methods and physics relationship operators,
/// and vector quantity types implementing IVector2/3/4 interfaces.
/// </summary>
[Generator]
public class QuantitiesGenerator : GeneratorBase<DimensionsMetadata>
{
	public QuantitiesGenerator() : base("dimensions.json") { }

	protected override void Generate(SourceProductionContext context, DimensionsMetadata metadata, CodeBlocker codeBlocker)
	{
		if (metadata.PhysicalDimensions == null || metadata.PhysicalDimensions.Count == 0)
		{
			return;
		}

		// Build a map of quantity name -> definition for cross-reference validation
		Dictionary<string, QuantityDefinition> quantityMap = [];
		foreach (PhysicalDimension dimension in metadata.PhysicalDimensions)
		{
			foreach (QuantityDefinition quantity in dimension.Quantities)
			{
				quantityMap[quantity.Name] = quantity;
			}
		}

		// Generate types for each quantity
		foreach (PhysicalDimension dimension in metadata.PhysicalDimensions)
		{
			foreach (QuantityDefinition quantity in dimension.Quantities)
			{
				if (quantity.Scalar)
				{
					GenerateScalarQuantity(context, quantity, quantityMap);
				}

				if (quantity.Vectors)
				{
					GenerateVectorQuantity(context, quantity, 2);
					GenerateVectorQuantity(context, quantity, 3);
					GenerateVectorQuantity(context, quantity, 4);
				}
			}
		}
	}

	private void GenerateScalarQuantity(
		SourceProductionContext context,
		QuantityDefinition quantity,
		Dictionary<string, QuantityDefinition> quantityMap)
	{
		using CodeBlocker cb = CodeBlocker.Create();

		SourceFileTemplate sourceFile = new()
		{
			FileName = $"{quantity.Name}.g.cs",
			Namespace = "ktsu.Semantics.Quantities",
			Usings = ["System.Numerics"],
		};

		ClassTemplate quantityClass = new()
		{
			Comments =
			[
				"/// <summary>",
				$"/// {quantity.Description}",
				"/// </summary>",
				"/// <typeparam name=\"T\">The numeric storage type.</typeparam>",
			],
			Keywords = ["public", "record"],
			Name = $"{quantity.Name}<T>",
			BaseClass = $"SemanticQuantity<{quantity.Name}<T>, T>",
			Constraints = ["where T : struct, INumber<T>"],
		};

		// Factory method from the first available unit (the SI base unit)
		if (quantity.AvailableUnits.Count > 0)
		{
			string firstUnit = quantity.AvailableUnits[0];
			quantityClass.Members.Add(new MethodTemplate()
			{
				Comments =
				[
					"/// <summary>",
					$"/// Creates a new <see cref=\"{quantity.Name}{{T}}\"/> from a value in {firstUnit}.",
					"/// </summary>",
					$"/// <param name=\"value\">The value in {firstUnit}.</param>",
					$"/// <returns>A new <see cref=\"{quantity.Name}{{T}}\"/> instance.</returns>",
				],
				Keywords = ["public", "static", $"{quantity.Name}<T>"],
				Name = $"From{firstUnit}",
				Parameters = [new ParameterTemplate { Type = "T", Name = "value" }],
				BodyFactory = (body) => body.Write(" => Create(value);"),
			});
		}

		// Integral operators (multiplication): Self * Other = Result
		foreach (RelationshipDefinition integral in quantity.Integrals)
		{
			// Only generate if both referenced quantities exist and have scalar types
			if (!quantityMap.TryGetValue(integral.Other, out QuantityDefinition otherQty) || !otherQty.Scalar)
			{
				continue;
			}

			if (!quantityMap.TryGetValue(integral.Result, out QuantityDefinition resultQty) || !resultQty.Scalar)
			{
				continue;
			}

			quantityClass.Members.Add(new MethodTemplate()
			{
				Comments =
				[
					"/// <summary>",
					$"/// Multiplies {quantity.Name} by {integral.Other} to produce {integral.Result}.",
					"/// </summary>",
					"/// <param name=\"left\">The left operand.</param>",
					"/// <param name=\"right\">The right operand.</param>",
					$"/// <returns>The resulting <see cref=\"{integral.Result}{{T}}\"/>.</returns>",
				],
				Keywords = ["public", "static", $"{integral.Result}<T>"],
				Name = "operator *",
				Parameters =
				[
					new ParameterTemplate { Type = $"{quantity.Name}<T>", Name = "left" },
					new ParameterTemplate { Type = $"{integral.Other}<T>", Name = "right" },
				],
				BodyFactory = (body) => body.Write($" => Multiply<{integral.Result}<T>>(left, right);"),
			});
		}

		// Derivative operators (division): Self / Other = Result
		foreach (RelationshipDefinition derivative in quantity.Derivatives)
		{
			// Skip self-division (base class already handles TSelf / TSelf => TStorage)
			if (derivative.Other == quantity.Name)
			{
				continue;
			}

			// Only generate if both referenced quantities exist and have scalar types
			if (!quantityMap.TryGetValue(derivative.Other, out QuantityDefinition otherQty) || !otherQty.Scalar)
			{
				continue;
			}

			if (!quantityMap.TryGetValue(derivative.Result, out QuantityDefinition resultQty) || !resultQty.Scalar)
			{
				continue;
			}

			quantityClass.Members.Add(new MethodTemplate()
			{
				Comments =
				[
					"/// <summary>",
					$"/// Divides {quantity.Name} by {derivative.Other} to produce {derivative.Result}.",
					"/// </summary>",
					"/// <param name=\"left\">The left operand.</param>",
					"/// <param name=\"right\">The right operand.</param>",
					$"/// <returns>The resulting <see cref=\"{derivative.Result}{{T}}\"/>.</returns>",
				],
				Keywords = ["public", "static", $"{derivative.Result}<T>"],
				Name = "operator /",
				Parameters =
				[
					new ParameterTemplate { Type = $"{quantity.Name}<T>", Name = "left" },
					new ParameterTemplate { Type = $"{derivative.Other}<T>", Name = "right" },
				],
				BodyFactory = (body) => body.Write($" => Divide<{derivative.Result}<T>>(left, right);"),
			});
		}

		sourceFile.Classes.Add(quantityClass);
		WriteSourceFileTo(cb, sourceFile);
		context.AddSource(sourceFile.FileName, cb.ToString());
	}

	private void GenerateVectorQuantity(
		SourceProductionContext context,
		QuantityDefinition quantity,
		int dims)
	{
		string[] components = dims switch
		{
			2 => ["X", "Y"],
			3 => ["X", "Y", "Z"],
			4 => ["X", "Y", "Z", "W"],
			_ => throw new ArgumentOutOfRangeException(nameof(dims)),
		};

		string typeName = $"{quantity.Name}Vector{dims}";
		string fullType = $"{typeName}<T>";
		string interfaceName = $"IVector{dims}<{fullType}, T>";

		using CodeBlocker cb = CodeBlocker.Create();

		// Header and pragmas
		WriteHeaderTo(cb);
		cb.WriteLine("#pragma warning disable IDE0040 // Accessibility modifiers required");
		cb.WriteLine("#pragma warning disable CA2225 // Operator overloads have named alternates");
		cb.NewLine();

		// Namespace and usings
		cb.WriteLine("namespace ktsu.Semantics.Quantities;");
		cb.NewLine();
		cb.WriteLine("using System;");
		cb.WriteLine("using System.Numerics;");
		cb.NewLine();

		// Class declaration
		cb.WriteLine("/// <summary>");
		cb.WriteLine($"/// {dims}D vector representation of {quantity.Name}.");
		cb.WriteLine("/// </summary>");
		cb.WriteLine("/// <typeparam name=\"T\">The numeric component type.</typeparam>");
		cb.WriteLine($"public record {fullType} : {interfaceName}");
		cb.WriteLine("\twhere T : struct, INumber<T>");

		using (new Scope(cb))
		{
			WriteVectorComponentProperties(cb, components);
			WriteVectorStaticProperties(cb, fullType, components);
			WriteVectorMethods(cb, fullType, components, dims);
			WriteVectorOperators(cb, fullType, components);
		}

		context.AddSource($"{typeName}.g.cs", cb.ToString());
	}

	private static void WriteVectorComponentProperties(CodeBlocker cb, string[] components)
	{
		foreach (string comp in components)
		{
			cb.WriteLine($"/// <summary>Gets the {comp} component.</summary>");
			cb.WriteLine($"public T {comp} {{ get; init; }}");
			cb.NewLine();
		}
	}

	private static void WriteVectorStaticProperties(CodeBlocker cb, string fullType, string[] components)
	{
		// Zero
		string zeroInit = string.Join(", ", components.Select(c => $"{c} = T.Zero"));
		cb.WriteLine("/// <summary>Gets a vector with all components set to zero.</summary>");
		cb.WriteLine($"public static {fullType} Zero => new() {{ {zeroInit} }};");
		cb.NewLine();

		// One
		string oneInit = string.Join(", ", components.Select(c => $"{c} = T.One"));
		cb.WriteLine("/// <summary>Gets a vector with all components set to one.</summary>");
		cb.WriteLine($"public static {fullType} One => new() {{ {oneInit} }};");
		cb.NewLine();

		// Unit vectors
		foreach (string comp in components)
		{
			string unitInit = string.Join(", ", components.Select(c => $"{c} = {(c == comp ? "T.One" : "T.Zero")}"));
			cb.WriteLine($"/// <summary>Gets the unit vector for the {comp}-axis.</summary>");
			cb.WriteLine($"public static {fullType} Unit{comp} => new() {{ {unitInit} }};");
			cb.NewLine();
		}
	}

	private static void WriteVectorMethods(CodeBlocker cb, string fullType, string[] components, int dims)
	{
		string sumOfSquares = string.Join(" + ", components.Select(c => $"({c} * {c})"));

		// Length()
		cb.WriteLine("/// <summary>Calculates the length of the vector.</summary>");
		cb.WriteLine("/// <returns>The length of the vector.</returns>");
		cb.WriteLine("public T Length()");
		cb.WriteLine("{");
		cb.WriteLine($"\tT sum = {sumOfSquares};");
		cb.WriteLine("\tdouble asDouble = double.CreateChecked(sum);");
		cb.WriteLine("\treturn T.CreateChecked(Math.Sqrt(asDouble));");
		cb.WriteLine("}");
		cb.NewLine();

		// LengthSquared()
		cb.WriteLine("/// <summary>Calculates the squared length of the vector.</summary>");
		cb.WriteLine("/// <returns>The squared length of the vector.</returns>");
		cb.WriteLine($"public T LengthSquared() => {sumOfSquares};");
		cb.NewLine();

		// Dot()
		string dotExpr = string.Join(" + ", components.Select(c => $"({c} * other.{c})"));
		cb.WriteLine("/// <summary>Calculates the dot product of two vectors.</summary>");
		cb.WriteLine("/// <param name=\"other\">The other vector.</param>");
		cb.WriteLine("/// <returns>The dot product.</returns>");
		cb.WriteLine($"public T Dot({fullType} other) => {dotExpr};");
		cb.NewLine();

		// Cross() - Vector3 only
		if (dims == 3)
		{
			cb.WriteLine("/// <summary>Calculates the cross product of two vectors.</summary>");
			cb.WriteLine("/// <param name=\"other\">The other vector.</param>");
			cb.WriteLine("/// <returns>The cross product.</returns>");
			cb.WriteLine($"public {fullType} Cross({fullType} other)");
			cb.WriteLine("{");
			cb.WriteLine($"\treturn new() {{ X = (Y * other.Z) - (Z * other.Y), Y = (Z * other.X) - (X * other.Z), Z = (X * other.Y) - (Y * other.X) }};");
			cb.WriteLine("}");
			cb.NewLine();
		}

		// Distance()
		cb.WriteLine("/// <summary>Calculates the distance between two vectors.</summary>");
		cb.WriteLine("/// <param name=\"other\">The other vector.</param>");
		cb.WriteLine("/// <returns>The distance between the vectors.</returns>");
		cb.WriteLine($"public T Distance({fullType} other)");
		cb.WriteLine("{");
		foreach (string comp in components)
		{
			cb.WriteLine($"\tT d{comp} = {comp} - other.{comp};");
		}

		string distSum = string.Join(" + ", components.Select(c => $"(d{c} * d{c})"));
		cb.WriteLine($"\tT sum = {distSum};");
		cb.WriteLine("\tdouble asDouble = double.CreateChecked(sum);");
		cb.WriteLine("\treturn T.CreateChecked(Math.Sqrt(asDouble));");
		cb.WriteLine("}");
		cb.NewLine();

		// DistanceSquared()
		cb.WriteLine("/// <summary>Calculates the squared distance between two vectors.</summary>");
		cb.WriteLine("/// <param name=\"other\">The other vector.</param>");
		cb.WriteLine("/// <returns>The squared distance between the vectors.</returns>");
		cb.WriteLine($"public T DistanceSquared({fullType} other)");
		cb.WriteLine("{");
		foreach (string comp in components)
		{
			cb.WriteLine($"\tT d{comp} = {comp} - other.{comp};");
		}

		string distSqSum = string.Join(" + ", components.Select(c => $"(d{c} * d{c})"));
		cb.WriteLine($"\treturn {distSqSum};");
		cb.WriteLine("}");
		cb.NewLine();

		// Normalize()
		cb.WriteLine("/// <summary>Returns a normalized version of the vector.</summary>");
		cb.WriteLine("/// <returns>The normalized vector.</returns>");
		cb.WriteLine($"public {fullType} Normalize()");
		cb.WriteLine("{");
		cb.WriteLine("\tT len = Length();");
		string normInit = string.Join(", ", components.Select(c => $"{c} = {c} / len"));
		cb.WriteLine($"\treturn new() {{ {normInit} }};");
		cb.WriteLine("}");
		cb.NewLine();
	}

	private static void WriteVectorOperators(CodeBlocker cb, string fullType, string[] components)
	{
		// Addition
		string addInit = string.Join(", ", components.Select(c => $"{c} = left.{c} + right.{c}"));
		cb.WriteLine("/// <summary>Adds two vectors.</summary>");
		cb.WriteLine($"public static {fullType} operator +({fullType} left, {fullType} right) => new() {{ {addInit} }};");
		cb.NewLine();

		// Subtraction
		string subInit = string.Join(", ", components.Select(c => $"{c} = left.{c} - right.{c}"));
		cb.WriteLine("/// <summary>Subtracts two vectors.</summary>");
		cb.WriteLine($"public static {fullType} operator -({fullType} left, {fullType} right) => new() {{ {subInit} }};");
		cb.NewLine();

		// Scalar multiplication (vector * scalar)
		string mulInit = string.Join(", ", components.Select(c => $"{c} = vector.{c} * scalar"));
		cb.WriteLine("/// <summary>Multiplies a vector by a scalar.</summary>");
		cb.WriteLine($"public static {fullType} operator *({fullType} vector, T scalar) => new() {{ {mulInit} }};");
		cb.NewLine();

		// Scalar multiplication (scalar * vector)
		string mulRevInit = string.Join(", ", components.Select(c => $"{c} = scalar * vector.{c}"));
		cb.WriteLine("/// <summary>Multiplies a scalar by a vector.</summary>");
		cb.WriteLine($"public static {fullType} operator *(T scalar, {fullType} vector) => new() {{ {mulRevInit} }};");
		cb.NewLine();

		// Scalar division
		string divInit = string.Join(", ", components.Select(c => $"{c} = vector.{c} / scalar"));
		cb.WriteLine("/// <summary>Divides a vector by a scalar.</summary>");
		cb.WriteLine($"public static {fullType} operator /({fullType} vector, T scalar) => new() {{ {divInit} }};");
		cb.NewLine();

		// Unary negation
		string negInit = string.Join(", ", components.Select(c => $"{c} = -vector.{c}"));
		cb.WriteLine("/// <summary>Negates a vector.</summary>");
		cb.WriteLine($"public static {fullType} operator -({fullType} vector) => new() {{ {negInit} }};");
	}
}
