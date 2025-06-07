using System;

public class TestDebug
{
    public static void Main()
    {
        Console.WriteLine("=== Testing .NET String IndexOf/LastIndexOf Behavior ===");

        // Test the exact cases that are failing
        string test1 = "Hello World Hello";
        string test2 = "hello world hello";

        Console.WriteLine($"String 1: '{test1}' (length: {test1.Length})");
        Console.WriteLine($"String 2: '{test2}' (length: {test2.Length})");
        Console.WriteLine();

        // Test case 1: LastIndexOf with StringComparison (line 740)
        Console.WriteLine("=== Test 1: LastIndexOf(\"HELLO\", 10, OrdinalIgnoreCase) ===");
        var result1 = test1.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"Result: {result1} (expected: 0)");
        Console.WriteLine($"Test passes: {result1 == 0}");
        Console.WriteLine();

        // Test case 2: IndexOf with start index (line 755)
        Console.WriteLine("=== Test 2: IndexOf(\"hello\", 5) ===");
        var result2 = test2.IndexOf("hello", 5);
        Console.WriteLine($"Result: {result2} (expected: 12)");
        Console.WriteLine($"Test passes: {result2 == 12}");
        Console.WriteLine();

        // Test case 3: LastIndexOf with start index (line 769)
        Console.WriteLine("=== Test 3: LastIndexOf(\"hello\", 10) ===");
        var result3 = test2.LastIndexOf("hello", 10);
        Console.WriteLine($"Result: {result3} (expected: 0)");
        Console.WriteLine($"Test passes: {result3 == 0}");
        Console.WriteLine();

        // Let's debug the LastIndexOf behavior more carefully
        Console.WriteLine("=== Detailed LastIndexOf analysis ===");

        // For "Hello World Hello" searching for "HELLO" starting at position 10
        for (int i = 0; i <= test1.Length; i++)
        {
            var r = test1.LastIndexOf("HELLO", i, StringComparison.OrdinalIgnoreCase);
            Console.WriteLine($"LastIndexOf(\"HELLO\", {i}, OrdinalIgnoreCase): {r}");
        }

        Console.WriteLine();
        Console.WriteLine("=== Detailed IndexOf analysis ===");

        // For "hello world hello" searching for "hello" starting at position 5
        for (int i = 0; i <= test2.Length; i++)
        {
            var r = test2.IndexOf("hello", i);
            Console.WriteLine($"IndexOf(\"hello\", {i}): {r}");
        }
    }
}
