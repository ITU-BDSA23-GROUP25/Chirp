// See https://aka.ms/new-console-template for more information
// used for time converter: https://www.educba.com/timestamp-to-date-c-sharp/
// used for CSV formatting https://www.csharptutorial.net/csharp-file/csharp-read-csv-file/

/*
* CSV helper coded. co-authored-by co-author: Adam - aaab@itu.dk
* co-author: Karl - kagl@itu.dk
* co-author: Sebastian - segb@itu.dk
* co-author: Silas - siwo@itu.dk 
*/

using System.Text.RegularExpressions;
using CsvHelper;
using System.Globalization;
class Cheep
{
    public string? Author { get; set; }
    public string? Message { get; set; }
    public string? Timestamp { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        if (args[0].Equals("read"))
            Read();
        else if (args[0].Equals("cheep"))
            Store(args[1]);
        else
            Console.WriteLine("Insert dotnet run followed by either read or cheep☠️");
    }

    static void Read(int? limit = null)
    {
        using var reader = new StreamReader("../SimpleDB/chirp_cli_db.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // read CSV file
        var records = csv.GetRecords<Cheep>();

        // output
        foreach (var r in records)
        {
            Console.WriteLine($"{r.Author}" + " @ " + $"{timeConverter(Double.Parse(r.Timestamp!))}" + ": " + $"{r.Message}");
        }
    }
    

    static string timeConverter(double timeStamp)
    {
        DateTime sd = new(1970, 1, 1, 2, 0, 0, 0);
        sd = sd.AddSeconds(timeStamp);
        string w = sd.ToString("MM/dd/yy HH:mm:ss");
        return w;
    }

    static string getUNIXTime(){
        return Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());;
    }

    static string getUsername(){
        return Environment.UserName;
    }

    static void Store(string record)
    {
        string name = getUsername();
        string time = getUNIXTime();
        string csv = string.Format("{0},{1},{2}\n", name, "\"" + record + "\"", time);
        File.AppendAllText("../SimpleDB/chirp_cli_db.csv", csv);
    }
}