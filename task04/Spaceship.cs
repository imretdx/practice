using System;

namespace task04;

public interface ISpaceship
{
    void MoveForward();      // Движение вперед
    void Rotate(int angle);  // Поворот на угол (градусы)
    void Fire();             // Выстрел ракетой
    int Speed { get; }       // Скорость корабля
    int FirePower { get; }   // Мощность выстрела
}

public class Cruiser : ISpaceship
{
    public int Speed => 50;         // Крейсер: медленный
    public int FirePower => 100;    // Крейсер: мощные ракеты

    public void MoveForward() => Console.WriteLine("Крейсер медленно движется вперед.");
    public void Rotate(int angle) => Console.WriteLine($"Крейсер поворачивает на {angle} градусов.");
    public void Fire() => Console.WriteLine("Крейсер производит мощный залп фотонными ракетами!");
}

public class Fighter : ISpaceship
{
    public int Speed => 100;        // Истребитель: быстрый
    public int FirePower => 20;     // Истребитель: слабые ракеты

    public void MoveForward() => Console.WriteLine("Истребитель стремительно летит вперед.");
    public void Rotate(int angle) => Console.WriteLine($"Истребитель мгновенно разворачивается на {angle} градусов.");
    public void Fire() => Console.WriteLine("Истребитель выпускает легкую ракету.");
}
