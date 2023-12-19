using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;
using Microsoft.Data.SqlClient;

namespace Chirp.Razor.Areas.Identity.Pages;


public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;

    private readonly IAuthorRepository _authorRepo;

    private readonly IReactionRepository _reactions;

    private readonly IFollowerRepository _followerRepo;
    public List<CheepDTO> Cheeps { get; set; }

    public List<AuthorDTO> Authors { get; set; }

    public PaginationModel? PaginationModel { get; set; }

    public Dictionary<string, bool> FollowStatus { get; set; } = new Dictionary<string, bool>();

    public bool IsFollowing { get; set; } = false;

    public string SortOrder { get; set; } = "Newest";

    public UserTimelineModel(ICheepRepository service, IAuthorRepository authorRepo, IFollowerRepository followerRepo, IReactionRepository reactions)
    {
        _service = service;
        _authorRepo = authorRepo;
        _reactions = reactions;
        _followerRepo = followerRepo;
    }

    public async Task<ActionResult> OnGet([FromQuery] int? page, string author, string SortOrder)
    {   

        var amountOfCheeps = 0;
        // Retrieve the username from the user's claims
        var username = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

        if (!page.HasValue || page < 1)
        {
            page = 1; //if page is null or negative, set page to 1
        }

        Cheeps = _service.GetCheepsFromAuthor((int)page, author, SortOrder).Result.ToList();

        amountOfCheeps += _service.AuthorsCheepTotal(author).Result;

        if(username == author) {
             Authors = _followerRepo.GetFollowerAuthor(author).Result.ToList();

        foreach (var authorDTO in Authors)
        {
            var authorCheeps = _service.GetCheepsFromAuthor((int)page, authorDTO.Name, SortOrder).Result.ToList();
            Cheeps.AddRange(authorCheeps);
            amountOfCheeps += _service.AuthorsCheepTotal(authorDTO.Name).Result;
        }
        }

        Cheeps = _service.SortCheeps(Cheeps, SortOrder).Result.ToList();

        PaginationModel = new PaginationModel(amountOfCheeps, (int)page, SortOrder);


        foreach (var cheep in Cheeps)
        {

            var userName = cheep.Author;
            var followersFromUser = await _followerRepo.GetFollowedAuthor(userName);
            var isFollowing = followersFromUser.Any(follower => follower.Name == User.Identity.Name);

            FollowStatus[userName] = isFollowing;

        }

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
            return RedirectToPage("UserTimeline");
        }

    public async Task<IActionResult> OnPostFollow(string Username, string FollowerName)
    {
        await _followerRepo.AddOrRemoveFollower(FollowerName, Username);

        return RedirectToPage("UserTimeline");
    }

    public async Task<bool> HasUserReacted(Guid cheepId, string authorName, ReactionType reactionType)
    {
        return await _reactions.HasUserReacted(cheepId, authorName, reactionType);
    }

    public async Task<int> GetLikeCount(Guid cheepId, ReactionType reactionType)
    {
        return await _reactions.GetReactionAmount(cheepId, reactionType);
    }

    public async Task<IActionResult> OnPostHandleReaction(ReactionType reactionType, Guid cheepId, string username)
    {
        await _reactions.ReactionOnCheep(reactionType, cheepId, username);
        return RedirectToPage("UserTimeline");
    }
}