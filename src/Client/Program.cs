using CommandLine;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Client
{
    public class Options
    {
        [Value(index: 0, Required = true, HelpText = "Use <read> or <cheep>")]
        public string? Command { get; set; }

        [Value(index: 1, Required = false, HelpText = "Write your Cheep!")]
        public string? Cheep { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string baseURL = "https://bdsagroup25chirpremotedb.azurewebsites.net";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseURL);

            var result = await Parser.Default.ParseArguments<Options>(args).WithParsedAsync<Options>(async o =>
            {
                if (o.Command == "read")
                {
                    var response = await client.GetFromJsonAsync<Cheep[]>($"{baseURL}/cheeps");
                    
                    if (response != null)
                    {
                        Console.WriteLine("Cheep Data:");
                        foreach (var cheep in response)
                        {
                            Console.WriteLine($"Author: {cheep.Author}, Message: {cheep.Message}, Timestamp: {cheep.Timestamp}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Cheep data found.");
                    }
                }
                else if (o.Command == "cheep")
                {
                    if (!string.IsNullOrEmpty(o.Cheep))
                    {
                        // Create a Cheep object with the message
                        var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        var author = Environment.UserName;
                        var newCheep = new Cheep(author, o.Cheep, timeStamp);

                        // Serialize the Cheep object to JSON
                        var jsonContent = JsonSerializer.Serialize(newCheep);

                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        // Send a POST request with JSON content in a try/catch
                        try
                        {
                            var response = await client.PostAsync("/cheep", content);
                            Console.WriteLine(response.StatusCode);
                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Cheep added successfully.");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to add Cheep. Status code: {response.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An exception occurred: {ex}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cheep message is missing.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid command.");
                }
            });
        }

        public record Cheep(string Author, string Message, long Timestamp);
    }
}
