namespace Core;
public interface IAuthorRepository
{
    // Get
    public Task<AuthorDTO> GetAuthorByName(string author_name);

    // post
    public Task CreateAuthor(string name);

    public Task RemoveAuthor(AuthorDTO authorDTO);

}