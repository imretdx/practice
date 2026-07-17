using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using task10;

namespace task10tests;

// Тестовые классы плагинов для проверки графа зависимостей
[PluginLoad("PluginA")]
public class PluginA : IPluginCommand
{
    public void Execute() => Console.WriteLine("A");
}

[PluginLoad("PluginB", "PluginA")] // B зависит от A
public class PluginB : IPluginCommand
{
    public void Execute() => Console.WriteLine("B");
}

[PluginLoad("PluginC", "PluginB")] // C зависит от B
public class PluginC : IPluginCommand
{
    public void Execute() => Console.WriteLine("C");
}

public class PluginSystemTests
{
    [Fact]
    public void SortPlugins_ShouldOrderDependenciesCorrectly()
    {
        var engine = new PluginEngine();
        // Подаем список типов в хаотичном порядке
        var unsortedTypes = new List<Type> { typeof(PluginC), typeof(PluginB), typeof(PluginA) };

        var sorted = engine.SortPlugins(unsortedTypes).ToList();

        // Проверяем, что индексы соответствуют правильному порядку: сначала A, потом B, затем C
        int indexA = sorted.IndexOf(typeof(PluginA));
        int indexB = sorted.IndexOf(typeof(PluginB));
        int indexC = sorted.IndexOf(typeof(PluginC));

        Assert.True(indexA < indexB, "PluginA должен идти перед PluginB");
        Assert.True(indexB < indexC, "PluginB должен идти перед PluginC");
    }
}
