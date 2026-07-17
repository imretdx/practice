using System;

namespace task06;

// Делегат для события изменения цены
public delegate void PriceChangedHandler(string stockName, decimal newPrice);

public class StockMarket
{
    private decimal _price;
    public string StockName { get; }

    // Событие, на которое будут подписываться наблюдатели
    public event PriceChangedHandler? OnPriceChanged;

    public StockMarket(string stockName, decimal initialPrice)
    {
        StockName = stockName;
        _price = initialPrice;
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                // Вызываем событие и уведомляем всех подписчиков
                OnPriceChanged?.Invoke(StockName, _price);
            }
        }
    }
}

public class Investor
{
    public string Name { get; }
    public string LastNotification { get; private set; } = string.Empty;

    public Investor(string name) => Name = name;

    // Метод-обработчик события (колбэк)
    public void Update(string stockName, decimal newPrice)
    {
        LastNotification = $"Инвестор {Name} уведомлен: Акция {stockName} теперь стоит {newPrice}";
    }
}
