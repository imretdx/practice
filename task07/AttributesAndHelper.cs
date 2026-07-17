using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task07;

// Шаг 4. Создание пользовательских атрибутов
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public DisplayNameAttribute(string displayName) => DisplayName = displayName;
}

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }
    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}

// Шаг 5. Создание класса с атрибутами
[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod() { }
}

// Шаг 6. Напишите статический класс ReflectionHelper
public static class ReflectionHelper
{
    public static string PrintTypeInfo(Type type)
    {
        if (type == null) return string.Empty;

        var classDisplay = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? "Нет имени";
        var classVersion = type.GetCustomAttribute<VersionAttribute>();
        var versionStr = classVersion != null ? $"{classVersion.Major}.{classVersion.Minor}" : "0.0";

        var propsInfo = type.GetProperties()
            .Select(p => p.GetCustomAttribute<DisplayNameAttribute>())
            .Where(a => a != null)
            .Select(a => $"Свойство: {a!.DisplayName}");

        var methodsInfo = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .Select(m => m.GetCustomAttribute<DisplayNameAttribute>())
            .Where(a => a != null)
            .Select(a => $"Метод: {a!.DisplayName}");

        var allMembers = propsInfo.Concat(methodsInfo);

        return $"Класс: {classDisplay}, Версия: {versionStr}\n" + string.Join("\n", allMembers);
    }
}
