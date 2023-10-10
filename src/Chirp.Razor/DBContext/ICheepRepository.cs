namespace CheepRepository;

public interface ICheepRepository<TDTO, T>
{
    public Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string author);
}