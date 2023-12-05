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
                Email =  Guid.NewGuid().ToString(),
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