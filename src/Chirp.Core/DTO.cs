namespace Core;

public record CheepDTO(Guid Id, string Author, string Message, string Timestamp);
public record AuthorDTO(string Name, string Email);