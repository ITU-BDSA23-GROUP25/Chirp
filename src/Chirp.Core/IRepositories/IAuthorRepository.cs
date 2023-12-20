namespace Core;

/// <summary>
/// This is the interface for the AuthorRepository, where the methodsignitures are displayed
/// </summary>
public interface IAuthorRepository
{
    // Get

    /// <summary>
    /// This method returns the AuthorDTO when given the authors name (string)
    /// </summary>
    /// <param name="name">The authors name in form of a string</param>
    /// <returns></returns>
    public Task<AuthorDTO> GetAuthorByName(string name);

    // post

    /// <summary>
    /// This method creates an author in the AspNetUsers table
    /// only containing its name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task CreateAuthor(string name);

    /// <summary>
    /// This method removes the author from the AspNetUsers table
    /// </summary>
    /// <param name="authorDTO"></param>
    /// <returns></returns>
    public Task RemoveAuthor(AuthorDTO authorDTO);

}