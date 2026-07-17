using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace task04;

public class DataDownloader
{
    // Асингулярный метод загрузки одной строки по "URL" с симуляцией задержки
    public async Task<string> DownloadDataAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL не может быть пустым", nameof(url));

        // Имитируем сетевую задержку в 100 миллисекунд
        await Task.Delay(100);
        
        return $"Данные из источника: {url}";
    }

    // Параллельная загрузка пачки URL-адресов
    public async Task<IEnumerable<string>> DownloadAllParallelAsync(IEnumerable<string> urls)
    {
        if (urls == null) return Enumerable.Empty<string>();

        // Создаем массив задач
        var tasks = urls.Select(url => DownloadDataAsync(url));
        
        // Ждем выполнения всех задач параллельно
        var results = await Task.WhenAll(tasks);
        
        return results;
    }
}
