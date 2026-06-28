// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

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
		CollectionAssert.AreEqual(expected, midi);
	}

	[TestMethod]
	public void Voice_FirstInversionRaisesTheRoot()
	{
		int[] expected = [64, 67, 72]; // E4 G4 C5
		int[] midi = [.. Chord.Parse("C").Voice(4, 1).Select(p => p.Midi)];
		CollectionAssert.AreEqual(expected, midi);
	}

	[TestMethod]
	public void Voice_SecondInversionRaisesRootAndThird()
	{
		int[] expected = [67, 72, 76]; // G4 C5 E5
		int[] midi = [.. Chord.Parse("C").Voice(4, 2).Select(p => p.Midi)];
		CollectionAssert.AreEqual(expected, midi);
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
