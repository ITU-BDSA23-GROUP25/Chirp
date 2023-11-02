using Microsoft.AspNetCore.Identity;

namespace Repository;


public class Author : IdentityUser
{
    //public Guid AuthorId { get; set; }
    public required string Name { get; set; }
    public List<Cheep> Cheeps { get; set; } = new();
}