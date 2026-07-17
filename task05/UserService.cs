using System;

namespace task05;

// Кастомное исключение для ошибок валидации
public class UserValidationException : Exception
{
    public UserValidationException(string message) : base(message) { }
}

public class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class UserService
{
    public bool RegisterUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Объект пользователя не может быть null");

        if (string.IsNullOrWhiteSpace(user.Username))
            throw new UserValidationException("Имя пользователя не может быть пустым");

        if (user.Password.Length < 6)
            throw new UserValidationException("Пароль должен содержать минимум 6 символов");

        if (user.Age < 18)
            throw new UserValidationException("Регистрация доступна только лицам старше 18 лет");

        // Если все проверки пройдены успешно
        return true;
    }
}
