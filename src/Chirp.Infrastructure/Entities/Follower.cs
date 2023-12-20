using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Repository;
public class Follower
{
    [Key]
    public string? FollowerId {get; set;}
    
    [Key]
    public string? FollowedId {get; set;}
    public Author? FollowerAuthor {get; set;}
    public Author? FollowedAuthor {get; set;}
}