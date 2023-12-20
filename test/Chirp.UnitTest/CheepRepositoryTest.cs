using System.Diagnostics;
using Microsoft.Identity.Client;

namespace ChirpRepository_test;


public class CheepRepository_tests
{
    private readonly ICheepRepository _cheepRepository;
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
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(page, author, "Newest");

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

        var cheeps = await _cheepRepository.GetCheepsFromAuthor(999, author.Name, "Newest");

        Assert.Empty(cheeps);
    }

    [Fact]
    public async void CheepsTotal_findsAllCheeps()
    {
        var cheeps = await _cheepRepository.CheepTotal();

        var cheeps_Database = await _context.Cheeps
        .Where(c => c.CheepId != null)
        .ToListAsync();

        Assert.Equal(cheeps_Database.Count(), cheeps);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public async Task GetCheep_GetsSpecifiedCheep(string name)
    {
        var cheep = await _context.Cheeps
        .Where(c => c.Author.Name == name)
        .FirstOrDefaultAsync();

        var cheep_from_testmethod = await _cheepRepository.GetCheep(cheep.CheepId);

        Assert.Equal(cheep.CheepId, cheep_from_testmethod.Id);
    }

    [Fact]
    public async void RemoveAllCheepsFromAuthor()
    {
        string name = "Jacqualine Gilcoine";

        var cheeps = await _context.Cheeps
        .Where(c => c.Author.Name == name)
        .ToListAsync();

        Assert.NotEmpty(cheeps);

        var author = await _context.Authors
            .Where(c => c.Name == name)
            .Select(a =>
            new AuthorDTO
            {
                Name = a.Name
            }).FirstOrDefaultAsync();

        await _cheepRepository.RemoveAllCheepsFromAuthor(author);

        cheeps = await _context.Cheeps
        .Where(c => c.Author.Name == name)
        .ToListAsync();

        Assert.Empty(cheeps);
    }
}