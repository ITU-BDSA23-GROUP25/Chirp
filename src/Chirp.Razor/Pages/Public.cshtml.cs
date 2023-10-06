using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly DBFacade _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(DBFacade service)
    {
        _service = service;
    }

    public ActionResult OnGet(int pagen)
    {   
        if(pagen == null){pagen = 0;}
        Cheeps = _service.GetCheeps(pagen);
        return Page();
    }
}
