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

class Program
{
    static void Main(string[] args)
    {
        Class1 x = new();
        if (args[0].Equals("read"))
            x.displayCheeps(x.Read());
        else if (args[0].Equals("cheep"));
            //x.Store(args[1]);
            
        else
            Console.WriteLine("Insert dotnet run followed by either read or cheep☠️");
    }

   
    



   
}