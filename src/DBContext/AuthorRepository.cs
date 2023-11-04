
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

    public void CreateAuthor(string email)
    {

        var NameCheck = _databaseContext.Authors.Any(a => a.Email == email);
        var EmailCheck = _databaseContext.Authors.Any(a => a.Email == email);

        if (NameCheck)
        {
            throw new ArgumentException($"Username {email} is already in use, please pick another username");
        }

        if (EmailCheck)
        {
            throw new ArgumentException($"{email} is already in use, please pick another email address");
        }

        var author = new Author
        {
            Name = email,
            Email = email,
            Cheeps = new List<Cheep>()
        };
        _databaseContext.Authors.Add(author);
        _databaseContext.SaveChanges();
    }

    public async Task<IEnumerable<AuthorDTO>> GetAuthorByName(string author_name) =>
        await _databaseContext.Authors

        .Where(a => a.Email == author_name)
        .Select(a =>
            new AuthorDTO(a.Email))
        .ToListAsync();


    public async Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string author_Email) =>
        await _databaseContext.Authors

        .Where(a => a.Email == author_Email)
        .Select(c =>
            new AuthorDTO(c.Email))
        .ToListAsync();
}
