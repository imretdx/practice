using System;
using Xunit;
using task11;

namespace task11tests;

public class DynamicCalculatorTests
{
    // Строка с кодом, модифицированная для реализации нашего интерфейса ICalculator
    private const string CalculatorSourceCode = @"
    using task11;

    namespace task11;

    public class Calculator : ICalculator
    {
        public int Add(int a, int b) => a + b;
        public int Minus(int a, int b) => a - b;
        public int Mul(int a, int b) => a * b;
        public int Div(int a, int b) => a / b;
    }";

    [Fact]
    public void DynamicCalculator_ShouldPerformArithmeticOperationsWithoutReflection()
    {
        // Генерируем класс на лету
        ICalculator calc = DynamicCalculatorFactory.CreateFromString(CalculatorSourceCode);

        // Проверяем вызовы методов напрямую через интерфейс (БЕЗ РЕФЛЕКСИИ)
        Assert.Equal(10, calc.Add(7, 3));
        Assert.Equal(4, calc.Minus(7, 3));
        Assert.Equal(21, calc.Mul(7, 3));
        Assert.Equal(2, calc.Div(6, 3));
    }
}
