using System.Runtime.InteropServices;
using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Chirp.IntegrationTest;


public class FollowerRepositoryTest
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IReactionRepository _reactionRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IFollowerRepository _followerRepository;
    private readonly DatabaseContext _context;

    public FollowerRepositoryTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;

        _context = new DatabaseContext(options);
        _context.Database.Migrate();

        _reactionRepository = new ReactionRepository(_context);
        _cheepRepository = new CheepRepository(_context);
        _authorRepository = new AuthorRepository(_context);
        _followerRepository = new FollowerRepository(_context);
    }

    [Theory]
    [InlineData("Lars Ulrich", "Hansi Hinterseer")]
    public async void GetFollowerAuthor_OnOneFollower(string userName, string userName2)
    {

        // Arrange
        await _authorRepository.CreateAuthor(userName);
        await _authorRepository.CreateAuthor(userName2);

        // Author should have been created
        var author = await _context.Authors
        .AnyAsync(c => c.Name == userName);

        Assert.True(author);

        // Author2 should have been created
        var author2 = await _context.Authors
        .AnyAsync(c => c.Name == userName);

        Assert.True(author2);

        // user2 follows user 1
        await _followerRepository.AddOrRemoveFollower(userName, userName2);

        // Act
        // Get followers
        var Followers = await _followerRepository.GetFollowerAuthor(userName);

        // Assert
        foreach (var followerDTO in Followers)
        {
            Assert.Equal(userName2, followerDTO.Name);
        }
    }

    [Theory]
    [InlineData("Lars Ulrich", "Hansi Hinterseer", "Justin Timberlake", "Morten Olsen")]
    public async void GetFollowerAuthor_OnMultipleFollowers(string userName, string userName2, string userName3, string userName4)
    {
        // Arrange
        await _authorRepository.CreateAuthor(userName);
        await _authorRepository.CreateAuthor(userName2);
        await _authorRepository.CreateAuthor(userName3);
        await _authorRepository.CreateAuthor(userName4);

        // Act
        // Make users 2, 3, and 4 follow user 1
        await _followerRepository.AddOrRemoveFollower(userName, userName2);
        await _followerRepository.AddOrRemoveFollower(userName, userName3);
        await _followerRepository.AddOrRemoveFollower(userName, userName4);

        // Get followers
        var followers = await _followerRepository.GetFollowerAuthor(userName);

        // Assert
        Assert.Equal(3, followers.Count()); // Make sure there are three followers

        // Check that each follower are there
        Assert.Contains(followers, follower => follower.Name == userName2);
        Assert.Contains(followers, follower => follower.Name == userName3);
        Assert.Contains(followers, follower => follower.Name == userName4);
    }


    [Theory]
    [InlineData("Lars Ulrich")]
    public async void GetFollowedAuthor_OnNoFollowers(string userName)
    {

        // Arrange
        await _authorRepository.CreateAuthor(userName);

        // Author should have been created
        var author = await _context.Authors
        .AnyAsync(c => c.Name == userName);

        Assert.True(author);

        // Act
        // Get followers
        var Followers = await _followerRepository.GetFollowedAuthor(userName);

        // Assert
        Assert.Empty(Followers);
    }

    [Theory]
    [InlineData("Lars Ulrich", "Hansi Hinterseer")]
    public async void GetFollowedAuthor_OnOneFollower(string userName, string userName2)
    {

        // Arrange
        await _authorRepository.CreateAuthor(userName);
        await _authorRepository.CreateAuthor(userName2);

        // Author should have been created
        var author = await _context.Authors
        .AnyAsync(c => c.Name == userName);

        Assert.True(author);

        // Author2 should have been created
        var author2 = await _context.Authors
        .AnyAsync(c => c.Name == userName);

        Assert.True(author2);

        // user2 follows user 1
        await _followerRepository.AddOrRemoveFollower(userName, userName2);

        // Act
        // Get followers
        var Followers = await _followerRepository.GetFollowedAuthor(userName);

        // Assert
        foreach (var followerDTO in Followers)
        {
            Assert.Equal(userName2, followerDTO.Name);
        }
    }

    [Theory]
    [InlineData("Lars Ulrich", "Hansi Hinterseer", "Justin Timberlake", "Morten Olsen")]
    public async void GetFollowedAuthor_OnMultipleFollowers(string userName, string userName2, string userName3, string userName4)
    {
        // Arrange
        await _authorRepository.CreateAuthor(userName);
        await _authorRepository.CreateAuthor(userName2);
        await _authorRepository.CreateAuthor(userName3);
        await _authorRepository.CreateAuthor(userName4);

        // Act
        // Make users 2, 3, and 4 follow user 1
        await _followerRepository.AddOrRemoveFollower(userName2, userName);
        await _followerRepository.AddOrRemoveFollower(userName3, userName);
        await _followerRepository.AddOrRemoveFollower(userName4, userName);

        // Get followers
        var followers = await _followerRepository.GetFollowedAuthor(userName);

        // Assert
        Assert.Equal(3, followers.Count()); // Make sure there are three followers

        // Check that each follower are there
        Assert.Contains(followers, follower => follower.Name == userName2);
        Assert.Contains(followers, follower => follower.Name == userName3);
        Assert.Contains(followers, follower => follower.Name == userName4);
    }

    [Theory]
    [InlineData("Lars Ulrich", "Hansi Hinterseer")]
    public async void AddOrRemoveFollower_AddFollower(string userName, string userName2)
    {
        // Arrange
        await _authorRepository.CreateAuthor(userName);
        await _authorRepository.CreateAuthor(userName2);

        // Act 

        await _followerRepository.AddOrRemoveFollower(userName, userName2);

        var Followers = await _followerRepository.GetFollowerAuthor(userName2);

        // Assert
        foreach (var followerDTO in Followers)
        {
            Assert.Equal(userName2, followerDTO.Name);
        }
    }

    [Theory]
    [InlineData("Lars Ulrich", "Hansi Hinterseer")]
    public async void AddOrRemoveFollower_RemoveFollower(string userName, string userName2)
    {
        // Arrange
        await _authorRepository.CreateAuthor(userName);
        await _authorRepository.CreateAuthor(userName2);

        // Act 
        // Add follower
        await _followerRepository.AddOrRemoveFollower(userName, userName2);

        // Assert
        var followersAfterAdding = await _followerRepository.GetFollowerAuthor(userName);
        Assert.Contains(followersAfterAdding, follower => follower.Name == userName2);

        // Act
        // Remove follower
        await _followerRepository.AddOrRemoveFollower(userName, userName2);

        // Assert
        var followersAfterRemoving = await _followerRepository.GetFollowerAuthor(userName2);
        Assert.DoesNotContain(followersAfterRemoving, follower => follower.Name == userName);
        Assert.Empty(followersAfterRemoving); // Ensure that the list is empty after removal.
    }

    [Theory]
    [InlineData("Lars Ulrich")]
    public async void AddOrRemoveFollower_UserFollowItself(string userName)
    {
        // Arrange
        await _authorRepository.CreateAuthor(userName);

        // Act 
        // Add follower (attempt to follow themselves)
        try
        {
            await _followerRepository.AddOrRemoveFollower(userName, userName);
            // Assert
            var followersAfterAdding = await _followerRepository.GetFollowerAuthor(userName);
            Assert.Empty(followersAfterAdding);
        }
        catch
        {
            // Assert
            var followersAfterAdding = await _followerRepository.GetFollowerAuthor(userName);
            Assert.Empty(followersAfterAdding);
        }
    }

    [Theory]
    [InlineData("Lars Ulrich", "Hansi Hinterseer", "Michael Bundesen")]
    public async Task RemoveAllFollowersToUser(string userName, string userName2, string userName3)
    {
        // Arrange
        await _authorRepository.CreateAuthor(userName);
        await _authorRepository.CreateAuthor(userName2);
        await _authorRepository.CreateAuthor(userName3);

        // User2 follows user1
        await _followerRepository.AddOrRemoveFollower(userName2, userName);

        // User3 follows user1
        await _followerRepository.AddOrRemoveFollower(userName3, userName);

        // Act
        // Remove all followers
        await _followerRepository.RemoveAllFollowersToUser(userName);

        // Assert
        var followersAfterRemoval = await _followerRepository.GetFollowerAuthor(userName);

        // Check that the follower list is empty
        Assert.Empty(followersAfterRemoval);
    }

    [Theory]
    [InlineData("Lars Ulrich", "Hansi Hinterseer", "Michael Bundesen")]
    public async Task RemoveFollowers(string userName, string userName2, string userName3)
    {
        // Arrange
        await _authorRepository.CreateAuthor(userName);
        await _authorRepository.CreateAuthor(userName2);
        await _authorRepository.CreateAuthor(userName3);

        // User2 follows user1
        await _followerRepository.AddOrRemoveFollower(userName, userName2);

        // User3 follows user1
        await _followerRepository.AddOrRemoveFollower(userName, userName3);

        // Act
        // Remove followers
        await _followerRepository.RemoveFollowers(userName);

        // Assert
        var followersAfterRemoval = await _followerRepository.GetFollowerAuthor(userName);

        // Check that the follower list is empty
        //
        Assert.Empty(followersAfterRemoval);
    }
}