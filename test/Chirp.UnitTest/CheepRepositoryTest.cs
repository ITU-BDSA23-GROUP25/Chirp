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

  /*   public async void Get_All_Cheeps()
    {} */

    [Theory]
    [InlineData(1)]
    public async void publictimeline_returns32Cheeps(int page)
    {
        var cheeps = await _cheepRepository.GetCheeps(page, "Newest");

        Assert.Equal(32, cheeps.Count());
    }

    [Theory]
    [InlineData(1, "Jacqualine Gilcoine")]
    public async void authortimeline_returns32Cheeps(int page, string author)
    {
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(page, author ,"Newest");

        Assert.Equal(32, cheeps.Count());
    }


   /*  public async void GetCheeps_ReturnsEmpty()
    {}

    public async void CreateCheep()
    {} */

    /* public GetCheepsFromUsertimeline_ReturnsCorrectPage(string name, string email, int page)
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
    } */

}