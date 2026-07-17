using System;
using System.Threading;
using System.Linq;

namespace task14;

public class DefiniteIntegral
{
    // Вспомогательный метод для атомарного добавления double через Interlocked
    private static void InterlockedAddDouble(ref double location, double value)
    {
        double newCurrentValue = location;
        while (true)
        {
            double currentValue = newCurrentValue;
            double newValue = currentValue + value;
            newCurrentValue = Interlocked.CompareExchange(ref location, newValue, currentValue);
            if (newCurrentValue == currentValue)
                break;
        }
    }

    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
    {
        if (threadsnumber <= 0) return 0.0;

        double globalResult = 0.0;
        
        // Барьер для синхронизации: ждем threadsnumber вычислительных потоков + 1 главный поток (всего threadsnumber + 1)
        using Barrier barrier = new Barrier(threadsnumber + 1);

        double totalRange = b - a;
        double rangePerThread = totalRange / threadsnumber;

        // Создаем и запускаем потоки через LINQ без использования циклов
        var threads = Enumerable.Range(0, threadsnumber).Select(i =>
        {
            return new Thread(() =>
            {
                // Вычисляем границы отрезка для текущего потока
                double threadA = a + i * rangePerThread;
                double threadB = (i == threadsnumber - 1) ? b : threadA + rangePerThread;

                double localSum = 0.0;
                double currentX = threadA;

                // Метод трапеций для выделенного отрезка
                while (currentX < threadB)
                {
                    double nextX = currentX + step;
                    if (nextX > threadB) nextX = threadB;

                    localSum += (function(currentX) + function(nextX)) * (nextX - currentX) / 2.0;
                    currentX = nextX;
                }

                // Безопасное суммирование результатов в общую переменную
                InterlockedAddDouble(ref globalResult, localSum);

                // Поток сигнализирует о завершении своей части работы
                barrier.SignalAndWait();
            });
        }).ToList();

        // Запускаем все потоки
        threads.ForEach(t => t.Start());

        // Главный поток дожидается выполнения всех вычислительных потоков на Барьере
        barrier.SignalAndWait();

        return globalResult;
    }
}
