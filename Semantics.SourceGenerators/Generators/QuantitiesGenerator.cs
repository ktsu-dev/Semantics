// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using ktsu.SourceGeneratorToolkit;
using ktsu.Semantics.Vocabulary;
using Semantics.SourceGenerators.Models;
using ktsu.CodeBlocker.Templates;
using TypeKind = ktsu.CodeBlocker.Templates.TypeKind;

/// <summary>
/// Source generator that creates quantity types from the unified vector schema in dimensions.json.
/// Uses a two-phase approach: first collects all cross-dimensional operators globally,
/// then generates each type with its assigned operators.
/// </summary>
[Generator]
public class QuantitiesGenerator : SemanticsMultiFileGenerator
{
	/// <summary>
	/// Both metadata files, because per-unit conversion factors are needed to emit
	/// <c>From{Unit}</c> factories for units that are not the SI base unit.
	/// </summary>
	/// <remarks>
	/// This used to require overriding <see cref="GeneratorBase.Initialize"/> outright, along with a
	/// private JSON loader, a combining type, and a dead shim to satisfy the single-file base's
	/// abstract contract. Multi-file metadata is the normal case, so the base handles it.
	/// </remarks>
	protected override IReadOnlyList<string> MetadataFileNames => ["dimensions.json", "units.json"];

	/// <inheritdoc/>
	protected override void Generate(SourceProductionContext context, MetadataSet metadata)
	{
		MetadataFile? dimensionsFile = metadata["dimensions.json"];
		DimensionsMetadata? dimensions = dimensionsFile?.Deserialize<DimensionsMetadata>(context, MetadataParseFailed);
		if (dimensions is null)
		{
			return;
		}

		// A missing units.json is already reported as SEM006; carrying on with an empty set keeps
		// the base-unit factories generatable. A malformed one is now reported as SEM007 rather
		// than swallowed, which is what used to leave the generator silently emitting identity
		// conversions.
		UnitsMetadata units =
			metadata["units.json"]?.Deserialize<UnitsMetadata>(context, MetadataParseFailed) ?? new UnitsMetadata();

		GenerateInner(context, dimensions, units, dimensionsFile);
	}

	private void GenerateInner(SourceProductionContext context, DimensionsMetadata metadata, UnitsMetadata units, MetadataFile? dimensionsFile)
	{
		if (metadata.PhysicalDimensions == null || metadata.PhysicalDimensions.Count == 0)
		{
			return;
		}

		// Issue #60: surface schema-level metadata problems as diagnostics so they
		// show up in the build log instead of crashing mid-emit.
		List<string> validationIssues = metadata.Validate();
		foreach (string issue in validationIssues)
		{
			context.Report(SemanticsDiagnostics.MetadataValidationFailed, issue);
		}

		// One physics model, resolved once. Everything below is a spelling of what it says: which
		// types exist, what each one is, and which operators relate them. The C++ projection reads
		// the same resolution from the same code, which is what keeps the two from drifting -- and
		// what stops a relationship the exponents refuse from being written by one side and not
		// the other.
		QuantityVocabulary vocabulary = QuantityVocabulary.FromDimensions(metadata.ToDeclarations());
		ReportRefusals(context, vocabulary, dimensionsFile);

		Dictionary<string, UnitDefinition> unitMap = BuildUnitMap(units);

		// Issue #58/#48 follow-up: surface dimensions.json availableUnits entries that
		// don't exist in units.json. The generator's BuildToBaseExpression silently falls
		// back to identity conversion in that case, which is wrong for any non-base unit
		// — a typo (e.g. "Kilometres" vs "Kilometers") would silently produce a factory
		// with no scale factor. SEM004 catches that at build time.
		ReportUnknownUnitReferences(context, metadata, unitMap, dimensionsFile);

		Emission emission = new(
			BuildDimensionMap(metadata),
			BuildOverloadMap(metadata),
			GroupBy(CollectOperators(vocabulary), op => op.OwnerTypeName),
			GroupBy(CollectProducts(vocabulary), product => product.SelfTypeName),
			BuildTypeFormMap(vocabulary),
			unitMap);

		foreach (QuantityType type in vocabulary.Types)
		{
			EmitType(context, type, emission);
		}
	}

	/// <summary>
	/// Everything an emitter needs beyond the quantity it is writing.
	/// </summary>
	/// <param name="Dimensions">Every dimension, keyed by name.</param>
	/// <param name="Overloads">Every semantic overload, keyed by the type name it produces.</param>
	/// <param name="OperatorsByOwner">The cross-dimensional operators each type declares.</param>
	/// <param name="ProductsByOwner">The dot and cross products each vector type declares.</param>
	/// <param name="TypeFormMap">How many components each generated type has.</param>
	/// <param name="Units">Every unit, keyed by name.</param>
	/// <remarks>
	/// Six things that travel together to every emitter and are built once. Passed individually
	/// they put every one of these methods over the analyzer's parameter limit, and made the call
	/// sites hard to read for no gain: not one of them varies per type.
	/// </remarks>
	private sealed record Emission(
		IReadOnlyDictionary<string, PhysicalDimension> Dimensions,
		IReadOnlyDictionary<string, OverloadDefinition> Overloads,
		IReadOnlyDictionary<string, List<OperatorInfo>> OperatorsByOwner,
		IReadOnlyDictionary<string, List<ProductInfo>> ProductsByOwner,
		IReadOnlyDictionary<string, int> TypeFormMap,
		IReadOnlyDictionary<string, UnitDefinition> Units);

	/// <summary>
	/// Writes one quantity, as whichever kind of type the vocabulary says it is.
	/// </summary>
	/// <param name="context">Where the source goes.</param>
	/// <param name="type">The quantity to write.</param>
	/// <param name="emission">Everything shared between the types being written.</param>
	/// <remarks>
	/// Two questions decide it, and the vocabulary answers both: whether the quantity refines
	/// another one, and how many components it stores. A base and an overload differ in what they
	/// declare — an overload has the conversions to and from its base and nothing else does — and a
	/// scalar and a vector differ in how they store a value at all.
	/// </remarks>
	private void EmitType(SourceProductionContext context, QuantityType type, Emission emission)
	{
		PhysicalDimension dim = emission.Dimensions[type.Owner];

		if (type.Refines is null)
		{
			switch (type.Form)
			{
				case 0:
					EmitV0BaseType(context, type, dim, emission);
					break;

				case 1:
					EmitV1BaseType(context, type, dim, emission);
					break;

				default:
					EmitVectorType(context, type, dim, emission);
					break;
			}

			return;
		}

		if (type.Form <= 1)
		{
			EmitOverloadType(context, type, dim, emission);
		}
		else
		{
			// V2/3/4 overloads are rare — the strategy document shows them mainly for V3
			// (Position3D, Translation3D) — and carry no units or relationships of their own.
			EmitVectorOverloadType(context, type);
		}
	}

	#region Phase A: Resolving the vocabulary

	/// <summary>
	/// Every dimension, keyed by name, so an emitter holding a <see cref="QuantityType"/> can get
	/// back to the entry it was read from.
	/// </summary>
	/// <remarks>
	/// What the emitters need from that entry is the part the vocabulary has no opinion on: which
	/// units a quantity can be built from, and what its dimension is called in the generated
	/// <c>PhysicalDimensions</c> table. The physics itself comes from the vocabulary.
	/// </remarks>
	private static Dictionary<string, PhysicalDimension> BuildDimensionMap(DimensionsMetadata metadata)
	{
		Dictionary<string, PhysicalDimension> map = [];
		foreach (PhysicalDimension dim in metadata.PhysicalDimensions)
		{
			map[dim.Name] = dim;
		}

		return map;
	}

	/// <summary>
	/// Every semantic overload, keyed by the type name it produces.
	/// </summary>
	/// <remarks>
	/// The one thing an overload declares that is not physics: <c>relationships</c>, the
	/// <c>Diameter.ToRadius()</c> pairs. They are C# expressions written in the metadata and
	/// pasted into the generated source, so there is nothing in them for a shared model to hold —
	/// the same reason <c>SourceFile.Imports</c> is per-language in ktsu.Coder.
	/// </remarks>
	private static Dictionary<string, OverloadDefinition> BuildOverloadMap(DimensionsMetadata metadata)
	{
		Dictionary<string, OverloadDefinition> map = [];
		foreach (PhysicalDimension dim in metadata.PhysicalDimensions)
		{
			int[] forms = [0, 1, 2, 3, 4];
			foreach (int f in forms)
			{
				VectorFormDefinition? form = GetFormDef(dim, f);
				if (form == null)
				{
					continue;
				}

				foreach (OverloadDefinition overload in form.Overloads)
				{
					map[overload.Name] = overload;
				}
			}
		}

		return map;
	}

	/// <summary>
	/// How many components each generated type has, keyed by its name.
	/// </summary>
	/// <remarks>
	/// Read off the vocabulary rather than recomputed from the metadata: an operator is emitted
	/// differently depending on whether its operands store one value or several, and the
	/// vocabulary is what decided which types exist at which form in the first place.
	/// </remarks>
	private static Dictionary<string, int> BuildTypeFormMap(QuantityVocabulary vocabulary)
	{
		Dictionary<string, int> map = [];
		foreach (QuantityType type in vocabulary.Types)
		{
			map[type.Name] = type.Form;
		}

		return map;
	}

	/// <summary>
	/// Every operator to emit, expanded from the relationships the vocabulary resolved.
	/// </summary>
	/// <remarks>
	/// The vocabulary states each relationship once, in the direction the metadata declares it:
	/// <c>Velocity3D * Duration -&gt; Displacement3D</c>. C# emits four operators from that — the
	/// one declared, its commutation, and the two divisions that undo it — because a caller who
	/// writes <c>duration * velocity</c> is not making a different claim about physics, and an
	/// overload that is missing is a compile error rather than a wrong answer.
	/// <para>
	/// That expansion is the language's business rather than the model's, which is why it lives
	/// here and the C++ projection, which emits only the declared direction, does not have it.
	/// What both take from the vocabulary is which relationships there are at all — so a
	/// relationship the exponents refuse produces no operator here either, in any of its four
	/// directions.
	/// </para>
	/// </remarks>
	private static List<OperatorInfo> CollectOperators(QuantityVocabulary vocabulary)
	{
		HashSet<string> seen = [];
		List<OperatorInfo> result = [];

		foreach (QuantityRelationship relationship in vocabulary.Relationships)
		{
			switch (relationship.Kind)
			{
				case RelationshipKind.Product:
					AddProductOperators(result, seen, relationship);
					break;

				case RelationshipKind.Quotient:
					AddQuotientOperators(result, seen, relationship);
					break;

				default:
					// A dot or a cross product is a method rather than an operator, because
					// neither has a symbol in C#. They are collected as products below.
					break;
			}
		}

		return result;
	}

	/// <summary>
	/// The four spellings of <c>Left * Right -&gt; Result</c>.
	/// </summary>
	/// <remarks>
	/// The last of them only exists at the magnitude form: <c>Displacement3D / Velocity3D</c>
	/// would have to be a componentwise division to produce a <c>Duration</c>, and that is not
	/// what dividing one vector by another means.
	/// </remarks>
	private static void AddProductOperators(List<OperatorInfo> operators, HashSet<string> seen, QuantityRelationship relationship)
	{
		// Written against the relationship's own parts rather than through locals, so each line
		// says which of the three each operand is. As declared, commuted, and undone:
		AddOp(operators, seen, "*", relationship.Left, relationship.Right, relationship.Result, relationship.Left);
		AddOp(operators, seen, "*", relationship.Right, relationship.Left, relationship.Result, relationship.Right);
		AddOp(operators, seen, "/", relationship.Result, relationship.Right, relationship.Left, relationship.Result);

		if (relationship.Form == 0)
		{
			AddOp(operators, seen, "/", relationship.Result, relationship.Left, relationship.Right, relationship.Result);
		}
	}

	/// <summary>
	/// The three spellings of <c>Left / Right -&gt; Result</c>.
	/// </summary>
	/// <remarks>
	/// Three rather than four, because the fourth — <c>Left / Result -&gt; Right</c> — is the
	/// division the metadata already declares in the other direction wherever it holds, and
	/// asserting it from here would emit it for the cases where it does not.
	/// </remarks>
	private static void AddQuotientOperators(List<OperatorInfo> operators, HashSet<string> seen, QuantityRelationship relationship)
	{
		AddOp(operators, seen, "/", relationship.Left, relationship.Right, relationship.Result, relationship.Left);
		AddOp(operators, seen, "*", relationship.Result, relationship.Right, relationship.Left, relationship.Result);
		AddOp(operators, seen, "*", relationship.Right, relationship.Result, relationship.Left, relationship.Right);
	}

	/// <summary>
	/// Every dot and cross product to emit, which is one method each in the direction declared.
	/// </summary>
	/// <remarks>
	/// No commutation and no inverse: <c>cross(a, b)</c> is <c>-cross(b, a)</c> rather than the
	/// same thing, and neither product has one.
	/// </remarks>
	private static List<ProductInfo> CollectProducts(QuantityVocabulary vocabulary)
	{
		HashSet<string> seen = [];
		List<ProductInfo> result = [];

		foreach (QuantityRelationship relationship in vocabulary.Relationships)
		{
			string method = relationship.Kind switch
			{
				RelationshipKind.Dot => "Dot",
				RelationshipKind.Cross => "Cross",
				_ => string.Empty,
			};

			if (method.Length == 0)
			{
				continue;
			}

			string key = $"{method}:{relationship.Left}:{relationship.Right}:{relationship.Result}";
			if (seen.Add(key))
			{
				result.Add(new ProductInfo(method, relationship.Left, relationship.Right, relationship.Result, relationship.Form));
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

	/// <summary>
	/// Reports everything the vocabulary refused, each as the diagnostic this project already has
	/// for that kind of problem.
	/// </summary>
	/// <param name="context">Where the diagnostics go.</param>
	/// <param name="vocabulary">The resolved vocabulary.</param>
	/// <param name="dimensionsFile">The file the metadata was read from, for locations.</param>
	/// <remarks>
	/// Every refusal now costs an operator or a type, which is what makes reporting all five kinds
	/// worth doing from one place. Before the vocabulary drove emission this reported only the two
	/// kinds SEM008 covers, because the other three were dropped by the metadata-walking code that
	/// also reported them — and saying the same thing twice in two voices would have been the only
	/// effect.
	/// </remarks>
	private static void ReportRefusals(
		SourceProductionContext context,
		QuantityVocabulary vocabulary,
		MetadataFile? dimensionsFile)
	{
		foreach (VocabularyIssue issue in vocabulary.Refused)
		{
			switch (issue.Kind)
			{
				case VocabularyIssueKind.UnknownDimension:
					ReportUnknownReference(context, dimensionsFile, issue);
					break;

				case VocabularyIssueKind.MissingVectorForm:
					ReportFormMissing(context, dimensionsFile, issue);
					break;

				case VocabularyIssueKind.NoMagnitudeForm:
					// Schema-level rather than physics: a dimension with no vector0 has nothing
					// for its other forms to answer Magnitude() with, and now generates no types
					// at all rather than generating some of them without it.
					context.Report(SemanticsDiagnostics.MetadataValidationFailed, $"dimension '{issue.Subject}' {issue.Reason}");
					break;

				default:
					context.Report(SemanticsDiagnostics.RelationshipNotDimensionallyTrue, issue.Subject, issue.Reason);
					break;
			}
		}
	}

	/// <summary>
	/// Reports SEM001 at the position in <c>dimensions.json</c> where the unknown name is written.
	/// </summary>
	/// <remarks>
	/// Unscoped, because the name is by definition a typo and so occurs exactly once. The scoped
	/// overload would only help if the same typo were made twice, and would point at the first of
	/// them either way.
	/// </remarks>
	private static void ReportUnknownReference(
		SourceProductionContext context,
		MetadataFile? dimensionsFile,
		VocabularyIssue issue)
	{
		IssueSite site = issue.Site!;

		// The vocabulary checks "other" before "result" and stops at the first it cannot resolve,
		// so the name it refused is whichever of the two it matches.
		string field = string.Equals(site.Offending, site.Other, StringComparison.Ordinal) ? "other" : "result";

		context.ReportAt(
			SemanticsDiagnostics.UnknownDimensionReference,
			dimensionsFile?.FindLocation(site.Offending),
			site.Owner,
			site.Offending,
			$"{FieldPath(site)}.{field}");
	}

	/// <summary>
	/// Reports SEM003 at the relationship that requested the missing form.
	/// </summary>
	/// <remarks>
	/// Scoped, unlike SEM001: every name involved here is spelled correctly and appears throughout
	/// the file, so an unscoped search would land on an unrelated entry. Anchoring on the owning
	/// dimension's own <c>"name"</c> property and then looking for the relationship's <c>"other"</c>
	/// within it puts the location on the declaration that is actually wrong.
	/// </remarks>
	private static void ReportFormMissing(
		SourceProductionContext context,
		MetadataFile? dimensionsFile,
		VocabularyIssue issue)
	{
		IssueSite site = issue.Site!;

		context.ReportAt(
			SemanticsDiagnostics.RelationshipFormMissing,
			dimensionsFile?.FindLocation($"\"name\": \"{site.Owner}\"", $"\"other\": \"{site.Other}\""),
			site.Owner,
			FieldPath(site),
			site.Form,
			site.Offending);
	}

	/// <summary>
	/// Where a relationship is written in the metadata, in the form the diagnostics quote.
	/// </summary>
	/// <param name="site">The refusal's site.</param>
	/// <returns>A path like <c>integrals[Time -&gt; Length]</c>.</returns>
	/// <remarks>
	/// The array names belong to this reader rather than to the vocabulary: the vocabulary knows a
	/// product from a quotient, and <c>integrals</c> and <c>derivatives</c> are what
	/// <c>dimensions.json</c> happens to call the two arrays they are read from.
	/// </remarks>
	private static string FieldPath(IssueSite site) =>
		$"{ArrayName(site.Kind)}[{site.Other} -> {site.Result}]";

	private static string ArrayName(RelationshipKind kind) => kind switch
	{
		RelationshipKind.Product => "integrals",
		RelationshipKind.Quotient => "derivatives",
		RelationshipKind.Cross => "crossProducts",
		_ => "dotProducts",
	};

	private static Dictionary<string, UnitDefinition> BuildUnitMap(UnitsMetadata units)
	{
		Dictionary<string, UnitDefinition> map = [];
		if (units.UnitCategories == null)
		{
			return map;
		}

		foreach (UnitCategory cat in units.UnitCategories)
		{
			foreach (UnitDefinition unit in cat.Units)
			{
				map[unit.Name] = unit;
			}
		}

		return map;
	}

	/// <summary>
	/// Walks every <c>availableUnits</c> entry across the dimensions metadata and emits
	/// <c>SEM004</c> for any unit name that doesn't appear in <paramref name="unitMap"/>.
	/// Deduplicates by (unit, dimension) so a typo on a unit shared by many dimensions
	/// reports once per offending dimension instead of per-form/overload.
	/// </summary>
	private static void ReportUnknownUnitReferences(
		SourceProductionContext context,
		DimensionsMetadata metadata,
		Dictionary<string, UnitDefinition> unitMap,
		MetadataFile? dimensionsFile)
	{
		// If units.json wasn't loaded the map is empty; treating every unit as "unknown"
		// would flood the build log. Its absence is already reported as SEM006, so bail
		// rather than add a wall of warnings on top of it.
		if (unitMap.Count == 0)
		{
			return;
		}

		HashSet<string> seen = [];
		foreach (PhysicalDimension dim in metadata.PhysicalDimensions)
		{
			foreach (string unitName in dim.AvailableUnits)
			{
				if (string.IsNullOrEmpty(unitName) || unitMap.ContainsKey(unitName))
				{
					continue;
				}

				string key = $"{dim.Name}::{unitName}";
				if (!seen.Add(key))
				{
					continue;
				}

				// Pointed at where the name is actually written, so the warning is navigable
				// rather than just naming a string to go and search a large file for.
				context.ReportAt(
					SemanticsDiagnostics.UnknownUnitReference,
					dimensionsFile?.FindLocation(unitName),
					unitName,
					dim.Name);
			}
		}
	}

	/// <summary>
	/// Emits one <c>From{Unit}</c> static factory per entry in <paramref name="availableUnits"/>.
	/// The first unit is treated as the SI base unit (no conversion). Subsequent units use the
	/// conversion factor / magnitude / offset declared in <paramref name="unitMap"/>.
	/// When <paramref name="applyV0Guard"/> is true, the converted value is wrapped with
	/// <c>Vector0Guards.EnsureNonNegative</c> so a negative input — including one that becomes
	/// negative after a unit conversion (e.g. <c>FromCelsius(-300)</c>) — throws
	/// <see cref="System.ArgumentException"/>. This locks in the V0 non-negativity invariant
	/// from #50 across every per-unit factory introduced for #48.
	/// When <paramref name="strictPositive"/> is also true (V0 overloads with
	/// <c>physicalConstraints.minExclusive: "0"</c> per #51) the guard is upgraded to
	/// <c>Vector0Guards.EnsurePositive</c>, which rejects zero as well as negative values.
	/// <paramref name="strictPositive"/> is ignored when <paramref name="applyV0Guard"/> is false.
	/// </summary>
	private static void AddUnitFactories(
		ClassTemplate cls,
		List<string> availableUnits,
		IReadOnlyDictionary<string, UnitDefinition> unitMap,
		string fullType,
		string crefForComment,
		bool applyV0Guard,
		bool strictPositive = false)
	{
		if (availableUnits == null || availableUnits.Count == 0)
		{
			return;
		}

		string guardMethod = strictPositive ? "EnsurePositive" : "EnsureNonNegative";

		string baseUnit = availableUnits[0];
		foreach (string unitName in availableUnits)
		{
			bool isBase = unitName == baseUnit;
			string conversionExpr = isBase
				? Emit.ValueParameter
				: BuildToBaseExpression(unitName, unitMap);

			string body = applyV0Guard
				? $"=> Create(Vector0Guards.{guardMethod}({conversionExpr}, nameof(value)));"
				: $"=> Create({conversionExpr});";

			// Issue #49: factory names are the unit's singular lemma — the unit name verbatim
			// (e.g. From{Meter}, From{Kilogram}, From{MeterPerSecond}). units.json carries the
			// singular lemma for every unit including compounds, so the generator never has to
			// know English pluralisation: the rule is purely mechanical, From{Name}.
			string factorySuffix = unitName;

			List<string> comments =
			[
				Emit.SummaryOpen,
				$"/// Creates a new {crefForComment} from a value in {unitName}.",
				Emit.SummaryClose,
				$"/// <param name=\"value\">The value in {unitName}.</param>",
				$"/// <returns>A new {crefForComment} instance.</returns>",
			];
			if (applyV0Guard)
			{
				comments.Add("/// <exception cref=\"System.ArgumentException\">Thrown when the resulting magnitude would be negative.</exception>");
			}

			cls.Members.Add(new MethodTemplate()
			{
				Keywords = {Emit.Public, Emit.Static, fullType},
				Name = $"From{factorySuffix}",
				Parameters = {new ParameterTemplate { Type = "T", Name = Emit.ValueParameter }},
				BodyFactory = (b) => b.Write(body),
			}.WithComments(comments));
		}
	}

	/// <summary>
	/// Builds the C# expression converting <c>value</c> in <paramref name="unitName"/> to the SI
	/// base unit. Honours magnitude (<c>Kilo</c>, <c>Centi</c>, …), conversionFactor (lookup in
	/// <see cref="ConversionConstants"/>), and offset (additive, after scaling).
	/// </summary>
	private static string BuildToBaseExpression(string unitName, IReadOnlyDictionary<string, UnitDefinition> unitMap)
	{
		// If we don't have unit metadata, fall back to identity. The dimensions.json author is
		// responsible for keeping availableUnits in sync with units.json; if a unit is missing,
		// the emitted factory passes the value through unchanged so the build still succeeds.
		// (A future SEM00x diagnostic could surface this gap.)
		if (!unitMap.TryGetValue(unitName, out UnitDefinition? unit) || unit == null)
		{
			return Emit.ValueParameter;
		}

		string scaled = Emit.ValueParameter;
		bool hasMagnitude = !string.IsNullOrEmpty(unit.Magnitude) && unit.Magnitude != "1";
		bool hasFactor = !string.IsNullOrEmpty(unit.ConversionFactor) && unit.ConversionFactor != "1";

		if (hasMagnitude)
		{
			scaled = $"(value * T.CreateChecked(MetricMagnitudes.{unit.Magnitude}))";
		}
		else if (hasFactor)
		{
			scaled = $"(value * T.CreateChecked(Units.ConversionConstants.{unit.ConversionFactor}))";
		}

		bool hasOffset = !string.IsNullOrEmpty(unit.Offset) && unit.Offset != "0";
		if (hasOffset)
		{
			scaled = $"({scaled} + T.CreateChecked(Units.ConversionConstants.{unit.Offset}))";
		}

		return scaled;
	}

	/// <summary>
	/// Adds the per-quantity surface required by <see cref="ktsu.Semantics.Quantities.IPhysicalQuantity{T}"/>
	/// (#59): a <c>Dimension</c> override returning <c>PhysicalDimensions.{dim}</c>, plus a
	/// typed <c>In(I{dim}Unit)</c> method that converts the stored SI-base value into the
	/// caller's unit. Emitted for V0 and V1 (scalar-storage) types only; vector V2+ types
	/// have per-component conversion needs and are deferred.
	/// </summary>
	private static void AddDimensionAndInMembers(ClassTemplate cls, PhysicalDimension dim)
	{
		cls.Members.Add(new FieldTemplate()
		{
			Comments = {$"/// <summary>Gets the physical dimension this quantity belongs to.</summary>"},
			Keywords = {Emit.Public, "DimensionInfo"},
			Name = $"Dimension => PhysicalDimensions.{dim.Name}",
		});

		cls.Members.Add(new MethodTemplate()
		{
			Comments =
			{
				Emit.SummaryOpen,
				$"/// Converts this quantity's SI-base value to the value in <paramref name=\"unit\"/>.",
				"/// Cross-dimension calls (e.g. passing a non-" + dim.Name + " unit) fail at compile time.",
				Emit.SummaryClose,
				"/// <param name=\"unit\">The dimensionally-compatible target unit.</param>",
				"/// <returns>The value expressed in <paramref name=\"unit\"/>.</returns>",
			},
			Keywords = {Emit.Public, "T"},
			Name = "In",
			Parameters =
			{
				new ParameterTemplate { Type = $"global::ktsu.Semantics.Quantities.I{dim.Name}Unit", Name = "unit" },
			},
			BodyFactory = (body) => body.Write("=> unit.FromBase(Value);"),
		});
	}

	/// <summary>
	/// Adds the members a scalar-storage quantity used to inherit from the abstract
	/// <c>PhysicalQuantity</c> record: the stored value, construction, arithmetic, ordering and
	/// the <see cref="ktsu.Semantics.Quantities.IPhysicalQuantity{T}"/> surface.
	/// </summary>
	/// <remarks>
	/// A quantity is a struct, so none of this can be inherited. Emitting it per type is not
	/// only the way to keep the surface — it is the point: an operator declared on the type
	/// itself is a direct call the JIT can inline, where the shared generic base allocated a
	/// new object for every result.
	/// </remarks>
	/// <param name="cls">The type being built.</param>
	/// <param name="fullType">The type's name including its type argument, e.g. <c>Mass&lt;T&gt;</c>.</param>
	/// <param name="isV0">
	/// Whether this is a magnitude (Vector0) type. A V0 declares its own subtraction —
	/// <c>T.Abs(left - right)</c>, per the locked decision in #52 — so the plain one is skipped
	/// rather than emitted twice.
	/// </param>
	private static void AddValueTypeCore(ClassTemplate cls, string fullType, bool isV0)
	{
		// A doc comment is XML, so the type argument is spelled with braces: Mass{T}, not Mass<T>.
		string docRef = $"<see cref=\"{fullType.Replace('<', '{').Replace('>', '}')}\"/>";

		cls.Members.Add(new PropertyTemplate()
		{
			Comments = {"/// <summary>Gets the stored value, in the dimension's SI base unit.</summary>"},
			Keywords = {Emit.Public},
			Type = "T",
			Name = "Quantity",
			Getter = new AccessorTemplate { Kind = AccessorKind.Auto },
			Setter = new AccessorTemplate { Kind = AccessorKind.Auto },
			SetterIsInitOnly = true,
		});

		cls.Members.Add(new FieldTemplate()
		{
			Comments = {"/// <summary>Gets the value stored in this quantity (in the dimension's SI base unit).</summary>"},
			Keywords = {Emit.Public, "T"},
			Name = "Value => Quantity",
		});

		cls.Members.Add(new FieldTemplate()
		{
			Comments = {"/// <summary>Gets whether this quantity is finite and not NaN.</summary>"},
			Keywords = {Emit.Public, "bool"},
			Name = "IsPhysicallyValid => PhysicalQuantityCore.IsPhysicallyValid(Quantity)",
		});

		cls.Members.Add(new MethodTemplate()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Creates a quantity holding <paramref name=\"value\"/>, in the SI base unit.",
				Emit.SummaryClose,
				"/// <param name=\"value\">The value in the SI base unit.</param>",
				"/// <returns>A new quantity holding <paramref name=\"value\"/>.</returns>",
			},
			Keywords = {Emit.Public, Emit.Static, fullType},
			Name = "Create",
			Parameters = {new ParameterTemplate { Type = "T", Name = Emit.ValueParameter }},
			BodyFactory = (body) => body.Write("=> new() { Quantity = value };"),
		});

		cls.Members.Add(new MethodTemplate()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Compares this quantity to another of the same physical dimension.",
				Emit.SummaryClose,
				"/// <param name=\"other\">The quantity to compare against.</param>",
				"/// <returns>A negative number, zero or a positive number as this sorts before, with, or after <paramref name=\"other\"/>.</returns>",
				"/// <exception cref=\"System.ArgumentException\">When the two do not share a dimension.</exception>",
			},
			Keywords = {Emit.Public, "int"},
			Name = "CompareTo",
			Parameters = {new ParameterTemplate { Type = "IPhysicalQuantity<T>", Name = "other" }},
			BodyFactory = (body) => body.Write($"=> PhysicalQuantityCore.Compare<{fullType}, T>(this, other);"),
		});

		cls.Members.Add(new MethodTemplate()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Reports whether this quantity shares a dimension and a value with <paramref name=\"other\"/>.",
				Emit.SummaryClose,
				"/// <param name=\"other\">The quantity to compare against.</param>",
				"/// <returns><see langword=\"true\"/> when both dimension and value match.</returns>",
			},
			Keywords = {Emit.Public, "bool"},
			Name = "Equals",
			Parameters = {new ParameterTemplate { Type = "IPhysicalQuantity<T>", Name = "other" }},
			BodyFactory = (body) => body.Write($"=> PhysicalQuantityCore.AreEqual<{fullType}, T>(this, other);"),
		});

		AddBinaryOperator(cls, fullType, "+", fullType, fullType, "=> Create(left.Quantity + right.Quantity);",
			$"Adds two {docRef} values.");

		if (!isV0)
		{
			AddBinaryOperator(cls, fullType, "-", fullType, fullType, "=> Create(left.Quantity - right.Quantity);",
				$"Subtracts one {docRef} from another.");
		}

		cls.Members.Add(new MethodTemplate()
		{
			Comments = {$"/// <summary>Negates a {docRef}.</summary>"},
			Attributes = {Emit.PhysicsOperatorSuppression},
			Keywords = {Emit.Public, Emit.Static, fullType},
			Name = "operator -",
			Parameters = {new ParameterTemplate { Type = fullType, Name = Emit.ValueParameter }},
			BodyFactory = (body) => body.Write("=> Create(-value.Quantity);"),
		});

		AddBinaryOperator(cls, fullType, "*", fullType, "T", "=> Create(left.Quantity * right);",
			$"Scales a {docRef} by a bare number.");
		AddBinaryOperator(cls, fullType, "*", "T", fullType, "=> Create(left * right.Quantity);",
			$"Scales a {docRef} by a bare number.");
		AddBinaryOperator(cls, fullType, "/", fullType, "T",
			"=> T.IsZero(right) ? throw new System.DivideByZeroException(\"Cannot divide by zero.\") : Create(left.Quantity / right);",
			$"Divides a {docRef} by a bare number.");
		AddBinaryOperator(cls, "T", "/", fullType, fullType,
			"=> T.IsZero(right.Quantity) ? throw new System.DivideByZeroException(\"Cannot divide by zero.\") : left.Quantity / right.Quantity;",
			$"Divides one {docRef} by another, giving the bare ratio.");

		AddBinaryOperator(cls, "bool", "<", fullType, fullType, "=> left.Quantity < right.Quantity;",
			"Reports whether the left value sorts before the right.");
		AddBinaryOperator(cls, "bool", "<=", fullType, fullType, "=> left.Quantity <= right.Quantity;",
			"Reports whether the left value sorts before or with the right.");
		AddBinaryOperator(cls, "bool", ">", fullType, fullType, "=> left.Quantity > right.Quantity;",
			"Reports whether the left value sorts after the right.");
		AddBinaryOperator(cls, "bool", ">=", fullType, fullType, "=> left.Quantity >= right.Quantity;",
			"Reports whether the left value sorts after or with the right.");

		cls.Members.Add(new MethodTemplate()
		{
			Comments =
			{
				"/// <summary>Returns the stored value as text.</summary>",
				"/// <returns>The value in the SI base unit, rendered by <typeparamref name=\"T\"/>.</returns>",
			},
			Keywords = {Emit.Public, "override", "string"},
			Name = "ToString",
			Parameters = {},
			BodyFactory = (body) => body.Write("=> Quantity.ToString() ?? string.Empty;"),
		});
	}

	/// <summary>
	/// Adds one binary operator to <paramref name="cls"/>.
	/// </summary>
	/// <param name="cls">The type being built.</param>
	/// <param name="returnType">What the operator returns.</param>
	/// <param name="symbol">The operator symbol.</param>
	/// <param name="leftType">The left operand's type.</param>
	/// <param name="rightType">The right operand's type.</param>
	/// <param name="body">The expression body, including its leading arrow.</param>
	/// <param name="summary">One line of documentation.</param>
	private static void AddBinaryOperator(
		ClassTemplate cls,
		string returnType,
		string symbol,
		string leftType,
		string rightType,
		string body,
		string summary)
	{
		cls.Members.Add(new MethodTemplate()
		{
			Comments = {$"/// <summary>{summary}</summary>"},
			Attributes = {Emit.PhysicsOperatorSuppression},
			Keywords = {Emit.Public, Emit.Static, returnType},
			Name = $"operator {symbol}",
			Parameters =
			{
				new ParameterTemplate { Type = leftType, Name = "left" },
				new ParameterTemplate { Type = rightType, Name = Emit.RightParameter },
			},
			BodyFactory = (b) => b.Write(body),
		});
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
		QuantityType type,
		PhysicalDimension dim,
		Emission emission)
	{
		string typeName = type.Name;
		string fullType = $"{typeName}<T>";

		using CodeBlocker cb = CreateCodeBlocker();

		SourceFileTemplate sourceFile = new()
		{
			FileName = $"{typeName}.g.cs",
			Namespace = "ktsu.Semantics.Quantities",
			Usings = {"System.Numerics"},
		};

		ClassTemplate cls = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				$"/// Magnitude (Vector0) quantity for the {dim.Name} dimension.",
				Emit.SummaryClose,
				"/// <typeparam name=\"T\">The numeric storage type.</typeparam>",
			},
			Kind = TypeKind.RecordStruct,
			Keywords = {Emit.Public, "readonly", "partial"},
			Name = fullType,
			Interfaces = {$"IVector0<{fullType}, T>", $"IPhysicalQuantity<{fullType}, T>"},
			Constraints = {"where T : struct, INumber<T>"},
		};

		// Everything a quantity used to inherit from the PhysicalQuantity record.
		AddValueTypeCore(cls, fullType, isV0: true);

		// Zero property (satisfies IVector0)
		cls.Members.Add(new FieldTemplate()
		{
			Comments = {"/// <summary>Gets a quantity with value zero.</summary>"},
			Keywords = {Emit.Public, Emit.Static, fullType},
			Name = "Zero => Create(T.Zero)",
		});

		// Factory methods for every available unit (#48). The V0 non-negativity invariant from
		// #50 is enforced by AddUnitFactories — for non-base units the guard runs *after* the
		// conversion, so an input that is non-negative in its source unit but negative in the
		// SI base unit (e.g. -300 °C → -26.85 K) still throws.
		AddUnitFactories(
			cls,
			dim.AvailableUnits,
			emission.Units,
			fullType,
			"<see cref=\"" + typeName + "{T}\"/>",
			applyV0Guard: true);

		// Dimension override + typed In() (#59).
		AddDimensionAndInMembers(cls, dim);

		// V0 - V0 returns the same V0 of T.Abs(left - right) (locked decision in #52).
		// We emit this on every V0 base type so the derived operator wins overload resolution
		// over PhysicalQuantity's plain subtraction (which can produce a negative magnitude
		// and would trip the non-negativity guard from #50).
		cls.Members.Add(new MethodTemplate()
		{
			Comments =
			{
				Emit.SummaryOpen,
				$"/// Subtracts two {typeName} values, returning the absolute difference as a non-negative {typeName}.",
				"/// Magnitude subtraction stays a magnitude (per the unified-vector model).",
				Emit.SummaryClose,
			},
			Attributes = {Emit.PhysicsOperatorSuppression},
			Keywords = {Emit.Public, Emit.Static, fullType},
			Name = "operator -",
			Parameters =
			{
				new ParameterTemplate { Type = fullType, Name = "left" },
				new ParameterTemplate { Type = fullType, Name = Emit.RightParameter },
			},
			BodyFactory = (body) => body.Write("=> Create(T.Abs(left.Quantity - right.Quantity));"),
		});

		// Cross-dimensional operators
		EmitScalarOperators(cls, typeName, emission);

		sourceFile.Classes.Add(cls);
		WriteSourceFileTo(cb, sourceFile);
		context.AddSource(sourceFile.FileName, cb.ToString());
	}

	private void EmitV1BaseType(
		SourceProductionContext context,
		QuantityType type,
		PhysicalDimension dim,
		Emission emission)
	{
		string typeName = type.Name;
		string fullType = $"{typeName}<T>";

		// Never absent: a dimension with no magnitude form generates no types at all, because
		// there would be nothing for this one to answer Magnitude() with.
		string v0TypeName = type.MagnitudeType;

		using CodeBlocker cb = CreateCodeBlocker();

		SourceFileTemplate sourceFile = new()
		{
			FileName = $"{typeName}.g.cs",
			Namespace = "ktsu.Semantics.Quantities",
			Usings = {"System.Numerics"},
		};

		ClassTemplate cls = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				$"/// Signed one-dimensional (Vector1) quantity for the {dim.Name} dimension.",
				Emit.SummaryClose,
				"/// <typeparam name=\"T\">The numeric storage type.</typeparam>",
			},
			Kind = TypeKind.RecordStruct,
			Keywords = {Emit.Public, "readonly", "partial"},
			Name = fullType,
			Interfaces = {$"IVector1<{fullType}, T>", $"IPhysicalQuantity<{fullType}, T>"},
			Constraints = {"where T : struct, INumber<T>"},
		};

		// Everything a quantity used to inherit from the PhysicalQuantity record.
		AddValueTypeCore(cls, fullType, isV0: false);

		// Zero property (satisfies IVector1)
		cls.Members.Add(new FieldTemplate()
		{
			Comments = {"/// <summary>Gets a quantity with value zero.</summary>"},
			Keywords = {Emit.Public, Emit.Static, fullType},
			Name = "Zero => Create(T.Zero)",
		});

		// Factory methods for every available unit.
		// V1 quantities are signed; no V0 non-negativity guard.
		AddUnitFactories(
			cls,
			dim.AvailableUnits,
			emission.Units,
			fullType,
			"<see cref=\"" + typeName + "{T}\"/>",
			applyV0Guard: false);

		// Dimension override + typed In() (#59).
		AddDimensionAndInMembers(cls, dim);

		// Magnitude method returning V0 base
		cls.Members.Add(new MethodTemplate()
		{
			Comments =
			{
				Emit.SummaryOpen,
				$"/// Gets the magnitude of this quantity as a <see cref=\"{v0TypeName}{{T}}\"/>.",
				Emit.SummaryClose,
				$"/// <returns>The non-negative magnitude.</returns>",
			},
			Keywords = {Emit.Public, $"{v0TypeName}<T>"},
			Name = "Magnitude",
			Parameters = {},
			BodyFactory = (body) => body.Write($"=> {v0TypeName}<T>.Create(T.Abs(Value));"),
		});

		// Cross-dimensional operators
		EmitScalarOperators(cls, typeName, emission);

		sourceFile.Classes.Add(cls);
		WriteSourceFileTo(cb, sourceFile);
		context.AddSource(sourceFile.FileName, cb.ToString());
	}

	private static void EmitVectorType(
		SourceProductionContext context,
		QuantityType type,
		PhysicalDimension dim,
		Emission emission)
	{
		int dims = type.Form;
		string[] components = Components(dims);

		string typeName = type.Name;
		string fullType = $"{typeName}<T>";
		string interfaceName = $"IVector{dims}<{fullType}, T>";
		string v0TypeName = type.MagnitudeType;

		using CodeBlocker cb = CreateCodeBlocker();

		WriteHeaderTo(cb);
		cb.WriteLine("#pragma warning disable IDE0040 // Accessibility modifiers required");
		cb.WriteLine("#pragma warning disable CA2225 // Operator overloads have named alternates");
		cb.NewLine();

		cb.WriteLine("namespace ktsu.Semantics.Quantities;");
		cb.NewLine();
		cb.WriteLine("using System;");
		cb.WriteLine("using System.Numerics;");
		cb.NewLine();

		cb.WriteLine(Emit.SummaryOpen);
		cb.WriteLine($"/// {dims}D vector representation of {dim.Name}.");
		cb.WriteLine(Emit.SummaryClose);
		cb.WriteLine("/// <typeparam name=\"T\">The numeric component type.</typeparam>");
		cb.WriteLine($"public readonly partial record struct {fullType} : {interfaceName}");
		cb.WriteLine("\twhere T : struct, INumber<T>");

		using (new ScopeWithTrailingSemicolon(cb))
		{
			WriteVectorComponentProperties(cb, components);
			WriteVectorStaticProperties(cb, fullType, components);

			// Typed Magnitude() method returning V0 base
			cb.WriteLine($"/// <summary>Gets the magnitude as a <see cref=\"{v0TypeName}{{T}}\"/>.</summary>");
			cb.WriteLine($"public {v0TypeName}<T> Magnitude() => {v0TypeName}<T>.Create(Length());");
			cb.NewLine();

			WriteVectorMethods(cb, fullType, components, dims);
			WriteVectorOperators(cb, fullType, components);

			// Cross-dimensional operators (inlined for VN types)
			EmitVectorCrossDimOperators(cb, typeName, emission);

			// Typed dot product methods
			if (emission.ProductsByOwner.TryGetValue(typeName, out List<ProductInfo>? products))
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
		QuantityType type,
		PhysicalDimension dim,
		Emission emission)
	{
		int vectorForm = type.Form;
		string typeName = type.Name;
		string baseTypeName = type.Refines!;
		string fullType = $"{typeName}<T>";
		string baseFullType = $"{baseTypeName}<T>";

		using CodeBlocker cb = CreateCodeBlocker();

		string interfaceName = vectorForm == 0 ? $"IVector0<{fullType}, T>" : $"IVector1<{fullType}, T>";

		SourceFileTemplate sourceFile = new()
		{
			FileName = $"{typeName}.g.cs",
			Namespace = "ktsu.Semantics.Quantities",
			Usings = {"System.Numerics"},
		};

		ClassTemplate cls = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				$"/// {type.Description}",
				$"/// Semantic overload of <see cref=\"{baseTypeName}{{T}}\"/>.",
				Emit.SummaryClose,
				"/// <typeparam name=\"T\">The numeric storage type.</typeparam>",
			},
			Kind = TypeKind.RecordStruct,
			Keywords = {Emit.Public, "readonly", "partial"},
			Name = fullType,
			Interfaces = {interfaceName, $"IPhysicalQuantity<{fullType}, T>"},
			Constraints = {"where T : struct, INumber<T>"},
		};

		// Everything a quantity used to inherit from the PhysicalQuantity record.
		AddValueTypeCore(cls, fullType, isV0: vectorForm == 0);

		// Zero property
		cls.Members.Add(new FieldTemplate()
		{
			Comments = {"/// <summary>Gets a quantity with value zero.</summary>"},
			Keywords = {Emit.Public, Emit.Static, fullType},
			Name = "Zero => Create(T.Zero)",
		});

		// Factory methods for every available unit (#48); overloads inherit the dimension's
		// units. V0 overloads enforce the same non-negativity invariant as their V0 base
		// type (#50). V0 overloads that declare physicalConstraints.minExclusive in
		// dimensions.json (#51, e.g. Wavelength, Period, HalfLife) get the stricter
		// EnsurePositive guard so a zero input is rejected too. V1 overloads accept
		// any sign.
		bool strictPositive = type.Magnitude == Magnitude.Positive;
		AddUnitFactories(
			cls,
			dim.AvailableUnits,
			emission.Units,
			fullType,
			typeName,
			applyV0Guard: vectorForm == 0,
			strictPositive: strictPositive);

		// Dimension override + typed In() (#59).
		AddDimensionAndInMembers(cls, dim);

		// Implicit widening to base type
		cls.Members.Add(new MethodTemplate()
		{
			Comments = {$"/// <summary>Implicit conversion to {baseTypeName}.</summary>"},
			Keywords = {Emit.Public, Emit.Static, "implicit", "operator"},
			Name = baseFullType,
			Parameters = {new ParameterTemplate { Type = fullType, Name = Emit.ValueParameter }},
			BodyFactory = (body) => body.Write($"=> {baseFullType}.Create(value.Value);"),
		});

		// Explicit narrowing from base type
		cls.Members.Add(new MethodTemplate()
		{
			Comments = {$"/// <summary>Explicit conversion from {baseTypeName}.</summary>"},
			Keywords = {Emit.Public, Emit.Static, "explicit", "operator"},
			Name = fullType,
			Parameters = {new ParameterTemplate { Type = baseFullType, Name = Emit.ValueParameter }},
			BodyFactory = (body) => body.Write($"=> Create(value.Value);"),
		});

		// Factory-style narrowing from base
		cls.Members.Add(new MethodTemplate()
		{
			Comments = {$"/// <summary>Creates a {typeName} from a {baseTypeName} value.</summary>"},
			Keywords = {Emit.Public, Emit.Static, fullType},
			Name = "From",
			Parameters = {new ParameterTemplate { Type = baseFullType, Name = Emit.ValueParameter }},
			BodyFactory = (body) => body.Write("=> Create(value.Value);"),
		});

		// V0 overload subtraction returns the same V0 of T.Abs(left - right) (locked
		// in #52). The overload-typed operator hides the base PhysicalQuantity's plain
		// subtraction so overloads stay in their own type and the magnitude invariant
		// is preserved.
		if (vectorForm == 0)
		{
			cls.Members.Add(new MethodTemplate()
			{
				Comments = {$"/// <summary>Subtracts two {typeName} values, returning the absolute difference as a non-negative {typeName}.</summary>"},
				Attributes = {Emit.PhysicsOperatorSuppression},
				Keywords = {Emit.Public, Emit.Static, fullType},
				Name = "operator -",
				Parameters =
				{
					new ParameterTemplate { Type = fullType, Name = "left" },
					new ParameterTemplate { Type = fullType, Name = Emit.RightParameter },
				},
				BodyFactory = (body) => body.Write("=> Create(T.Abs(left.Quantity - right.Quantity));"),
			});
		}

		// Relationship methods (e.g., Diameter.ToRadius(), Diameter.FromRadius()). The one
		// thing an overload declares that the vocabulary does not carry: these are C#
		// expressions written in the metadata and pasted through, so there is nothing in
		// them for a language-agnostic model to hold.
		foreach (KeyValuePair<string, string> rel in emission.Overloads[typeName].Relationships)
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
					Comments = {$"/// <summary>Converts this {typeName} to a {targetName}.</summary>"},
					Keywords = {Emit.Public, targetType},
					Name = methodName,
					Parameters = {},
					BodyFactory = (body) => body.Write($"=> {targetType}.Create({expr});"),
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
					Comments = {$"/// <summary>Creates a {typeName} from a {sourceName} value.</summary>"},
					Keywords = {Emit.Public, Emit.Static, fullType},
					Name = methodName,
					Parameters = {new ParameterTemplate { Type = sourceType, Name = "source" }},
					BodyFactory = (body) => body.Write($"=> Create({expr});"),
				});
			}
		}

		sourceFile.Classes.Add(cls);
		WriteSourceFileTo(cb, sourceFile);
		context.AddSource(sourceFile.FileName, cb.ToString());
	}

	private static void EmitVectorOverloadType(SourceProductionContext context, QuantityType type)
	{
		int dims = type.Form;
		string[] components = Components(dims);

		string typeName = type.Name;
		string baseTypeName = type.Refines!;
		string fullType = $"{typeName}<T>";
		string baseFullType = $"{baseTypeName}<T>";
		string interfaceName = $"IVector{dims}<{fullType}, T>";

		using CodeBlocker cb = CreateCodeBlocker();

		WriteHeaderTo(cb);
		cb.WriteLine("#pragma warning disable IDE0040 // Accessibility modifiers required");
		cb.WriteLine("#pragma warning disable CA2225 // Operator overloads have named alternates");
		cb.NewLine();

		cb.WriteLine("namespace ktsu.Semantics.Quantities;");
		cb.NewLine();
		cb.WriteLine("using System;");
		cb.WriteLine("using System.Numerics;");
		cb.NewLine();

		cb.WriteLine(Emit.SummaryOpen);
		cb.WriteLine($"/// {type.Description}");
		cb.WriteLine($"/// Semantic overload of <see cref=\"{baseTypeName}{{T}}\"/>.");
		cb.WriteLine(Emit.SummaryClose);
		cb.WriteLine($"public readonly partial record struct {fullType} : {interfaceName}");
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

	private static void EmitScalarOperators(ClassTemplate cls, string ownerTypeName, Emission emission)
	{
		if (!emission.OperatorsByOwner.TryGetValue(ownerTypeName, out List<OperatorInfo>? ops))
		{
			return;
		}

		foreach (OperatorInfo op in ops)
		{
			int leftForm = GetFormOrDefault(emission.TypeFormMap, op.LeftTypeName);
			int rightForm = GetFormOrDefault(emission.TypeFormMap, op.RightTypeName);

			// For V0/V1 owner types, use Multiply/Divide helpers when both operands are V0/V1
			if (leftForm <= 1 && rightForm <= 1)
			{
				// The inherited Multiply/Divide helpers went away with the record base. A
				// quantity constructs its result directly now, which is also what makes the
				// operator a plain arithmetic expression the JIT can inline.
				cls.Members.Add(new MethodTemplate()
				{
					Comments =
					{
						Emit.SummaryOpen,
						$"/// {(op.Op == "*" ? "Multiplies" : "Divides")} {op.LeftTypeName} by {op.RightTypeName} to produce {op.ReturnTypeName}.",
						Emit.SummaryClose,
					},
					Attributes = {Emit.PhysicsOperatorSuppression},
					Keywords = {Emit.Public, Emit.Static, $"{op.ReturnTypeName}<T>"},
					Name = $"operator {op.Op}",
					Parameters =
					{
						new ParameterTemplate { Type = $"{op.LeftTypeName}<T>", Name = "left" },
						new ParameterTemplate { Type = $"{op.RightTypeName}<T>", Name = Emit.RightParameter },
					},
					BodyFactory = (body) => body.Write(
						$"=> {op.ReturnTypeName}<T>.Create(left.Quantity {op.Op} right.Quantity);"),
				});
			}
			else
			{
				// One operand is V2+ (VN type, multi-component)
				// The owner is V0/V1, the other operand is VN
				// Generate inline: left.Value {op} right.X, etc. OR left.X {op} right.Value, etc.
				EmitInlineCrossDimOp(cls, op, emission);
			}
		}
	}

	private static void EmitInlineCrossDimOp(ClassTemplate cls, OperatorInfo op, Emission emission)
	{
		int leftForm = GetFormOrDefault(emission.TypeFormMap, op.LeftTypeName);
		int rightForm = GetFormOrDefault(emission.TypeFormMap, op.RightTypeName);
		int resultForm = GetFormOrDefault(emission.TypeFormMap, op.ReturnTypeName);

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
			bodyExpr = $"=> new() {{ {initExpr} }};";
		}
		else if (leftForm <= 1 && rightForm >= 2)
		{
			// V0 op VN: component-wise with left.Value
			if (op.Op == "*")
			{
				string initExpr = string.Join(", ", resultComponents.Select(c => $"{c} = left.Value {op.Op} right.{c}"));
				bodyExpr = $"=> new() {{ {initExpr} }};";
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
			{
				Emit.SummaryOpen,
				$"/// {(op.Op == "*" ? "Multiplies" : "Divides")} {op.LeftTypeName} by {op.RightTypeName} to produce {op.ReturnTypeName}.",
				Emit.SummaryClose,
			},
			Attributes = {Emit.PhysicsOperatorSuppression},
			Keywords = {Emit.Public, Emit.Static, $"{op.ReturnTypeName}<T>"},
			Name = $"operator {op.Op}",
			Parameters =
			{
				new ParameterTemplate { Type = $"{op.LeftTypeName}<T>", Name = "left" },
				new ParameterTemplate { Type = $"{op.RightTypeName}<T>", Name = Emit.RightParameter },
			},
			BodyFactory = (body) => body.Write(bodyExpr),
		});
	}

	private static void EmitVectorCrossDimOperators(CodeBlocker cb, string ownerTypeName, Emission emission)
	{
		if (!emission.OperatorsByOwner.TryGetValue(ownerTypeName, out List<OperatorInfo>? ops))
		{
			return;
		}

		foreach (OperatorInfo op in ops)
		{
			int leftForm = GetFormOrDefault(emission.TypeFormMap, op.LeftTypeName);
			int rightForm = GetFormOrDefault(emission.TypeFormMap, op.RightTypeName);
			int resultForm = GetFormOrDefault(emission.TypeFormMap, op.ReturnTypeName);

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

	private static int GetFormOrDefault(IReadOnlyDictionary<string, int> map, string key)
	{
		if (map.TryGetValue(key, out int value))
		{
			return value;
		}

		return -1;
	}

	/// <summary>
	/// What a vector type's components are called, by how many it has.
	/// </summary>
	/// <param name="dims">The component count, from two to four.</param>
	/// <returns>The component names, in order.</returns>
	private static string[] Components(int dims) => dims switch
	{
		2 => ["X", "Y"],
		3 => ["X", "Y", "Z"],
		4 => ["X", "Y", "Z", "W"],
		_ => throw new ArgumentOutOfRangeException(nameof(dims)),
	};

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
