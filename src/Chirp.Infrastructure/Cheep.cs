namespace Repository;

public class Cheep
{
    public Guid CheepId { get; set; }
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public required Author Author { get; set; }
    public string AuthorId { get; set; }
}