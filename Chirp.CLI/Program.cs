// See https://aka.ms/new-console-template for more information
// used for time converter: https://www.educba.com/timestamp-to-date-c-sharp/


using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;


if (args[0].Equals("read"))
    show();
else if (args[0].Equals("cheep"))
    postCheep(args[1]);
else
    Console.WriteLine("n");


void showData()
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


void show()
{
    try
    {
        using (var reader = new StreamReader("chirp_cli_db.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<Cheep>();
            foreach (Cheep r in records)
            {
                Console.WriteLine(records);
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }

}



string timeConverter(double timeStamp)
{
    DateTime sd = new(1970, 1, 1, 2, 0, 0, 0);
    sd = sd.AddSeconds(timeStamp);
    string w = sd.ToString("MM/dd/yy HH:mm:ss");
    return w;
}

void postCheep(string cheep)
{
    string name = Environment.UserName;
    string time = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    string csv = string.Format("{0},{1},{2}\n", name, "\"" + cheep + "\"", time);
    File.AppendAllText("chirp_cli_db.csv", csv);
}

public record Cheep(string Author, string Message, long Timestamp);