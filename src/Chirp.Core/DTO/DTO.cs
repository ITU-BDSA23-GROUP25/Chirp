namespace Core;
/// <summary>
/// DTO or Data Transer Objects are used to take data from our database, 
/// and make them into object that can we handled and used in the code.
/// </summary>
/// <param name="Id">Each cheep has an unique Id, which is generated on creation</param>
/// <param name="Author">The name of the author of the cheep</param>
/// <param name="Message">The message of the cheep</param>
/// <param name="Timestamp">The ufc time of the post of the cheep</param>
/// <param name="Reactions">The possible reactions of the cheep</param>

public record CheepDTO(Guid Id, string Author, string Message, string Timestamp, ICollection<ReactionDTO> Reactions);

/// <summary>
/// AuthorDTO contains the name of the author
/// </summary>
public record AuthorDTO
{
    public required string Name { get; init;}
}

/// <summary>
/// 
/// </summary>
/// <param name="Reactiontype">One of the reactiontypes in the ReactionType enum</param>
/// <param name="Count">The amount of reactions of the reactionType</param>
public record ReactionDTO(ReactionType Reactiontype, int Count);

/// <summary>
/// ReactionType has 3 option, displayed below
/// </summary>
public enum ReactionType
{
    Like,
    Dislike,
    Skull
}