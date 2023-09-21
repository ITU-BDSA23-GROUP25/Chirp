using SimpleDB;

namespace UI;

class UserInterface{


    public void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        // output
        foreach (var r in cheeps.ToList())
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
}