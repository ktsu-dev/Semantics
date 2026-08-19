// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test;

using System.Text.Json;

using ktsu.RoundTripStringJsonConverter;
using ktsu.Semantics.Paths;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers the JSON contract of semantic strings. Semantics deliberately ships no
/// <c>JsonConverter</c> attribute on <see cref="SemanticString{TDerived}"/>; registering a converter
/// is the consumer's responsibility. These tests exercise that registration exactly as a consumer
/// would, and pin the default behaviour so a change to it is visible rather than silent.
/// </summary>
[TestClass]
public class SemanticStringJsonTests
{
	public record CardName : SemanticString<CardName> { }

	public sealed record Card(CardName Name);

	private static JsonSerializerOptions RegisteredOptions => new()
	{
		Converters = { new RoundTripStringJsonConverterFactory() },
	};

	[TestMethod]
	public void Serialize_WithRegisteredConverter_WritesPlainString()
	{
		// Arrange
		CardName name = SemanticString<CardName>.Create<CardName>("Pikachu");

		// Act
		string json = JsonSerializer.Serialize(name, RegisteredOptions);

		// Assert
		Assert.AreEqual("\"Pikachu\"", json);
	}

	[TestMethod]
	public void Deserialize_WithRegisteredConverter_ReadsPlainString()
	{
		// Act
		CardName name = JsonSerializer.Deserialize<CardName>("\"Pikachu\"", RegisteredOptions)!;

		// Assert
		Assert.AreEqual("Pikachu", name.WeakString);
	}

	[TestMethod]
	public void RoundTrip_WithRegisteredConverter_PreservesNestedValue()
	{
		// Arrange
		Card card = new(SemanticString<CardName>.Create<CardName>("Charizard"));

		// Act
		string json = JsonSerializer.Serialize(card, RegisteredOptions);
		Card restored = JsonSerializer.Deserialize<Card>(json, RegisteredOptions)!;

		// Assert
		Assert.AreEqual("{\"Name\":\"Charizard\"}", json);
		Assert.AreEqual(card.Name, restored.Name);
	}

	[TestMethod]
	public void RoundTrip_WithRegisteredConverter_PreservesPathTypes()
	{
		// Arrange
		AbsoluteFilePath path = SemanticString<AbsoluteFilePath>.Create<AbsoluteFilePath>(
			System.IO.Path.Combine(System.IO.Path.GetTempPath(), "semantics", "probe.txt"));

		// Act
		string json = JsonSerializer.Serialize(path, RegisteredOptions);
		AbsoluteFilePath restored = JsonSerializer.Deserialize<AbsoluteFilePath>(json, RegisteredOptions)!;

		// Assert
		Assert.AreEqual(path, restored);
	}

	[TestMethod]
	public void Deserialize_WithoutRegisteredConverter_RejectsPlainString()
	{
		// A semantic string is an IEnumerable<char>, so with no converter registered
		// System.Text.Json treats it as a collection and cannot read a JSON string.
		// This is why consumers must register a converter.
		Assert.ThrowsExactly<JsonException>(
			() => JsonSerializer.Deserialize<CardName>("\"Pikachu\""));
	}
}
