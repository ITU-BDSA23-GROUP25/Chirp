using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;
using System.Configuration;

namespace Chirp.Razor.Areas.Identity.Pages;
public class PublicModel : PageModel
{
    private readonly ICheepRepository _service;
    public List<CheepDTO> Cheeps { get; set; }
    public PaginationModel? PaginationModel { get; set; }

    public string Text {get; set;}


    public PublicModel(ICheepRepository service)
    {
        Cheeps = new List<CheepDTO>();
        _service = service;
    }

    public IActionResult OnGet([FromQuery] int? page)
    {
        if (!page.HasValue || page < 1)
        {
            page = 1; //if page is null or negative, set page to 1
        }
        Cheeps = _service.GetCheeps((int)page - 1).Result.ToList();

        var amountOfCheeps = _service.CheepTotal().Result;
        PaginationModel = new PaginationModel(amountOfCheeps, (int)page);

        return Page();
    }

   public IActionResult OnPost()
    {
        _service.CreateCheep(Request.Form["Text"], User.Identity?.Name!);
        return RedirectToPage("Public");
    }
}