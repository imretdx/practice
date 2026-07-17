using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using task13;

namespace task13tests;

public class JsonStudentTests : IDisposable
{
    private readonly JsonStudentService _service = new();
    private readonly string _tempFilePath;

    public JsonStudentTests()
    {
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"student_{Guid.NewGuid()}.json");
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath)) File.Delete(_tempFilePath);
    }

    [Fact]
    public void Serialize_WithNullGrades_ShouldIgnoreGradesProperty()
    {
        var student = new Student { FirstName = "Иван", LastName = "Петров", BirthDate = new DateTime(2005, 5, 12), Grades = null };

        string json = _service.SerializeStudent(student);

        Assert.DoesNotContain("Grades", json);
    }

    [Fact]
    public void Deserialize_EmptyName_ShouldThrowValidationException()
    {
        string invalidJson = "{\"FirstName\":\"\",\"LastName\":\"Попов\",\"BirthDate\":\"2004-01-01T00:00:00\"}";

        Assert.Throws<ValidationException>(() => _service.DeserializeStudent(invalidJson));
    }

    [Fact]
    public void FileIO_SaveAndLoad_ReturnsMatchingData()
    {
        var originalStudent = new Student
        {
            FirstName = "Олег",
            LastName = "Сидоров",
            BirthDate = new DateTime(2003, 10, 25),
            Grades = new List<Subject> { new() { Name = "Программирование", Grade = 5 } }
        };

        _service.SaveToFile(_tempFilePath, originalStudent);
        var loadedStudent = _service.LoadFromFile(_tempFilePath);

        Assert.Equal(originalStudent.FirstName, loadedStudent.FirstName);
        Assert.Equal(originalStudent.LastName, loadedStudent.LastName);
        Assert.Single(loadedStudent.Grades!);
        Assert.Equal("Программирование", loadedStudent.Grades![0].Name);
    }
}
