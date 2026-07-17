using System.Globalization;
using Xunit;
using task06;

namespace task06tests;

public class StockMarketTests
{
    [Fact]
    public void PriceChange_NotifiesSubscribedInvestor()
    {
        var market = new StockMarket("AAPL", 150.00m);
        var investor = new Investor("Алексей");

        market.OnPriceChanged += investor.Update;

        market.Price = 155.50m;

        // Формируем ожидаемую строку с учетом локали текущего ПК (заменяем точку на системный разделитель)
        string expectedPrice = (155.50m).ToString(CultureInfo.CurrentCulture);
        string expectedMessage = $"Инвестор Алексей уведомлен: Акция AAPL теперь стоит {expectedPrice}";

        Assert.Equal(expectedMessage, investor.LastNotification);
    }

    [Fact]
    public void UnsubscribedInvestor_DoesNotReceiveNotifications()
    {
        var market = new StockMarket("MSFT", 300.00m);
        var investor = new Investor("Игорь");

        market.OnPriceChanged += investor.Update;
        market.OnPriceChanged -= investor.Update;

        market.Price = 310.00m;

        Assert.Empty(investor.LastNotification);
    }
}
