using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;
using Microsoft.Data.SqlClient;

namespace Chirp.Razor.Areas.Identity.Pages;


public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;

    private readonly IAuthorRepository _authorRepo;
    public List<CheepDTO> Cheeps { get; set; }

    public PaginationModel? PaginationModel { get; set; }

    public string SortOrder { get; set;} = "Newest";

    public UserTimelineModel(ICheepRepository service, IAuthorRepository authorRepo)
    {
        _service = service;
        _authorRepo = authorRepo;
    }

    public ActionResult OnGet([FromQuery] int? page, String author)
    {
        if (!page.HasValue || page < 1)
            {
                page = 1; // Set a default page value if it is null or negative
            }
            
        if (page == null) { page = 0; }
        Cheeps = _service.GetCheepsFromAuthor((int)page, author, SortOrder).Result.ToList();
   

        foreach (var item in Cheeps)
        {
            Console.WriteLine(item.Message);
        }

        var amountOfCheeps = _service.AuthorsCheepTotal(author).Result;
        PaginationModel = new PaginationModel(amountOfCheeps, (int)page);

        return Page();
    }
}