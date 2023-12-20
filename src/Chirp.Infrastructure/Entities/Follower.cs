using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Repository;

/// <summary>
/// The Follower entity contains a unique primary key combination between a
/// Follower id and the Follewed's id
/// Also it stores the two Authors
/// </summary>
public class Follower
{
    [Key]
    public string? FollowerId {get; set;}
    
    [Key]
    public string? FollowedId {get; set;}
    public Author? FollowerAuthor {get; set;}
    public Author? FollowedAuthor {get; set;}
}