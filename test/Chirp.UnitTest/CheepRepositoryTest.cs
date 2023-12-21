using System.Diagnostics;
using Microsoft.Identity.Client;

namespace ChirpRepository_test;

/// <summary>
/// Unit test for the CheepRepository
/// </summary>

public class CheepRepository_tests
{
    private readonly ICheepRepository _cheepRepository;
    private readonly DatabaseContext _context;

    public CheepRepository_tests()
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
        _cheepRepository = new CheepRepository(_context);
    }

    /// <summary>
    /// Tests the GetAllCheepsFromAuthor method with a specific author name.
    /// </summary>
    /// <param name="author">The name of the author whose cheeps are to be retrieved.</param>

    [Theory]
    [InlineData("Jacqualine Gilcoine")]
    public async void GetAllCheepsFromAuthor(string author)
    {
        // Act: Retrieve cheeps using the GetAllCheepsFromAuthor method
        var cheeps = await _cheepRepository.GetAllCheepsFromAuthor(author);

        // Act: Retrieve cheeps from the database using LINQ
        var cheeps_Database = await _context.Cheeps.Where(c => c.Author.Name == author).ToListAsync();

        // Assert: Check if the count of retrieved cheeps matches the count in the database
        Assert.Equal(cheeps_Database.Count(), cheeps.Count());
    }

    /// <summary>
    /// Unit test for the GetCheeps method in the CheepRepository, ensuring it retrieves 32 cheeps for the public timeline.
    /// </summary>

    [Theory]
    [InlineData(1)]
    public async void publictimeline_returns32Cheeps(int page)
    {
        // Act: Retrieve cheeps using the GetCheeps method for the public timeline
        var cheeps = await _cheepRepository.GetCheeps(page, "Newest");

        // Assert: Check if the count of retrieved cheeps matches the expected count
        Assert.Equal(32, cheeps.Count());
    }

    /// <summary>
    /// Tests the GetCheepsFromAuthor method for an author's timeline with a specified page, author name, and sorting order.
    /// </summary>
    /// <param name="page">The page number to retrieve cheeps from.</param>
    /// <param name="author">The name of the author whose cheeps are to be retrieved.</param>
    [Theory]
    [InlineData(1, "Jacqualine Gilcoine")]
    public async void authortimeline_Returns32Cheeps(int page, string author)
    {
        // Act: Retrieve cheeps using the GetCheepsFromAuthor method for the specified author's timeline
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(page, author, "Newest");

        // Assert: Check if the count of retrieved cheeps matches the expected count
        Assert.Equal(32, cheeps.Count());
    }

    /// <summary>
    /// Unit test for the GetCheeps method in the CheepRepository, ensuring it returns an empty collection for a non-existing page.
    /// </summary>

    [Fact]
    public async void GetCheeps_FromNonExistingPage()
    {
        // Act: Retrieve cheeps using the GetCheeps method for a non-existing page
        var cheeps = await _cheepRepository.GetCheeps(999, "Newest");

        // Assert: Check if the retrieved cheeps collection is empty
        Assert.Empty(cheeps);
    }


    /// <summary>
    /// Tests the GetCheepsFromAuthor method for a non-existing page with an author name and email, and a specified page number and sorting order.
    /// </summary>
    /// <param name="name">The name of the author.</param>
    /// <param name="email">The email of the author.</param>

    [Theory]
    [InlineData("Helge", "ropf@itu.dk")]
    [InlineData("Rasmus", "rnie@itu.dk")]
    public async void GetCheepsFromAuthorExist_OutOfRange(string name, string email)
    {
        
        // Arrange: Create an author with the specified name and email
        var author = new Author
        {
            Name = name,
            Email = email
        };

        // Act: Retrieve cheeps using the GetCheepsFromAuthor method for a non-existing page
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(999, author.Name, "Newest");

        // Assert: Check if the retrieved cheeps collection is empty
        Assert.Empty(cheeps);
       
    }

    /// <summary>
    /// Tests the CheepTotal method, verifying that it returns the total count of all cheeps in the database.
    /// </summary>

    [Fact]
    public async void CheepsTotal_findsAllCheeps()
    {
        // Act: Retrieve the total count of cheeps using the CheepTotal method
        var cheeps = await _cheepRepository.CheepTotal();

        // Act: Retrieve the total count of cheeps from the database using LINQ
        var cheeps_Database = await _context.Cheeps
        .Where(c => c.CheepId != null)
        .ToListAsync();

        // Assert: Check if the count of retrieved cheeps matches the count in the database
        Assert.Equal(cheeps_Database.Count(), cheeps);
    }

    /// <summary>
    /// Tests the CheepTotal method, verifying that it returns the total count of all cheeps in the database.
    /// </summary>

    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public async Task GetCheep_GetsSpecifiedCheep(string name)
    {
        // Act: Retrieve the total count of cheeps using the CheepTotal method
        var cheep = await _context.Cheeps
        .Where(c => c.Author.Name == name)
        .FirstOrDefaultAsync();

        // Act: Retrieve the total count of cheeps from the database using LINQ
        var cheep_from_testmethod = await _cheepRepository.GetCheep(cheep.CheepId);

        // Assert: Check if the count of retrieved cheeps matches the count in the database
        Assert.Equal(cheep.CheepId, cheep_from_testmethod.Id);
    }

    /// <summary>
    /// Tests the RemoveAllCheepsFromAuthor method, verifying that it removes all cheeps associated with the specified author.
    /// </summary>

    [Fact]
    public async void RemoveAllCheepsFromAuthor()
    {
        // Arrange: Define the name of the author
        string name = "Jacqualine Gilcoine";

        // Act: Retrieve cheeps associated with the specified author from the database
        var cheeps = await _context.Cheeps
        .Where(c => c.Author.Name == name)
        .ToListAsync();

        // Assert: Check if there are cheeps associated with the specified author before removal
        Assert.NotEmpty(cheeps);

        // Act: Retrieve author information from the database
        var author = await _context.Authors
            .Where(c => c.Name == name)
            .Select(a =>
            new AuthorDTO
            {
                Name = a.Name
            }).FirstOrDefaultAsync();

        // Act: Remove all cheeps associated with the specified author using the RemoveAllCheepsFromAuthor method
        await _cheepRepository.RemoveAllCheepsFromAuthor(author);

         // Act: Retrieve cheeps associated with the specified author from the database after removal
        cheeps = await _context.Cheeps

        .Where(c => c.Author.Name == name)
        .ToListAsync();

        // Assert: Check if there are no cheeps associated with the specified author after removal
        Assert.Empty(cheeps);
    }
}