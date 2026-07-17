using System.Linq;
using Xunit;
using task03;

namespace task03tests;

public class FibonacciTests
{
    [Fact]
    public void Generate_ReturnsCorrectFirstElements()
    {
        // Проверяем первые 6 чисел Фибоначчи: 0, 1, 1, 2, 3, 5
        var result = FibonacciGenerator.Generate().Take(6).ToList();
        
        Assert.Equal(new long[] { 0, 1, 1, 2, 3, 5 }, result);
    }

    [Fact]
    public void GetFilteredSequence_EvenNumbers_ReturnsOnlyEven()
    {
        // Проверяем генерацию первых 3 ЧЕТНЫХ чисел Фибоначчи: 0, 2, 8
        var result = FibonacciGenerator.GetFilteredSequence(3, x => x % 2 == 0).ToList();
        
        Assert.Equal(new long[] { 0, 2, 8 }, result);
    }

    [Fact]
    public void GetFilteredSequence_NegativeCount_ReturnsEmpty()
    {
        var result = FibonacciGenerator.GetFilteredSequence(-5, x => true).ToList();
        
        Assert.Empty(result);
    }
}
