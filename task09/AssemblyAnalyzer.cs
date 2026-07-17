using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace task09;

public class AssemblyAnalyzer
{
    // Метод возвращает полную строку с метаданными сборки
    public string AnalyzeAssembly(string assemblyPath)
    {
        if (string.IsNullOrWhiteSpace(assemblyPath) || !File.Exists(assemblyPath))
            return "Ошибка: Указанный файл библиотеки не найден.";

        try
        {
            // Динамически загружаем внешнюю DLL
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            
            var classesInfo = assembly.GetTypes()
                .Where(t => t.IsClass)
                .Select(GetClassMetadata);

            return $"=== Метаданные сборки: {assembly.GetName().Name} ===\n" + string.Join("\n", classesInfo);
        }
        catch (Exception ex)
        {
            return $"Ошибка при анализе сборки: {ex.Message}";
        }
    }

    // Собираем метаданные конкретного класса
    private string GetClassMetadata(Type type)
    {
        var attributes = type.GetCustomAttributes(false)
            .Select(a => $"  [Атрибут: {a.GetType().Name}]");

        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Select(c => $"  Конструктор: {type.Name}({GetParametersString(c)})");

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName) // исключаем авто-методы свойств
            .Select(m => $"  Метод: {m.ReturnType.Name} {m.Name}({GetParametersString(m)})");

        var allMetadata = attributes
            .Concat(constructors)
            .Concat(methods);

        return $"Класс: {type.FullName}\n" + (allMetadata.Any() ? string.Join("\n", allMetadata) : "  (Нет членов класса)") + "\n";
    }

    // Вспомогательный метод для получения параметров метода или конструктора через LINQ
    private string GetParametersString(MethodBase method)
    {
        return string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
    }
}
