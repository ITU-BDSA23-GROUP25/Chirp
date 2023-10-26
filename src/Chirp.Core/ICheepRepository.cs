namespace Core;

public interface ICheepRepository
{
    // Get
    public Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string author);
    
    // Post
    public void CreateCheep(string message, Guid UserId);
}