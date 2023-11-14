using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;

namespace Chirp.Razor.Areas.Identity.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _service;

    public List<CheepDTO> Cheeps { get; set; }


    public PublicModel(ICheepRepository service)
    {
        Cheeps = new List<CheepDTO>();
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int? page)
    {
        if (page == null) { page = 0; }
        Cheeps = _service.GetCheeps((int)page).Result.ToList();
        return Page();
    }
}
