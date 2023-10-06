using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.Sqlite;

//TODO ask TA if this is correct
public class DBFacade {

    private CheepService cs = new();
    private List<CheepViewModel> cheeps = new();

    public List<CheepViewModel> GetCheeps(int page)
    {
        cheeps = cs.GetCheeps(page);
        return cheeps;
    }

    public List<CheepViewModel> GetCheeps(int page, string author)
    {
        cheeps = cs.GetCheepsFromAuthor(page, author);
        return cheeps;
    }
}