namespace AuthorRepositoryTest;

public class AuthorRepositoryTest
{

    private readonly IAuthorRepository _authorRepository;

    private readonly ICheepRepository _cheepRepository;
    private readonly DatabaseContext _context;

    public AuthorRepositoryTest()
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
    public async void CreateAndDeleteAuthor()
    {
        var name = "Sebb";

        var author = _authorRepository.CreateAuthor(name);

        var author_database = await _context.Authors.Where(c => c.Name == name).FirstOrDefaultAsync();

        Assert.Equal(author_database.Name, name);

        var author_to_remove = await _context.Authors
            .Where(c => c.Name == name)
            .Select(a =>
            new AuthorDTO
            {
                Name = a.Name
            }).FirstOrDefaultAsync();

        await _authorRepository.RemoveAuthor(author_to_remove);

        author_database = await _context.Authors.Where(c => c.Name == name).FirstOrDefaultAsync();

        Assert.Null(author_database);
    }

}