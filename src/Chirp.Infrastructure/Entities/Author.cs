using System.ComponentModel.DataAnnotations;

namespace Repository;

/// <summary>
/// An Author inherrits from identityUser, which is an AspNetCore class
/// used for authentication.
/// An Author is the user and the owner of their cheeps
/// </summary>
public class Author : IdentityUser
{
    public required string Name { get; set; }
    public List<Cheep> Cheeps { get; set; } = new();  
}