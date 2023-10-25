namespace Core;

public interface ICheepRepository
{
    // Get
    public Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string author);
    public Task<AuthorDTO> GetAuthorByName(string author_name);
    public Task<AuthorDTO> GetAuthorByEmail(string Email);

    // Post
    public void CreateAuthor(string name, string Email);
    public void CreateCheep(string message, Guid UserId);
}