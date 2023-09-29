using CsvHelper;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var DB = "../../src/SimpleDB/chirp_cli_db.csv";

app.MapGet("/cheeps", () => {
    //Path to csv from CLI: "../SimpleDB/chirp_cli_db.csv"
    using var reader = new StreamReader(DB);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    var records = csv.GetRecords<Cheep>().ToList<Cheep>();

    return records;
});

app.MapPost("/cheep", (Cheep cheep) => {
    string csv = string.Format("{0},{1},{2}\n", cheep.Author, "\"" + cheep.Message + "\"", cheep.Timestamp);
    File.AppendAllText(DB, csv);
});




app.Run();


public record Cheep(string Author, string Message, long Timestamp);