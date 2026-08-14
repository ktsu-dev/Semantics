// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

/// <summary>
/// A scale degree: a 1-based diatonic degree plus a chromatic alteration in semitones.
/// </summary>
/// <param name="Degree">The 1-based diatonic degree (1..mode degree count).</param>
/// <param name="Alteration">Signed semitone alteration: 0 diatonic, -1 flat, +1 sharp.</param>
public readonly record struct ScaleDegree(int Degree, int Alteration);
