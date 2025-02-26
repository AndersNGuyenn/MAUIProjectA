using Assignment_A2_04.Models;
using Assignment_A2_04.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Xml.Serialization;

namespace Assignment_A2_04;

class Program
{
    
    static async Task Main(string[] args)
    {

        NewsService newsService = new NewsService();
        newsService.NewsServiceArticleAvailableEH += IsNewsArticleAvailable;

        Task<NewsResponse>[] tasks = { null, null, null, null, null, null, null, null, null, null };
        Exception exception = null;

        try
        {
            tasks[0] = newsService.GetNewsAsync(NewsCategory.World);
            tasks[1] = newsService.GetNewsAsync(NewsCategory.Technology);
            tasks[2] = newsService.GetNewsAsync(NewsCategory.Business);
            tasks[3] = newsService.GetNewsAsync(NewsCategory.Sports);
            tasks[4] = newsService.GetNewsAsync(NewsCategory.Entertainment);
            

            Task.WaitAll(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]);
            
            tasks[5] = newsService.GetNewsAsync(NewsCategory.World);
            tasks[6] = newsService.GetNewsAsync(NewsCategory.Technology);
            tasks[7] = newsService.GetNewsAsync(NewsCategory.Business);
            tasks[8] = newsService.GetNewsAsync(NewsCategory.Sports);
            tasks[9] = newsService.GetNewsAsync(NewsCategory.Entertainment);

            Task.WaitAll(tasks[5], tasks[6], tasks[7], tasks[8], tasks[9]);

        }
        catch (Exception ex)
        {
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Error: {ex.Message}");

        }

        foreach (var task in tasks)
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

        foreach (var info in news.Articles)
        {
            Console.WriteLine($" -{info.DatePublished}: {info.Title}");
        }
        Console.WriteLine();


    }

    
}

