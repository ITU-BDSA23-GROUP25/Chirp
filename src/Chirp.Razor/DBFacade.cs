using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.Sqlite;

public class DBFacade {

    List<String> messages = new();
public void Connect(){
    var sqlDBFilePath = "db.db";
    var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";
    using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
{
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = sqlQuery;

    using var reader = command.ExecuteReader();
    int i = 0;
    
    while (reader.Read())
    {
        messages.Add(reader.GetString(0));
    }
    Console.WriteLine(messages.Count);
}   

    

}
public List<String> GetMessages(){
        return messages;
    }
}