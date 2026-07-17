using System;
using System.Collections.Generic;

namespace task03;

public static class FibonacciGenerator
{
    // Метод использует yield return для ленивой генерации бесконечной последовательности
    public static IEnumerable<long> Generate()
    {
        long current = 0;
        long next = 1;

        while (true)
        {
            yield return current;
            
            long temp = current + next;
            current = next;
            next = temp;
        }
    }

    // Ленивый фильтр: выбирает первые N чисел, соответствующие условию (например, только четные)
    public static IEnumerable<long> GetFilteredSequence(int count, Func<long, bool> predicate)
    {
        if (count <= 0) yield break;

        int generatedCount = 0;
        foreach (var number in Generate())
        {
            if (predicate(number))
            {
                yield return number;
                generatedCount++;
                
                if (generatedCount == count)
                    yield break;
            }
        }
    }
}
