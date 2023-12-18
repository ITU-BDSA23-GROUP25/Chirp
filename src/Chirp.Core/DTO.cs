namespace Core;

public record CheepDTO(Guid Id, string Author, string Message, string Timestamp);
public record AuthorDTO
{
    public required string Name { get; init;}
}