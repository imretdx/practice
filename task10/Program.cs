using System;
using System.IO;

namespace task10;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== Запуск интеллектуальной системы плагинов ===");
        
        string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        Directory.CreateDirectory(pluginsDir);

        var engine = new PluginEngine();
        engine.LoadAndRunPlugins(pluginsDir);
    }
}
