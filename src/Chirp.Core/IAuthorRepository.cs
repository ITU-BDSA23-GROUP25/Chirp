namespace Core;
public interface IAuthorRepository
{
    // Get
    public Task<IEnumerable<AuthorDTO>> GetAuthorByName(string author_name);
    public Task<IEnumerable<AuthorDTO>> GetAuthorByEmail(string Email);

    // post
    public void CreateAuthor(string name, string Email);

    public Task RemoveAuthor(AuthorDTO authorDTO);

}