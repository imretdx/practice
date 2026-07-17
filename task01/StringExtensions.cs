using System;
using System.Linq;

namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (input == null) return false;

        // Очищаем строку через LINQ
        var cleanChars = input
            .ToLower()
            .Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c))
            .ToArray();

        if (cleanChars.Length == 0) return false;

        // Проверяем на палиндром методом SequenceEqual (без циклов)
        return cleanChars.SequenceEqual(cleanChars.Reverse());
    }

}
