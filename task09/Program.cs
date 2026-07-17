using System;

namespace task09;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Использование: task09 <путь_к_файлу_библиотеки.dll>");
            return;
        }

        var analyzer = new AssemblyAnalyzer();
        string output = analyzer.AnalyzeAssembly(args[0]);
        Console.WriteLine(output);
    }
}
