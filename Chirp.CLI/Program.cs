// See https://aka.ms/new-console-template for more information
// used for time converter: https://www.educba.com/timestamp-to-date-c-sharp/
// used for CSV formatting https://www.csharptutorial.net/csharp-file/csharp-read-csv-file/
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
            //showData();
            readCheeps();
        else if (args[0].Equals("cheep"))
            postCheep(args[1]);
        else
            Console.WriteLine("n");
    }

    static void readCheeps()
    {
        using var reader = new StreamReader("chirp_cli_db.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // read CSV file
        var records = csv.GetRecords<Cheep>();

        // output
        foreach (var r in records)
        {
            Console.WriteLine($"{r.Author}" + " @ " + $"{timeConverter(Double.Parse(r.Timestamp))}" + ": " + $"{r.Message}");
        }
    }

//TODO Remove showData()
    static void showData()
    {
        string[] line;
        string regex = """([a-zA-Z0-9_-]+),"([a-zA-Z0-9_, -:.]+)",([0-9]+)""";
        try
        {
            StreamReader sr = new StreamReader("chirp_cli_db.csv");
            //line = sr.ReadLine().Split(",");
            string next = sr.ReadLine();
            next = sr.ReadLine();


            while (next != null)
            {
                Match m = Regex.Match(next, regex);
                if (m.Success)
                {
                    Console.WriteLine(m.Groups[1].Value + " @ " + timeConverter(Double.Parse(m.Groups[3].Value)) + ": " + $"\"{m.Groups[2].Value}\"");

                }
                else
                {

                    Console.WriteLine("fejl");
                }
                next = sr.ReadLine();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
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

    static void postCheep(string cheep)
    {
        string name = getUsername();
        string time = getUNIXTime();
        string csv = string.Format("{0},{1},{2}\n", name, "\"" + cheep + "\"", time);
        File.AppendAllText("chirp_cli_db.csv", csv);
    }
}