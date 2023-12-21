namespace Chirp.UnitTest;

public class AuthorRepositoryTest
{

    private readonly IAuthorRepository _authorRepository;
    private readonly DatabaseContext _context;

    public AuthorRepositoryTest()
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
    }

    /// <summary>
    /// Unit tests the GetAuthorByName method with various author names.
    /// </summary>
    /// <param name="author_name">The name of the author to retrieve.</param>
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public async void GetAuthorByName_returnsAuthor(string author_name){

        // Act: Retrieve the author using the GetAuthorByName method
        var author = await _authorRepository.GetAuthorByName(author_name);

        // Assert: Check if the retrieved author's name matches the expected author_name
        Assert.Equal(author.Name, author_name);
    }

    /// <summary>
    /// Unit test for the GetAuthorByName method in the AuthorRepository, ensuring it returns null for non-existent authors.
    /// </summary>

    [Theory]
    [InlineData("Thorvald")]
    [InlineData("ABCD")]
      public async void GetAuthorByName_returnsEmpty(string author_name){

        // Act: Retrieve the author using the GetAuthorByName method
        var author = await _authorRepository.GetAuthorByName(author_name);
        
        // Assert: Check if the retrieved author is null, indicating the author does not exist
        Assert.Null(author);
    }
}