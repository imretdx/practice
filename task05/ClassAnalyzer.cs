using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task05;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type ?? throw new ArgumentNullException(nameof(type));
    }

    // Список публичных методов (исключая методы-геттеры/сеттеры свойств для чистоты)
    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !m.IsSpecialName) // Специальные имена имеют геттеры и сеттеры свойств
            .Select(m => m.Name);
    }

    // Список параметров, имен параметров и возвращаемого значения публичного метода
    public IEnumerable<string> GetMethodParams(string methodname)
    {
        var method = _type.GetMethod(methodname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if (method == null) return Enumerable.Empty<string>();

        // Формируем список: сначала тип возвращаемого значения, затем параметры в формате "Тип Имя"
        var returnType = $"Return: {method.ReturnType.Name}";
        var parameters = method.GetParameters()
            .Select(p => $"{p.ParameterType.Name} {p.Name}");

        return new[] { returnType }.Concat(parameters);
    }

    // Список имен полей (включая приватные и публичные)
    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Select(f => f.Name);
    }

    // Список имен свойств
    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Select(p => p.Name);
    }

    // Наличие атрибута указанного типа у класса
    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.GetCustomAttribute<T>() != null;
    }
}
