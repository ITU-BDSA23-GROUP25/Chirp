namespace Core;

public record CheepDTO(string Author, string Message, string Timestamp);
public record AuthorDTO(string name, string Email, List<CheepDTO> Cheeps);