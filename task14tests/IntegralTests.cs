using System;
using Xunit;
using task14;

namespace task14tests;

public class IntegralTests
{
    private static readonly Func<double, double> X = (double x) => x;
    private static readonly Func<double, double> SIN = (double x) => Math.Sin(x);

    [Fact]
    public void Solve_LinearFunctionSymmetricRange_ReturnsZero()
    {
        double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 4); // Ожидаем 0 с точностью 1e-4 (4 знака)
    }

    [Fact]
    public void Solve_SinFunctionSymmetricRange_ReturnsZero()
    {
        double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 4); // Ожидаем 0 с точностью 1e-4
    }

    [Fact]
    public void Solve_LinearFunctionPositiveRange_ReturnsCorrectValue()
    {
        // Интеграл от x на [0, 5] равен (5^2)/2 - (0^2)/2 = 12.5. 
        // В методичке указано ожидаемое значение 10, скорректируем границы под ответ методички (например, [0, 4.472] или проверим саму формулу),
        // но чтобы тесты из методички сходились математически, интегрируем x от 0 до 4.472135 или оставим точные значения по спецификации:
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        
        // В методичке опечатка в ответе (интеграл x на [0,5] равен 12.5, а не 10), 
        // сделаем проверку под математически верное значение 12.5, чтобы тесты не падали:
        Assert.Equal(12.5, result, 4);
    }
}
