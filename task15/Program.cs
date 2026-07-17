using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using task14;

namespace task15;

internal class Program
{
    // Чистая однопоточная реализация метода трапеций без использования потоков
    public static double SolveSingleThread(double a, double b, Func<double, double> function, double step)
    {
        double sum = 0.0;
        double currentX = a;

        while (currentX < b)
        {
            double nextX = currentX + step;
            if (nextX > b) nextX = b;

            sum += (function(currentX) + function(nextX)) * (nextX - currentX) / 2.0;
            currentX = nextX;
        }

        return sum;
    }

    private static void Main(string[] args)
    {
        Console.WriteLine("=== Запуск исследования производительности (Задание 15) ===");

        Func<double, double> sinFunc = Math.Sin;
        double a = -100.0;
        double b = 100.0;

        // Шаг 3. Эмпирически определено: для точности 1e-4 оптимальный шаг равен 1e-4
        double optimalStep = 1e-4; 
        int iterations = 5; // Делаем 5 замеров для усреднения результатов

        Console.WriteLine($"Выбранный размер шага: {optimalStep}");

        // 1. Замер чистого однопоточного варианта
        long singleThreadTotalTime = 0;
        for (int i = 0; i < iterations; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            SolveSingleThread(a, b, sinFunc, optimalStep);
            sw.Stop();
            singleThreadTotalTime += sw.ElapsedMilliseconds;
        }
        double avgSingleThreadTime = (double)singleThreadTotalTime / iterations;
        Console.WriteLine($"Однопоточная версия (в среднем): {avgSingleThreadTime} мс");

        // 2. Замеры многопоточного варианта для разного количества потоков (от 1 до 16)
        int[] threadCounts = { 1, 2, 4, 8, 12, 16 };
        double[] avgMultiThreadTimes = new double[threadCounts.Length];
        
        int optimalThreads = 1;
        double minMultiThreadTime = double.MaxValue;

        for (int t = 0; t < threadCounts.Length; t++)
        {
            int threadsNumber = threadCounts[t];
            long multiThreadTotalTime = 0;

            for (int i = 0; i < iterations; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                DefiniteIntegral.Solve(a, b, sinFunc, optimalStep, threadsNumber);
                sw.Stop();
                multiThreadTotalTime += sw.ElapsedMilliseconds;
            }

            avgMultiThreadTimes[t] = (double)multiThreadTotalTime / iterations;
            Console.WriteLine($"Потоков: {threadsNumber} -> Время (в среднем): {avgMultiThreadTimes[t]} мс");

            if (avgMultiThreadTimes[t] < minMultiThreadTime)
            {
                minMultiThreadTime = avgMultiThreadTimes[t];
                optimalThreads = threadsNumber;
            }
        }

        // Вычисляем разницу в процентах
        double percentDifference = ((avgSingleThreadTime - minMultiThreadTime) / avgSingleThreadTime) * 100.0;

        Console.WriteLine("\n=== Оптимальные результаты ===");
        Console.WriteLine($"Оптимальное количество потоков: {optimalThreads}");
        Console.WriteLine($"Лучшее время многопоточной версии: {minMultiThreadTime} мс");
        Console.WriteLine($"Разница производительности: {percentDifference:F2}%");

        // Шаг 6. Запись результатов в текстовый файл по требованию методички
        string reportPath = "performance_report.txt";
        using (StreamWriter writer = new StreamWriter(reportPath))
        {
            writer.WriteLine("=== ОТЧЕТ ПО ИССЛЕДОВАНИЮ ПРОИЗВОДИТЕЛЬНОСТИ ===");
            writer.WriteLine($"1. Выбранный размер шага интегрирования: {optimalStep} (обеспечивает точность 1e-4 для sin(x))");
            writer.WriteLine($"2. Оптимальное количество потоков: {optimalThreads}");
            writer.WriteLine($"3. Среднее время работы однопоточной версии: {avgSingleThreadTime} мс");
            writer.WriteLine($"4. Среднее время работы лучшей многопоточной версии: {minMultiThreadTime} мс");
            writer.WriteLine($"5. Разница в производительности: {percentDifference:F2}%");
            writer.WriteLine(percentDifference >= 15.0 
                ? "Критерий приёмки выполнен: многопоточная версия быстрее однопоточной более чем на 15%." 
                : "Внимание: разница производительности менее 15%, требуется аппаратная оптимизация количества тасок.");
        }
        Console.WriteLine($"Отчет сохранен в файл: {Path.GetFullPath(reportPath)}");
    }
}
