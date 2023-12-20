using System.Globalization;

namespace Chirp.EndToEndTest;

/// <summary>
/// Test that checks that sorting still works after authorization
/// </summary>
[TestFixture]
public class AuthorizedSortingTest
{
    //Login info
    private string UserNameDummy = "BDSADummy";
    private string PasswordDummy = "BDSAGroup25";
    
    /// <summary>
    /// Authorizes the user. Is run before all tests
    /// </summary>
    [SetUp]
    public async Task Authorization()
    {
        // Initialize Playwright, launch a Chromium browser, and open a new page for testing.
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false, // Headless mode might be disabled for OAuth flows
        });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to our website
        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        // Click 'Login'
        await page.ClickAsync("a#Login");

        // Wait for navigation to login page
        await page.WaitForURLAsync("https://bdsagroup25chirprazor.azurewebsites.net/Identity/Account/Login");

        // Click the 'Sign in with GitHub' button
        await page.ClickAsync("button.btn.btn-primary");

        // Wait for GitHub login page
        await page.WaitForURLAsync(url => url.StartsWith("https://github.com/login"));

        // Fill in the username and password
        await page.FillAsync("input#login_field", $"{UserNameDummy}");
        await page.FillAsync("input#password", $"{PasswordDummy}");

        // Click the 'Sign in' button
        await page.ClickAsync("input[type='submit'][value='Sign in']");
        
        //If you authorize for the first time, uncomment these next 2 lines:
        //await page.WaitForURLAsync(url => url.StartsWith("https://github.com/login/oauth/authorize"));
        //await page.ClickAsync("input[type='submit'][id='js-oauth-authorize-btn']");
        
        await page.WaitForURLAsync("https://bdsagroup25chirprazor.azurewebsites.net/");
        
        //These next asserts validate a proper authorization by cheching for fundamental elements, only available to users that are logged in
        // Find cheepbox
        string expectedText = $"What's on your mind {UserNameDummy}?";

        // Locate the h3 tag within the cheepbox div and get its text
        var cheepboxH3 = page.Locator("div.cheepbox h3");
        bool isCheepboxVisible = await cheepboxH3.IsVisibleAsync();
        string cheepboxText = await cheepboxH3.TextContentAsync() ?? "";

        // Check if the cheepbox is visible and contains the expected text
        Assert.IsTrue(isCheepboxVisible && cheepboxText.Contains(expectedText), "Cheepbox not available.");
    }
    
    /// <summary>
    /// Reuses test-flow from unauthorized sorting, but has authorization in the setup
    /// Sorts by newest cheep first
    /// </summary>
    [Test]
    public async Task TestSortingByNewestPosts()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to website
        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        // Click the sort button
        await page.Locator("button.dropbtn").ClickAsync();

        // Wait for the 'Newest' option to become visible
        await page.Locator("div.dropdown-content >> text='Newest'").WaitForAsync();

        // Click the 'Newest' option
        await page.Locator("div.dropdown-content >> text='Newest'").ClickAsync();

        // Wait for URL to change
        await page.WaitForURLAsync("https://bdsagroup25chirprazor.azurewebsites.net/?SortOrder=Newest");

        // Get all posts
        var posts = await page.Locator("ul#messagelist > li").ElementHandlesAsync();
    
        DateTime previousPostDateTime = DateTime.MaxValue;

        for (int i = 0; i < posts.Count; i++)
        {
            // Extract clean timestamp from each post
            var smallElement = await posts[i].QuerySelectorAsync("p small");
            string postTimestampString = await smallElement.TextContentAsync() ?? "";
            string postTimestamp = postTimestampString.TrimStart('—', ' ');

            // Parse timestamp into DateTime object
            DateTime postDateTime = DateTime.ParseExact(postTimestamp, "MM'/'dd'/'yy HH':'mm':'ss", new CultureInfo("en-US"));

            // Assert that this post is newer or equal to the previous one
            Assert.IsTrue(postDateTime <= previousPostDateTime, $"Post at index {i} is not newer than the previous post.");

            // Update previousPostDateTime for the next iteration
            previousPostDateTime = postDateTime;
        }

    }
    
    /// <summary>
    /// Reuses test-flow from unauthorized sorting, but has authorization in the setup
    /// Sorts by oldest cheep first
    /// </summary>
    [Test]
    public async Task TestSortingByOldestPosts()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to the website
        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        // Click the sort button
        await page.Locator("button.dropbtn").ClickAsync();

        // Wait for the 'Oldest' option to become visible
        await page.Locator("div.dropdown-content >> text='Oldest'").WaitForAsync();

        // Click the 'Oldest' option
        await page.Locator("div.dropdown-content >> text='Oldest'").ClickAsync();

        // Wait for URL to change
        await page.WaitForURLAsync("https://bdsagroup25chirprazor.azurewebsites.net/?SortOrder=Oldest");

        // Get all posts
        var posts = await page.Locator("ul#messagelist > li").ElementHandlesAsync();

        DateTime previousPostDateTime = DateTime.MinValue;

        for (int i = 0; i < posts.Count; i++)
        {
            // Extract timestamp from each post
            var smallElement = await posts[i].QuerySelectorAsync("p small");
            string postTimestampString = await smallElement.TextContentAsync() ?? "";
            string postTimestamp = postTimestampString.TrimStart('—', ' ');

            // Parse timestamp into DateTime object
            DateTime postDateTime = DateTime.ParseExact(postTimestamp, "MM'/'dd'/'yy HH':'mm':'ss", new CultureInfo("en-US"));

            // Assert that this post is older or equal to the previous one (since it's sorted by oldest first)
            Assert.IsTrue(postDateTime >= previousPostDateTime, $"Post at index {i} is not older than the previous post.");

            // Update previousPostDateTime for the next iteration
            previousPostDateTime = postDateTime;
        }
    }
}