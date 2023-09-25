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
using static SimpleDB.Class1;
using SimpleDB;
using UI;
using CommandLine;

class Program
{
    public class Options
{
    [Value(index: 0, Required = true, HelpText = "Use <read> or <cheep>")]
    public string Command {get; set;}

    [Value(index: 1, Required = false, HelpText = "Write your Cheep!")]
    public string Cheep {get; set;}
}
    static void Main(string[] args)
    {
        Class1 x = Class1.Instance;
        UserInterface ui = new();
        Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (o.Command == "read")
                       {
                           ui.PrintCheeps(x.Read());
                       }
                       else if (o.Command == "cheep")
                       {
                           x.Store(x.GetCheep(o.Cheep));
                       }
                   });
    }
}