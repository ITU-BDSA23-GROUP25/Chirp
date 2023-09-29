using Microsoft.Data.Sqlite;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    // These would normally be loaded from a database for example
    private readonly List<CheepViewModel> _cheeps = new();

    public List<CheepViewModel> GetCheeps()
    {
        var sqlDBFilePath = "db.db";
        var sqlQuery = @"SELECT u.username, m.text, m.pub_date
                         FROM message m
                         JOIN user u ON u.user_id = m.author_id
                         ORDER by m.pub_date asc";
        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
    
            while (reader.Read())
            {
                var author = reader.GetString(0);
                var message = reader.GetString(1);
                var time = reader.GetString(2);
                _cheeps.Add(new CheepViewModel(author, message, UnixTimeStampToDateTimeString(double.Parse(time))));
            }
        }
        return _cheeps;  
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        _cheeps.Clear();
        // filter by the provided author name

        var sqlDBFilePath = "db.db";
        var sqlQuery = @"SELECT u.username, m.text, m.pub_date
                         FROM message m
                         JOIN user u ON u.user_id = m.author_id
                         WHERE u.username = $author 
                         ORDER by m.pub_date asc";
        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddWithValue("$author", author);

            using var reader = command.ExecuteReader();
    
            while (reader.Read())
            {
                var _author = reader.GetString(0);
                var message = reader.GetString(1);
                var time = reader.GetString(2);
                _cheeps.Add(new CheepViewModel(_author, message, UnixTimeStampToDateTimeString(double.Parse(time))));
            }
        }
        return _cheeps; 

        //return _cheeps.Where(x => x.Author == author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
