
namespace Repository;

public class AuthorRepository : IAuthorRepository
{

    private readonly DatabaseContext _databaseContext;
    private const int CheepsPerPage = 32;

    public AuthorRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        _databaseContext.InitializeDB();
    }

    public async Task CreateAuthor(String name)
    {

        var NameCheck = _databaseContext.Authors.Any(a => a.Name == name);

        if (NameCheck)
        {
            throw new ArgumentException($"Username {name} is already in use, please pick another username");
        }

        var author = new Author
        {
            Name = name,
            Cheeps = new List<Cheep>()
        };
        await _databaseContext.Authors.AddAsync(author);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task<AuthorDTO> GetAuthorByName(string author_name) =>
        await _databaseContext.Authors

        .Where(a => a.Name == author_name)
        .Select(a =>
            new AuthorDTO
            {
                Name = a.Name
            })
        .FirstOrDefaultAsync();
        

    public async Task RemoveAuthor(AuthorDTO authorDTO)
    {
        var authorToRemove = _databaseContext.Authors
            .FirstOrDefault(c => c.Name == authorDTO.Name);

        if (authorToRemove != null)
        {
            //Remove the author entity from the database
            _databaseContext.Authors.Remove(authorToRemove);

            // Save changes to persist the removal
            _databaseContext.SaveChangesAsync();
        }
    }
}
