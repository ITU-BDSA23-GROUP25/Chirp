using System.Globalization;
using CsvHelper;

namespace SimpleDB;


public sealed class Class1 : IDatabaseRepository<Cheep>
{

    private static Class1 instance = null!;

    public static Class1 Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Class1();
            }
            return instance;
        }
    }
    public IEnumerable<Cheep> Read(int? limit = null)
    {
        //Path to csv from CLI: "../SimpleDB/chirp_cli_db.csv"
        using var reader = new StreamReader("./test/Chirp.CSVDB.Tests/test_db.csv");
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
        File.AppendAllText("./test/Chirp.CSVDB.Tests/test_db.csv", csv);
    }

    public static string getUNIXTime(){
        return Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());;
    }

    public static string getUsername(){
        return Environment.UserName;
    }
}
