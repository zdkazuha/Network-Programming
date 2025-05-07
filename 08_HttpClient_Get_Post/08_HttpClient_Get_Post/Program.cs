using System.Reflection;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;


public class Post
{
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
}
internal class Program
{
    private static async Task Main(string[] args)
    {
        // Get 

        //var url = "https://jsonplaceholder.typicode.com/users";
        var url = "https://jsonplaceholder.typicode.com/posts";
        
        //using var client = new HttpClient();

        //var response = await client.GetAsync(url);
        //Console.WriteLine($"Status : {response.StatusCode} \n{response.RequestMessage}");

        //var result = await response.Content.ReadAsStringAsync();
        //Console.WriteLine(result);

        // Post

        var post = new Post()
        {
            Title = "TestTitle",
            Body = "TestBody",
            UserId = 101
        };
        var json = JsonSerializer.Serialize(post);
        var data = new StringContent(json,Encoding.UTF8,"applicatoin/json");

        using var client = new HttpClient();
        var response = await client.PostAsync(url,data);

        Console.WriteLine($"Status : {response.StatusCode}");
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);

    }
}