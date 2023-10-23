using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.DTO;
using Repository;

namespace Chirp.Razor.Pages;


public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;
    public List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet([FromQuery]int pageNumber, String author)
    {
        if(pageNumber == null){pageNumber = 0;}
        Author authorTemp = _service.GetAuthorByName(author);
        Cheeps = Cheeps = _service.GetCheepsFromAuthor(pageNumber, authorTemp).Result.ToList();
        return Page();
    }
}
