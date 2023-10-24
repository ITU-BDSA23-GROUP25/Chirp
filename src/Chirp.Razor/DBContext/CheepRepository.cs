using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Repository.DTO;

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


    public  async Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber = 0) 
        => await _databaseContext.Cheeps
        .Include(c => c.Author)
        .Skip(CheepsPerPage * pageNumber)
        .Take(CheepsPerPage)
        .Select(c => new CheepDTO(c.Author.Name, c.Text, c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
        .ToListAsync();    

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, Author author_name) =>
        await _databaseContext.Cheeps

        .Include( c => c.Author)
        .Where(c => c.Author.Name == author_name.Name)
        .Skip(CheepsPerPage * (pageNumber - 1))
        .Take(CheepsPerPage)
        .Select(c =>
            new CheepDTO(c.Author.Name, c.Text, c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
        .ToListAsync();

    public void CreateAuthor(String name, String email){

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

    public void CreateCheep(string Message, Guid UserId ){

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
        
    public Author GetAuthorByName(String author_name){
        var author = _databaseContext.Authors.Where(a => a.Name == author_name).Single();
        return author;
    }

    public Author GetAuthorByEmail(String Email){
        var author = _databaseContext.Authors.Where(a => a.Email == Email).Single();
        return author;
        } 
}
