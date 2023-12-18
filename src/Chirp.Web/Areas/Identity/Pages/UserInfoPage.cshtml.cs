using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;
using System.Linq;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Razor.Areas.Identity.Pages
{
    public class UserInfoModel : PageModel
    {
        private readonly ICheepRepository _service;
        private readonly IAuthorRepository _authorRepo;

        private readonly IFollowerRepository _followerRepo;

        public List<CheepDTO> Cheeps { get; set; }
        public PaginationModel? PaginationModel { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "Newest";

        public UserInfoModel(ICheepRepository service, IAuthorRepository authorRepo, IFollowerRepository followerRepo)
        {
            _service = service;
            _authorRepo = authorRepo;
            _followerRepo = followerRepo;
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

            Console.WriteLine($"USERNAME: {username}");
            
            await _followerRepo.RemoveFollowers(username);

            //Removes all cheeps of the user
            //var findfirstCheepFromUser = _service.GetAllCheepsFromAuthor(username).Result.FirstOrDefault();
            var userDTO = _authorRepo.GetAuthorByName(username).Result;
            if(userDTO != null){
                await _service.RemoveAllCheepsFromAuthor(userDTO);
                await _authorRepo.RemoveAuthor(userDTO);
            }
    
            /* //Removes the author of the user
            var userToRemove = await _authorRepo.GetAuthorByName(username);
            await _authorRepo.RemoveAuthor(userToRemove); */

            //Signes the user out of the website
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToPage("Account/Login");
        }
    }
}
