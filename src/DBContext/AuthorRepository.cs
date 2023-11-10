
namespace Repository;

public class AuthorRepository : IAuthorRepository
{

    private readonly DatabaseContext _databaseContext;
    private const int CheepsPerPage = 32;

    public AuthorRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        //_databaseContext = new DatabaseContext();
        //_databaseContext.InitializeDB();
    }

    public void CreateAuthor(string Name, string Email)
    {

        var NameCheck = _databaseContext.Authors.Any(a => a.Name == Name);
        var EmailCheck = _databaseContext.Authors.Any(a => a.Email == Email);

        if (NameCheck)
        {
            throw new ArgumentException($"Username {Name} is already in use, please pick another username");
        }

        if (EmailCheck)
        {
            throw new ArgumentException($"{Email} is already in use, please pick another email address");
        }

        var author = new Author
        {
            Name = Name,
            Email = Email,
            Cheeps = new List<Cheep>()
        };
        _databaseContext.Authors.Add(author);
        _databaseContext.SaveChanges();
    }

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByName(string author_Name) =>
        await _databaseContext.Authors

        .Where(a => a.Name == author_Name)
        .Select(a =>
            new AuthorDTO(a.Name))
        .ToListAsync();


    public async Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string author_Email) =>
        await _databaseContext.Authors

        .Where(a => a.Email == author_Email)
        .Select(c =>
            new AuthorDTO(c.Email))
        .ToListAsync();
}
