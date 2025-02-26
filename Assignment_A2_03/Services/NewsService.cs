﻿using System.Net;
using Newtonsoft.Json;

using Assignment_A2_03.Models;
using System.Collections.Concurrent;

namespace Assignment_A2_03.Services;

public class NewsService
{
    readonly string _subscriptionKey = "256970bad92b4d5398613d17fcba4a7f";
    readonly string _endpoint = "https://api.bing.microsoft.com/v7.0/news";
    readonly HttpClient _httpClient = new HttpClient();

    readonly ConcurrentDictionary<(NewsCategory category, string), NewsResponse> _cachedNews = new ConcurrentDictionary<(NewsCategory category, string), NewsResponse>();
   

    public event EventHandler<string> NewsServiceArticleAvailableEH;
    protected virtual void NewsServiceArticleAvailable(string message)
    {
        NewsServiceArticleAvailableEH?.Invoke(this, message);
    }
    public NewsService()
    {
        _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
    }

    public async Task<NewsResponse> GetNewsAsync(NewsCategory category)
    {

        string timeWithoutSeconds = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        var key = (category, timeWithoutSeconds);

        if (_cachedNews.TryGetValue((key), out var cachedForecast))
        {
            NewsServiceArticleAvailable($"Event message from news service: News in category is available: {category}");
            return cachedForecast;
        }

        //To ensure not too many requests per second for BingNewsApi free plan
        await Task.Delay(2000);

        // make the http request and ensure success
        string uri = $"{_endpoint}?mkt=en-us&category={Uri.EscapeDataString(category.ToString())}";
        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        //Convert Json to NewsResponse
        string content = await response.Content.ReadAsStringAsync();
        var newsResponse = JsonConvert.DeserializeObject<NewsResponse>(content);
        newsResponse.Category = category;

        _cachedNews[(key)] = newsResponse;
        NewsServiceArticleAvailable($"Event message from weather service: Cached news in category is available: {category}");
        return newsResponse;
    }
}

