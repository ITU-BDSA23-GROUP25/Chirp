using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core;
using System.Configuration;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.CodeAnalysis.CSharp;

namespace Chirp.Razor.Areas.Identity.Pages;
public class PublicModel : PageModel
{
    private readonly ICheepRepository _service;
    private readonly IFollowerRepository _followerRepository;
    public List<CheepDTO> Cheeps { get; set; }
    public PaginationModel? PaginationModel { get; set; }

    public Dictionary<string, bool> FollowStatus { get; set; } = new Dictionary<string, bool>();

    public bool IsFollowing { get; set; } = false;
    public string Text {get; set;}

    [BindProperty(SupportsGet = true)]
    public string SortOrder { get; set; } = "Newest";
    


    public PublicModel(ICheepRepository service, IFollowerRepository followerRepository)
    {
        Cheeps = new List<CheepDTO>();
        _service = service;
        _followerRepository = followerRepository;

    }

    public async Task<IActionResult> OnGetAsync([FromQuery] int? page)
    {
        if (!page.HasValue || page < 1)
        {
            page = 1;
        }

        // Sort cheeps based on the selected order
        Cheeps = _service.GetCheeps((int)page - 1, SortOrder).Result.ToList();

        var amountOfCheeps = _service.CheepTotal().Result;
        PaginationModel = new PaginationModel(amountOfCheeps, (int)page);

        foreach (var cheep in Cheeps)
            {
              
                var userName = cheep.Author;
                var followersFromUser = await _followerRepository.GetFollowedAuthor(userName);
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
    return RedirectToPage("Public");
}

    public IActionResult OnPost()
    {
        _service.CreateCheep(Request.Form["Text"], User.Identity?.Name!);
        return RedirectToPage("Public");
        
    }

    public async Task<IActionResult> OnPostFollow(string Username, string FollowerName)
    {
        await _followerRepository.AddOrRemoveFollower(FollowerName, Username);

        return RedirectToPage("Public");
    }
}