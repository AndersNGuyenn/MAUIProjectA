using System.Collections.Concurrent;
using Newtonsoft.Json;

using Assignment_A1_03.Models;
using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Assignment_A1_03.Services;

public class OpenWeatherService
{
    readonly HttpClient _httpClient = new HttpClient();
    readonly ConcurrentDictionary<(double, double, string), Forecast> _cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
    readonly ConcurrentDictionary<(string, string), Forecast> _cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

    // Your API Key
    readonly string apiKey = "17f32d1d8fabca4ddf6d9c2b7e09a350";

    //Event declaration
    public event EventHandler<string> WeatherForecastAvailable;
    protected virtual void OnWeatherForecastAvailable(string message)
    {
        WeatherForecastAvailable?.Invoke(this, message);
    }
    public async Task<Forecast> GetForecastAsync(string City)
    {
        //part of cache code here to check if forecast in Cache
        //generate an event that shows forecast was from cache
        //Your code
        string timeWithOutSeconds = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        var key = (City,timeWithOutSeconds);

        if (_cachedCityForecasts.TryGetValue((key), out var cachedForecast))
        {
            OnWeatherForecastAvailable($"Event message from weather service: Cached weather for {City} is available");
            return cachedForecast;
        }


        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";
        Forecast forecast = await ReadWebApiAsync(uri);

        //part of event and cache code here
        //generate an event with different message if cached data
        //Your code
        _cachedCityForecasts[(key)] = forecast;
        OnWeatherForecastAvailable($"Event message from weather service: Forecast for {City} is cached");
        return forecast;

    }
    public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
    {
        //part of cache code here to check if forecast in Cache
        //generate an event that shows forecast was from cache
        //Your code
        string timeWithOutSeconds = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        var key = (latitude, longitude, timeWithOutSeconds);

        if (_cachedGeoForecasts.TryGetValue((key), out var cachedGeoForecast))
        {
            OnWeatherForecastAvailable($"Event message from weather service: Cached weather for {latitude}, {longitude} is available");
            return cachedGeoForecast;
        }
        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

        Forecast forecast = await ReadWebApiAsync(uri);

        //part of event and cache code here
        //generate an event with different message if cached data
        //Your code
        _cachedGeoForecasts[(key)] = forecast;
        OnWeatherForecastAvailable($"Event message from weather service: Forecast for {latitude}, {longitude} is cached");
        return forecast;
    }
    private async Task<Forecast> ReadWebApiAsync(string uri)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        //Convert Json to NewsResponse
        string content = await response.Content.ReadAsStringAsync();
        WeatherApiData wd = JsonConvert.DeserializeObject<WeatherApiData>(content);

        //Convert WeatherApiData to Forecast using Linq.
        //Your code
        var forecast = new Forecast()
        {
            City = wd.city.name,
            Items = wd.list.Select(items => new ForecastItem()
            {

                Icon = $"http://openweathermap.org/img/w/{items.weather.First().icon}.png",
                DateTime = UnixTimeStampToDateTime(items.dt),
                Temperature = items.main.temp,
                WindSpeed = items.wind.speed,
                Description = items.weather[0].description,

            }).ToList()
        };

        return forecast;
    }
  

    private DateTime UnixTimeStampToDateTime(double unixTimeStamp) => DateTime.UnixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
}

