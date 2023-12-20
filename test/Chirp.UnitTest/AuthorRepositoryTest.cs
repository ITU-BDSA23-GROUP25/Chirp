using System.IO.Pipes;
using Microsoft.Identity.Client;

namespace Chirp.UnitTest;

public class AuthorRepositoryTest
{

    private readonly IAuthorRepository _authorRepository;
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
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public async void GetAuthorByName_returnsAuthor(string author_name){

        var author = await _authorRepository.GetAuthorByName(author_name);

        Assert.Equal(author.Name, author_name);
    }
    [Theory]
    [InlineData("Thorvald")]
    [InlineData("ABCD")]
      public async void GetAuthorByName_returnsEmpty(string author_name){

        var author = await _authorRepository.GetAuthorByName(author_name);

        Assert.Null(author);
    }
}