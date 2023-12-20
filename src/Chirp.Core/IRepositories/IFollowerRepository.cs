namespace Core;

/// <summary>
/// This is the interface for the FollowerRepository, where the methodsignitures are displayed
/// </summary>
public interface IFollowerRepository
{
    // Get

    /// <summary>
    /// This method returns an IEnumerable of AuthorDTO's corresponding 
    /// to all authors that the author follows
    /// </summary>
    /// <param name="userName">The name of the author</param>
    /// <returns></returns>
    public Task<IEnumerable<AuthorDTO>> GetFollowerAuthor(string userName);

    /// <summary>
    /// This method returns an IEnumerable of AuthorDTO's corresponding 
    /// to all authors that follows the author
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public Task<IEnumerable<AuthorDTO>> GetFollowedAuthor(string userName);

    // Do

    /// <summary>
    /// This method either add or remove a follower in the Follower Table
    /// </summary>
    /// <param name="followerName">the user that are being followed</param>
    /// <param name="userName">the name user which wants to stop or start following someone</param>
    /// <returns></returns>
    public Task AddOrRemoveFollower(string followerName, string userName);

    /// <summary>
    /// This method removes all follows to a user
    /// </summary>
    /// <param name="userName">the name of the user which all 
    /// follows to that user needs to be deleted</param>
    /// <returns></returns>
    public Task RemoveAllFollowersToUser(string userName);

    /// <summary>
    /// This method removes all follow a user has 
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public Task RemoveFollowers(string userName);

}