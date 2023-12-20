namespace Chirp.EndToEndTest;

/// <summary>
/// Test for a comprehensive flow through the application, including authorization, cheeping, deleting cheeps, navigation
/// and deleting the user
/// </summary>
[TestFixture]
public class CheepingUserFlow
{
    // loginfo (login info)
    private string UserNameDummy = "BDSADummy";
    private string PasswordDummy = "BDSAGroup25";
    
    [Test]
    public async Task UserFlowOfEverything()
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
        
        //Wait for redirection
        await page.WaitForURLAsync("https://bdsagroup25chirprazor.azurewebsites.net/");
        
        //These next asserts validate a proper authorization by checking for fundamental elements, only available to users that are logged in
        // Find cheepbox
        string expectedText = $"What's on your mind {UserNameDummy}?";

        // Locate the h3 tag within the cheepbox div and get its text
        var cheepboxH3 = page.Locator("div.cheepbox h3");
        bool isCheepboxVisible = await cheepboxH3.IsVisibleAsync();
        string cheepboxText = await cheepboxH3.TextContentAsync() ?? "";

        // Check if the cheepbox is visible and has the users username in the title/welcome message
        Assert.IsTrue(isCheepboxVisible && cheepboxText.Contains(expectedText), "Cheepbox not available.");
        
        /*Post Authorized-and-validated era*/
        
        // Cheep 'DUMMY TEST'
        await page.FillAsync("div.cheepbox input[type='text']", "DUMMY TEST");
        await page.ClickAsync("div.cheepbox input[type='submit'][value='Share']");

        // Wait to give the submission a chance to be submitted properly
        await Task.Delay(2000);

        // Navigate to the users own timeline
        await page.ClickAsync("a#privateTimeline");

        // Wait for navigation to the user's private timeline page
        await page.WaitForURLAsync($"https://bdsagroup25chirprazor.azurewebsites.net/{UserNameDummy}");

        // Assert the current URL is the expected one
        string currentUrl = page.Url;
        Assert.AreEqual($"https://bdsagroup25chirprazor.azurewebsites.net/{UserNameDummy}", currentUrl, "Did not navigate to the user's private timeline.");
        
        // Click delete button, to delete cheep. We know there is only one cheep, as we have just made the user and
        // made only one cheep
        await page.ClickAsync("button:has-text('Delete')");
        
        // Wait for a short duration to allow for the deletion to take effect
        await Task.Delay(1000);

        // Check that there are no more 'Delete' buttons present
        int deleteButtonsCount = await page.Locator("button:has-text('Delete')").CountAsync();
        Assert.AreEqual(0, deleteButtonsCount, "Deletion failed, 'Delete' button still present.");
        
        //Navigate to 'About Me'
        await page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        
        // Wait for navigation to the About Me page
        await page.WaitForURLAsync("https://bdsagroup25chirprazor.azurewebsites.net/User-info");

        // Assert the current URL is the expected one
        currentUrl = page.Url;
        Assert.AreEqual("https://bdsagroup25chirprazor.azurewebsites.net/User-info", currentUrl, "Did not navigate to the About Me page.");
        
        // Delete user
        await page.GetByRole(AriaRole.Button, new() { Name = "Forget me!" }).ClickAsync();
        
        //Wait shortly, for dramatic effect
        await Task.Delay(2000);
    }
}