using System.Globalization;

namespace Chirp.EndToEndTest;

/// <summary>
/// Tests if a user can sort both by newest and oldest cheep
/// </summary>
[TestFixture]
public class UnauthorizedSortingTest
{
    /// <summary>
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

        // Navigate to the website
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
            // Extract timestamp from each post
            var smallElement = await posts[i].QuerySelectorAsync("p small");
            string postTimestampString = await smallElement.TextContentAsync() ?? "";
            string postTimestamp = postTimestampString.TrimStart('—', ' ');

            // Parse timestamp into DateTime object
            DateTime postDateTime = DateTime.ParseExact(postTimestamp, "MM'/'dd'/'yy HH':'mm':'ss", new CultureInfo("en-US"));

            // Assert that this post is newer than the previous one
            Assert.IsTrue(postDateTime <= previousPostDateTime, $"Post at index {i} is not newer than the previous post.");

            // Update previousPostDateTime for the next iteration
            previousPostDateTime = postDateTime;
        }

    }
    
    /// <summary>
    /// Sorts by oldest
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