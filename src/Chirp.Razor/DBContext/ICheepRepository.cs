using Repository.DTO;

namespace Repository;

public interface ICheepRepository
{
    public Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, Author author);
    public Author GetAuthorByName(String author_name);
    public Author GetAuthorByEmail(String Emial);
}