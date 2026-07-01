// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>The ordered realization of a piece: a sequence of sections in performance order.</summary>
public sealed record Arrangement
{
	/// <summary>Gets the home key used for whole-piece analysis; a section may override it locally.</summary>
	public Key Key { get; private init; } = Key.Create(PitchClass.Create(0), Mode.Major);

	/// <summary>Gets the sections in performance order; repetition is expressed by repeated entries.</summary>
	public IReadOnlyList<Section> Sections { get; private init; } = [];

	/// <summary>Gets the total length of the arrangement in bars.</summary>
	public double TotalBars => Sections.Sum(section => section.Bars);

	/// <summary>Gets the structural form of the arrangement.</summary>
	public Form Form => Form.Of(this);

	/// <summary>Creates an arrangement.</summary>
	/// <param name="key">The home key.</param>
	/// <param name="sections">The sections in performance order; must be non-empty.</param>
	/// <returns>A new arrangement.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="sections"/> is empty.</exception>
	public static Arrangement Create(Key key, IEnumerable<Section> sections)
	{
		Ensure.NotNull(key);
		Ensure.NotNull(sections);
		List<Section> list = [.. sections];
		if (list.Count == 0)
		{
			throw new ArgumentException("An arrangement must contain at least one section.", nameof(sections));
		}

		return new() { Key = key, Sections = list };
	}
}
