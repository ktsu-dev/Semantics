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
/// Source generator that creates quantity types from the unified vector schema in dimensions.json.
/// Uses a two-phase approach: first collects all cross-dimensional operators globally,
/// then generates each type with its assigned operators.
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

		// Phase A: Build maps and collect operators
		Dictionary<string, PhysicalDimension> dimensionMap = BuildDimensionMap(metadata);
		Dictionary<string, int> typeFormMap = BuildTypeFormMap(metadata);
		List<OperatorInfo> allOperators = CollectAllOperators(metadata, dimensionMap);
		List<ProductInfo> allProducts = CollectAllProducts(metadata, dimensionMap);
		Dictionary<string, List<OperatorInfo>> operatorsByOwner = GroupBy(allOperators, o => o.OwnerTypeName);
		Dictionary<string, List<ProductInfo>> productsByOwner = GroupBy(allProducts, p => p.SelfTypeName);

		// Phase B: Generate types
		foreach (PhysicalDimension dim in metadata.PhysicalDimensions)
		{
			if (dim.Quantities.Vector0 != null)
			{
				EmitV0BaseType(context, dim, operatorsByOwner, typeFormMap);
			}

			if (dim.Quantities.Vector1 != null)
			{
				EmitV1BaseType(context, dim, operatorsByOwner, typeFormMap);
			}

			int[] vectorDims = [2, 3, 4];
			foreach (int d in vectorDims)
			{
				VectorFormDefinition? form = GetFormDef(dim, d);
				if (form != null)
				{
					EmitVectorType(context, dim, d, form, operatorsByOwner, productsByOwner, typeFormMap);
				}
			}

			// Emit semantic overloads for all vector forms
			int[] allForms = [0, 1, 2, 3, 4];
			foreach (int f in allForms)
			{
				VectorFormDefinition? form = GetFormDef(dim, f);
				if (form != null)
				{
					foreach (OverloadDefinition overload in form.Overloads)
					{
						EmitOverloadType(context, dim, f, form.Base, overload, typeFormMap);
					}
				}
			}
		}
	}

	#region Phase A: Map Building and Operator Collection

	private static Dictionary<string, PhysicalDimension> BuildDimensionMap(DimensionsMetadata metadata)
	{
		Dictionary<string, PhysicalDimension> map = [];
		foreach (PhysicalDimension dim in metadata.PhysicalDimensions)
		{
			map[dim.Name] = dim;
		}

		return map;
	}

	private static Dictionary<string, int> BuildTypeFormMap(DimensionsMetadata metadata)
	{
		Dictionary<string, int> map = [];
		foreach (PhysicalDimension dim in metadata.PhysicalDimensions)
		{
			int[] forms = [0, 1, 2, 3, 4];
			foreach (int f in forms)
			{
				VectorFormDefinition? form = GetFormDef(dim, f);
				if (form != null)
				{
					map[form.Base] = f;
					foreach (OverloadDefinition overload in form.Overloads)
					{
						map[overload.Name] = f;
					}
				}
			}
		}

		return map;
	}

	private static List<OperatorInfo> CollectAllOperators(DimensionsMetadata metadata, Dictionary<string, PhysicalDimension> dimMap)
	{
		HashSet<string> seen = [];
		List<OperatorInfo> result = [];

		foreach (PhysicalDimension dim in metadata.PhysicalDimensions)
		{
			// Process integrals: Self * Other = Result
			foreach (RelationshipDefinition integral in dim.Integrals)
			{
				if (!dimMap.TryGetValue(integral.Other, out PhysicalDimension? otherDim))
				{
					continue;
				}

				if (!dimMap.TryGetValue(integral.Result, out PhysicalDimension? resultDim))
				{
					continue;
				}

				// V0(Other) is the scalar multiplier
				string? v0Other = otherDim.Quantities.Vector0?.Base;
				if (v0Other == null)
				{
					continue;
				}

				int[] forms = [0, 1, 2, 3, 4];
				foreach (int vn in forms)
				{
					string? selfType = GetBaseTypeName(dim, vn);
					string? resultType = GetBaseTypeName(resultDim, vn);
					if (selfType == null || resultType == null)
					{
						continue;
					}

					// Forward: VN(Self) * V0(Other) => VN(Result)
					AddOp(result, seen, "*", selfType, v0Other, resultType, selfType);
					// Commutative: V0(Other) * VN(Self) => VN(Result)
					AddOp(result, seen, "*", v0Other, selfType, resultType, v0Other);
					// Inverse: VN(Result) / V0(Other) => VN(Self)
					AddOp(result, seen, "/", resultType, v0Other, selfType, resultType);
					// Inverse: VN(Result) / VN(Self) => V0(Other) -- only if VN == V0
					if (vn == 0)
					{
						AddOp(result, seen, "/", resultType, selfType, v0Other, resultType);
					}
				}
			}

			// Process derivatives: Self / Other = Result
			foreach (RelationshipDefinition derivative in dim.Derivatives)
			{
				if (!dimMap.TryGetValue(derivative.Other, out PhysicalDimension? otherDim))
				{
					continue;
				}

				if (!dimMap.TryGetValue(derivative.Result, out PhysicalDimension? resultDim))
				{
					continue;
				}

				string? v0Other = otherDim.Quantities.Vector0?.Base;
				if (v0Other == null)
				{
					continue;
				}

				int[] forms = [0, 1, 2, 3, 4];
				foreach (int vn in forms)
				{
					string? selfType = GetBaseTypeName(dim, vn);
					string? resultType = GetBaseTypeName(resultDim, vn);
					if (selfType == null || resultType == null)
					{
						continue;
					}

					// Forward: VN(Self) / V0(Other) => VN(Result)
					AddOp(result, seen, "/", selfType, v0Other, resultType, selfType);
					// Inverse integral: VN(Result) * V0(Other) => VN(Self)
					AddOp(result, seen, "*", resultType, v0Other, selfType, resultType);
					// Commutative inverse: V0(Other) * VN(Result) => VN(Self)
					AddOp(result, seen, "*", v0Other, resultType, selfType, v0Other);
				}
			}
		}

		return result;
	}

	private static List<ProductInfo> CollectAllProducts(DimensionsMetadata metadata, Dictionary<string, PhysicalDimension> dimMap)
	{
		HashSet<string> seen = [];
		List<ProductInfo> result = [];

		foreach (PhysicalDimension dim in metadata.PhysicalDimensions)
		{
			// Dot products: VN(Self) . VN(Other) => V0(Result)
			foreach (RelationshipDefinition dot in dim.DotProducts)
			{
				if (!dimMap.TryGetValue(dot.Other, out PhysicalDimension? otherDim))
				{
					continue;
				}

				if (!dimMap.TryGetValue(dot.Result, out PhysicalDimension? resultDim))
				{
					continue;
				}

				string? v0Result = resultDim.Quantities.Vector0?.Base;
				if (v0Result == null)
				{
					continue;
				}

				// Dot product for V1+ forms where both self and other have that form
				int[] forms = [1, 2, 3, 4];
				foreach (int vn in forms)
				{
					string? selfType = GetBaseTypeName(dim, vn);
					string? otherType = GetBaseTypeName(otherDim, vn);
					if (selfType == null || otherType == null)
					{
						continue;
					}

					string key = $"Dot:{selfType}:{otherType}:{v0Result}";
					if (seen.Add(key))
					{
						result.Add(new ProductInfo("Dot", selfType, otherType, v0Result, vn));
					}
				}
			}

			// Cross products: V3(Self) x V3(Other) => V3(Result)
			foreach (RelationshipDefinition cross in dim.CrossProducts)
			{
				if (!dimMap.TryGetValue(cross.Other, out PhysicalDimension? otherDim))
				{
					continue;
				}

				if (!dimMap.TryGetValue(cross.Result, out PhysicalDimension? resultDim))
				{
					continue;
				}

				string? selfV3 = GetBaseTypeName(dim, 3);
				string? otherV3 = GetBaseTypeName(otherDim, 3);
				string? resultV3 = GetBaseTypeName(resultDim, 3);
				if (selfV3 == null || otherV3 == null || resultV3 == null)
				{
					continue;
				}

				string key = $"Cross:{selfV3}:{otherV3}:{resultV3}";
				if (seen.Add(key))
				{
					result.Add(new ProductInfo("Cross", selfV3, otherV3, resultV3, 3));
				}
			}
		}

		return result;
	}

	private static void AddOp(List<OperatorInfo> list, HashSet<string> seen, string op, string left, string right, string ret, string owner)
	{
		// Skip self-division (base class already handles TSelf / TSelf => TStorage)
		if (op == "/" && left == right)
		{
			return;
		}

		string key = $"{op}:{left}:{right}:{ret}";
		if (seen.Add(key))
		{
			list.Add(new OperatorInfo(op, left, right, ret, owner));
		}
	}

	private static Dictionary<string, List<T>> GroupBy<T>(List<T> items, Func<T, string> keySelector)
	{
		Dictionary<string, List<T>> groups = [];
		foreach (T item in items)
		{
			string key = keySelector(item);
			if (!groups.TryGetValue(key, out List<T>? group))
			{
				group = [];
				groups[key] = group;
			}

			group.Add(item);
		}

		return groups;
	}

	#endregion

	#region Phase B: Type Generation

	private void EmitV0BaseType(
		SourceProductionContext context,
		PhysicalDimension dim,
		Dictionary<string, List<OperatorInfo>> operatorsByOwner,
		Dictionary<string, int> typeFormMap)
	{
		VectorFormDefinition v0 = dim.Quantities.Vector0!;
		string typeName = v0.Base;
		string fullType = $"{typeName}<T>";
		string? v1TypeName = dim.Quantities.Vector1?.Base;

		using CodeBlocker cb = CodeBlocker.Create();

		SourceFileTemplate sourceFile = new()
		{
			FileName = $"{typeName}.g.cs",
			Namespace = "ktsu.Semantics.Quantities",
			Usings = ["System.Numerics"],
		};

		ClassTemplate cls = new()
		{
			Comments =
			[
				"/// <summary>",
				$"/// Magnitude (Vector0) quantity for the {dim.Name} dimension.",
				"/// </summary>",
				"/// <typeparam name=\"T\">The numeric storage type.</typeparam>",
			],
			Keywords = ["public", "record"],
			Name = fullType,
			BaseClass = $"PhysicalQuantity<{fullType}, T>",
			Interfaces = [$"IVector0<{fullType}, T>"],
			Constraints = ["where T : struct, INumber<T>"],
		};

		// Zero property (satisfies IVector0)
		cls.Members.Add(new FieldTemplate()
		{
			Comments = ["/// <summary>Gets a quantity with value zero.</summary>"],
			Keywords = ["public", "static", fullType],
			Name = "Zero => Create(T.Zero)",
		});

		// Factory methods from available units
		if (dim.AvailableUnits.Count > 0)
		{
			string firstUnit = dim.AvailableUnits[0];
			cls.Members.Add(new MethodTemplate()
			{
				Comments =
				[
					"/// <summary>",
					$"/// Creates a new <see cref=\"{typeName}{{T}}\"/> from a value in {firstUnit}.",
					"/// </summary>",
					$"/// <param name=\"value\">The value in {firstUnit}.</param>",
					$"/// <returns>A new <see cref=\"{typeName}{{T}}\"/> instance.</returns>",
				],
				Keywords = ["public", "static", fullType],
				Name = $"From{firstUnit}",
				Parameters = [new ParameterTemplate { Type = "T", Name = "value" }],
				BodyFactory = (body) => body.Write(" => Create(value);"),
			});
		}

		// V0 subtraction hiding: returns V1 if V1 exists for this dimension
		if (v1TypeName != null)
		{
			cls.Members.Add(new MethodTemplate()
			{
				Comments =
				[
					"/// <summary>",
					$"/// Subtracts two {typeName} values, returning a signed {v1TypeName} result.",
					"/// </summary>",
				],
				Attributes = ["System.Diagnostics.CodeAnalysis.SuppressMessage(\"Usage\", \"CA2225:Operator overloads have named alternates\", Justification = \"Physics quantity operator\")"],
				Keywords = ["public", "static", $"{v1TypeName}<T>"],
				Name = "operator -",
				Parameters =
				[
					new ParameterTemplate { Type = fullType, Name = "left" },
					new ParameterTemplate { Type = fullType, Name = "right" },
				],
				BodyFactory = (body) => body.Write($" => {v1TypeName}<T>.Create(left.Quantity - right.Quantity);"),
			});
		}

		// Cross-dimensional operators
		EmitScalarOperators(cls, typeName, operatorsByOwner, typeFormMap);


		sourceFile.Classes.Add(cls);
		WriteSourceFileTo(cb, sourceFile);
		context.AddSource(sourceFile.FileName, cb.ToString());
	}

	private void EmitV1BaseType(
		SourceProductionContext context,
		PhysicalDimension dim,
		Dictionary<string, List<OperatorInfo>> operatorsByOwner,
		Dictionary<string, int> typeFormMap)
	{
		VectorFormDefinition v1 = dim.Quantities.Vector1!;
		string typeName = v1.Base;
		string fullType = $"{typeName}<T>";
		string? v0TypeName = dim.Quantities.Vector0?.Base;

		using CodeBlocker cb = CodeBlocker.Create();

		SourceFileTemplate sourceFile = new()
		{
			FileName = $"{typeName}.g.cs",
			Namespace = "ktsu.Semantics.Quantities",
			Usings = ["System.Numerics"],
		};

		ClassTemplate cls = new()
		{
			Comments =
			[
				"/// <summary>",
				$"/// Signed one-dimensional (Vector1) quantity for the {dim.Name} dimension.",
				"/// </summary>",
				"/// <typeparam name=\"T\">The numeric storage type.</typeparam>",
			],
			Keywords = ["public", "record"],
			Name = fullType,
			BaseClass = $"PhysicalQuantity<{fullType}, T>",
			Interfaces = [$"IVector1<{fullType}, T>"],
			Constraints = ["where T : struct, INumber<T>"],
		};

		// Zero property (satisfies IVector1)
		cls.Members.Add(new FieldTemplate()
		{
			Comments = ["/// <summary>Gets a quantity with value zero.</summary>"],
			Keywords = ["public", "static", fullType],
			Name = "Zero => Create(T.Zero)",
		});

		// Factory methods
		if (dim.AvailableUnits.Count > 0)
		{
			string firstUnit = dim.AvailableUnits[0];
			cls.Members.Add(new MethodTemplate()
			{
				Comments =
				[
					"/// <summary>",
					$"/// Creates a new <see cref=\"{typeName}{{T}}\"/> from a value in {firstUnit}.",
					"/// </summary>",
					$"/// <param name=\"value\">The value in {firstUnit}.</param>",
					$"/// <returns>A new <see cref=\"{typeName}{{T}}\"/> instance.</returns>",
				],
				Keywords = ["public", "static", fullType],
				Name = $"From{firstUnit}",
				Parameters = [new ParameterTemplate { Type = "T", Name = "value" }],
				BodyFactory = (body) => body.Write(" => Create(value);"),
			});
		}

		// Magnitude method returning V0 base
		if (v0TypeName != null)
		{
			cls.Members.Add(new MethodTemplate()
			{
				Comments =
				[
					"/// <summary>",
					$"/// Gets the magnitude of this quantity as a <see cref=\"{v0TypeName}{{T}}\"/>.",
					"/// </summary>",
					$"/// <returns>The non-negative magnitude.</returns>",
				],
				Keywords = ["public", $"{v0TypeName}<T>"],
				Name = "Magnitude",
				Parameters = [],
				BodyFactory = (body) => body.Write($" => {v0TypeName}<T>.Create(T.Abs(Value));"),
			});
		}

		// Cross-dimensional operators
		EmitScalarOperators(cls, typeName, operatorsByOwner, typeFormMap);

		sourceFile.Classes.Add(cls);
		WriteSourceFileTo(cb, sourceFile);
		context.AddSource(sourceFile.FileName, cb.ToString());
	}

	private void EmitVectorType(
		SourceProductionContext context,
		PhysicalDimension dim,
		int dims,
		VectorFormDefinition form,
		Dictionary<string, List<OperatorInfo>> operatorsByOwner,
		Dictionary<string, List<ProductInfo>> productsByOwner,
		Dictionary<string, int> typeFormMap)
	{
		string[] components = dims switch
		{
			2 => ["X", "Y"],
			3 => ["X", "Y", "Z"],
			4 => ["X", "Y", "Z", "W"],
			_ => throw new ArgumentOutOfRangeException(nameof(dims)),
		};

		string typeName = form.Base;
		string fullType = $"{typeName}<T>";
		string interfaceName = $"IVector{dims}<{fullType}, T>";
		string? v0TypeName = dim.Quantities.Vector0?.Base;

		using CodeBlocker cb = CodeBlocker.Create();

		WriteHeaderTo(cb);
		cb.WriteLine("#pragma warning disable IDE0040 // Accessibility modifiers required");
		cb.WriteLine("#pragma warning disable CA2225 // Operator overloads have named alternates");
		cb.NewLine();

		cb.WriteLine("namespace ktsu.Semantics.Quantities;");
		cb.NewLine();
		cb.WriteLine("using System;");
		cb.WriteLine("using System.Numerics;");
		cb.NewLine();

		cb.WriteLine("/// <summary>");
		cb.WriteLine($"/// {dims}D vector representation of {dim.Name}.");
		cb.WriteLine("/// </summary>");
		cb.WriteLine("/// <typeparam name=\"T\">The numeric component type.</typeparam>");
		cb.WriteLine($"public record {fullType} : {interfaceName}");
		cb.WriteLine("\twhere T : struct, INumber<T>");

		using (new ScopeWithTrailingSemicolon(cb))
		{
			WriteVectorComponentProperties(cb, components);
			WriteVectorStaticProperties(cb, fullType, components);

			// Typed Magnitude() method returning V0 base
			if (v0TypeName != null)
			{
				cb.WriteLine($"/// <summary>Gets the magnitude as a <see cref=\"{v0TypeName}{{T}}\"/>.</summary>");
				cb.WriteLine($"public {v0TypeName}<T> Magnitude() => {v0TypeName}<T>.Create(Length());");
				cb.NewLine();
			}

			WriteVectorMethods(cb, fullType, components, dims);
			WriteVectorOperators(cb, fullType, components);

			// Cross-dimensional operators (inlined for VN types)
			EmitVectorCrossDimOperators(cb, typeName, fullType, components, operatorsByOwner, typeFormMap);

			// Typed dot product methods
			if (productsByOwner.TryGetValue(typeName, out List<ProductInfo>? products))
			{
				foreach (ProductInfo prod in products)
				{
					if (prod.Method == "Dot")
					{
						string dotExpr = string.Join(" + ", components.Select(c => $"({c} * other.{c})"));
						cb.WriteLine($"/// <summary>Typed dot product: {typeName} . {prod.OtherTypeName} = {prod.ReturnTypeName}.</summary>");
						cb.WriteLine($"public {prod.ReturnTypeName}<T> Dot({prod.OtherTypeName}<T> other) => {prod.ReturnTypeName}<T>.Create({dotExpr});");
						cb.NewLine();
					}
					else if (prod.Method == "Cross" && dims == 3)
					{
						cb.WriteLine($"/// <summary>Typed cross product: {typeName} x {prod.OtherTypeName} = {prod.ReturnTypeName}.</summary>");
						cb.WriteLine($"public {prod.ReturnTypeName}<T> Cross({prod.OtherTypeName}<T> other)");
						using (new Scope(cb))
						{
							cb.WriteLine($"return new() {{ X = (Y * other.Z) - (Z * other.Y), Y = (Z * other.X) - (X * other.Z), Z = (X * other.Y) - (Y * other.X) }};");
						}

						cb.NewLine();
					}
				}
			}
		}

		context.AddSource($"{typeName}.g.cs", cb.ToString());
	}

	private void EmitOverloadType(
		SourceProductionContext context,
		PhysicalDimension dim,
		int vectorForm,
		string baseTypeName,
		OverloadDefinition overload,
		Dictionary<string, int> typeFormMap)
	{
		string typeName = overload.Name;
		string fullType = $"{typeName}<T>";
		string baseFullType = $"{baseTypeName}<T>";
		string? v1TypeName = dim.Quantities.Vector1?.Base;

		// V0/V1 overloads inherit from PhysicalQuantity
		if (vectorForm <= 1)
		{
			using CodeBlocker cb = CodeBlocker.Create();

			string interfaceName = vectorForm == 0 ? $"IVector0<{fullType}, T>" : $"IVector1<{fullType}, T>";

			SourceFileTemplate sourceFile = new()
			{
				FileName = $"{typeName}.g.cs",
				Namespace = "ktsu.Semantics.Quantities",
				Usings = ["System.Numerics"],
			};

			ClassTemplate cls = new()
			{
				Comments =
				[
					"/// <summary>",
					$"/// {overload.Description}",
					$"/// Semantic overload of <see cref=\"{baseTypeName}{{T}}\"/>.",
					"/// </summary>",
					"/// <typeparam name=\"T\">The numeric storage type.</typeparam>",
				],
				Keywords = ["public", "record"],
				Name = fullType,
				BaseClass = $"PhysicalQuantity<{fullType}, T>",
				Interfaces = [interfaceName],
				Constraints = ["where T : struct, INumber<T>"],
			};

			// Zero property
			cls.Members.Add(new FieldTemplate()
			{
				Comments = ["/// <summary>Gets a quantity with value zero.</summary>"],
				Keywords = ["public", "static", fullType],
				Name = "Zero => Create(T.Zero)",
			});

			// Factory methods
			if (dim.AvailableUnits.Count > 0)
			{
				string firstUnit = dim.AvailableUnits[0];
				cls.Members.Add(new MethodTemplate()
				{
					Comments = [$"/// <summary>Creates a new {typeName} from a value in {firstUnit}.</summary>"],
					Keywords = ["public", "static", fullType],
					Name = $"From{firstUnit}",
					Parameters = [new ParameterTemplate { Type = "T", Name = "value" }],
					BodyFactory = (body) => body.Write(" => Create(value);"),
				});
			}

			// Implicit widening to base type
			cls.Members.Add(new MethodTemplate()
			{
				Comments = [$"/// <summary>Implicit conversion to {baseTypeName}.</summary>"],
				Keywords = ["public", "static", "implicit", "operator"],
				Name = baseFullType,
				Parameters = [new ParameterTemplate { Type = fullType, Name = "value" }],
				BodyFactory = (body) => body.Write($" => {baseFullType}.Create(value.Value);"),
			});

			// Explicit narrowing from base type
			cls.Members.Add(new MethodTemplate()
			{
				Comments = [$"/// <summary>Explicit conversion from {baseTypeName}.</summary>"],
				Keywords = ["public", "static", "explicit", "operator"],
				Name = fullType,
				Parameters = [new ParameterTemplate { Type = baseFullType, Name = "value" }],
				BodyFactory = (body) => body.Write($" => Create(value.Value);"),
			});

			// Factory-style narrowing from base
			cls.Members.Add(new MethodTemplate()
			{
				Comments = [$"/// <summary>Creates a {typeName} from a {baseTypeName} value.</summary>"],
				Keywords = ["public", "static", fullType],
				Name = "From",
				Parameters = [new ParameterTemplate { Type = baseFullType, Name = "value" }],
				BodyFactory = (body) => body.Write(" => Create(value.Value);"),
			});

			// V0 overload subtraction hiding (returns V1 base if exists)
			if (vectorForm == 0 && v1TypeName != null)
			{
				cls.Members.Add(new MethodTemplate()
				{
					Comments = [$"/// <summary>Subtracts two {typeName} values, returning a signed {v1TypeName} result.</summary>"],
					Attributes = ["System.Diagnostics.CodeAnalysis.SuppressMessage(\"Usage\", \"CA2225:Operator overloads have named alternates\", Justification = \"Physics quantity operator\")"],
					Keywords = ["public", "static", $"{v1TypeName}<T>"],
					Name = "operator -",
					Parameters =
					[
						new ParameterTemplate { Type = fullType, Name = "left" },
						new ParameterTemplate { Type = fullType, Name = "right" },
					],
					BodyFactory = (body) => body.Write($" => {v1TypeName}<T>.Create(left.Quantity - right.Quantity);"),
				});
			}

			// Relationship methods (e.g., Diameter.ToRadius(), Diameter.FromRadius())
			foreach (KeyValuePair<string, string> rel in overload.Relationships)
			{
				// rel.Key is like "toRadius" or "fromRadius", rel.Value is the C# expression
				string methodName = char.ToUpperInvariant(rel.Key[0]) + rel.Key.Substring(1);

				if (methodName.StartsWith("To", StringComparison.Ordinal))
				{
					// Instance method: e.g., ToRadius() returns Radius<T>
					string targetName = methodName.Substring(2);
					string targetType = $"{targetName}<T>";
					string expr = rel.Value; // uses "Value" referring to this instance
					cls.Members.Add(new MethodTemplate()
					{
						Comments = [$"/// <summary>Converts this {typeName} to a {targetName}.</summary>"],
						Keywords = ["public", targetType],
						Name = methodName,
						Parameters = [],
						BodyFactory = (body) => body.Write($" => {targetType}.Create({expr});"),
					});
				}
				else if (methodName.StartsWith("From", StringComparison.Ordinal))
				{
					// Static factory: e.g., FromRadius(Radius<T> value) returns this type
					string sourceName = methodName.Substring(4);
					string sourceType = $"{sourceName}<T>";
					// Replace "Value" with "source.Value" since this is a static method
					string expr = rel.Value.Replace("Value", "source.Value");
					cls.Members.Add(new MethodTemplate()
					{
						Comments = [$"/// <summary>Creates a {typeName} from a {sourceName} value.</summary>"],
						Keywords = ["public", "static", fullType],
						Name = methodName,
						Parameters = [new ParameterTemplate { Type = sourceType, Name = "source" }],
						BodyFactory = (body) => body.Write($" => Create({expr});"),
					});
				}
			}

			sourceFile.Classes.Add(cls);
			WriteSourceFileTo(cb, sourceFile);
			context.AddSource(sourceFile.FileName, cb.ToString());
		}
		else
		{
			// V2/3/4 overloads: these are more complex, generate as standalone records
			// For now, V2+ overloads are rare and can be added later
			// The strategy document shows them mainly for V3 (Position3D, Translation3D)
			EmitVectorOverloadType(context, dim, vectorForm, baseTypeName, overload);
		}
	}

	private void EmitVectorOverloadType(
		SourceProductionContext context,
		PhysicalDimension dim,
		int dims,
		string baseTypeName,
		OverloadDefinition overload)
	{
		string[] components = dims switch
		{
			2 => ["X", "Y"],
			3 => ["X", "Y", "Z"],
			4 => ["X", "Y", "Z", "W"],
			_ => throw new ArgumentOutOfRangeException(nameof(dims)),
		};

		string typeName = overload.Name;
		string fullType = $"{typeName}<T>";
		string baseFullType = $"{baseTypeName}<T>";
		string interfaceName = $"IVector{dims}<{fullType}, T>";

		using CodeBlocker cb = CodeBlocker.Create();

		WriteHeaderTo(cb);
		cb.WriteLine("#pragma warning disable IDE0040 // Accessibility modifiers required");
		cb.WriteLine("#pragma warning disable CA2225 // Operator overloads have named alternates");
		cb.NewLine();

		cb.WriteLine("namespace ktsu.Semantics.Quantities;");
		cb.NewLine();
		cb.WriteLine("using System;");
		cb.WriteLine("using System.Numerics;");
		cb.NewLine();

		cb.WriteLine("/// <summary>");
		cb.WriteLine($"/// {overload.Description}");
		cb.WriteLine($"/// Semantic overload of <see cref=\"{baseTypeName}{{T}}\"/>.");
		cb.WriteLine("/// </summary>");
		cb.WriteLine($"public record {fullType} : {interfaceName}");
		cb.WriteLine("\twhere T : struct, INumber<T>");

		using (new ScopeWithTrailingSemicolon(cb))
		{
			WriteVectorComponentProperties(cb, components);
			WriteVectorStaticProperties(cb, fullType, components);
			WriteVectorMethods(cb, fullType, components, dims);
			WriteVectorOperators(cb, fullType, components);

			// Implicit widening to base
			string baseInit = string.Join(", ", components.Select(c => $"{c} = value.{c}"));
			cb.WriteLine($"/// <summary>Implicit conversion to {baseTypeName}.</summary>");
			cb.WriteLine($"public static implicit operator {baseFullType}({fullType} value) => new() {{ {baseInit} }};");
			cb.NewLine();

			// Explicit narrowing from base
			cb.WriteLine($"/// <summary>Explicit conversion from {baseTypeName}.</summary>");
			cb.WriteLine($"public static explicit operator {fullType}({baseFullType} value) => new() {{ {baseInit} }};");
			cb.NewLine();
		}

		context.AddSource($"{typeName}.g.cs", cb.ToString());
	}

	#endregion

	#region Operator Emission Helpers

	private static void EmitScalarOperators(
		ClassTemplate cls,
		string ownerTypeName,
		Dictionary<string, List<OperatorInfo>> operatorsByOwner,
		Dictionary<string, int> typeFormMap)
	{
		if (!operatorsByOwner.TryGetValue(ownerTypeName, out List<OperatorInfo>? ops))
		{
			return;
		}

		foreach (OperatorInfo op in ops)
		{
			int leftForm = GetFormOrDefault(typeFormMap, op.LeftTypeName);
			int rightForm = GetFormOrDefault(typeFormMap, op.RightTypeName);

			// For V0/V1 owner types, use Multiply/Divide helpers when both operands are V0/V1
			if (leftForm <= 1 && rightForm <= 1)
			{
				string helperName = op.Op == "*" ? "Multiply" : "Divide";
				cls.Members.Add(new MethodTemplate()
				{
					Comments =
					[
						"/// <summary>",
						$"/// {(op.Op == "*" ? "Multiplies" : "Divides")} {op.LeftTypeName} {(op.Op == "*" ? "by" : "by")} {op.RightTypeName} to produce {op.ReturnTypeName}.",
						"/// </summary>",
					],
					Attributes = ["System.Diagnostics.CodeAnalysis.SuppressMessage(\"Usage\", \"CA2225:Operator overloads have named alternates\", Justification = \"Physics quantity operator\")"],
					Keywords = ["public", "static", $"{op.ReturnTypeName}<T>"],
					Name = $"operator {op.Op}",
					Parameters =
					[
						new ParameterTemplate { Type = $"{op.LeftTypeName}<T>", Name = "left" },
						new ParameterTemplate { Type = $"{op.RightTypeName}<T>", Name = "right" },
					],
					BodyFactory = (body) => body.Write($" => {helperName}<{op.ReturnTypeName}<T>>(left, right);"),
				});
			}
			else
			{
				// One operand is V2+ (VN type, multi-component)
				// The owner is V0/V1, the other operand is VN
				// Generate inline: left.Value {op} right.X, etc. OR left.X {op} right.Value, etc.
				EmitInlineCrossDimOp(cls, op, typeFormMap);
			}
		}
	}

	private static void EmitInlineCrossDimOp(
		ClassTemplate cls,
		OperatorInfo op,
		Dictionary<string, int> typeFormMap)
	{
		int leftForm = GetFormOrDefault(typeFormMap, op.LeftTypeName);
		int rightForm = GetFormOrDefault(typeFormMap, op.RightTypeName);
		int resultForm = GetFormOrDefault(typeFormMap, op.ReturnTypeName);

		string[] resultComponents = resultForm switch
		{
			2 => ["X", "Y"],
			3 => ["X", "Y", "Z"],
			4 => ["X", "Y", "Z", "W"],
			_ => [],
		};

		if (resultComponents.Length == 0)
		{
			return;
		}

		string bodyExpr;
		if (leftForm >= 2 && rightForm <= 1)
		{
			// VN op V0: component-wise with right.Value
			string initExpr = string.Join(", ", resultComponents.Select(c => $"{c} = left.{c} {op.Op} right.Value"));
			bodyExpr = $" => new() {{ {initExpr} }};";
		}
		else if (leftForm <= 1 && rightForm >= 2)
		{
			// V0 op VN: component-wise with left.Value
			if (op.Op == "*")
			{
				string initExpr = string.Join(", ", resultComponents.Select(c => $"{c} = left.Value {op.Op} right.{c}"));
				bodyExpr = $" => new() {{ {initExpr} }};";
			}
			else
			{
				// V0 / VN doesn't make physical sense, skip
				return;
			}
		}
		else
		{
			return;
		}

		cls.Members.Add(new MethodTemplate()
		{
			Comments =
			[
				"/// <summary>",
				$"/// {(op.Op == "*" ? "Multiplies" : "Divides")} {op.LeftTypeName} {(op.Op == "*" ? "by" : "by")} {op.RightTypeName} to produce {op.ReturnTypeName}.",
				"/// </summary>",
			],
			Attributes = ["System.Diagnostics.CodeAnalysis.SuppressMessage(\"Usage\", \"CA2225:Operator overloads have named alternates\", Justification = \"Physics quantity operator\")"],
			Keywords = ["public", "static", $"{op.ReturnTypeName}<T>"],
			Name = $"operator {op.Op}",
			Parameters =
			[
				new ParameterTemplate { Type = $"{op.LeftTypeName}<T>", Name = "left" },
				new ParameterTemplate { Type = $"{op.RightTypeName}<T>", Name = "right" },
			],
			BodyFactory = (body) => body.Write(bodyExpr),
		});
	}

	private static void EmitVectorCrossDimOperators(
		CodeBlocker cb,
		string ownerTypeName,
		string ownerFullType,
		string[] components,
		Dictionary<string, List<OperatorInfo>> operatorsByOwner,
		Dictionary<string, int> typeFormMap)
	{
		if (!operatorsByOwner.TryGetValue(ownerTypeName, out List<OperatorInfo>? ops))
		{
			return;
		}

		foreach (OperatorInfo op in ops)
		{
			int leftForm = GetFormOrDefault(typeFormMap, op.LeftTypeName);
			int rightForm = GetFormOrDefault(typeFormMap, op.RightTypeName);
			int resultForm = GetFormOrDefault(typeFormMap, op.ReturnTypeName);

			string[] resultComponents = resultForm switch
			{
				2 => ["X", "Y"],
				3 => ["X", "Y", "Z"],
				4 => ["X", "Y", "Z", "W"],
				_ => [],
			};

			if (leftForm >= 2 && rightForm <= 1)
			{
				// VN * V0 or VN / V0 => VN result
				string initExpr = string.Join(", ", resultComponents.Select(c => $"{c} = left.{c} {op.Op} right.Value"));
				cb.WriteLine($"/// <summary>{op.LeftTypeName} {op.Op} {op.RightTypeName} = {op.ReturnTypeName}.</summary>");
				cb.WriteLine($"public static {op.ReturnTypeName}<T> operator {op.Op}({op.LeftTypeName}<T> left, {op.RightTypeName}<T> right) => new() {{ {initExpr} }};");
				cb.NewLine();
			}
			else if (leftForm <= 1 && rightForm >= 2 && op.Op == "*")
			{
				// V0 * VN => VN result (commutative multiplication)
				string initExpr = string.Join(", ", resultComponents.Select(c => $"{c} = left.Value * right.{c}"));
				cb.WriteLine($"/// <summary>{op.LeftTypeName} {op.Op} {op.RightTypeName} = {op.ReturnTypeName}.</summary>");
				cb.WriteLine($"public static {op.ReturnTypeName}<T> operator {op.Op}({op.LeftTypeName}<T> left, {op.RightTypeName}<T> right) => new() {{ {initExpr} }};");
				cb.NewLine();
			}
			else if (leftForm <= 1 && rightForm <= 1)
			{
				// Both V0/V1 - use Multiply/Divide. But wait, this owner is a VN type.
				// This shouldn't happen because VN types don't get V0*V0 operators assigned.
				// Skip.
			}
		}
	}

	#endregion

	#region Vector Generation Helpers (reused from original)

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
		string zeroInit = string.Join(", ", components.Select(c => $"{c} = T.Zero"));
		cb.WriteLine("/// <summary>Gets a vector with all components set to zero.</summary>");
		cb.WriteLine($"public static {fullType} Zero => new() {{ {zeroInit} }};");
		cb.NewLine();

		string oneInit = string.Join(", ", components.Select(c => $"{c} = T.One"));
		cb.WriteLine("/// <summary>Gets a vector with all components set to one.</summary>");
		cb.WriteLine($"public static {fullType} One => new() {{ {oneInit} }};");
		cb.NewLine();

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

		cb.WriteLine("/// <summary>Calculates the length of the vector.</summary>");
		cb.WriteLine("public T Length()");
		using (new Scope(cb))
		{
			cb.WriteLine($"T sum = {sumOfSquares};");
			cb.WriteLine("double asDouble = double.CreateChecked(sum);");
			cb.WriteLine("return T.CreateChecked(Math.Sqrt(asDouble));");
		}

		cb.NewLine();

		cb.WriteLine("/// <summary>Calculates the squared length of the vector.</summary>");
		cb.WriteLine($"public T LengthSquared() => {sumOfSquares};");
		cb.NewLine();

		string dotExpr = string.Join(" + ", components.Select(c => $"({c} * other.{c})"));
		cb.WriteLine("/// <summary>Calculates the dot product of two vectors.</summary>");
		cb.WriteLine($"public T Dot({fullType} other) => {dotExpr};");
		cb.NewLine();

		if (dims == 3)
		{
			cb.WriteLine("/// <summary>Calculates the cross product of two vectors.</summary>");
			cb.WriteLine($"public {fullType} Cross({fullType} other)");
			using (new Scope(cb))
			{
				cb.WriteLine($"return new() {{ X = (Y * other.Z) - (Z * other.Y), Y = (Z * other.X) - (X * other.Z), Z = (X * other.Y) - (Y * other.X) }};");
			}

			cb.NewLine();
		}

		cb.WriteLine("/// <summary>Calculates the distance between two vectors.</summary>");
		cb.WriteLine($"public T Distance({fullType} other)");
		using (new Scope(cb))
		{
			foreach (string comp in components)
			{
				cb.WriteLine($"T d{comp} = {comp} - other.{comp};");
			}

			string distSum = string.Join(" + ", components.Select(c => $"(d{c} * d{c})"));
			cb.WriteLine($"T sum = {distSum};");
			cb.WriteLine("double asDouble = double.CreateChecked(sum);");
			cb.WriteLine("return T.CreateChecked(Math.Sqrt(asDouble));");
		}

		cb.NewLine();

		cb.WriteLine("/// <summary>Calculates the squared distance between two vectors.</summary>");
		cb.WriteLine($"public T DistanceSquared({fullType} other)");
		using (new Scope(cb))
		{
			foreach (string comp in components)
			{
				cb.WriteLine($"T d{comp} = {comp} - other.{comp};");
			}

			string distSqSum = string.Join(" + ", components.Select(c => $"(d{c} * d{c})"));
			cb.WriteLine($"return {distSqSum};");
		}

		cb.NewLine();

		cb.WriteLine("/// <summary>Returns a normalized version of the vector.</summary>");
		cb.WriteLine($"public {fullType} Normalize()");
		using (new Scope(cb))
		{
			cb.WriteLine("T len = Length();");
			string normInit = string.Join(", ", components.Select(c => $"{c} = {c} / len"));
			cb.WriteLine($"return new() {{ {normInit} }};");
		}

		cb.NewLine();
	}

	private static void WriteVectorOperators(CodeBlocker cb, string fullType, string[] components)
	{
		string addInit = string.Join(", ", components.Select(c => $"{c} = left.{c} + right.{c}"));
		cb.WriteLine("/// <summary>Adds two vectors.</summary>");
		cb.WriteLine($"public static {fullType} operator +({fullType} left, {fullType} right) => new() {{ {addInit} }};");
		cb.NewLine();

		string subInit = string.Join(", ", components.Select(c => $"{c} = left.{c} - right.{c}"));
		cb.WriteLine("/// <summary>Subtracts two vectors.</summary>");
		cb.WriteLine($"public static {fullType} operator -({fullType} left, {fullType} right) => new() {{ {subInit} }};");
		cb.NewLine();

		string mulInit = string.Join(", ", components.Select(c => $"{c} = vector.{c} * scalar"));
		cb.WriteLine("/// <summary>Multiplies a vector by a scalar.</summary>");
		cb.WriteLine($"public static {fullType} operator *({fullType} vector, T scalar) => new() {{ {mulInit} }};");
		cb.NewLine();

		string mulRevInit = string.Join(", ", components.Select(c => $"{c} = scalar * vector.{c}"));
		cb.WriteLine("/// <summary>Multiplies a scalar by a vector.</summary>");
		cb.WriteLine($"public static {fullType} operator *(T scalar, {fullType} vector) => new() {{ {mulRevInit} }};");
		cb.NewLine();

		string divInit = string.Join(", ", components.Select(c => $"{c} = vector.{c} / scalar"));
		cb.WriteLine("/// <summary>Divides a vector by a scalar.</summary>");
		cb.WriteLine($"public static {fullType} operator /({fullType} vector, T scalar) => new() {{ {divInit} }};");
		cb.NewLine();

		string negInit = string.Join(", ", components.Select(c => $"{c} = -vector.{c}"));
		cb.WriteLine("/// <summary>Negates a vector.</summary>");
		cb.WriteLine($"public static {fullType} operator -({fullType} vector) => new() {{ {negInit} }};");
	}

	#endregion

	#region Helper Methods

	private static VectorFormDefinition? GetFormDef(PhysicalDimension dim, int form) => form switch
	{
		0 => dim.Quantities.Vector0,
		1 => dim.Quantities.Vector1,
		2 => dim.Quantities.Vector2,
		3 => dim.Quantities.Vector3,
		4 => dim.Quantities.Vector4,
		_ => null,
	};

	private static string? GetBaseTypeName(PhysicalDimension dim, int form) => GetFormDef(dim, form)?.Base;

	private static int GetFormOrDefault(Dictionary<string, int> map, string key)
	{
		if (map.TryGetValue(key, out int value))
		{
			return value;
		}

		return -1;
	}

	#endregion

	#region Internal Types

	private sealed class OperatorInfo(string op, string leftTypeName, string rightTypeName, string returnTypeName, string ownerTypeName)
	{
		public string Op { get; } = op;
		public string LeftTypeName { get; } = leftTypeName;
		public string RightTypeName { get; } = rightTypeName;
		public string ReturnTypeName { get; } = returnTypeName;
		public string OwnerTypeName { get; } = ownerTypeName;
	}

	private sealed class ProductInfo(string method, string selfTypeName, string otherTypeName, string returnTypeName, int vectorForm)
	{
		public string Method { get; } = method;
		public string SelfTypeName { get; } = selfTypeName;
		public string OtherTypeName { get; } = otherTypeName;
		public string ReturnTypeName { get; } = returnTypeName;
		public int VectorForm { get; } = vectorForm;
	}

	#endregion
}
