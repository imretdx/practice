using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace task10;

// Базовый интерфейс для команд плагинов
public interface IPluginCommand
{
    void Execute();
}

// Атрибут разметки плагинов с указанием их зависимостей
[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute
{
    public string PluginName { get; }
    public string[] Dependencies { get; }

    public PluginLoadAttribute(string pluginName, params string[] dependencies)
    {
        PluginName = pluginName;
        Dependencies = dependencies ?? Array.Empty<string>();
    }
}

// Движок плагинной системы
public class PluginEngine
{
    // Топологическая сортировка плагинов через рекурсивный обход графа (DFS) без циклов
    public IEnumerable<Type> SortPlugins(IEnumerable<Type> pluginTypes)
    {
        var visited = new HashSet<string>();
        var sorted = new List<Type>();
        
        // Создаем словарь для быстрого поиска типов по имени плагина
        var pluginDict = pluginTypes
            .Where(t => t.GetCustomAttribute<PluginLoadAttribute>() != null)
            .ToDictionary(t => t.GetCustomAttribute<PluginLoadAttribute>()!.PluginName, t => t);

        // Рекурсивная функция обхода соседа (DFS)
        void Visit(string pluginName)
        {
            if (visited.Contains(pluginName) || !pluginDict.ContainsKey(pluginName)) return;

            var type = pluginDict[pluginName];
            var attr = type.GetCustomAttribute<PluginLoadAttribute>()!;

            // Сначала обходим все зависимости (соседей по графу)
            Array.ForEach(attr.Dependencies, Visit);

            visited.Add(pluginName);
            sorted.Add(type);
        }

        // Запускаем обход для каждого плагина
        pluginDict.Keys.ToList().ForEach(Visit);

        return sorted;
    }

    // Обнаружение, загрузка и выполнение плагинов из папки
    public void LoadAndRunPlugins(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) return;

        // Ищем все .dll файлы в папке
        var assemblies = Directory.GetFiles(directoryPath, "*.dll")
            .Select(file => { try { return Assembly.LoadFrom(file); } catch { return null; } })
            .Where(asm => asm != null);

        // Извлекаем все подходящие типы плагинов
        var pluginTypes = assemblies
            .SelectMany(asm => asm!.GetTypes())
            .Where(t => typeof(IPluginCommand).IsAssignableFrom(t) && !t.IsInterface && t.GetCustomAttribute<PluginLoadAttribute>() != null);

        // Сортируем с учетом зависимостей и выполняем
        var sortedTypes = SortPlugins(pluginTypes);

        sortedTypes.ToList().ForEach(type =>
        {
            var instance = Activator.CreateInstance(type) as IPluginCommand;
            instance?.Execute();
        });
    }
}
