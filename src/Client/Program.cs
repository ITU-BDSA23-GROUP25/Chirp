using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

// Create an HTTP client object
var baseURL = "https://bdsagroup25chirpremotedb.azurewebsites.net/";
using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
client.BaseAddress = new Uri(baseURL);

// Sequential execution
var watch = System.Diagnostics.Stopwatch.StartNew();
// first HTTP request
var response = await client.GetAsync("/cheeps");
// second HTTP request
response = await client.GetAsync("/cheep");

//var cheep = await client.GetFromJsonAsync<Cheep>("cheeps");

watch.Stop();

Console.WriteLine($"Sequential HTTP requests ... done after {watch.ElapsedMilliseconds}ms");

// Concurrent execution
watch = System.Diagnostics.Stopwatch.StartNew();
// first HTTP request
var fstRequestTask = client.GetAsync("/cheep");
// second HTTP request
var sndRequestTask = client.GetAsync("/cheeps");

var fstResponse = await fstRequestTask;
var sndResponse = await sndRequestTask;
watch.Stop();

Console.WriteLine($"Concurrent HTTP requests ... done after {watch.ElapsedMilliseconds}ms");


public record Cheep(string Author, string Message, long Timestamp);