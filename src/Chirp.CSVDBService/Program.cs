using CsvHelper;
using System.Globalization;
using SimpleDB;
using Azure.Storage.Blobs;

// Initialize the BlobServiceClient
BlobServiceClient blobServiceClient = new BlobServiceClient("YourConnectionStringHere");

// Get a reference to the container where your CSV files are stored
BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerClient.Name);

// Get a reference to the CSV file within the container
BlobClient blobClient = containerClient.GetBlobClient("../../src/SimpleDB/chirp_cli_db.csv");

// Download the CSV file to a stream
BlobDownloadInfo blobDownloadInfo = blobClient.OpenRead();
Stream stream = blobDownloadInfo.Content;

// Create a CsvReader and read records from the stream
using var reader = new StreamReader(stream);
using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
var records = csv.GetRecords<Cheep>();


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string dbPath = "../../src/SimpleDB/chirp_cli_db.csv";
DB<Cheep> x = DB<Cheep>.Instance(dbPath);

app.MapGet("/cheeps", () =>
{   
    Console.WriteLine("musssi");
    var cheeps = x.Read();
    Console.WriteLine("the csv file has been given to client");
    return Results.Ok(records);
});

app.MapPost("/cheep", (Cheep cheep) =>
{
        Console.WriteLine("musssi");
    x.Store(cheep);
    Console.WriteLine("a cheep has been stored");
    return Results.Created($"/cheep", cheep);
});




app.Run();


public record Cheep(string Author, string Message, long Timestamp);