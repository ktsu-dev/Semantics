// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

/// <summary>
/// Literal fragments specific to this repository's generated output.
/// </summary>
/// <remarks>
/// The general C# vocabulary that used to sit alongside these — the <c>public</c> and
/// <c>static</c> modifiers, the XML documentation delimiters — has moved to the reusable layer as
/// <c>CodeGen.CSharpKeywords</c>. What is left is the part that only means something here: the
/// names this generator gives its parameters, and a suppression that is about physics.
/// </remarks>
internal static class Emit
{
	/// <summary>The <c>public</c> modifier.</summary>
	internal const string Public = CodeGen.CSharpKeywords.Public;

	/// <summary>The <c>static</c> modifier.</summary>
	internal const string Static = CodeGen.CSharpKeywords.Static;

	/// <summary>Opening delimiter of an XML documentation summary.</summary>
	internal const string SummaryOpen = "/// <summary>";

	/// <summary>Closing delimiter of an XML documentation summary.</summary>
	internal const string SummaryClose = "/// </summary>";

	/// <summary>Conventional name of the single-value parameter on generated members.</summary>
	internal const string ValueParameter = "value";

	/// <summary>Conventional name of the right-hand operand on generated binary operators.</summary>
	internal const string RightParameter = "right";

	/// <summary>
	/// Suppression emitted onto generated physics operators. CA2225 wants named alternates such as
	/// <c>Add</c> or <c>Multiply</c>, but those names do not carry the dimensional meaning the
	/// operator does, so the operators are deliberately the only spelling.
	/// </summary>
	internal const string PhysicsOperatorSuppression =
		"System.Diagnostics.CodeAnalysis.SuppressMessage(\"Usage\", \"CA2225:Operator overloads have named alternates\", Justification = \"Physics quantity operator\")";
}
