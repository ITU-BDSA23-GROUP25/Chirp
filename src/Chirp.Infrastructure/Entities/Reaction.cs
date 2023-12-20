namespace Repository;

public class Reaction
{

    public required Guid CheepId { get; set; }
    public required string AuthorName { get; set; }
    public ReactionType ReactionType { get; set; }
}