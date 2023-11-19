namespace ChirpRepository_test;


public class CheepRepository_tests
{
    private readonly ICheepRepository _Cheeprepository;
    private readonly DatabaseContext _context;
    
    public CheepRepository_tests()
    {   
        connection.Open();
        _context = new DatabaseContext();
        _context.InitializeDB();

        _Cheeprepository = new CheepRepository();
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async void GetCheeps_returns32Cheeps(int page)
    {
        var cheeps = await _Cheeprepository.GetCheeps(page);

        Assert.Equal(32, cheeps.Count());
    }

    [Fact]
    public async void GetCheeps_ReturnsEmpty()
    {
        var cheeps = await _Cheeprepository.GetCheeps(999);

        Assert.Empty(cheeps);
    }   

    [Theory]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 1)]
    [InlineData("Jacqualine Gilcoine", "Jacqualine.Gilcoine@gmail.com", 2)]

    public async void GetCheepsFromAuthor_givenAuthorAndPage_returns32Cheeps(string name, string email, int page)
    {
        var author = new Author
        {
            Name = name,
            Email = email
        };

        var cheeps = await _Cheeprepository.GetCheepsFromAuthor(page, author.Name);

        Assert.Equal(32, cheeps.Count());
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

        var cheeps = await _Cheeprepository.GetCheepsFromAuthor(999 , author.Name);

        Assert.Empty(cheeps);
    }

}