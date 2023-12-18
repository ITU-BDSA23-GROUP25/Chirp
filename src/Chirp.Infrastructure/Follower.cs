namespace Repository;

public class Follower
{
    public string? FollowerId {get; set;}
    public string? FollowedId {get; set;}
    public Author? FollowerAuthor {get; set;}
    public Author? FollowedAuthor {get; set;}
}