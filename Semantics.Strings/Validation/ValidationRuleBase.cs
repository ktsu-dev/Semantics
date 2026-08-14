// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

/// <summary>
/// Base class for validation rules that provides common functionality.
/// </summary>
public abstract class ValidationRuleBase : IValidationRule
{
	/// <inheritdoc/>
	public abstract string Name { get; }

	/// <inheritdoc/>
	public virtual int Priority => 0;

	/// <inheritdoc/>
	public abstract bool Validate(ISemanticString semanticString);

	/// <inheritdoc/>
	public virtual string GetErrorMessage(ISemanticString semanticString)
		=> $"Validation rule '{Name}' failed for value: {semanticString}";
}
