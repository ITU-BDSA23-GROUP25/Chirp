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

    //This query was made to get all cheeps from an author for deletion
    public async Task<IEnumerable<CheepDTO>> GetAllCheepsFromAuthor(string author_name)
    {
        IQueryable<Cheep> query = _databaseContext.Cheeps;

        var cheeps = await query
            .Include(c => c.Author)
            .Where(c => c.Author.Name == author_name)
            .Select(c => new CheepDTO(c.CheepId, c.Author.Name, c.Text, c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
            .ToListAsync();

        return cheeps;
    }

    //This query was made to get all cheeps from an author for deletion
    public async Task RemoveAllCheepsFromAuthor(CheepDTO cheepDTO)
    {
        IQueryable<Cheep> query = _databaseContext.Cheeps;

        var cheepsToRemove = await query
            .Where(c => c.CheepId == cheepDTO.Id)
            .ExecuteDeleteAsync();
    }

    public void CreateCheep(string Message, string username)
    {
        // var ValidateCheep = new ValidateCheep();

        // var cheepValidationResult = ValidateCheep.Validate(new NewCheep {Text = Message});
        //if (!cheepValidationResult.IsValid)
        //{
        // throw new ValidationException(cheepValidationResult.Errors);
        //}

        var existingAuthor = _databaseContext.Authors.FirstOrDefault(a => a.Name == username);

        if (existingAuthor is null)
        {
            throw new ArgumentException($"No existing author with that name found: {username}");
        }

        var cheep = new Cheep
        {
            CheepId = Guid.NewGuid(),
            Text = Message,
            TimeStamp = DateTime.UtcNow,
            Author = existingAuthor,
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

    public async Task RemoveCheep(CheepDTO cheepDto)
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