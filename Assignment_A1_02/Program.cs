using Assignment_A1_02.Models;
using Assignment_A1_02.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Assignment_A1_02;

class Program
{
    static async Task Main(string[] args)
    {
        OpenWeatherService service = new OpenWeatherService();
        //Register the event
        //Your Code
        service.WeatherForecastAvailable += IsForecastAvailable;

        

        
        Task<Forecast>[] tasks = { null, null };
        Exception exception = null;
        try
        {

            double latitude = 60.6745;
            double longitude = 17.1417;

            //Create the two tasks and wait for comletion
            tasks[0] = service.GetForecastAsync(latitude, longitude);
            tasks[1] = service.GetForecastAsync("Ockelbo");

            Task.WaitAll(tasks[0], tasks[1]);

            Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

            

        }
        catch (Exception ex)
        {
            
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Error: {ex.Message}");
        }

        foreach (var task in tasks)
        {
            //How to deal with successful and fault tasks
            //Your Code
            if (task.IsCompletedSuccessfully)
            {
                ShowForecast(task.Result);
            }
            else if (task.IsFaulted)
            {
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine($"{task.Exception.Message}");
            }
            
        }
    }

    //Event handler declaration
    //Your Code

    public static void IsForecastAvailable(object sender, string message)
    {
        Console.WriteLine(message);
    }

    public static void ShowForecast(Forecast forecast)
    {
        //Your Code to present each forecast item in a grouped list
        Console.WriteLine("-------------------------------------------------------------------");
        Console.WriteLine($"Weather forecast for {forecast.City}");

        var DateGroupedFC = forecast.Items.GroupBy(d => d.DateTime.ToString("d")).ToList();
        foreach (var date in DateGroupedFC)
        {

            Console.WriteLine($"{date.Key}");


            var TimeGroupedFC = date.GroupBy(t => t.DateTime.ToString("HH:mm")).ToList();
            foreach (var time in TimeGroupedFC)
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
