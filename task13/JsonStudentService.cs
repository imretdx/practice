using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class Subject
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
}

public class Student
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // Кастомный формат даты настраивается через JsonSerializerOptions, а здесь защитим от null
    public DateTime BirthDate { get; set; }
    
    // Игнорируем свойство при сериализации, если оно null (по требованию задания)
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Subject>? Grades { get; set; }
}

public class JsonStudentService
{
    private readonly JsonSerializerOptions _options;

    public JsonStudentService()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true, // Красивое форматирование с отступами
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Глобальное игнорирование null
        };
    }

    // Сериализация объекта в строку JSON
    public string SerializeStudent(Student student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));
        return JsonSerializer.Serialize(student, _options);
    }

    // Десериализация строки JSON обратно в объект с валидацией данных
    public Student DeserializeStudent(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON строка пуста");
        
        var student = JsonSerializer.Deserialize<Student>(json, _options);
        
        // Валидация данных при десериализации (по требованию задания)
        if (student == null) throw new InvalidOperationException("Не удалось десериализовать студента.");
        if (string.IsNullOrWhiteSpace(student.FirstName) || string.IsNullOrWhiteSpace(student.LastName))
            throw new ValidationException("Имя или фамилия студента не могут быть пустыми.");
            
        return student;
    }

    // Сохранение JSON в файл
    public void SaveToFile(string filePath, Student student)
    {
        string json = SerializeStudent(student);
        File.WriteAllText(filePath, json);
    }

    // Загрузка JSON из файла
    public Student LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Файл не найден", filePath);
        string json = File.ReadAllText(filePath);
        return DeserializeStudent(json);
    }
}

// Кастомное исключение для валидации десериализации
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
