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

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "Newest";

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

            Cheeps = _service.GetCheepsFromAuthor((int)page, username, SortOrder).Result.ToList();

            foreach (var item in Cheeps)
            {
                Console.WriteLine(item.Message);
            }

            var amountOfCheeps = _service.AuthorsCheepTotal(username).Result;
            PaginationModel = new PaginationModel(amountOfCheeps, (int)page);

            return Page();
        }

        public async Task<IActionResult> OnPostDelete(Guid cheepId)
        {
            // Perform cheep deletion logic here
            var cheepToRemove = await _service.GetCheep(cheepId);


            if (cheepToRemove != null)
            {
                _service.RemoveCheep(cheepToRemove);
            }

            // Redirect back to the public page after deletion
            return RedirectToPage("UserInfoPage");
        }

        public async Task<IActionResult> OnPostDeleteUser()
        {
            // Retrieve the username from the user's claims
            var username = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

            var cheepsToRemove = _service.GetAllCheepsFromAuthor(username).Result.ToList();

            foreach (CheepDTO cheepDTO in cheepsToRemove)
            {
                if (cheepDTO != null)
                {
                    _service.RemoveCheep(cheepDTO);
                }
            }

            return RedirectToPage("public");
        }
    }
}
