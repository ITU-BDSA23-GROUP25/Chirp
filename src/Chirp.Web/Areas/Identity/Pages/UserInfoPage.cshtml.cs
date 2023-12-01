using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;
using System.Configuration;

namespace Chirp.Razor.Areas.Identity.Pages;
public class userInfoModel : PageModel
{
    AuthorDTO User;
    IAuthorRepository _service;

    public userInfoModel(IAuthorRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {

        return Page();
    }
}