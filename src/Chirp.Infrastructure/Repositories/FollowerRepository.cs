using System.Runtime.CompilerServices;

namespace Repository;

public class FollowerRepository : IFollowerRepository
{

    private readonly DatabaseContext _databaseContext;
    private const int CheepsPerPage = 32;

    public FollowerRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<IEnumerable<AuthorDTO>> GetFollowerAuthor(string AuthorName) => await _databaseContext.Followers

        .Where(f => f.FollowedAuthor.Name == AuthorName)
        .Select(f => f.FollowerAuthor)
        .Select<Author, AuthorDTO>(f => new AuthorDTO()
        {
           Name = f.Name,
        }).ToListAsync();

    public async Task<IEnumerable<AuthorDTO>> GetFollowedAuthor(string AuthorName) => await _databaseContext.Followers

        .Where(f => f.FollowerAuthor.Name == AuthorName)
        .Select(f => f.FollowedAuthor)
        .Select<Author, AuthorDTO>(f => new AuthorDTO()
        {
           Name = f.Name,
        }).ToListAsync();

    public async Task AddOrRemoveFollower(string userName, string followerName)
    {
        var user = await _databaseContext.Users.SingleOrDefaultAsync(a => a.Name == userName);
        var follower = await _databaseContext.Users.SingleOrDefaultAsync(a => a.Name == followerName);
        bool isFollowing = false;

        if (user == follower)
        {
            throw new ArgumentException("User and follower cannot be equal to one another: ", nameof(userName));
        }

        if (user is null)
        {
            throw new ArgumentException("User does not exist: ", nameof(userName));
        }

        if (follower is null)
        {
            throw new ArgumentException("Follower does not exist: ", nameof(followerName));
        }

        var existingFollower = await _databaseContext.Followers.SingleOrDefaultAsync(f =>
            f.FollowedAuthor.Name == userName && f.FollowerAuthor.Name == followerName);


        if (existingFollower != null)
        {
            _databaseContext.Followers.Remove(existingFollower);
            isFollowing = false; // User is unfollowing
        }
        else
        {
            var newFollower = new Follower()
            {
                FollowerId = follower.Id,
                FollowedId = user.Id,
                FollowerAuthor = follower,
                FollowedAuthor = user
            };

            _databaseContext.Followers.Add(newFollower);
            isFollowing = true; // User is following
        }

        await _databaseContext.SaveChangesAsync();
    }

    public async Task RemoveFollowers(string userName)
    {
            
            await _databaseContext.Followers
            .Where(f => f.FollowedAuthor.Name == userName)
            .ExecuteDeleteAsync();
    }

    public async Task RemoveAllFollowersToUser(string userName)
    {
            
            await _databaseContext.Followers
            .Where(f => f.FollowerAuthor.Name == userName)
            .ExecuteDeleteAsync();
    }

}