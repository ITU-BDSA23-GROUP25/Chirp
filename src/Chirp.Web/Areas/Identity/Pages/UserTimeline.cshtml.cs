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

    public ActionResult OnGet([FromQuery] int? pageNumber, String author)
    {
        if (pageNumber == null) { pageNumber = 0; }
        Cheeps = Cheeps = _service.GetCheepsFromAuthor((int)pageNumber, author).Result.ToList();
        return Page();
    }
}
