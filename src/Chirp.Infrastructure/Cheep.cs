namespace Repository;

public class Cheep
{
    public Guid CheepId { get; set; }
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public required Author Author { get; set; }
    public ICollection<Reaction>? Reactions { get; set; } = new List<Reaction>();
}