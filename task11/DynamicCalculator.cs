using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11;

// Интерфейс, который позволит вызывать методы динамического класса БЕЗ рефлексии
public interface ICalculator
{
    int Add(int a, int b);
    int Minus(int a, int b);
    int Mul(int a, int b);
    int Div(int a, int b);
}

public static class DynamicCalculatorFactory
{
    public static ICalculator CreateFromString(string classCode)
    {
        // Парсим строку кода в синтаксическое дерево
        var syntaxTree = CSharpSyntaxTree.ParseText(classCode);

        // Настраиваем ссылки на системные сборки, необходимые для компиляции
        var references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location) // Ссылка на наш интерфейс
        };

        // Настраиваем параметры компиляции в динамическую библиотеку (DLL)
        var compilation = CSharpCompilation.Create(
            "DynamicCalculatorAssembly_" + Guid.NewGuid().ToString("N"),
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var failures = string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()));
            throw new InvalidOperationException($"Ошибка компиляции кода:\n{failures}");
        }

        // Загружаем скомпилированную сборку из памяти
        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        // Находим созданный тип Calculator и создаем его экземпляр
        var type = assembly.GetType("task11.Calculator") ?? throw new Exception("Класс task11.Calculator не найден в скомпилированной сборке.");
        
        // Возвращаем объект, приведенный к интерфейсу (теперь методы можно вызывать без рефлексии!)
        return (ICalculator)Activator.CreateInstance(type)!;
    }
}
