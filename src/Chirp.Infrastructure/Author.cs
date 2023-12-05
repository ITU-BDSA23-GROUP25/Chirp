namespace Repository;

public class Author : IdentityUser //added <string>
{
    //public Guid AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public List<Cheep> Cheeps { get; set; } = new();
}