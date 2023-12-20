namespace Core;

public interface ICheepRepository
{
    // Get
    public Task<IEnumerable<CheepDTO>> GetCheeps(int page, string sortOrder);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string author, string sortOrder);
    public Task<IEnumerable<CheepDTO>> GetAllCheepsFromAuthor(string author);
    public Task<int> CheepTotal();
    public Task<CheepDTO> GetCheep(Guid cheepId);
    public Task<int> AuthorsCheepTotal(string author);
    public Task<IEnumerable<CheepDTO>> SortCheeps(List<CheepDTO> cheeps, string sortOrder);



    // Post
    public void CreateCheep(string message, string Username);
    public void RemoveCheep(CheepDTO cheepDto);
    public Task RemoveAllCheepsFromAuthor(AuthorDTO authorDTO);

}