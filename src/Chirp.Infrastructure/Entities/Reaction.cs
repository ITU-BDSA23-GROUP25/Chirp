namespace Repository;

/// <summary>
/// The Reaction entity is used for users to be able to react with a reactionType 
/// to a given Cheep 
/// </summary>
public class Reaction
{
    public required Guid CheepId { get; set; }
    public required string AuthorName { get; set; }
    public ReactionType ReactionType { get; set; }
}