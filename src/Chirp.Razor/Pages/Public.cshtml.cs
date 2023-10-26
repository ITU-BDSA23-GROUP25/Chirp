using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _service;

    public List<CheepDTO> Cheeps { get; set; }


    public PublicModel(ICheepRepository service)
    {
        Cheeps = new List<CheepDTO>();
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int? pageNumber)
    {
        if (pageNumber == null) { pageNumber = 0; }
        Cheeps = _service.GetCheeps((int)pageNumber).Result.ToList();
        return Page();
    }
}
