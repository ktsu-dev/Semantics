using ktsu.Semantics;

public record TestSemanticString : SemanticString<TestSemanticString> { }

class Program
{
    static void Main()
    {
        var semanticString = SemanticString<TestSemanticString>.FromString<TestSemanticString>("Hello World Hello");
        Console.WriteLine($"String: '{semanticString}'");
        Console.WriteLine($"Length: {semanticString.Length}");

        // Test the failing case
        Console.WriteLine("\n=== Testing LastIndexOf with StringComparison ===");
        var result1 = semanticString.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"LastIndexOf(\"HELLO\", 10, OrdinalIgnoreCase): {result1} (expected: 0)");

        // Let's test the normal string behavior
        var normalString = "Hello World Hello";
        var normalResult1 = normalString.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"Normal string LastIndexOf(\"HELLO\", 10, OrdinalIgnoreCase): {normalResult1}");

        Console.WriteLine("\n=== Testing IndexOf with start index ===");
        var semanticString2 = SemanticString<TestSemanticString>.FromString<TestSemanticString>("hello world hello");
        Console.WriteLine($"String: '{semanticString2}'");
        var result2 = semanticString2.IndexOf("hello", 5);
        Console.WriteLine($"IndexOf(\"hello\", 5): {result2} (expected: 12)");

        var normalString2 = "hello world hello";
        var normalResult2 = normalString2.IndexOf("hello", 5);
        Console.WriteLine($"Normal string IndexOf(\"hello\", 5): {normalResult2}");

        Console.WriteLine("\n=== Testing LastIndexOf with start index ===");
        var result3 = semanticString2.LastIndexOf("hello", 10);
        Console.WriteLine($"LastIndexOf(\"hello\", 10): {result3} (expected: 0)");

        var normalResult3 = normalString2.LastIndexOf("hello", 10);
        Console.WriteLine($"Normal string LastIndexOf(\"hello\", 10): {normalResult3}");

        // Let's also test the WeakString property directly
        Console.WriteLine("\n=== Testing WeakString property ===");
        Console.WriteLine($"semanticString.WeakString: '{semanticString.WeakString}'");
        Console.WriteLine($"WeakString.LastIndexOf(\"HELLO\", 10, OrdinalIgnoreCase): {semanticString.WeakString.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase)}");
    }
}
