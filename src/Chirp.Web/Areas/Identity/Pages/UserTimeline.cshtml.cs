using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;

namespace Chirp.Razor.Areas.Identity.Pages;


public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;
    public List<CheepDTO> Cheeps { get; set; }

    public PaginationModel? PaginationModel { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string SortOrder { get; set; } = "Newest";

    public UserTimelineModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int? page, String author)
    {

        if (!page.HasValue || page < 1)
        {
            page = 1; //if page is null or negative, set page to 1
        }
        Cheeps = _service.GetCheepsFromAuthor((int)page, author, SortOrder).Result.ToList();
   

        foreach (var item in Cheeps)
        {
            Console.WriteLine(item.Message);
        }

        var amountOfCheeps = _service.AuthorsCheepTotal(author).Result;
        PaginationModel = new PaginationModel(amountOfCheeps, (int)page);

        return Page();
    }
}
