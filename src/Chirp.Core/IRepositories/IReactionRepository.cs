namespace Core;

/// <summary>
/// This is the interface for the ReactionRepository, where the methodsignitures are displayed
/// </summary>
public interface IReactionRepository
{

    //Get

    /// <summary>
    /// This method return an integer of the count on a given cheep, 
    /// and a given reactionType
    /// </summary>
    /// <param name="cheepId">the unique Id of each cheep</param>
    /// <param name="reactionType">1 of 3 reactionTypes (Like, Dislike or Skull)</param>
    /// <returns></returns>
    public Task<int> GetReactionAmount(Guid cheepId, ReactionType reactionType);

    /// <summary>
    /// This method return a boolean, 
    /// saying if a user has reaction with a specific reactionType on a given cheep
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="userName">the name of the user who reacted</param>
    /// <param name="reactionType"></param>
    /// <returns></returns>
    public Task<bool> HasUserReacted(Guid cheepId, string userName, ReactionType reactionType);


    //Post

    /// <summary>
    /// This method creates a reaction in the Reactions table,
    /// inserting the reactionType, the cheepId and the name of the reacter
    /// </summary>
    /// <param name="reactionType"></param>
    /// <param name="cheepId"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    public Task ReactionOnCheep(ReactionType reactionType, Guid cheepId, string userName);

    /// <summary>
    /// This methods removes a reaction in the Reactions table,
    /// when given the name of the reacter
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public Task RemoveAllReactionsByUser(string username);

}