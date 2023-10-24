using Repository.DTO;

namespace Repository;

public interface ICheepRepository
{
    // Get
    public Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, Author author);
    public Author GetAuthorByName(String author_name);
    public Author GetAuthorByEmail(String Emial);

    // Post
    public void CreateAuthor(String name, String Email);
    public void CreateCheep(string message, Guid UserId );
}