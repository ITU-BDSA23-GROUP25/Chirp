namespace Chirp.Razor;


public class Cheep
{
    public int CheepId { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
    public int AuthorId { get; set; }

    override public string ToString()
    {
        return string.Format("{0},{1},{2}\n", AuthorId, "\"" + Message + "\"", Timestamp); ;
    }
}