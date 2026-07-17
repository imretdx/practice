using System;
using System.IO;
using System.Linq;

namespace task08;

public class DirectorySizeCommand : ICommand
{
    private readonly string _dirPath;
    public long CalculatedSize { get; private set; }

    public DirectorySizeCommand(string dirPath) => _dirPath = dirPath;

    public void Execute()
    {
        if (!Directory.Exists(_dirPath))
        {
            Console.WriteLine($"Каталог {_dirPath} не существует.");
            return;
        }

        // Вычисляем размер через LINQ без циклов
        CalculatedSize = Directory.EnumerateFiles(_dirPath, "*", SearchOption.AllDirectories)
            .Select(f => new FileInfo(f).Length)
            .Sum();

        Console.WriteLine($"Размер каталога '{_dirPath}': {CalculatedSize} байт.");
    }
}

public class FindFilesCommand : ICommand
{
    private readonly string _dirPath;
    private readonly string _searchPattern;
    public string[] FoundFiles { get; private set; } = Array.Empty<string>();

    public FindFilesCommand(string dirPath, string searchPattern)
    {
        _dirPath = dirPath;
        _searchPattern = searchPattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(_dirPath))
        {
            Console.WriteLine($"Каталог {_dirPath} не существует.");
            return;
        }

        // Ищем файлы по маске
        FoundFiles = Directory.GetFiles(_dirPath, _searchPattern, SearchOption.TopDirectoryOnly);
        
        Console.WriteLine($"Найдено файлов по маске '{_searchPattern}': {FoundFiles.Length}");
        Array.ForEach(FoundFiles, f => Console.WriteLine($" - {Path.GetFileName(f)}"));
    }
}
