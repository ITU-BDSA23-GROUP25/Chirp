namespace CheepRepositoryTest;


/// <summary>
/// Integration test for CheepRepository
/// </summary>
public class CheepRepositoryTest
{

    private readonly IAuthorRepository _authorRepository;

    private readonly ICheepRepository _cheepRepository;

    private readonly DatabaseContext _context;

    /// <summary>
    /// Tests the creation and removal of a Cheep.
    /// </summary>
    public CheepRepositoryTest()
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
    }

    [Fact]
    public async void CreateAndRemoveCheep()
    {
         // Arrange
        var text = "Hej med dig";
        var name = "Sebb";

        // Act: Create an author and retrieve it using the AuthorRepository
        await _authorRepository.CreateAuthor(name);
        var author = await _authorRepository.GetAuthorByName(name);

        // Act: Create a Cheep using the CheepRepository
        _cheepRepository.CreateCheep(text, author.Name);

        // Act: Retrieve the Cheep from the database
        var cheep = await _context.Cheeps
        .Include(a => a.Author)
        .Where(a => a.Author.Name == name)
        .Select(c => new CheepDTO(c.CheepId,c.Author.Name,c.Text, c.TimeStamp.AddHours(1).ToString("MM/dd/yy H:mm:ss"),Array.Empty<ReactionDTO>() ))
        .FirstOrDefaultAsync();

       
        // Assert: Check if the created Cheep matches the expected text and author name
        Assert.Equal(cheep.Message, text);
        Assert.Equal(cheep.Author, name);

         // Act: Remove the Cheep using the CheepRepository
        _cheepRepository.RemoveCheep(cheep);

        // Act: Attempt to retrieve the Cheep from the database after removal
        cheep = await _context.Cheeps
        .Include(a => a.Author)
        .Where(a => a.Author.Name == name)
        .Select(c => new CheepDTO(c.CheepId, c.Author.Name, c.Text, c.TimeStamp.AddHours(1).ToString("MM/dd/yy H:mm:ss"), Array.Empty<ReactionDTO>()))
        .FirstOrDefaultAsync();

        // Assert: Check if the Cheep is null, indicating successful removal
        Assert.Null(cheep);
    }   
}