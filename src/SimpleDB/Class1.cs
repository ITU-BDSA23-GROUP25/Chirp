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
        var records = csv.GetRecords<Cheep>().ToList<Cheep>();
        Console.WriteLine(records);
        return records;
    }

    public Cheep GetCheep(string messsage)
    {
        Cheep c = new();
        c.Author= getUsername();
        c.Timestamp = getUNIXTime();
        c.Message = messsage;
        return c;
    }

    public void Store(Cheep record)
    {
        string csv = string.Format("{0},{1},{2}\n", record.Author, "\"" + record.Message + "\"", record.Timestamp);
        File.AppendAllText("../SimpleDB/chirp_cli_db.csv", csv);
    }

    static string getUNIXTime(){
        return Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());;
    }

    static string getUsername(){
        return Environment.UserName;
    }
}
