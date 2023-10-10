using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly CheepRepository _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(CheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet(int pagen, string author)
    {
        if(pagen == null){pagen = 0;}
        Cheeps = _service.GetCheepsFromAuthor(pagen, author);
        return Page();
    }
}
