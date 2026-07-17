using System;
using System.Reflection;
using Xunit;
using task09;

namespace task09tests;

public class AssemblyAnalyzerTests
{
    [Fact]
    public void AnalyzeAssembly_CurrentAssembly_ReturnsCorrectMetadata()
    {
        var analyzer = new AssemblyAnalyzer();
        // Передаем путь до текущей выполняемой сборки
        string currentAssemblyPath = Assembly.GetExecutingAssembly().Location;

        string result = analyzer.AnalyzeAssembly(currentAssemblyPath);

        // Проверяем, что в выводе рефлексии зафиксированы наши классы, методы и параметры
        Assert.Contains("Класс: task09tests.AssemblyAnalyzerTests", result);
        Assert.Contains("Метод: Void AnalyzeAssembly_CurrentAssembly_ReturnsCorrectMetadata", result);
    }

    [Fact]
    public void AnalyzeAssembly_NonExistentFile_ReturnsErrorMessage()
    {
        var analyzer = new AssemblyAnalyzer();
        string result = analyzer.AnalyzeAssembly("missing_library.dll");

        Assert.Contains("Ошибка: Указанный файл библиотеки не найден.", result);
    }
}
