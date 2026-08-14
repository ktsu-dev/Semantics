// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

/// <summary>
/// Literal fragments the generators emit into the generated C#. Naming them keeps the emission
/// sites readable and means a typo in a keyword or a documentation delimiter is a compile error
/// rather than malformed generated source.
/// </summary>
internal static class Emit
{
	/// <summary>The <c>public</c> modifier.</summary>
	internal const string Public = "public";

	/// <summary>The <c>static</c> modifier.</summary>
	internal const string Static = "static";

	/// <summary>Opening delimiter of an XML documentation summary.</summary>
	internal const string SummaryOpen = "/// <summary>";

	/// <summary>Closing delimiter of an XML documentation summary.</summary>
	internal const string SummaryClose = "/// </summary>";

	/// <summary>Conventional name of the single-value parameter on generated members.</summary>
	internal const string ValueParameter = "value";

	/// <summary>Conventional name of the right-hand operand on generated binary operators.</summary>
	internal const string RightParameter = "right";

	/// <summary>Category reported on generator diagnostics.</summary>
	internal const string DiagnosticCategory = "Semantics.SourceGenerators";

	/// <summary>
	/// Suppression emitted onto generated physics operators. CA2225 wants named alternates such as
	/// <c>Add</c> or <c>Multiply</c>, but those names do not carry the dimensional meaning the
	/// operator does, so the operators are deliberately the only spelling.
	/// </summary>
	internal const string PhysicsOperatorSuppression =
		"System.Diagnostics.CodeAnalysis.SuppressMessage(\"Usage\", \"CA2225:Operator overloads have named alternates\", Justification = \"Physics quantity operator\")";
}
