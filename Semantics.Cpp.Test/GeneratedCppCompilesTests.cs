// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp.Test;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
		File.WriteAllText(Path.Join(directory, "main.cpp"), "#include \"quantities.hpp\"\nint main() { return 0; }\n");

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
		File.WriteAllText(Path.Join(directory, "wrong.cpp"), """
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

	/// <summary>
	/// The vector forms mean what they say, and the compiler is what says so.
	/// </summary>
	/// <remarks>
	/// Every claim below is a <c>static_assert</c>, so this needs no run: a wrong answer is a
	/// compile error and <c>-fsyntax-only</c> reaches it. That matters more for the vector forms
	/// than it did for the magnitudes, because a good deal of what they promise is arithmetic
	/// rather than shape -- that the length of (3, 4, 0) is 5, that scaling by a duration lands in
	/// the right type, that an overload survives the trip out to its base and back.
	/// <para>
	/// The layout assertions are the other half. Holotype copies a vector whole across a language
	/// boundary and onto the wire, which only works if the class is exactly its components with
	/// nothing added.
	/// </para>
	/// </remarks>
	[TestMethod]
	public void TheVectorFormsMeanWhatTheySay()
	{
		string directory = Emit();
		File.WriteAllText(Path.Join(directory, "meaning.cpp"), """
			#include "quantities.hpp"
			#include <type_traits>

			using namespace holo;

			// Exactly its components, which is what lets one be copied whole.
			static_assert(sizeof(Displacement3D) == 3 * sizeof(float));
			static_assert(std::is_trivially_copyable_v<Displacement3D>);
			static_assert(std::is_standard_layout_v<Displacement3D>);

			constexpr Displacement3D d{ Displacement3D::component{ 3.0f }, Displacement3D::component{ 4.0f }, Displacement3D::component{ 0.0f } };
			static_assert(d.magnitude_squared().count() == 25.0f);

			// A velocity scaled by a duration is a displacement, and it is componentwise.
			constexpr Duration t{ Duration::underlying{ 2.0f } };
			constexpr Velocity3D v{ Velocity3D::component{ 1.0f }, Velocity3D::component{ 2.0f }, Velocity3D::component{ 3.0f } };
			static_assert((v * t).x().count() == 2.0f);
			static_assert((v * t).z().count() == 6.0f);

			// The one-component form is signed, and its magnitude is not.
			constexpr Displacement1D back{ Displacement1D::underlying{ -5.0f } };
			static_assert(back.magnitude().value().count() == 5.0f);
			static_assert((-back).value().count() == 5.0f);

			// An overload widens to its base implicitly and narrows back by name, across every
			// component rather than only the first.
			constexpr Position3D p{ Position3D::component{ 1.0f }, Position3D::component{ 2.0f }, Position3D::component{ 3.0f } };
			constexpr Displacement3D widened = p;
			static_assert(widened.z().count() == 3.0f);
			static_assert(Position3D::from(widened).z().count() == 3.0f);

			int main() { return 0; }
			""");

		(int exitCode, string output) = Compile(directory, "meaning.cpp");

		Assert.AreEqual(0, exitCode, $"the vector forms should behave as generated:\n{output}");
	}

	/// <summary>
	/// The structural layer checks a componentwise relationship the same way it checks a scalar
	/// one.
	/// </summary>
	/// <remarks>
	/// The vector half of <see cref="AProductWithTheWrongDimensionDoesNotCompile"/>, and worth
	/// having separately: a generator that expanded the components correctly but lost the
	/// dimension on the way would pass the scalar test and fail here.
	/// </remarks>
	[TestMethod]
	public void AComponentwiseProductWithTheWrongDimensionDoesNotCompile()
	{
		string directory = Emit();
		File.WriteAllText(Path.Join(directory, "wrongvector.cpp"), """
			#include "Displacement3D.hpp"
			#include "Velocity3D.hpp"
			#include "Duration.hpp"

			// A displacement times a duration is L T, and a velocity is L T⁻¹. The components are
			// expanded correctly and the dimension is still wrong, which is the case that would
			// slip past a test that only looked at the shape.
			holo::Velocity3D wrong(holo::Displacement3D l, holo::Duration d)
			{
				return holo::Velocity3D{ l.x() * d.value(), l.y() * d.value(), l.z() * d.value() };
			}

			int main() { return 0; }
			""");

		(int exitCode, _) = Compile(directory, "wrongvector.cpp");

		Assert.AreNotEqual(0, exitCode, "a componentwise product whose exponents do not match the result type should be refused");
	}

	private static string Emit()
	{
		string directory = Path.Join(Path.GetTempPath(), $"semantics-cpp-{Guid.NewGuid():N}");
		Directory.CreateDirectory(directory);

		CppQuantityOutput output = new CppQuantityGenerator(new CppQuantityOptions { Namespace = "holo" })
			.Generate(QuantityMetadata.Parse(File.ReadAllText(
				Path.Join(AppContext.BaseDirectory, "Metadata", "dimensions.json"))));

		foreach ((string name, string text) in output.Files)
		{
			File.WriteAllText(Path.Join(directory, name), text);
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
		// The name alone, and joined rather than combined: a PATH entry is the directory, so a
		// candidate that turned out to be rooted would silently be the answer instead of a
		// directory's file.
		string name = Path.GetFileName(executable);

		return (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
			.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
			.Select(directory => Path.Join(directory, name))
			.FirstOrDefault(File.Exists);
	}
}
