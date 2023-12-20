namespace Chirp.EndToEndTest;

/// <summary>
/// A simple authorization workflow, that logs in, and validates certain elements, such as the cheepbox, is present
/// </summary>
[TestFixture]
public class AuthorizeUserTest
{
    //Login info
    private string UserNameDummy = "BDSADummy";
    private string PasswordDummy = "BDSAGroup25";
    
    [Test]
    public async Task AuthorizeAndValidateUser()
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
}