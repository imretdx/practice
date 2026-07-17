using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using task04;

namespace task04tests;

public class DataDownloaderTests
{
    private readonly DataDownloader _downloader = new();

    [Fact]
    public async Task DownloadDataAsync_ValidUrl_ReturnsCorrectString()
    {
        string url = "yandex.ru";
        string result = await _downloader.DownloadDataAsync(url);
        
        Assert.Equal("Данные из источника: yandex.ru", result);
    }

    [Fact]
    public async Task DownloadDataAsync_EmptyUrl_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _downloader.DownloadDataAsync(""));
    }

    [Fact]
    public async Task DownloadAllParallelAsync_MultipleUrls_ReturnsAllData()
    {
        var urls = new List<string> { "google.com", "github.com", "omgtu.ru" };
        
        var results = (await _downloader.DownloadAllParallelAsync(urls)).ToList();
        
        Assert.Equal(3, results.Count);
        Assert.Contains("Данные из источника: google.com", results);
        Assert.Contains("Данные из источника: github.com", results);
        Assert.Contains("Данные из источника: omgtu.ru", results);
    }
}
