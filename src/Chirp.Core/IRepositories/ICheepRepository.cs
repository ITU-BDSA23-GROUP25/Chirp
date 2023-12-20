namespace Core;

/// <summary>
/// This is the interface for the CheepRepository, where the methodsignitures are displayed
/// </summary>
public interface ICheepRepository
{
    // Get

    /// <summary>
    /// This method returns an IEnumerable of CheepDTO's corresponding to a pagenumber and sortorder    
    /// </summary>
    /// <param name="pageNumber">The page number</param>
    /// <param name="sortOrder">the sort order, currently Newest or Oldest</param>
    /// <returns></returns>
    public Task<IEnumerable<CheepDTO>> GetCheeps(int pageNumber, string sortOrder);

    /// <summary>
    /// This method returns an IEnumerable of CheepDTO's corresponding 
    /// to a specific author on a given page in a specific sort order
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="author"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string author, string sortOrder);

    /// <summary>
    /// This method returns an IEnumerable of CheepDTO's corresponding
    /// to a specific author
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public Task<IEnumerable<CheepDTO>> GetAllCheepsFromAuthor(string author);

    /// <summary>
    /// This method returns an integer corresponding
    /// to the total count of cheeps in the database
    /// </summary>
    /// <returns></returns>
    public Task<int> CheepTotal();

    /// <summary>
    /// This method returns a CheepDTO corresponding
    /// to its cheepId
    /// </summary>
    /// <param name="cheepId">A unique guid given to each cheep at creation</param>
    /// <returns></returns>
    public Task<CheepDTO> GetCheep(Guid cheepId);

    /// <summary>
    /// This method returns an integer corresponding
    /// to the total count of cheeps posted by an author
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public Task<int> AuthorsCheepTotal(string author);

    /// <summary>
    /// This method returns an sorted IEnumerable of CheepDTO's corresponding
    /// to all cheeps in the database
    /// </summary>
    /// <param name="cheeps"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    public Task<IEnumerable<CheepDTO>> SortCheeps(List<CheepDTO> cheeps, string sortOrder);



    // Post

    /// <summary>
    /// This method creates a cheep in Cheeps table
    /// with the message, authors name and a new guid as Id
    /// </summary>
    /// <param name="message"></param>
    /// <param name="author">the name of the author</param>
    public void CreateCheep(string message, string author);

    /// <summary>
    /// This method removes the cheep in the Cheeps table
    /// when given the matching CheepDTO 
    /// </summary>
    /// <param name="cheepDto"></param>
    public void RemoveCheep(CheepDTO cheepDto);

    /// <summary>
    /// This method removes all cheeps from a specific author
    /// in the database, when given the matching AuthorDTO
    /// </summary>
    /// <param name="authorDTO"></param>
    /// <returns></returns>
    public Task RemoveAllCheepsFromAuthor(AuthorDTO authorDTO);

}