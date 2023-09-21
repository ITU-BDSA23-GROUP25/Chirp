using SimpleDB;

namespace UI;

public class UserInterface
{


    public void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        // output
        foreach (var r in cheeps.ToList())
        {
            Console.WriteLine(convert_toString(r));
        }
    }

    public String convert_toString(Cheep r)
    {

        UserInterface _UI = new UserInterface();
        String convert = $"{r.Author}" + " @ " + $"{_UI.timeConverter(Double.Parse(r.Timestamp!))}" + ": " + $"{r.Message}";

        return convert;
    }

    public string timeConverter(double timeStamp)
    {
        DateTime sd = new(1970, 1, 1, 2, 0, 0, 0);
        sd = sd.AddSeconds(timeStamp);
        string w = sd.ToString("MM/dd/yy HH:mm:ss");
        return w;
    }
}