namespace Core;

public interface IReactionRepository
{

    //Get
    public Task<int> GetReactionAmount(Guid cheepId, ReactionType reactionType);
    public Task<bool> HasUserReacted(Guid cheepId, string userName, ReactionType reactionType);


    //Post
    public Task ReactionOnCheep(ReactionType reactionType, Guid cheepId, string userName);
    public Task RemoveAllReactionsByUser(string username);

}