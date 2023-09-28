namespace SimpleDB;
public class Cheep
{
    public string? Author { get; set; }
    public string? Message { get; set; }
    public string? Timestamp { get; set; }

    override
    public string ToString(){
        return string.Format("{0},{1},{2}\n", Author, "\"" + Message + "\"", Timestamp);;
    }
}