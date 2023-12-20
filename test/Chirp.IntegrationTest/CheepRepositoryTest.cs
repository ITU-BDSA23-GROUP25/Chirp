namespace CheepRepositoryTest;

public class CheepRepositoryTest
{

    private readonly IAuthorRepository _authorRepository;

    private readonly ICheepRepository _cheepRepository;

    private readonly DatabaseContext _context;

    public CheepRepositoryTest()
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
    }

    [Fact]
    public async void CreateAndRemoveCheep()
    {
        var text = "Hej med dig";
        var name = "Sebb";

        await _authorRepository.CreateAuthor(name);
        var author = await _authorRepository.GetAuthorByName(name);

        _cheepRepository.CreateCheep(text, author.Name);

        var cheep = await _context.Cheeps
        .Include(a => a.Author)
        .Where(a => a.Author.Name == name)
        .Select(c => new CheepDTO(
            c.CheepId,
            c.Author.Name,
            c.Text,
            c.TimeStamp.AddHours(1).ToString("MM/dd/yy H:mm:ss")))
        .FirstOrDefaultAsync();

        Assert.Equal(cheep.Message, text);
        Assert.Equal(cheep.Author, name);

        _cheepRepository.RemoveCheep(cheep);

        cheep = await _context.Cheeps
        .Include(a => a.Author)
        .Where(a => a.Author.Name == name)
        .Select(c => new CheepDTO(
            c.CheepId,
            c.Author.Name,
            c.Text,
            c.TimeStamp.AddHours(1).ToString("MM/dd/yy H:mm:ss")))
        .FirstOrDefaultAsync();

        Assert.Null(cheep);
    }
   
}