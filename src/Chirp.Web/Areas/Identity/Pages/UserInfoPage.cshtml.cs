using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;
using System.Configuration;

namespace Chirp.Razor.Areas.Identity.Pages;
public class userInfoModel : PageModel
{
    public List<CheepDTO> Cheeps { get; set; }
    ICheepRepository _service;

    public PaginationModel? PaginationModel { get; set; }

    public userInfoModel(ICheepRepository service)
    {
        Cheeps = new List<CheepDTO>();
        _service = service;
    }

    public ActionResult OnGet(int? page, string username)
    {
        if (!page.HasValue || page < 1)
        {
            page = 1; //if page is null or negative, set page to 1
        }
        Cheeps = _service.GetCheepsFromAuthor((int)page, username).Result.ToList();

        return Page();
    }
}