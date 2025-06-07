using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Debugging String IndexOf and LastIndexOf Issues ===\n");

        // Simulate the exact test cases that are failing
        var str1 = "Hello World Hello";
        var str2 = "hello world hello";

        Console.WriteLine($"Test string 1: '{str1}'");
        Console.WriteLine($"Test string 2: '{str2}'");
        Console.WriteLine();

        // Test case 1: LastIndexOf with StringComparison - from line 740
        Console.WriteLine("=== Test 1: LastIndexOf(\"HELLO\", 10, OrdinalIgnoreCase) ===");
        var result1 = str1.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"Result: {result1} (expected: 0)");
        Console.WriteLine($"Match: {result1 == 0}");
        Console.WriteLine();

        // Test case 2: IndexOf with start index - from line 755
        Console.WriteLine("=== Test 2: IndexOf(\"hello\", 5) ===");
        var result2 = str2.IndexOf("hello", 5);
        Console.WriteLine($"Result: {result2} (expected: 12)");
        Console.WriteLine($"Match: {result2 == 12}");
        Console.WriteLine();

        // Test case 3: LastIndexOf with start index - from line 769
        Console.WriteLine("=== Test 3: LastIndexOf(\"hello\", 10) ===");
        var result3 = str2.LastIndexOf("hello", 10);
        Console.WriteLine($"Result: {result3} (expected: 0)");
        Console.WriteLine($"Match: {result3 == 0}");
        Console.WriteLine();

        // Additional debugging - let's see what the strings actually contain
        Console.WriteLine("=== String Analysis ===");
        Console.WriteLine($"str1.Length: {str1.Length}");
        Console.WriteLine($"str2.Length: {str2.Length}");

        Console.WriteLine("\nPositions in str1:");
        for (int i = 0; i < str1.Length; i++)
        {
            Console.WriteLine($"[{i}] = '{str1[i]}'");
        }

        Console.WriteLine("\nPositions in str2:");
        for (int i = 0; i < str2.Length; i++)
        {
            Console.WriteLine($"[{i}] = '{str2[i]}'");
        }

        // Let's test step by step for the failing cases
        Console.WriteLine("\n=== Step by step analysis for case 1 ===");
        Console.WriteLine("Looking for 'HELLO' in 'Hello World Hello' with OrdinalIgnoreCase");
        Console.WriteLine("Starting from index 10 and searching backwards");

        // Manual search to understand what should happen
        var searchTarget = "HELLO";
        var searchStart = 10;
        Console.WriteLine($"Characters from position {searchStart} backwards:");
        for (int i = searchStart; i >= 0; i--)
        {
            Console.WriteLine($"[{i}] = '{str1[i]}'");
        }

        Console.WriteLine("\n=== Step by step analysis for case 2 ===");
        Console.WriteLine("Looking for 'hello' in 'hello world hello'");
        Console.WriteLine("Starting from index 5 and searching forwards");
        Console.WriteLine($"Characters from position 5 onwards:");
        for (int i = 5; i < str2.Length; i++)
        {
            Console.WriteLine($"[{i}] = '{str2[i]}'");
        }

        Console.WriteLine("\n=== Step by step analysis for case 3 ===");
        Console.WriteLine("Looking for 'hello' in 'hello world hello'");
        Console.WriteLine("Starting from index 10 and searching backwards");
        Console.WriteLine($"Characters from position 10 backwards:");
        for (int i = 10; i >= 0; i--)
        {
            Console.WriteLine($"[{i}] = '{str2[i]}'");
        }
    }
}
