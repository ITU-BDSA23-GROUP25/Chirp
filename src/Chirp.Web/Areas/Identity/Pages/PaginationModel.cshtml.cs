using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Areas.Identity.Pages;

public class PaginationModel : PageModel
{
    public int CurrentPage { get; set; } = 1;
    public int amountOfPages { get; set; }
    public int CheepsOnEachPage { get; set; } = 32;

    public PaginationModel(int amountOfCheeps, int currentPage)
    {
        CurrentPage = currentPage;
        amountOfPages = (amountOfCheeps + CheepsOnEachPage - 1) / CheepsOnEachPage;
        if (amountOfCheeps % CheepsOnEachPage == 1)
        {
            amountOfPages++;
        }
    }
}