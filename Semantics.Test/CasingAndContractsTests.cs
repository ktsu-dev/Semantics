// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class CasingAndContractsTests
{
	[IsCamelCase]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record CamelCaseString : SemanticString<CamelCaseString> { }

	[IsPascalCase]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record PascalCaseString : SemanticString<PascalCaseString> { }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record ContractString : SemanticString<ContractString> { }

	[TestMethod]
	public void CamelCase_Valid_ShouldPass()
	{
		CamelCaseString s = SemanticString<CamelCaseString>.Create<CamelCaseString>("camelCase");
		Assert.AreEqual("camelCase", s.WeakString);
	}

	[TestMethod]
	public void CamelCase_Invalid_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<CamelCaseString>.Create<CamelCaseString>("CamelCase"));
	}

	[TestMethod]
	public void CamelCase_Empty_ShouldPass()
	{
		CamelCaseString s = SemanticString<CamelCaseString>.Create<CamelCaseString>("");
		Assert.AreEqual("", s.WeakString);
	}

	[TestMethod]
	public void PascalCase_Valid_ShouldPass()
	{
		PascalCaseString s = SemanticString<PascalCaseString>.Create<PascalCaseString>("PascalCase");
		Assert.AreEqual("PascalCase", s.WeakString);
	}

	[TestMethod]
	public void PascalCase_Invalid_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<PascalCaseString>.Create<PascalCaseString>("pascalCase"));
	}

	[TestMethod]
	public void PascalCase_Empty_ShouldPass()
	{
		PascalCaseString s = SemanticString<PascalCaseString>.Create<PascalCaseString>("");
		Assert.AreEqual("", s.WeakString);
	}

	[TestMethod]
	public void Contracts_ValidateContracts_ShouldReturnTrue()
	{
		ContractString s = SemanticString<ContractString>.Create<ContractString>("abc");
		Assert.IsTrue(SemanticStringContracts.ValidateContracts(s));
	}

	[TestMethod]
	public void Contracts_ValidateEqualityContracts_ShouldReturnTrue()
	{
		ContractString a = SemanticString<ContractString>.Create<ContractString>("same");
		ContractString b = SemanticString<ContractString>.Create<ContractString>("same");
		Assert.IsTrue(SemanticStringContracts.ValidateEqualityContracts(a, b));
	}

	[TestMethod]
	public void Contracts_ValidateComparisonContracts_ShouldReturnTrue()
	{
		ContractString a = SemanticString<ContractString>.Create<ContractString>("a");
		ContractString b = SemanticString<ContractString>.Create<ContractString>("b");
		ContractString c = SemanticString<ContractString>.Create<ContractString>("c");
		Assert.IsTrue(SemanticStringContracts.ValidateComparisonContracts(a, b, c));
	}
}
