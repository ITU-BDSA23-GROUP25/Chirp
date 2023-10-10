namespace CheepRepository;

public class CheepRepository : iCheepRepository
{
    private CheepService cs = new();
    private List<CheepViewModel> cheeps = new();

    public List<CheepViewModel> GetCheeps(int page)
    {
        cheeps = cs.GetCheeps(page);
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(int page, string author)
    {
        cheeps = cs.GetCheepsFromAuthor(page, author);
        return cheeps;
    }
}