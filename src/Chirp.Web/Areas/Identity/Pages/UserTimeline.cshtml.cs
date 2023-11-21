using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;

namespace Chirp.Razor.Areas.Identity.Pages;


public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;
    public List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int? page, String author)
    {

        if (!page.HasValue || page < 1)
            {
                page = 1; // Set a default page value if it is null or negative
            }
            
        if (page == null) { page = 0; }
        Cheeps = Cheeps = _service.GetCheepsFromAuthor((int)page, author).Result.ToList();
        return Page();
    }
}
