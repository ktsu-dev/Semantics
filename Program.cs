using ktsu.Semantics;

public record TestSemanticString : SemanticString<TestSemanticString> { }

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Debugging IndexOf and LastIndexOf Issues ===\n");

        // Test case 1: LastIndexOf with StringComparison - line 740
        var semanticString1 = SemanticString<TestSemanticString>.FromString<TestSemanticString>("Hello World Hello");
        Console.WriteLine($"Test 1 - String: '{semanticString1}'");
        Console.WriteLine($"Length: {semanticString1.Length}");

        var result1 = semanticString1.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"semanticString.LastIndexOf(\"HELLO\", 10, OrdinalIgnoreCase): {result1} (expected: 0)");

        var normalString1 = "Hello World Hello";
        var normalResult1 = normalString1.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"normalString.LastIndexOf(\"HELLO\", 10, OrdinalIgnoreCase): {normalResult1}");

        // Test the WeakString directly
        var weakResult1 = semanticString1.WeakString.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"semanticString.WeakString.LastIndexOf(\"HELLO\", 10, OrdinalIgnoreCase): {weakResult1}");
        Console.WriteLine();

        // Test case 2: IndexOf with start index - line 755
        var semanticString2 = SemanticString<TestSemanticString>.FromString<TestSemanticString>("hello world hello");
        Console.WriteLine($"Test 2 - String: '{semanticString2}'");

        var result2 = semanticString2.IndexOf("hello", 5);
        Console.WriteLine($"semanticString.IndexOf(\"hello\", 5): {result2} (expected: 12)");

        var normalString2 = "hello world hello";
        var normalResult2 = normalString2.IndexOf("hello", 5);
        Console.WriteLine($"normalString.IndexOf(\"hello\", 5): {normalResult2}");

        var weakResult2 = semanticString2.WeakString.IndexOf("hello", 5);
        Console.WriteLine($"semanticString.WeakString.IndexOf(\"hello\", 5): {weakResult2}");
        Console.WriteLine();

        // Test case 3: LastIndexOf with start index - line 769
        Console.WriteLine($"Test 3 - String: '{semanticString2}'");

        var result3 = semanticString2.LastIndexOf("hello", 10);
        Console.WriteLine($"semanticString.LastIndexOf(\"hello\", 10): {result3} (expected: 0)");

        var normalResult3 = normalString2.LastIndexOf("hello", 10);
        Console.WriteLine($"normalString.LastIndexOf(\"hello\", 10): {normalResult3}");

        var weakResult3 = semanticString2.WeakString.LastIndexOf("hello", 10);
        Console.WriteLine($"semanticString.WeakString.LastIndexOf(\"hello\", 10): {weakResult3}");
        Console.WriteLine();

        // Let's test all overloads to see which ones work and which don't
        Console.WriteLine("=== Testing all IndexOf overloads ===");
        TestAllIndexOfOverloads(semanticString2);

        Console.WriteLine("\n=== Testing all LastIndexOf overloads ===");
        TestAllLastIndexOfOverloads(semanticString1);
    }

    static void TestAllIndexOfOverloads(TestSemanticString semanticString)
    {
        var str = semanticString.WeakString;
        Console.WriteLine($"Testing with string: '{str}'");

        // Basic overloads
        Console.WriteLine($"IndexOf('l'): semantic={semanticString.IndexOf('l')}, normal={str.IndexOf('l')}");
        Console.WriteLine($"IndexOf('l', 1): semantic={semanticString.IndexOf('l', 1)}, normal={str.IndexOf('l', 1)}");
        Console.WriteLine($"IndexOf('l', 1, 5): semantic={semanticString.IndexOf('l', 1, 5)}, normal={str.IndexOf('l', 1, 5)}");

        Console.WriteLine($"IndexOf(\"hello\"): semantic={semanticString.IndexOf("hello")}, normal={str.IndexOf("hello")}");
        Console.WriteLine($"IndexOf(\"hello\", 5): semantic={semanticString.IndexOf("hello", 5)}, normal={str.IndexOf("hello", 5)}");
        Console.WriteLine($"IndexOf(\"hello\", 5, 8): semantic={semanticString.IndexOf("hello", 5, 8)}, normal={str.IndexOf("hello", 5, 8)}");

        // StringComparison overloads
        Console.WriteLine($"IndexOf(\"HELLO\", OrdinalIgnoreCase): semantic={semanticString.IndexOf("HELLO", StringComparison.OrdinalIgnoreCase)}, normal={str.IndexOf("HELLO", StringComparison.OrdinalIgnoreCase)}");
        Console.WriteLine($"IndexOf(\"HELLO\", 1, OrdinalIgnoreCase): semantic={semanticString.IndexOf("HELLO", 1, StringComparison.OrdinalIgnoreCase)}, normal={str.IndexOf("HELLO", 1, StringComparison.OrdinalIgnoreCase)}");
        Console.WriteLine($"IndexOf(\"HELLO\", 0, 10, OrdinalIgnoreCase): semantic={semanticString.IndexOf("HELLO", 0, 10, StringComparison.OrdinalIgnoreCase)}, normal={str.IndexOf("HELLO", 0, 10, StringComparison.OrdinalIgnoreCase)}");
    }

    static void TestAllLastIndexOfOverloads(TestSemanticString semanticString)
    {
        var str = semanticString.WeakString;
        Console.WriteLine($"Testing with string: '{str}'");

        // Basic overloads
        Console.WriteLine($"LastIndexOf('l'): semantic={semanticString.LastIndexOf('l')}, normal={str.LastIndexOf('l')}");
        Console.WriteLine($"LastIndexOf('l', 10): semantic={semanticString.LastIndexOf('l', 10)}, normal={str.LastIndexOf('l', 10)}");
        Console.WriteLine($"LastIndexOf('l', 10, 5): semantic={semanticString.LastIndexOf('l', 10, 5)}, normal={str.LastIndexOf('l', 10, 5)}");

        Console.WriteLine($"LastIndexOf(\"Hello\"): semantic={semanticString.LastIndexOf("Hello")}, normal={str.LastIndexOf("Hello")}");
        Console.WriteLine($"LastIndexOf(\"Hello\", 10): semantic={semanticString.LastIndexOf("Hello", 10)}, normal={str.LastIndexOf("Hello", 10)}");
        Console.WriteLine($"LastIndexOf(\"Hello\", 10, 5): semantic={semanticString.LastIndexOf("Hello", 10, 5)}, normal={str.LastIndexOf("Hello", 10, 5)}");

        // StringComparison overloads
        Console.WriteLine($"LastIndexOf(\"HELLO\", OrdinalIgnoreCase): semantic={semanticString.LastIndexOf("HELLO", StringComparison.OrdinalIgnoreCase)}, normal={str.LastIndexOf("HELLO", StringComparison.OrdinalIgnoreCase)}");
        Console.WriteLine($"LastIndexOf(\"HELLO\", 10, OrdinalIgnoreCase): semantic={semanticString.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase)}, normal={str.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase)}");
        Console.WriteLine($"LastIndexOf(\"HELLO\", 10, 5, OrdinalIgnoreCase): semantic={semanticString.LastIndexOf("HELLO", 10, 5, StringComparison.OrdinalIgnoreCase)}, normal={str.LastIndexOf("HELLO", 10, 5, StringComparison.OrdinalIgnoreCase)}");
    }
}
