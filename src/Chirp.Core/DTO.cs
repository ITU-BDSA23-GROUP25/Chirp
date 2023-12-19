namespace Core;

public record CheepDTO(Guid Id, string Author, string Message, string Timestamp, ICollection<ReactionDTO> Reactions);
public record AuthorDTO
{
    public required string Name { get; init;}
}

public record ReactionDTO(ReactionType Reactiontype, int Count);

public enum ReactionType
{
    Like,
    Dislike,
    Skull
}