namespace Core;
public interface IFollowerRepository
{
    // Get
    public Task<IEnumerable<AuthorDTO>> GetFollowerAuthor(string AuthorName);
    public Task<IEnumerable<AuthorDTO>> GetFollowedAuthor(string AuthorName);

    // Do
    //public Task AddFollower(String AuthorName, string FollowerName);
    //public Task DeleteFollower(String AuthorName, string FollowerName);
    public Task AddOrRemoveFollower(string followerName, string userName);

    public Task RemoveFollowers(string userName);

}