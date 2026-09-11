// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp.Test;

using System;
using System.Diagnostics;
using System.IO;

using ktsu.Semantics.Cpp;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Compiles what the generator emits.
/// </summary>
/// <remarks>
/// Asserting on the text says the generator wrote what was expected; only a compiler says the
/// expectation was right. The whole vocabulary is put through one translation unit with warnings
/// on, which is the cheapest check that catches a missing include, a name that collides at
/// namespace scope, or a dimension written with the wrong number of arguments.
/// <para>
/// Skipped where no compiler is on PATH, which is how this behaves on a Windows runner. It is not
/// skipped silently: a run that never compiled anything reports inconclusive rather than green.
/// </para>
/// </remarks>
[TestClass]
public sealed class GeneratedCppCompilesTests
{
	private static string? Compiler => Find("g++") ?? Find("clang++");

	/// <summary>
	/// The whole vocabulary compiles, warnings included.
	/// </summary>
	[TestMethod]
	public void TheWholeVocabularyCompiles()
	{
		string directory = Emit();
		File.WriteAllText(Path.Combine(directory, "main.cpp"), "#include \"quantities.hpp\"\nint main() { return 0; }\n");

		(int exitCode, string output) = Compile(directory, "main.cpp");

		Assert.AreEqual(0, exitCode, $"the generated vocabulary should compile clean:\n{output}");
	}

	/// <summary>
	/// The structural layer actually checks the nominal one, rather than being carried along
	/// beside it.
	/// </summary>
	/// <remarks>
	/// This is the negative half of the claim the generator rests on. A relationship is emitted as
	/// <c>Result{ lhs.value() * rhs.value() }</c>, so the exponents have to agree with the declared
	/// result -- which is only worth saying if a disagreement really is a compile error. Here the
	/// product of two quantities is handed to a third whose dimension is something else, and the
	/// test fails if that is accepted.
	/// </remarks>
	[TestMethod]
	public void AProductWithTheWrongDimensionDoesNotCompile()
	{
		string directory = Emit();
		File.WriteAllText(Path.Combine(directory, "wrong.cpp"), """
			#include "Length.hpp"
			#include "Duration.hpp"
			#include "Speed.hpp"

			// A length times a duration is L T, and Speed is L T⁻¹. If this compiles, the
			// exponents are decoration and every relationship the generator "checked" was
			// checked against nothing.
			holo::Speed wrong(holo::Length l, holo::Duration d)
			{
				return holo::Speed{ l.value() * d.value() };
			}

			int main() { return 0; }
			""");

		(int exitCode, _) = Compile(directory, "wrong.cpp");

		Assert.AreNotEqual(0, exitCode, "a product whose exponents do not match the result type should be refused");
	}

	private static string Emit()
	{
		string directory = Path.Combine(Path.GetTempPath(), $"semantics-cpp-{Guid.NewGuid():N}");
		Directory.CreateDirectory(directory);

		CppQuantityOutput output = new CppQuantityGenerator(new CppQuantityOptions { Namespace = "holo" })
			.Generate(QuantityMetadata.Parse(File.ReadAllText(
				Path.Combine(AppContext.BaseDirectory, "Metadata", "dimensions.json"))));

		foreach ((string name, string text) in output.Files)
		{
			File.WriteAllText(Path.Combine(directory, name), text);
		}

		return directory;
	}

	private static (int ExitCode, string Output) Compile(string directory, string file)
	{
		string? compiler = Compiler;
		if (compiler is null)
		{
			Assert.Inconclusive("no C++ compiler on PATH, so the generated headers were not compiled.");
		}

		using Process process = new()
		{
			StartInfo = new ProcessStartInfo(compiler!)
			{
				WorkingDirectory = directory,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
			},
		};

		foreach (string argument in (string[])["-std=c++20", "-Wall", "-Wextra", "-fsyntax-only", "-I.", file])
		{
			process.StartInfo.ArgumentList.Add(argument);
		}

		process.Start();
		string output = process.StandardError.ReadToEnd() + process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		return (process.ExitCode, output);
	}

	private static string? Find(string executable)
	{
		string[] directories = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
			.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

		foreach (string directory in directories)
		{
			string candidate = Path.Combine(directory, executable);
			if (File.Exists(candidate))
			{
				return candidate;
			}
		}

		return null;
	}
}
