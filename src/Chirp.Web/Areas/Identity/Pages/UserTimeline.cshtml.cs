using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;
using Microsoft.Data.SqlClient;

namespace Chirp.Razor.Areas.Identity.Pages;


public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;

    private readonly IAuthorRepository _authorRepo;

     private readonly IFollowerRepository _followerRepo;
    public List<CheepDTO> Cheeps { get; set; }

    public List<AuthorDTO> Authors { get; set; }

    public PaginationModel? PaginationModel { get; set; }

    public Dictionary<string, bool> FollowStatus { get; set; } = new Dictionary<string, bool>();

    public bool IsFollowing { get; set; } = false;

    public string SortOrder { get; set;} = "Newest";

    public UserTimelineModel(ICheepRepository service, IAuthorRepository authorRepo, IFollowerRepository followerRepo)
    {
        _service = service;
        _authorRepo = authorRepo;
        _followerRepo = followerRepo;
    }

    public async Task<ActionResult> OnGet([FromQuery] int? page, String author, String SortOrder)
    
    {
        if (!page.HasValue || page < 1)
            {
                page = 1; // Set a default page value if it is null or negative
            }
            
        if (page == null) { page = 0; }

        Cheeps = _service.GetCheepsFromAuthor((int)page, author, SortOrder).Result.ToList();

        Authors = _followerRepo.GetFollowerAuthor(author).Result.ToList();


        foreach (var authorDTO in Authors)
        {

            var authorCheeps = _service.GetAllCheepsFromAuthor(authorDTO.Name).Result.ToList();
            Cheeps.AddRange(authorCheeps);
        }

        Cheeps = _service.SortCheeps(Cheeps, SortOrder).Result.ToList();

        foreach (var cheep in Cheeps)
            {
              
                var userName = cheep.Author;
                var followersFromUser = await _followerRepo.GetFollowedAuthor(userName);
                var isFollowing = followersFromUser.Any(follower => follower.Name == User.Identity.Name);

                FollowStatus[userName] = isFollowing;
               
            }

        var amountOfCheeps = _service.AuthorsCheepTotal(author).Result;
        PaginationModel = new PaginationModel(amountOfCheeps, (int)page);

        return Page();
    }

    public async Task<IActionResult> OnPostFollow(string Username, string FollowerName)
    {
        await _followerRepo.AddOrRemoveFollower(FollowerName, Username);

        return RedirectToPage("UserTimeline");
    }
}