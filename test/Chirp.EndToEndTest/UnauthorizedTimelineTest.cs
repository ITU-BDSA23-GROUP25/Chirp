namespace Chirp.EndToEndTest;

/// <summary>
/// This tests if an unauthorized user is able visit the public timeline, visit another users timeline, and return to
/// the public timeline through the 'Public Timeline' button
/// </summary>
[TestFixture]
public class PublicTimelineTest
{
    /// <summary>
    /// Tests if a user can switch between public and private timeline
    /// </summary>
    [Test]
    public async Task UnauthorizedUserGoesFromPublicTimelineToUserTimelineAndBack()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });
        var context = await browser.NewContextAsync();

        var page = await context.NewPageAsync();

        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        // Selector for the first author link in the list of posts
        var firstAuthorLinkSelector = ".cheeps li >> nth=0 >> p strong > a";

        // Wait for the first author link to be visible
        await page.Locator(firstAuthorLinkSelector).WaitForAsync();

        // Capture the author's name from the first link
        string expectedAuthorName = await page.Locator(firstAuthorLinkSelector).TextContentAsync() ?? "";
        
        // Click the first author link
        await page.Locator(firstAuthorLinkSelector).ClickAsync();
        
        // Wait for navigation and for the UserTimelineTitle to be visible
        await page.WaitForURLAsync($"https://bdsagroup25chirprazor.azurewebsites.net/{expectedAuthorName}");
        await page.Locator("#UserTimelineTitle").WaitForAsync();
        
        // Capture the user timelines title
        string actualPageHeading = await page.Locator("#UserTimelineTitle").TextContentAsync() ?? "";
        
        // Assert that the UserTimelineTitle text matches the expected author's timeline
        Assert.AreEqual($"{expectedAuthorName}'s Timeline", actualPageHeading, $"Expected author's timeline to be '{expectedAuthorName}'s Timeline', but found '{actualPageHeading}' instead.");
        
        // Navigate back to the Public Timeline
        await page.Locator("#PublicTimeline").ClickAsync();
        await page.WaitForURLAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        // Assert that we are back on the main page
        string currentUrl = page.Url;
        Assert.AreEqual("https://bdsagroup25chirprazor.azurewebsites.net/", currentUrl, "Should return to the main page");
    }
}