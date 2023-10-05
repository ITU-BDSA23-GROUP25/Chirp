using CsvHelper;
using System.Globalization;
using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string dbPath = "../../src/SimpleDB/chirp_cli_db.csv";
DB<Cheep> x = DB<Cheep>.Instance(dbPath);

app.MapGet("/cheeps", () =>
{
    var cheeps = x.Read();
    Console.WriteLine("the csv file has been given to client");
    return Results.Ok(cheeps);
});

app.MapPost("/cheep", (Cheep cheep) =>
{
    x.Store(cheep);
    Console.WriteLine("a cheep has been stored");
    return Results.Created($"/cheep", cheep);
});




app.Run();


public record Cheep(string Author, string Message, long Timestamp);