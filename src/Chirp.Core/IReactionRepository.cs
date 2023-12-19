namespace Core;

public interface IReactionRepository
{
    //Post
    public Task ReactionOnCheep(ReactionType reactionType, Guid cheepId, string userName);
    public Task<int> GetReactionAmount(Guid cheepId, ReactionType reactionType);
    public Task<bool> HasUserReacted(Guid cheepId, string userName, ReactionType reactionType);

}