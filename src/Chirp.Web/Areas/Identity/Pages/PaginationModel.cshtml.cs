using Microsoft.AspNetCore.Mvc.RazorPages;

public class PaginationModel : PageModel
{
    public int CurrentPage { get; set; } = 1;
    public int AmountOfPages { get; set; }
    public int CheepsOnEachPage { get; set; } = 32;
    public string SortOrder { get; set; } = "Newest";

    public PaginationModel(int amountOfCheeps, int currentPage, string sortOrder)
    {
        CurrentPage = currentPage;
        SortOrder = sortOrder;

        // Calculate the number of pages
        AmountOfPages = (int)Math.Ceiling((double)amountOfCheeps / CheepsOnEachPage);
    }
}
