// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using System.Collections.Generic;
using System.Globalization;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;

/// <summary>
/// Source generator that creates logarithmic-scale quantity types (decibel levels,
/// pitch intervals, pH) from logarithmic.json. Logarithmic scales don't obey linear
/// arithmetic, so they are emitted as standalone <c>readonly partial record struct</c>s
/// that convert to and from their linear generated counterparts via
/// <c>scale = multiplier · log_base(linear / reference)</c>. Bespoke members
/// (named constants, cross-scale conversions) live in hand-written partials.
/// </summary>
[Generator]
public class LogarithmicScalesGenerator : GeneratorBase<LogarithmicMetadata>
{
	private static readonly DiagnosticDescriptor InvalidScaleDefinition = new(
		id: "SEM005",
		title: "logarithmic.json scale definition is invalid",
		messageFormat: "logarithmic.json validation issue: {0}",
		category: "Semantics.SourceGenerators",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true);

	public LogarithmicScalesGenerator() : base("logarithmic.json") { }

	/// <inheritdoc/>
	protected override void Generate(SourceProductionContext context, LogarithmicMetadata metadata, CodeBlocker codeBlocker)
	{
		if (metadata.LogarithmicScales == null || metadata.LogarithmicScales.Count == 0)
		{
			return;
		}

		HashSet<string> seenNames = [];
		foreach (LogarithmicScaleDefinition scale in metadata.LogarithmicScales)
		{
			if (string.IsNullOrWhiteSpace(scale.Name))
			{
				Report(context, "a scale entry is missing its name");
				continue;
			}

			if (!seenNames.Add(scale.Name))
			{
				Report(context, $"duplicate scale name '{scale.Name}'");
				continue;
			}

			EmitScale(context, scale);
		}
	}

	private static void Report(SourceProductionContext context, string message) =>
		context.ReportDiagnostic(Diagnostic.Create(InvalidScaleDefinition, Location.None, message));

	private static void EmitScale(SourceProductionContext context, LogarithmicScaleDefinition scale)
	{
		using CodeBlocker cb = CodeBlocker.Create();
		string name = scale.Name;
		string fullType = $"{name}<T>";

		WriteHeaderTo(cb);
		cb.WriteLine("#nullable enable");
		cb.NewLine();
		cb.WriteLine("namespace ktsu.Semantics.Quantities;");
		cb.NewLine();
		cb.WriteLine("using System;");
		cb.WriteLine("using System.Globalization;");
		cb.WriteLine("using System.Numerics;");
		cb.NewLine();

		cb.WriteLine("/// <summary>");
		cb.WriteLine($"/// {scale.Description}");
		cb.WriteLine("/// </summary>");
		cb.WriteLine("/// <remarks>");
		if (!string.IsNullOrWhiteSpace(scale.Remarks))
		{
			cb.WriteLine($"/// {scale.Remarks}");
		}

		cb.WriteLine("/// Logarithmic scales don't obey linear arithmetic, so this type is generated as a");
		cb.WriteLine("/// standalone companion (from logarithmic.json) rather than a physical dimension.");
		cb.WriteLine("/// </remarks>");
		cb.WriteLine("/// <typeparam name=\"T\">The floating-point storage type.</typeparam>");
		cb.WriteLine("/// <param name=\"Value\">The scale value.</param>");
		cb.WriteLine($"public readonly partial record struct {fullType}(T Value) : IComparable<{fullType}>");
		cb.WriteLine("\twhere T : struct, INumber<T>");
		using (new Scope(cb))
		{
			WriteScalarFactory(cb, scale, fullType);

			foreach (LogarithmicConversionDefinition conversion in scale.Conversions)
			{
				if (string.IsNullOrWhiteSpace(conversion.Linear))
				{
					Report(context, $"scale '{scale.Name}' has a conversion with no linear type");
					continue;
				}

				WriteConversion(cb, scale, conversion, fullType);
			}

			if (scale.Arithmetic)
			{
				WriteArithmetic(cb, fullType);
			}

			WriteComparisons(cb, fullType);
			WriteToString(cb, scale);
		}

		context.AddSource($"{name}.g.cs", cb.ToString());
	}

	private static void WriteScalarFactory(CodeBlocker cb, LogarithmicScaleDefinition scale, string fullType)
	{
		cb.WriteLine("/// <summary>");
		cb.WriteLine("/// Creates a value from the raw scale number.");
		cb.WriteLine("/// </summary>");
		cb.WriteLine("/// <param name=\"value\">The raw scale value.</param>");
		cb.WriteLine($"/// <returns>A new <see cref=\"{scale.Name}{{T}}\"/>.</returns>");
		cb.WriteLine($"public static {fullType} {scale.ScalarFactory}(T value) => new(value);");
		cb.NewLine();
	}

	private static void WriteConversion(CodeBlocker cb, LogarithmicScaleDefinition scale, LogarithmicConversionDefinition conversion, string fullType)
	{
		string linear = conversion.Linear;
		string fromName = string.IsNullOrWhiteSpace(conversion.FromName) ? $"From{linear}" : conversion.FromName!;
		string toName = string.IsNullOrWhiteSpace(conversion.ToName) ? $"To{linear}" : conversion.ToName!;
		string multiplier = ToDoubleLiteral(conversion.Multiplier);
		string logBase = ToDoubleLiteral(conversion.LogBase);
		string referenceExpr = BuildReferenceExpression(conversion.Reference);

		// scale = multiplier · log_base(linear / reference)
		string ratioExpr = referenceExpr == null ? "linearValue" : "linearValue / reference";
		string logExpr = logBase switch
		{
			"10.0" => $"Math.Log10({ratioExpr})",
			"2.0" => $"Math.Log2({ratioExpr})",
			_ => $"Math.Log({ratioExpr}, {logBase})",
		};

		cb.WriteLine("/// <summary>");
		cb.WriteLine($"/// {conversion.FromSummary ?? $"Creates a value from the linear {linear}."}");
		cb.WriteLine("/// </summary>");
		cb.WriteLine($"/// <param name=\"linear\">The linear <see cref=\"{linear}{{T}}\"/>.</param>");
		cb.WriteLine($"/// <returns>A new <see cref=\"{scale.Name}{{T}}\"/>. A linear value of zero maps to negative infinity.</returns>");
		cb.WriteLine($"public static {fullType} {fromName}({linear}<T> linear)");
		using (new Scope(cb))
		{
			cb.WriteLine("ArgumentNullException.ThrowIfNull(linear);");
			cb.WriteLine("double linearValue = double.CreateChecked(linear.Value);");
			if (referenceExpr != null)
			{
				cb.WriteLine($"double reference = {referenceExpr};");
			}

			cb.WriteLine($"return new(T.CreateChecked({multiplier} * {logExpr}));");
		}

		cb.NewLine();

		// linear = reference · base^(scale / multiplier)
		string inverseExpr = referenceExpr == null
			? $"Math.Pow({logBase}, scaleValue / {multiplier})"
			: $"reference * Math.Pow({logBase}, scaleValue / {multiplier})";

		cb.WriteLine("/// <summary>");
		cb.WriteLine($"/// {conversion.ToSummary ?? $"Converts this value to the linear {linear}."}");
		cb.WriteLine("/// </summary>");
		cb.WriteLine($"/// <returns>The linear <see cref=\"{linear}{{T}}\"/>.</returns>");
		cb.WriteLine($"public {linear}<T> {toName}()");
		using (new Scope(cb))
		{
			cb.WriteLine("double scaleValue = double.CreateChecked(Value);");
			if (referenceExpr != null)
			{
				cb.WriteLine($"double reference = {referenceExpr};");
			}

			cb.WriteLine($"return {linear}<T>.Create(T.CreateChecked({inverseExpr}));");
		}

		cb.NewLine();
	}

	private static void WriteArithmetic(CodeBlocker cb, string fullType)
	{
		cb.WriteLine("/// <summary>Adds two values in log space (cascading two linear stages multiplies them).</summary>");
		cb.WriteLine("/// <param name=\"left\">The first value.</param>");
		cb.WriteLine("/// <param name=\"right\">The second value.</param>");
		cb.WriteLine("/// <returns>The summed value.</returns>");
		cb.WriteLine($"public static {fullType} operator +({fullType} left, {fullType} right) => new(left.Value + right.Value);");
		cb.NewLine();
		cb.WriteLine("/// <summary>Subtracts one value from another in log space.</summary>");
		cb.WriteLine("/// <param name=\"left\">The value to subtract from.</param>");
		cb.WriteLine("/// <param name=\"right\">The value to subtract.</param>");
		cb.WriteLine("/// <returns>The difference.</returns>");
		cb.WriteLine($"public static {fullType} operator -({fullType} left, {fullType} right) => new(left.Value - right.Value);");
		cb.NewLine();
		cb.WriteLine("/// <summary>Adds two values (friendly alternate for <c>operator +</c>).</summary>");
		cb.WriteLine("/// <param name=\"left\">The first value.</param>");
		cb.WriteLine("/// <param name=\"right\">The second value.</param>");
		cb.WriteLine("/// <returns>The summed value.</returns>");
		cb.WriteLine($"public static {fullType} Add({fullType} left, {fullType} right) => left + right;");
		cb.NewLine();
		cb.WriteLine("/// <summary>Subtracts one value from another (friendly alternate for <c>operator -</c>).</summary>");
		cb.WriteLine("/// <param name=\"left\">The value to subtract from.</param>");
		cb.WriteLine("/// <param name=\"right\">The value to subtract.</param>");
		cb.WriteLine("/// <returns>The difference.</returns>");
		cb.WriteLine($"public static {fullType} Subtract({fullType} left, {fullType} right) => left - right;");
		cb.NewLine();
	}

	private static void WriteComparisons(CodeBlocker cb, string fullType)
	{
		cb.WriteLine("/// <inheritdoc/>");
		cb.WriteLine($"public int CompareTo({fullType} other) => Value.CompareTo(other.Value);");
		cb.NewLine();
		(string op, string word)[] comparisons =
		[
			("<", "less than"),
			(">", "greater than"),
			("<=", "less than or equal to"),
			(">=", "greater than or equal to"),
		];
		foreach ((string op, string word) in comparisons)
		{
			cb.WriteLine($"/// <summary>Determines whether one value is {word} another.</summary>");
			cb.WriteLine("/// <param name=\"left\">The left value.</param>");
			cb.WriteLine("/// <param name=\"right\">The right value.</param>");
			cb.WriteLine($"/// <returns><see langword=\"true\"/> if <paramref name=\"left\"/> is {word} <paramref name=\"right\"/>.</returns>");
			cb.WriteLine($"public static bool operator {op}({fullType} left, {fullType} right) => left.CompareTo(right) {op} 0;");
			cb.NewLine();
		}
	}

	private static void WriteToString(CodeBlocker cb, LogarithmicScaleDefinition scale)
	{
		string interpolated = scale.DisplayFormat.Replace("{0}", "{Value}");
		cb.WriteLine("/// <summary>Returns a culture-invariant string representation of this value.</summary>");
		cb.WriteLine("/// <returns>The formatted value.</returns>");
		cb.WriteLine($"public override string ToString() => string.Create(CultureInfo.InvariantCulture, $\"{interpolated}\");");
	}

	/// <summary>
	/// Builds the C# expression for the conversion reference, or <see langword="null"/>
	/// when the reference is one (no scaling).
	/// </summary>
	private static string? BuildReferenceExpression(LogarithmicReferenceDefinition? reference)
	{
		if (reference == null)
		{
			return null;
		}

		if (!string.IsNullOrWhiteSpace(reference.Constant))
		{
			return $"PhysicalConstants.Generic.{reference.Constant}<double>()";
		}

		return string.IsNullOrWhiteSpace(reference.Value) ? null : ToDoubleLiteral(reference.Value!);
	}

	/// <summary>Normalises a metadata numeric string into a C# double literal.</summary>
	private static string ToDoubleLiteral(string value) =>
		double.Parse(value, CultureInfo.InvariantCulture).ToString("0.0###############", CultureInfo.InvariantCulture);
}
