using System.Linq;
using FluentValidation;

namespace Repository;

public class CheepRepository : ICheepRepository
{

    private readonly DatabaseContext _databaseContext;
    private const int CheepsPerPage = 32;

    public CheepRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        _databaseContext.InitializeDB();
    }

   public async Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber = 0, string sortOrder = "Newest")
{
    IQueryable<Cheep> query = _databaseContext.Cheeps;

    switch (sortOrder)
    {
        case "Oldest":
            query = query.OrderBy(c => c.TimeStamp);
            break;
        case "Newest":
        default:
            query = query.OrderByDescending(c => c.TimeStamp);
            break;
    }

    var cheeps = await query
        .Include(c => c.Author)
        .Skip(CheepsPerPage * pageNumber)
        .Take(CheepsPerPage)
        .Select(c => new CheepDTO(c.CheepId, c.Author.Name, c.Text, c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
        .ToListAsync();

    return cheeps;
}



    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string author_name, string sortOrder)
{
    IQueryable<Cheep> query = _databaseContext.Cheeps;

    switch (sortOrder)
    {
        case "Oldest":
            query = query.OrderBy(c => c.TimeStamp);
            break;
        case "Newest":
        default:
            query = query.OrderByDescending(c => c.TimeStamp);
            break;
    }

    var cheeps = await query
        .Include(c => c.Author)
        .Where(c => c.Author.Name == author_name)
        .Skip(CheepsPerPage * (pageNumber - 1))
        .Take(CheepsPerPage)
        .Select(c => new CheepDTO(c.CheepId, c.Author.Name, c.Text, c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
        .ToListAsync();

    return cheeps;
}

    public void CreateCheep(string Message, string username)
    {
        // var ValidateCheep = new ValidateCheep();

        // var cheepValidationResult = ValidateCheep.Validate(new NewCheep {Text = Message});
        //if (!cheepValidationResult.IsValid)
        //{
        // throw new ValidationException(cheepValidationResult.Errors);
        //}

        Author author;

        if (!_databaseContext.Authors.Any(a => a.Name == username))
        {

            author = new Author
            {
                Name = username,
                Email = Guid.NewGuid().ToString(),
            };
            _databaseContext.Authors.Add(author);
        }
        else
        {
            author = _databaseContext.Authors.SingleAsync(a => a.Name == username).Result;
        }

        var cheep = new Cheep
        {
            CheepId = Guid.NewGuid(),
            Text = Message,
            TimeStamp = DateTime.UtcNow,
            Author = author,
        };
        _databaseContext.Cheeps.Add(cheep);
        _databaseContext.SaveChanges();

    }

    public async Task<int> CheepTotal() =>
        await _databaseContext.Cheeps
        .CountAsync();

    public async Task<int> AuthorsCheepTotal(string author_name) =>
        await _databaseContext.Cheeps

        .Include(c => c.Author)
        .Where(c => c.Author.Name == author_name)
        .CountAsync();

    public async Task<CheepDTO> GetCheep(Guid cheepId) =>
        await _databaseContext.Cheeps
        
        .Include(c => c.Author)
        .Where(c => c.CheepId == cheepId)
        .Select(c => new CheepDTO(c.CheepId, c.Author.Name, c.Text, c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
        .FirstOrDefaultAsync();

public void RemoveCheep(CheepDTO cheepDto)
{
    // Find the corresponding Cheep entity
    var cheepToRemove = _databaseContext.Cheeps
        .FirstOrDefault(c => c.CheepId == cheepDto.Id);

    if (cheepToRemove != null)
    {
        // Remove the Cheep entity from the database
        _databaseContext.Cheeps.Remove(cheepToRemove);

        // Save changes to persist the removal
        _databaseContext.SaveChangesAsync();
    }
}
}

public class NewCheep
{
    public required string Text { get; set; }
}

public class ValidateCheep : AbstractValidator<NewCheep>
{
    public ValidateCheep()
    {
        RuleFor(c => c.Text).NotEmpty().MaximumLength(160);
    }
}