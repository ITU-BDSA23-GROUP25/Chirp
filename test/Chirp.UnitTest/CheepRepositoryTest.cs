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


    //This is the Arrange part
    public CheepRepository_tests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;

        _context = new DatabaseContext(options);
        _context.Database.Migrate();

        _cheepRepository = new CheepRepository(_context);
        _authorRepository = new AuthorRepository(_context);
    }

    public async void Get_All_Cheeps()
    {}

    public async void GetCheeps_returns32Cheeps(int page)
    {}

    public async void GetCheeps_ReturnsEmpty()
    {}

    public async void CreateCheep()
    {}

    public GetCheepsFromUsertimeline_ReturnsCorrectPage(string name, string email, int page)
    {}

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

        var cheeps = await _cheepRepository.GetCheepsFromAuthor(999 , author.Name);

        Assert.Empty(cheeps);
    }

}