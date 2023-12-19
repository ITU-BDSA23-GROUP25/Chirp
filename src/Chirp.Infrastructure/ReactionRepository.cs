using Azure;

namespace Repository
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly DatabaseContext _databaseContext;
        private const int CheepsPerPage = 32;

        public ReactionRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            _databaseContext.InitializeDB();
        }

        public async Task<bool> HasUserReacted(Guid cheepId, string authorName, ReactionType reactionType)
{
    var author = await _databaseContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);

    if (author != null)
    {
        var hasUserReacted = await _databaseContext.Reactions
            .AnyAsync(r => r.ReactionType == reactionType && r.CheepId == cheepId && r.AuthorName == author.Name);

        return hasUserReacted;
    }
    else
    {
        // If author is not found, return false instead of throwing an exception
        return false;
    }
}


        public async Task ReactionOnCheep(ReactionType reactionType, Guid cheepId, string authorName)
{
    var cheep = await _databaseContext.Cheeps
        .Include(c => c.Reactions)
        .FirstOrDefaultAsync(c => c.CheepId == cheepId);

    var author = await _databaseContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);

    if (cheep != null || author != null)
    {

        var existingReaction = cheep.Reactions.FirstOrDefault(r =>
            r.AuthorName == authorName && r.ReactionType != reactionType
        );

        if (existingReaction != null)
            {
                _databaseContext.Reactions.Remove(existingReaction);
            }

        var currentReaction = cheep.Reactions.FirstOrDefault(r =>
            r.AuthorName == authorName && r.ReactionType == reactionType
        );

        if (currentReaction == null)
        {
            var reaction = new Reaction
            {
                CheepId = cheepId,
                AuthorName = authorName,
                ReactionType = reactionType
            };

            _databaseContext.Reactions.Add(reaction);
        }
        else
        {
            _databaseContext.Reactions.Remove(currentReaction);
        }

        await _databaseContext.SaveChangesAsync();
    }
    else
    {
        throw new NullReferenceException("Cheep or Author not found");
    }
}


        public async Task<int> GetReactionAmount(Guid cheepId, ReactionType reactionType)
        {
            var reactions = await _databaseContext.Reactions
                .Where(r => r.ReactionType == reactionType && r.CheepId == cheepId)
                .ToListAsync();

            return reactions.Count;
        }
    }
}