using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace ChirpRepository_test;


public class CheepRepository_tests
{
    private readonly ICheepRepository _cheepRepository;

    private readonly IAuthorRepository _authorRepository;
    private readonly DatabaseContext _context;

    public CheepRepository_tests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;

        _context = new DatabaseContext(options);
        _context.Database.Migrate();
        _context.InitializeDB();

        _cheepRepository = new CheepRepository(_context);
        _authorRepository = new AuthorRepository(_context);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine")]
    public async void GetAllCheepsFromAuthor(string author)
    {
       var cheeps = await _cheepRepository.GetAllCheepsFromAuthor(author);

       var cheeps_Database = await _context.Cheeps.Where(c => c.Author.Name == author).ToListAsync();

       Assert.Equal(cheeps_Database.Count(), cheeps.Count());
    }

    [Theory]
    [InlineData(1)]
    public async void publictimeline_returns32Cheeps(int page)
    {
        var cheeps = await _cheepRepository.GetCheeps(page, "Newest");

        Assert.Equal(32, cheeps.Count());
    }

    [Theory]
    [InlineData(1, "Jacqualine Gilcoine")]
    public async void authortimeline_Returns32Cheeps(int page, string author)
    {
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(page, author ,"Newest");

        Assert.Equal(32, cheeps.Count());
    }

    [Fact]
    public async void GetCheeps_FromNonExistingPage()
    {
        var cheeps = await _cheepRepository.GetCheeps(999, "Newest");

        Assert.Empty(cheeps);
    }

    [Theory]
    [InlineData("Helge", "ropf@itu.dk")]
    [InlineData("Rasmus", "rnie@itu.dk")]
    public async void GetCheepsFromAuthorExist_OutOfRange(string name, string email)
    {
        var author = new Author
        {
            Name = name,
            Email = email
        };

        var cheeps = await _cheepRepository.GetCheepsFromAuthor(999 , author.Name, "Newest");

        Assert.Empty(cheeps);
    }

    [Fact]
    public async void CheepsTotal_findsAllCheeps()
    {
        var cheeps = await _cheepRepository.CheepTotal();

        var cheeps_Database = await _context.Cheeps.Where(c => c.CheepId != null).ToListAsync();
       
       Assert.Equal(cheeps_Database.Count(),cheeps);
    }

    [Theory]
    [InlineData("b4bf1c18-ce6d-4de3-bf92-020b82566b86")]
    [InlineData("b4008746-4a55-4ca1-ba80-01d440138f88")]
    [InlineData("04a3e0f3-075e-4fd3-895e-01bc80e5f9cb")]
    public async void GetCheep_GetsSpecifiedCheep(string cheepID)
    {
        var cheepID_guid = Guid.Parse(cheepID);
        //var cheep = await _cheepRepository.GetCheep(cheepID_guid);

        var cheep_database = await _context.Cheeps.Where(c => c.CheepId == cheepID_guid).FirstAsync();

        Assert.Equal("b4bf1c18-ce6d-4de3-bf92-020b82566b86", cheep_database.CheepId.ToString());
    }
}