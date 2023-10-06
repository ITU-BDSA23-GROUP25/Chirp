using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly DBFacade _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(DBFacade service)
    {
        _service = service;
    }

    public ActionResult OnGet(int pagen, string author)
    {
        if(pagen == null){pagen = 0;}
        Cheeps = _service.GetCheeps(pagen, author);
        return Page();
    }
}
