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
    
   
    public Author GetAuthorByName(String author_name){
        var author = _databaseContext.Authors.Where(a => a.Name == author_name).Single();
        return author;
    }

      public Author GetAuthorByEmail(String Email){
        var author = _databaseContext.Authors.Where(a => a.Email == Email).Single();
        return author;
    }
    
    
}
