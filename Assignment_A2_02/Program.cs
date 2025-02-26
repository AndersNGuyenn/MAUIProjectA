using Assignment_A2_02.Models;
using Assignment_A2_02.Services;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Assignment_A2_02;

class Program
{
    static async Task Main(string[] args)
    {
        NewsService newsService = new NewsService();
        newsService.NewsServiceArticleAvailableEH += IsNewsArticleAvailable;

        Task<NewsResponse>[] tasks = { null, null, null, null,null };
        Exception exception = null;

        try
        {
            tasks[0] = newsService.GetNewsAsync(NewsCategory.World);
            tasks[1] = newsService.GetNewsAsync(NewsCategory.Technology);
            tasks[2] = newsService.GetNewsAsync(NewsCategory.Business);
            tasks[3] = newsService.GetNewsAsync(NewsCategory.Sports);
            tasks[4] = newsService.GetNewsAsync(NewsCategory.Entertainment);

            Task.WaitAll(tasks);
        }
        catch(Exception ex)
        {
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Error: {ex.Message}");

        }

        foreach(var task in tasks)
        {
            if (task.IsCompletedSuccessfully)
            {
                Console.WriteLine("-------------------------------------------------------------------");
                ShowNews(task.Result);
            }
            else if (task.IsFaulted)
            {
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine($"{task.Exception.Message}");
            }
            
        }
        



    }

    public static void IsNewsArticleAvailable(object sender, string message)
    {
        Console.WriteLine(message);
    }

    public static void ShowNews(NewsResponse news)
    {
        
        Console.WriteLine($"News in category {news.Category}");

        foreach(var info in news.Articles)
        {
            Console.WriteLine($" -{info.DatePublished}: {info.Title}");
        }
        Console.WriteLine();
      

    }
}

