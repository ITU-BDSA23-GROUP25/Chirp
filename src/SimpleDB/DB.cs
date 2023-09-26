using System.Globalization;
using CsvHelper;

namespace SimpleDB;


public sealed class DB : IDatabaseRepository<Cheep>
{

    private static DB instance = null!;

    public static DB Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DB();
            }
            return instance;
        }
    }

    public IEnumerable<Cheep> Read(string dir, int? limit = null)
    {
        //Path to csv from CLI: "../SimpleDB/chirp_cli_db.csv"
        using var reader = new StreamReader(dir);
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

    public void Store(string dir, Cheep record)
    {
        string csv = string.Format("{0},{1},{2}\n", record.Author, "\"" + record.Message + "\"", record.Timestamp);
        File.AppendAllText(dir, csv);
    }

    public static string getUNIXTime(){
        return Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());;
    }

    public static string getUsername(){
        return Environment.UserName;
    }
}
