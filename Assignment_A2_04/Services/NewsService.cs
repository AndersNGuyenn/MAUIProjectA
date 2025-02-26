using System.Net;
using Newtonsoft.Json;

using Assignment_A2_04.Models;
using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace Assignment_A2_04.Services;
public class NewsService
{
    readonly string _subscriptionKey = "256970bad92b4d5398613d17fcba4a7f";
    readonly string _endpoint = "https://api.bing.microsoft.com/v7.0/news";
    readonly HttpClient _httpClient = new HttpClient();
    static readonly object _locker = new object();
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
        NewsCacheKey cacheKey = new NewsCacheKey(category, DateTime.Now);

        if (cacheKey.CacheExist)
        {
            var deserializedCacheKey = Deserialize(cacheKey.FileName);
            NewsServiceArticleAvailable($"Event message from news service: XML cached news in category is available: {category}");
            return deserializedCacheKey;
        }
        else
        {
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

            Serialize(newsResponse, cacheKey.FileName);

            NewsServiceArticleAvailable($"Event message from weather service: Cached news in category is available: {category}");
            return newsResponse;

        }

        
    }
    static string fname(string name)
    {
        var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        documentPath = Path.Combine(documentPath, "ADOP", "ProjectB");
        if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
        return Path.Combine(documentPath, name);
    }

    public static void Serialize(NewsResponse news, string fname)
    {
        lock (_locker)
        {
            var xs = new XmlSerializer(typeof(NewsResponse));
            using (Stream s = File.Create(fname))
                xs.Serialize(s, news);

        }

    }

    public static NewsResponse Deserialize(string fname)
    {
        lock (_locker)
        {
            NewsResponse news;

            var xs = new XmlSerializer(typeof(NewsResponse));
            using (Stream s = File.OpenRead(fname))
                news = (NewsResponse)xs.Deserialize(s);

            return news;
        }
    }

}