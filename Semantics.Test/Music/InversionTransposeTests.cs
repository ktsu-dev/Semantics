// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class InversionTransposeTests
{
	[TestMethod]
	public void Voice_RootPositionStacksFromRoot()
	{
		int[] expected = [60, 64, 67];
		int[] midi = [.. Chord.Parse("C").Voice(4, 0).Select(p => p.Midi)];
		Assert.AreSequenceEqual(expected, midi);
	}

	[TestMethod]
	public void Voice_FirstInversionRaisesTheRoot()
	{
		int[] expected = [64, 67, 72]; // E4 G4 C5
		int[] midi = [.. Chord.Parse("C").Voice(4, 1).Select(p => p.Midi)];
		Assert.AreSequenceEqual(expected, midi);
	}

	[TestMethod]
	public void Voice_SecondInversionRaisesRootAndThird()
	{
		int[] expected = [67, 72, 76]; // G4 C5 E5
		int[] midi = [.. Chord.Parse("C").Voice(4, 2).Select(p => p.Midi)];
		Assert.AreSequenceEqual(expected, midi);
	}

	[TestMethod]
	[DataRow("C9", 4, new[] { 74, 76, 79, 82, 84 })]
	[DataRow("C13", 4, new[] { 74, 76, 77, 79, 81, 82, 84 })]
	[DataRow("C13", 6, new[] { 81, 82, 84, 86, 88, 89, 91 })]
	public void Voice_InversionsReachTheTensions(string symbol, int inversion, int[] expected)
	{
		int[] midi = [.. Chord.Parse(symbol).Voice(4, inversion).Select(p => p.Midi)];
		Assert.AreSequenceEqual(expected, midi);
	}

	[TestMethod]
	public void Voice_EveryInversionPutsItsChordToneLowest()
	{
		string[] symbols = ["C", "Cmaj7", "Cm9", "C11", "C13", "C13b9#11", "C6/9", "Cmaj7#11", "C7sus4", "Cadd9", "F#m7b5"];
		foreach (string symbol in symbols)
		{
			Chord chord = Chord.Parse(symbol);
			IReadOnlyList<int> tones = chord.ChordTones();
			for (int inversion = 0; inversion < tones.Count; inversion++)
			{
				IReadOnlyList<Pitch> voicing = chord.Voice(4, inversion);
				Assert.AreEqual(PitchClass.Create(chord.Root.Value + tones[inversion]), voicing[0].PitchClass, $"{symbol} inversion {inversion}");
				Assert.AreEqual(tones.Count, voicing.Count, $"{symbol} inversion {inversion}");
				for (int i = 1; i < voicing.Count; i++)
				{
					Assert.IsTrue(voicing[i].Midi > voicing[i - 1].Midi, $"{symbol} inversion {inversion}");
				}
			}
		}
	}

	[TestMethod]
	public void Voice_OutOfMidiRangeThrowsOneException()
	{
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Chord.Parse("B").Voice(9));
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Chord.Parse("G").Voice(9));
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Chord.Parse("C/G").Voice(-1));
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Chord.Parse("C").Voice(int.MaxValue));
	}

	[TestMethod]
	public void Chord_TransposePreservesQualityAndMovesRootAndBass()
	{
		Chord transposed = Chord.Parse("Cmaj7/G").Transpose(2); // up a tone -> D
		Assert.AreEqual(2, transposed.Root.Value);
		Assert.AreEqual(ChordQuality.Major, transposed.Quality);
		Assert.AreEqual(SeventhType.Major, transposed.Seventh);
		Assert.IsNotNull(transposed.Bass);
		Assert.AreEqual(9, transposed.Bass!.Value); // G -> A
	}

	[TestMethod]
	public void Scale_TransposeMovesRootKeepsMode()
	{
		Scale gMajor = Scale.Create(PitchClass.Create(0), Mode.Major).Transpose(7);
		Assert.AreEqual(7, gMajor.Root.Value);
		Assert.AreEqual(Mode.Major, gMajor.Mode);
	}

	[TestMethod]
	public void Key_TransposeMovesTonic()
	{
		Key eFlat = Key.Create(PitchClass.Create(0), Mode.Major).Transpose(3);
		Assert.AreEqual(3, eFlat.Tonic.Value);
	}
}
