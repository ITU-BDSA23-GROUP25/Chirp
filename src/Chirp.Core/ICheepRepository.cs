namespace Core;

public interface ICheepRepository
{
    // Get
    public Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string author);
    public Task<int> cheepTotal();

    // Post
    public void CreateCheep(string message, string UserId);
}