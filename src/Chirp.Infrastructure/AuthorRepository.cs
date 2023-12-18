
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

    public void CreateAuthor(String name, String email)
    {

        var NameCheck = _databaseContext.Authors.Any(a => a.Name == name);
        var EmailCheck = _databaseContext.Authors.Any(a => a.Email == email);

        if (NameCheck)
        {
            throw new ArgumentException($"Username {name} is already in use, please pick another username");
        }

        if (EmailCheck)
        {
            throw new ArgumentException($"{email} is already in use, please pick another email address");
        }

        var author = new Author
        {
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>()
        };
        _databaseContext.Authors.Add(author);
        _databaseContext.SaveChanges();
    }

    public async Task<AuthorDTO> GetAuthorByName(string author_name) =>
        await _databaseContext.Authors

       .Where(a => a.Name == author_name)
        .Select(a =>
            new AuthorDTO(){
            Name = a.Name,
            Email = a.Email
        })
        .ToListAsync();


    public async Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string author_Email) =>
        await _databaseContext.Authors

        .Where(a => a.Email == author_Email)
        .Select(c =>
            new AuthorDTO(){
            Name = c.Name,
            Email = c.Email})
        .ToListAsync();

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
