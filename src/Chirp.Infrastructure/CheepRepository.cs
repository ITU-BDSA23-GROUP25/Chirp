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
        .Where(c => c.Author.Name == author_name)
        .Skip(CheepsPerPage * (pageNumber - 1))
        .Take(CheepsPerPage)
        .Select(c =>
            new CheepDTO(c.Author.Name, c.Text, c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
        .ToListAsync();

    public void CreateCheep(string Message, string UserId)
    {

        var author = _databaseContext.Authors.FirstOrDefault(a => a.Id == UserId);

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

    public async Task<int> cheepTotal()
    {
        var task = _databaseContext.Cheeps.CountAsync();
        int result = await task;
        return result;
    }
}
