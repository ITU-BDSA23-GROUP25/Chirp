namespace Chirp.Razor;

public class Author
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public List<Cheep> Cheeps { get; } = new();
}