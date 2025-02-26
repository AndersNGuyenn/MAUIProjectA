using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01;

class Program
{
    static async Task Main(string[] args)
    {
        double latitude = 60.6745;
        double longitude = 17.1417;

        Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

        //Your Code to present each forecast item in a grouped list
        Console.WriteLine($"Weather forecast for {forecast.City}");

        var DateGroupedFC = forecast.Items.GroupBy(d => d.DateTime.ToString("d")).ToList();
        foreach ( var date in DateGroupedFC )
        {
            
            Console.WriteLine($"{date.Key}");
            

            var TimeGroupedFC = date.GroupBy(t => t.DateTime.ToString("HH:mm")).ToList();
            foreach ( var time in TimeGroupedFC)
            {
                Console.WriteLine($"     [{time.Key}]");

                foreach (var info in time)
                {
                    Console.WriteLine($"     Info: {info.Description}, Temp: {info.Temperature} DegC, Wind: {info.WindSpeed} m/s ");
                    Console.WriteLine();
                }

            }
            

        }


    }
}
