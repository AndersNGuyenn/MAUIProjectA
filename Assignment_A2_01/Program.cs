using Assignment_A2_01.Models;
using Assignment_A2_01.Services;

namespace Assignment_A2_01;

class Program
{
    static async Task Main(string[] args)
    {

        NewsResponse news = await new NewsService().GetNewsAsync(NewsCategory.Sports);
        Console.WriteLine(news.Category);

        var sportsArticleKey = news.Articles.GroupBy(k => k.Title).ToList();
        foreach(var sport in sportsArticleKey)
        {
            Console.WriteLine(sport.Key);
        }
        
        
    }
}

