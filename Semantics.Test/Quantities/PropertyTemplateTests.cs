// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using ktsu.CodeBlocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using global::Semantics.SourceGenerators.Templates;

/// <summary>
/// Covers <see cref="PropertyTemplate"/>'s auto-property shorthand emission.
/// </summary>
/// <remarks>
/// <see cref="PropertyTemplate.AutoGet"/>, <see cref="PropertyTemplate.AutoSet"/> and
/// <see cref="PropertyTemplate.AutoInit"/> are the accessor factories the template compares against
/// by reference to decide whether a property can be written in shorthand. No generator assigns them
/// today, so the shorthand branches are currently unreachable from production code — these tests pin
/// the intended behaviour so the affordance is exercised rather than silently rotting.
/// </remarks>
[TestClass]
public class PropertyTemplateTests
{
	[TestMethod]
	public void WriteTo_AutoGetAndAutoSet_EmitsShorthand()
	{
		string output = Emit(new PropertyTemplate
		{
			Type = "int",
			Name = "Count",
			GetterFactory = PropertyTemplate.AutoGet,
			SetterFactory = PropertyTemplate.AutoSet,
		});

		Assert.Contains("int Count { get; set; }", output);
	}

	[TestMethod]
	public void WriteTo_AutoGetAndAutoInit_EmitsInitShorthand()
	{
		string output = Emit(new PropertyTemplate
		{
			Type = "string",
			Name = "Symbol",
			GetterFactory = PropertyTemplate.AutoGet,
			SetterFactory = PropertyTemplate.AutoInit,
		});

		Assert.Contains("string Symbol { get; init; }", output);
	}

	[TestMethod]
	public void WriteTo_NoAccessors_EmitsAbstractProperty()
	{
		string output = Emit(new PropertyTemplate
		{
			Type = "double",
			Name = "Value",
		});

		Assert.Contains("double Value;", output);
	}

	[TestMethod]
	public void WriteTo_CustomGetter_EmitsFullBody()
	{
		string output = Emit(new PropertyTemplate
		{
			Type = "int",
			Name = "Doubled",
			GetterFactory = (cb) => cb.Write("get => field * 2;"),
		});

		Assert.Contains("get => field * 2;", output);
		Assert.DoesNotContain("{ get;", output, "A custom getter must not be written as auto-property shorthand.");
	}

	private static string Emit(PropertyTemplate template)
	{
		using CodeBlocker codeBlocker = CodeBlocker.Create();
		template.WriteTo(codeBlocker);
		return codeBlocker.ToString();
	}
}
