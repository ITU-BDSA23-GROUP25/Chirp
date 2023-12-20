namespace AuthorRepositoryTest;

/// <summary>
/// Integration testsuit for authorRepository
/// </summary>

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
    /// Tests the creation and deletion of an author.
    /// </summary>

    [Fact]
    public async void CreateAndDeleteAuthor()
    {
        // Act: Create an author using the AuthorRepository
        var name = "Sebb";
        
        var author = _authorRepository.CreateAuthor(name);

        var author_database = await _context.Authors.Where(c => c.Name == name).FirstOrDefaultAsync();

        // Assert: Check if the created author matches the expected name
        Assert.Equal(author_database.Name, name);

        // Act: Retrieve an AuthorDTO to remove the author
        var author_to_remove = await _context.Authors
            .Where(c => c.Name == name)
            .Select(a =>
            new AuthorDTO
            {
                Name = a.Name
            }).FirstOrDefaultAsync();

        await _authorRepository.RemoveAuthor(author_to_remove);


        // Assert: Check if the author is null, indicating successful removal
        author_database = await _context.Authors.Where(c => c.Name == name).FirstOrDefaultAsync();

        Assert.Null(author_database);
    }

}