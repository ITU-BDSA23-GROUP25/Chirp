public class AboutMeTest
{

    private readonly IAuthorRepository _authorRepository;

    private readonly ICheepRepository _cheepRepository;

    private readonly IFollowerRepository _followerRepository;

    private readonly IReactionRepository _reactionRepository;

    private readonly DatabaseContext _context;

    public AboutMeTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;

        _context = new DatabaseContext(options);
        _context.Database.Migrate();
        _context.InitializeDB();

        _authorRepository = new AuthorRepository(_context);
        _cheepRepository = new CheepRepository(_context);
        _followerRepository = new FollowerRepository(_context);
        _reactionRepository = new ReactionRepository(_context);
    }


    [Theory]
    [InlineData("Sebb", "Karll", "Hej med dig")]
    public async void DeleteUser_WithDepedencies(string username, string username2, string message)
    {
        await _authorRepository.CreateAuthor(username);
        await _authorRepository.CreateAuthor(username2);

        _cheepRepository.CreateCheep(message, username2);
        _cheepRepository.CreateCheep(message, username);

        await _followerRepository.AddOrRemoveFollower(username2, username);
        await _followerRepository.AddOrRemoveFollower(username, username2);

        var cheep_from_username = await _context.Cheeps
        .FirstOrDefaultAsync(c => c.Author.Name == username);

        var cheep_from_username2 = await _context.Cheeps
        .FirstOrDefaultAsync(c => c.Author.Name == username2);
        
        var author = await _context.Authors
        .FirstOrDefaultAsync(a => a.Name == username);

        await _reactionRepository.ReactionOnCheep(ReactionType.Like, cheep_from_username2.CheepId, username);

        /* var author_to_remove = await _context.Authors
        .Where(c => c.Name == author.Name)
        .Select(a =>
        new AuthorDTO
        {
            Name = a.Name
        }).AsNoTracking()
        .FirstAsync(); */

        var author323 = await _authorRepository.GetAuthorByName(username);

        var cheep = await _context.Cheeps
        .Include(a => a.Author)
        .Where(a => a.Author.Name == author.Name)
        .Select(c => new CheepDTO(c.CheepId, c.Author.Name, c.Text, c.TimeStamp.AddHours(1).ToString("MM/dd/yy H:mm:ss"), Array.Empty<ReactionDTO>()))
        .FirstAsync();

        var reactionAdded = await _context.Reactions.AnyAsync(r =>
            r.CheepId == cheep_from_username2.CheepId && r.AuthorName == username && r.ReactionType == ReactionType.Like);
        var followadded = await _context.Followers.AnyAsync(f => f.FollowerId == author.Id);
        var cheepadded = await _context.Cheeps.AnyAsync(c => c.CheepId == cheep_from_username.CheepId);
        var authorcreated = await _context.Authors.AnyAsync(a => a.Name == author323.Name);

        Assert.True(followadded);
        Assert.True(reactionAdded);
        Assert.True(cheepadded);
        Assert.True(authorcreated);

        await _authorRepository.RemoveAuthor(author323);
        _cheepRepository.RemoveCheep(cheep);
        
        await _followerRepository.RemoveFollowers(username2);
        await _followerRepository.RemoveAllFollowersToUser(username);
        await _reactionRepository.RemoveAllReactionsByUser(username);  
     
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
