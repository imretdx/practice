using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace task08;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== Запуск динамического выполнения команд ===");

        // Шаг 5. Динамическая загрузка сборки (рефлексия во время выполнения)
        string assemblyPath = Assembly.GetExecutingAssembly().Location;
        Assembly asm = Assembly.LoadFrom(assemblyPath);

        string testDir = Path.Combine(Path.GetTempPath(), "DynamicTestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "LogData");

        // Динамически ищем типы, реализующие ICommand, через LINQ (без циклов)
        var commandTypes = asm.GetTypes()
            .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in commandTypes)
        {
            ICommand? cmd = null;

            // Динамически определяем параметры и создаем экземпляры через Активатор
            if (type == typeof(DirectorySizeCommand))
            {
                cmd = Activator.CreateInstance(type, new object[] { testDir }) as ICommand;
            }
            else if (type == typeof(FindFilesCommand))
            {
                cmd = Activator.CreateInstance(type, new object[] { testDir, "*.txt" }) as ICommand;
            }

            // Выполняем динамически загруженную команду
            cmd?.Execute();
        }

        Directory.Delete(testDir, true);
    }
}
