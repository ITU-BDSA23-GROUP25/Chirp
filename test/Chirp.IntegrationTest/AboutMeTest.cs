namespace AboutMeTest;

/// <summary>
/// Integrationtest for the functionality related to delete users account on the Aboutme page.
/// </summary>
public class AboutMeTest
{

    private readonly IAuthorRepository _authorRepository;

    private readonly ICheepRepository _cheepRepository;

    private readonly IFollowerRepository _followerRepository;

    private readonly IReactionRepository _reactionRepository;

    private readonly DatabaseContext _context;

    /// <summary>
    /// A test that first creates 2 authors, then 2 cheeps. 1 from each author. 
    /// It then makes the authors follow each other. Lastly it makes one of them react to the otherones cheep.
    /// Then it delets the user and checks if the user along with all the userdata is deleted from the database context.
    /// </summary>
    public AboutMeTest()
    {
        // Set up an in-memory SQLite database for testing
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        // Create a new DatabaseContext with the in-memory database and apply migrations
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;

        _context = new DatabaseContext(options);
        _context.Database.Migrate();
        _context.InitializeDB();

        // Initialize repositories with the in-memory database context
        _authorRepository = new AuthorRepository(_context);
        _cheepRepository = new CheepRepository(_context);
        _followerRepository = new FollowerRepository(_context);
        _reactionRepository = new ReactionRepository(_context);
    }

    /// <summary>
    /// /// Tests the deletion of a user with associated dependencies, including cheeps, followers, and reactions.
    /// </summary>
    /// <param name="username">First author</param>
    /// <param name="username2">Second author</param>
    /// <param name="message">Message for the creation of the cheeps</param>
    [Theory]
    [InlineData("Sebb", "Karll", "Hej med dig")]
    public async void DeleteUser_WithDepedencies(string username, string username2, string message)
    {
        // Create authors, cheeps, followers for testing
        await _authorRepository.CreateAuthor(username);
        await _authorRepository.CreateAuthor(username2);

        _cheepRepository.CreateCheep(message, username2);
        _cheepRepository.CreateCheep(message, username);

        await _followerRepository.AddOrRemoveFollower(username2, username);
        await _followerRepository.AddOrRemoveFollower(username, username2);

        // Retrieve entities for testing
        var cheep_from_username = await _context.Cheeps
        .FirstOrDefaultAsync(c => c.Author.Name == username);

        var cheep_from_username2 = await _context.Cheeps
        .FirstOrDefaultAsync(c => c.Author.Name == username2);
        
        var author = await _context.Authors
        .FirstOrDefaultAsync(a => a.Name == username);

        //Create reaction for testing
        await _reactionRepository.ReactionOnCheep(ReactionType.Like, cheep_from_username2.CheepId, username);

        // Retrieve entities for testing
        var author323 = await _authorRepository.GetAuthorByName(username);

        var cheep = await _context.Cheeps
        .Include(a => a.Author)
        .Where(a => a.Author.Name == author.Name)
        .Select(c => new CheepDTO(c.CheepId, c.Author.Name, c.Text, c.TimeStamp.AddHours(1).ToString("MM/dd/yy H:mm:ss"), Array.Empty<ReactionDTO>()))
        .FirstAsync();

        // Check if entities and relationships are created
        var reactionAdded = await _context.Reactions.AnyAsync(r =>
            r.CheepId == cheep_from_username2.CheepId && r.AuthorName == username && r.ReactionType == ReactionType.Like);
        var followadded = await _context.Followers.AnyAsync(f => f.FollowerId == author.Id);
        var cheepadded = await _context.Cheeps.AnyAsync(c => c.CheepId == cheep_from_username.CheepId);
        var authorcreated = await _context.Authors.AnyAsync(a => a.Name == author323.Name);

        Assert.True(followadded);
        Assert.True(reactionAdded);
        Assert.True(cheepadded);
        Assert.True(authorcreated);

        
        // Remove entities and relationships
        await _authorRepository.RemoveAuthor(author323);
        _cheepRepository.RemoveCheep(cheep);
        
        await _followerRepository.RemoveFollowers(username2);
        await _followerRepository.RemoveAllFollowersToUser(username);
        await _reactionRepository.RemoveAllReactionsByUser(username);  
     
        // Check if entities and relationships are removed
        var reactionremoved = await _context.Reactions.AnyAsync(r =>r.CheepId == cheep_from_username2.CheepId && r.AuthorName == username && r.ReactionType == ReactionType.Like);
        var followremoved = await _context.Followers.AnyAsync(f => f.FollowerId == author.Id);
        var followsremoved = await _context.Followers.AnyAsync(f => f.FollowedId== author.Id);
        var cheepremoved = await _context.Cheeps.AnyAsync(c => c.CheepId == cheep_from_username.CheepId);
        var authorremoved = await _context.Authors.AnyAsync(a => a.Name == author323.Name);

        Assert.False(followremoved);
        Assert.False(reactionremoved);
        Assert.False(cheepremoved);
        Assert.False(authorremoved);
    }

}
