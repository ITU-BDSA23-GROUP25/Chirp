using System.Globalization;
using CsvHelper;

namespace SimpleDB;


public class Class1 : IDatabaseRepository<Cheep>
{
    public IEnumerable<Cheep> Read(int? limit = null)
    {
        using var reader = new StreamReader("../SimpleDB/chirp_cli_db.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // read CSV file
        var records = csv.GetRecords<Cheep>();

        
        return records;
    }

    public void displayCheeps(IEnumerable<Cheep> records)
    {
        // output
        foreach (var r in records)
        {
            Console.WriteLine($"{r.Author}" + " @ " + $"{timeConverter(Double.Parse(r.Timestamp!))}" + ": " + $"{r.Message}");
        }
    }

     public void Store(Cheep record)
    {
        string name = getUsername();
        string time = getUNIXTime();
        string csv = string.Format("{0},{1},{2}\n", name, "\"" + record + "\"", time);
        File.AppendAllText("../SimpleDB/chirp_cli_db.csv", csv);
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
}
