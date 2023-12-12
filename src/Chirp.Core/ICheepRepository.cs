namespace Core;

public interface ICheepRepository
{
    // Get
    public Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string author);
    public Task<int> CheepTotal();
    public Task<CheepDTO> GetCheep(Guid cheepId);
    public Task<int> AuthorsCheepTotal(string author);


    // Post
    public void CreateCheep(string message, string Username);
    public void RemoveCheep(CheepDTO cheepDto);

}