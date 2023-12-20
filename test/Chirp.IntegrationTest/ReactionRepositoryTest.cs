using System.Runtime.InteropServices;
using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Chirp.IntegrationTest;


public class ReactionRepositoryTest
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IReactionRepository _reactionRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly DatabaseContext _context;

    public ReactionRepositoryTest()
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
    }

    // THIS IS AN INTEGRATION TEST
    [Theory]
    //Like
    [InlineData("Sebastian Klein", ReactionType.Like)]
    //Dislike
    [InlineData("Sebastian Klein", ReactionType.Dislike)]
    //Skull
    [InlineData("Sebastian Klein", ReactionType.Skull)]
    public async void ReactionOnCheep_AddsNewReaction(string authorName, ReactionType reactionType)
    {
        // Arrange
        await _authorRepository.CreateAuthor(authorName);
        _cheepRepository.CreateCheep("This is a test", authorName);

        var cheep = await _context.Cheeps
        .FirstOrDefaultAsync(c => c.Author.Name == authorName);

        // Counting Reaction.like's on cheepID
        var CountBeforeReaction = await _reactionRepository.GetReactionAmount(cheep.CheepId, reactionType);

        // Act
        await _reactionRepository.ReactionOnCheep(reactionType, cheep.CheepId, authorName);

        // Assert
        // Verify that a new reaction has been added
        var CountAfterReaction = await _reactionRepository.GetReactionAmount(cheep.CheepId, reactionType);

        // Are there one more reaction now
        Assert.Equal(CountBeforeReaction, CountAfterReaction - 1);
    }

    [Theory]
    //Like
    [InlineData("Sebastian Klein", ReactionType.Like)]
    //Dislike
    [InlineData("Sebastian Klein", ReactionType.Dislike)]
    //Skull
    [InlineData("Sebastian Klein", ReactionType.Skull)]
    public async void ReactionOnCheep_RemoveOldReaction(string authorName, ReactionType reactionType)
    {
        // Arrange
        await _authorRepository.CreateAuthor(authorName);
        _cheepRepository.CreateCheep("This is a test", authorName);

        var cheep = await _context.Cheeps
            .FirstOrDefaultAsync(c => c.Author.Name == authorName);

        // Act
        // Attempt to add a new reaction
        await _reactionRepository.ReactionOnCheep(reactionType, cheep.CheepId, authorName);

        // Assert
        // Verify that a new reaction has been added
        var reactionAdded = await _context.Reactions.AnyAsync(r =>
            r.CheepId == cheep.CheepId && r.AuthorName == authorName && r.ReactionType == reactionType);

        Assert.True(reactionAdded, "Reaction should be added initially");

        // Counting Reaction.like's on cheepID
        var countBeforeUnreaction = await _reactionRepository.GetReactionAmount(cheep.CheepId, reactionType);

        // Act
        // Attempt to remove the reaction
        await _reactionRepository.ReactionOnCheep(reactionType, cheep.CheepId, authorName);

        // Assert
        // Verify that the reaction has been removed
        var reactionRemoved = await _context.Reactions.AllAsync(r =>
            r.CheepId != cheep.CheepId || r.AuthorName != authorName || r.ReactionType != reactionType);

        Assert.True(reactionRemoved, "Reaction should be removed after unreacting");

        // Counting Reaction.like's on cheepID after unreaction
        var countAfterUnreaction = await _reactionRepository.GetReactionAmount(cheep.CheepId, reactionType);

        // The count should be the same as before unreacting
        Assert.Equal(countBeforeUnreaction, countAfterUnreaction + 1);
    }

    [Theory]
    //Like
    [InlineData("Sebastian Klein", ReactionType.Like, ReactionType.Dislike)]
    [InlineData("Sebastian Klein", ReactionType.Like, ReactionType.Skull)]
    //Dislike
    [InlineData("Sebastian Klein", ReactionType.Dislike, ReactionType.Like)]
    [InlineData("Sebastian Klein", ReactionType.Dislike, ReactionType.Skull)]
    //Skull
    [InlineData("Sebastian Klein", ReactionType.Skull, ReactionType.Like)]
    [InlineData("Sebastian Klein", ReactionType.Skull, ReactionType.Dislike)]
    public async void ReactionOnCheep_ChangeReactionType(string authorName, ReactionType FirstReactionType, ReactionType SecondReactionType)
    {
        // Arrange
        await _authorRepository.CreateAuthor(authorName);
        _cheepRepository.CreateCheep("This is a test", authorName);

        var cheep = await _context.Cheeps
            .FirstOrDefaultAsync(c => c.Author.Name == authorName);

        // Act
        // Attempt to add a new reaction with ReactionType.Like
        await _reactionRepository.ReactionOnCheep(FirstReactionType, cheep.CheepId, authorName);

        // Assert
        // Verify that a new reaction has been added with ReactionType
        var reactionAdded = await _context.Reactions.AnyAsync(r =>
            r.CheepId == cheep.CheepId && r.AuthorName == authorName && r.ReactionType == FirstReactionType);

        Assert.True(reactionAdded, "Reaction should be added initially with ReactionType.Like");

        // Counting Reaction.Like's on cheepID
        var FirstCountBeforeChange = await _reactionRepository.GetReactionAmount(cheep.CheepId, FirstReactionType);
        var SecondCountBeforeChange = await _reactionRepository.GetReactionAmount(cheep.CheepId, SecondReactionType);

        // Act
        // Attempt to change the reaction to ReactionType
        await _reactionRepository.ReactionOnCheep(SecondReactionType, cheep.CheepId, authorName);

        // Assert
        // Verify that the reaction has been changed to ReactionType
        var reactionChanged = await _context.Reactions.AnyAsync(r =>
            r.CheepId == cheep.CheepId && r.AuthorName == authorName && r.ReactionType == SecondReactionType);

        Assert.True(reactionChanged, "Reaction should be changed to Dislike");

        // Counting Reaction.Like's on cheepID after changing the reaction
        var FirstCounterAfterChange = await _reactionRepository.GetReactionAmount(cheep.CheepId, FirstReactionType);
        var SecondCountAfterChange = await _reactionRepository.GetReactionAmount(cheep.CheepId, SecondReactionType);

        // The count of Reaction.Like should be the same as before changing
        Assert.Equal(FirstCountBeforeChange, FirstCounterAfterChange + 1);

        // The count of Reaction.Dislike should be one more than before changing
        Assert.Equal(SecondCountBeforeChange, SecondCountAfterChange - 1);
    }

    [Theory]
    //Like
    [InlineData("Sebastian Klein", ReactionType.Like)]
    //Dislike
    [InlineData("Sebastian Klein", ReactionType.Dislike)]
    //Skull
    [InlineData("Sebastian Klein", ReactionType.Skull)]
    public async void HasUserReacted(string authorName, ReactionType reactionType)
    {
        // Arrange
        await _authorRepository.CreateAuthor(authorName);
        _cheepRepository.CreateCheep("This is a test", authorName);

        var cheep = await _context.Cheeps
            .FirstOrDefaultAsync(c => c.Author.Name == authorName);

        // Act
        // See if it is false before reaction (Assert)
        Assert.False(await _reactionRepository.HasUserReacted(cheep.CheepId, authorName, reactionType));

        // Attempt to add a new reaction with ReactionType
        await _reactionRepository.ReactionOnCheep(reactionType, cheep.CheepId, authorName);

        // Assert
        // See if it is now true after the reaction
        Assert.True(await _reactionRepository.HasUserReacted(cheep.CheepId, authorName, reactionType));


    }

    [Theory]
    //Like
    [InlineData("Sebastian Klein", "Rune Klan", "Michael Bundesen", ReactionType.Like)]
    //Dislike
    [InlineData("Sebastian Klein", "Rune Klan", "Michael Bundesen", ReactionType.Dislike)]
    //Skull
    [InlineData("Sebastian Klein", "Rune Klan", "Michael Bundesen", ReactionType.Skull)]
    public async void GetReactionAmount(string authorName1, string authorName2, string authorName3, ReactionType reactionType)
    {
        // Arrange
        await _authorRepository.CreateAuthor(authorName1);
        await _authorRepository.CreateAuthor(authorName2);
        await _authorRepository.CreateAuthor(authorName3);
        //create 5 cheeps
        _cheepRepository.CreateCheep("This is a test 1", authorName1);

        var cheep = await _context.Cheeps
            .FirstOrDefaultAsync(c => c.Author.Name == authorName1);

        // Act & Assert
        // See if it is 0 before reaction (Assert)
        Assert.Equal(0, await _reactionRepository.GetReactionAmount(cheep.CheepId, reactionType));

        // Attempt to add a new reaction with Reactiontype
        await _reactionRepository.ReactionOnCheep(reactionType, cheep.CheepId, authorName1);
        // See if it 1 after 1 reaction
        Assert.Equal(1, await _reactionRepository.GetReactionAmount(cheep.CheepId, reactionType));

        // Attempt to add a new reaction with Reactiontype
        await _reactionRepository.ReactionOnCheep(reactionType, cheep.CheepId, authorName2);
        // See if it 2 after 2 reactions
        Assert.Equal(2, await _reactionRepository.GetReactionAmount(cheep.CheepId, reactionType));

        // Attempt to add a new reaction with Reactiontype
        await _reactionRepository.ReactionOnCheep(reactionType, cheep.CheepId, authorName3);
        // See if it 3 after 3 reactions
        Assert.Equal(3, await _reactionRepository.GetReactionAmount(cheep.CheepId, reactionType));
    }
}