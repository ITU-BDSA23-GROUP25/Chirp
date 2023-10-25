using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Core;

namespace Repository;

public class CheepRepository : ICheepRepository
{

    private readonly DatabaseContext _databaseContext;
    private const int CheepsPerPage = 32;

    public CheepRepository()
    {
        _databaseContext = new DatabaseContext();
        _databaseContext.InitializeDB();
    }


    public async Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber = 0)
        => await _databaseContext.Cheeps
        .Include(c => c.Author)
        .Skip(CheepsPerPage * pageNumber)
        .Take(CheepsPerPage)
        .Select(c => new CheepDTO(c.Author.Name, c.Text, c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
        .ToListAsync();

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string author_name) =>
        await _databaseContext.Cheeps

        .Include(c => c.Author)
        .Where(c => c.Author.Name == (_databaseContext.Authors.Where(a => a.Name == author_name)))
        .Skip(CheepsPerPage * (pageNumber - 1))
        .Take(CheepsPerPage)
        .Select(c =>
            new CheepDTO(c.Author.Name, c.Text, c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
        .ToListAsync();

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
            AuthorId = Guid.NewGuid(),
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>()
        };
        _databaseContext.Authors.Add(author);
        _databaseContext.SaveChanges();

    }

    public void CreateCheep(string Message, Guid UserId)
    {

        var author = _databaseContext.Authors.FirstOrDefault(a => a.AuthorId == UserId);

        var Cheep = new Cheep
        {
            CheepId = Guid.NewGuid(),
            Text = Message,
            TimeStamp = DateTime.Now,
            AuthorId = UserId,
            Author = author,
        };
        _databaseContext.Authors.Add(author);
        _databaseContext.SaveChanges();
    }

    public async Task<AuthorDTO> GetAuthorByName(string author_name) =>
        await _databaseContext.Authors

        .Where(a => a.Name == author_name)
        .Select(c =>
            new AuthorDTO(c.Name, c.Email, c.Cheeps));



    public async Task<AuthorDTO> GetAuthorByEmail(string author_Email) =>
        await _databaseContext.Authors

        .Where(a => a.Email == author_Email)
        .Select(c =>
            new AuthorDTO(c.Name, c.Email, c.Cheeps));
}
