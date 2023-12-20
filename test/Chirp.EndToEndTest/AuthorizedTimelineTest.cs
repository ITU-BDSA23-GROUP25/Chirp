namespace Chirp.EndToEndTest;

/// <summary>
/// Test that checks the user can switch between timelines after authorization
/// </summary>
[TestFixture]
public class AuthorizedTimelineTest
{
    //Login info
    private string UserNameDummy = "BDSADummy";
    private string PasswordDummy = "BDSAGroup25";
    
    /// <summary>
    /// Authorizes user
    /// Runs before all tests
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
    /// Re-uses test-flow from unauthorized
    /// Jumps back and forth between public and user timeline
    /// </summary>
    [Test]
    public async Task TimelineTest()
    {
        // Initialize Playwright, launch a Chromium browser, and open a new page for testing.
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true, //Set to false, if you want to watch what happens in the browser. Keep true if you've gotten
                             //plenty of that from other tests already
        });
        var context = await browser.NewContextAsync();

        var page = await context.NewPageAsync();

        //Navigate to our site
        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        // Select the first author on the page
        var firstAuthorLinkSelector = ".cheeps li >> nth=0 >> p strong > a";

        // Wait for visibility, to ensure smoothness
        await page.Locator(firstAuthorLinkSelector).WaitForAsync();

        // Capture the author's name
        string expectedAuthorName = await page.Locator(firstAuthorLinkSelector).TextContentAsync() ?? "";
        
        // Click the author link
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