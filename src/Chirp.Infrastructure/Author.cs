using System.ComponentModel.DataAnnotations;

namespace Repository;

public class Author : IdentityUser<string> //added <string>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public List<Cheep> Cheeps { get; set; } = new();
}