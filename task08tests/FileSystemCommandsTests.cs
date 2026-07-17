using System;
using System.IO;
using Xunit;
using task08;

namespace task08tests;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDirSize_" + Guid.NewGuid());
        Directory.CreateDirectory(testDir);
        
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello"); // 5 байт
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World!"); // 6 байт

        var command = new DirectorySizeCommand(testDir);
        command.Execute();

        // Проверяем корректность вычислений (5 + 6 = 11 байт)
        Assert.Equal(11, command.CalculatedSize);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDirFind_" + Guid.NewGuid());
        Directory.CreateDirectory(testDir);
        
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();

        // Должен найти ровно 1 файл
        Assert.Single(command.FoundFiles);
        Assert.Equal("file1.txt", Path.GetFileName(command.FoundFiles[0]));

        Directory.Delete(testDir, true);
    }
}
