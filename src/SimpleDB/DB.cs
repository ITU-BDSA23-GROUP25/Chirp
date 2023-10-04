using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace SimpleDB
{
    public sealed class DB<T> : IDatabaseRepository<T>
    {
        private string dbPath;
        private static DB<T>? _instance;

        private DB(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public static DB<T> Instance(string dbPath)
        {
            if (_instance == null)
            {
                _instance = new DB<T>(dbPath);
            }
            return _instance;
        }

        public IEnumerable<T> Read(int? limit = null)
        {
            using var reader = new StreamReader(dbPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<T>().ToList<T>();
            return limit.HasValue ? records.Take(limit.Value) : records;
        }

        public void Store(T record)
        {
            using (var writer = new StreamWriter(dbPath, true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecord(record);
                writer.WriteLine(); // Add a newline character after writing the record
            }
        }

        public static string GetUNIXTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        /*public T GetCheep(string message)
        {
            var record = Activator.CreateInstance<T>();

            // Assuming T has properties like Author, Timestamp, and Message
            // Modify this logic as needed based on your actual record type.
            var authorProperty = typeof(T).GetProperty("Author");
            var timestampProperty = typeof(T).GetProperty("Timestamp");
            var messageProperty = typeof(T).GetProperty("Message");
            

            //we are using null propagation, instead of if null
            authorProperty?.SetValue(record, GetUsername());

            timestampProperty?.SetValue(record, GetUNIXTime());

            messageProperty?.SetValue(record, message);

            return record;
        }*/

        public static string GetUsername()
        {
            return Environment.UserName;
        }
    }
}
