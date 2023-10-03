using CsvHelper;
using System.Globalization;
using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string dbPath = "../../src/SimpleDB/chirp_cli_db.csv";
DB<Cheep> x = DB<Cheep>.Instance(dbPath);

app.MapGet("/cheeps", () => {
    //Path to csv from CLI: "../SimpleDB/chirp_cli_db.csv"
    /*using var reader = new StreamReader(dbPath);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    var records = csv.GetRecords<Cheep>().ToList<Cheep>();*/

    x.Read();

    //return records;
});

app.MapPost("/cheep", (Cheep cheep) => {
    /*string csv = string.Format("{0},{1},{2}\n", cheep.Author, "\"" + cheep.Message + "\"", cheep.Timestamp);
    File.AppendAllText(dbPath, csv);*/
    x.Store(cheep);
});




app.Run();


public record Cheep(string Author, string Message, long Timestamp);