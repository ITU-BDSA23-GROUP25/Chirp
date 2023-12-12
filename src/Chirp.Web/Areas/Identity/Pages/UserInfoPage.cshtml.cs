using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;
using System.Linq;

namespace Chirp.Razor.Areas.Identity.Pages
{
    public class UserInfoModel : PageModel
    {
        private readonly ICheepRepository _service;

        public List<CheepDTO> Cheeps { get; set; }
        public PaginationModel? PaginationModel { get; set; }

        public UserInfoModel(ICheepRepository service)
        {
            _service = service;
        }

        public ActionResult OnGet([FromQuery] int? page)
        {
            // Retrieve the username from the user's claims
            var username = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

            if (!page.HasValue || page < 1)
            {
                page = 1; //if page is null or negative, set page to 1
            }

            Cheeps = _service.GetCheepsFromAuthor((int)page, username).Result.ToList();

            foreach (var item in Cheeps)
            {
                Console.WriteLine(item.Message);
            }

            var amountOfCheeps = _service.AuthorsCheepTotal(username).Result;
            PaginationModel = new PaginationModel(amountOfCheeps, (int)page);

            return Page();
        }
    }
}
