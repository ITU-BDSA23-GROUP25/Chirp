namespace Chirp.EndToEndTest;

/// <summary>
/// Test that checks if pagination still works, after authorization
/// </summary>
[TestFixture]
public class AuthorizedPaginationTest
{
    // login info
    private string UserNameDummy = "BDSADummy";
    private string PasswordDummy = "BDSAGroup25";
    
    /// <summary>
    /// Does the authorization, and is executed before each Test
    /// </summary>
    [SetUp]
    public async Task Authorization()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false, // Headless mode might be disabled for OAuth flows
        });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to the website
        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        // Click the 'Login' link
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
    /// Same workflow as the unauthorized pagination test-flow. Just, after the setup has fixed authorization
    /// Uses the numbers in pagination
    /// </summary>
    [Test]
    public async Task UnauthorizedUserUsingNumberPagination()
    {
        // Initialize Playwright, launch a Chromium browser, and open a new page for testing.
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
        var context = await browser.NewContextAsync();

        var page = await context.NewPageAsync();

        // Navigate to website
        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        int currentPage = 1;
        int totalPages = 4; // Set to the number of pages to test

        // Navigate forward through the pages
        for (int nextPage = currentPage + 1; nextPage <= totalPages; nextPage++)
        {
            await NavigateAndAssertPage(page, currentPage);
        }

        // Navigate back through the pages
        for (int previousPage = totalPages - 1; previousPage >= 1; previousPage--)
        {
            await NavigateAndAssertPage(page, previousPage);
            currentPage--;
        }
    }

    // Sets uo logic for how navigation works. It is then used in the loops, that loop back and forth
    private async Task NavigateAndAssertPage(IPage page, int pageNumber)
    {
        // Selector for the pagination link
        var paginationLinkSelector = $"div.pagination >> text='{pageNumber}'";

        // Wait for the pagination link to be visible
        await page.Locator(paginationLinkSelector).WaitForAsync();

        // Click the pagination link
        await page.Locator(paginationLinkSelector).ClickAsync();

        // Wait for URL to change and start with the specified pattern
        await page.WaitForURLAsync(url => url.StartsWith($"https://bdsagroup25chirprazor.azurewebsites.net/?page={pageNumber}"));

        // Extract the current URL
        string currentPageUrl = page.Url;

        // Assert that the URL starts with the expected pattern
        bool isCorrectPage = currentPageUrl.StartsWith($"https://bdsagroup25chirprazor.azurewebsites.net/?page={pageNumber}");
        Assert.IsTrue(isCorrectPage, $"The page did not navigate to the expected pagination URL for page {pageNumber}.");
    }

    /// <summary>
    /// Reuses test-flow from the unauthorized pagination tests, but has authorization in setup
    /// Uses arrows for pagination
    /// </summary>
    [Test]
    public async Task UnauthorizedUserNavigatesPaginationUsingArrowButtons()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
        var context = await browser.NewContextAsync();

        var page = await context.NewPageAsync();

        // Navigate to the website
        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        int currentPage = 1;
        int totalPages = 7; // Adjust as needed for your test

        // Navigate forward using the 'next page' arrow button
        for (int nextPage = currentPage + 1; nextPage <= totalPages; nextPage++)
        {
            await NavigateAndAssertPage(page, ">", nextPage);
            currentPage++;
        }

        // Navigate backward using the 'previous page' arrow button
        for (int previousPage = totalPages - 1; previousPage >= 1; previousPage--)
        {
            await NavigateAndAssertPage(page, "<", previousPage);
            currentPage--;
        }
    }

    //Again, establishes navigation logic
    private async Task NavigateAndAssertPage(IPage page, string arrowButton, int expectedPage)
    {
        // Selector for the arrow button
        var arrowButtonSelector = $"div.pagination >> text='{arrowButton}'";

        // Wait for the arrow button to be visible
        await page.Locator(arrowButtonSelector).WaitForAsync();

        // Click the arrow button
        await page.Locator(arrowButtonSelector).ClickAsync();

        // Wait for URL to change and start with the specified pattern
        await page.WaitForURLAsync(url => url.StartsWith($"https://bdsagroup25chirprazor.azurewebsites.net/?page={expectedPage}"));

        // Extract the current URL
        string currentPageUrl = page.Url;

        // Assert that the URL starts with the expected pattern
        bool isCorrectPage = currentPageUrl.StartsWith($"https://bdsagroup25chirprazor.azurewebsites.net/?page={expectedPage}");
        Assert.IsTrue(isCorrectPage, $"The page did not navigate to the expected pagination URL for page {expectedPage}.");
    }
    
    /// <summary>
    /// Reuses test-flow from the unauthorized pagination
    /// Uses the double-arrows, that jumps straight to the end, or beginning
    /// </summary>
    [Test]
    public async Task TestJumpToFirstAndLastPagePagination()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
        var context = await browser.NewContextAsync();

        var page = await context.NewPageAsync();

        // Navigate to the website
        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        // Click the 'jump to last page' button (»)
        await ClickAndWaitForURLChange(page, "div.pagination >> text='»'", "?page=");

        // Assert that 'next page' (>) and 'jump to last page' (») buttons are not visible
        Assert.IsFalse(await page.Locator("div.pagination >> text='>'").IsVisibleAsync());
        Assert.IsFalse(await page.Locator("div.pagination >> text='»'").IsVisibleAsync());

        // Click the 'jump to first page' button («)
        await ClickAndWaitForURLChange(page, "div.pagination >> text='«'", "?page=1");

        // Assert that 'previous page' (<) and 'jump to first page' («) buttons are not visible
        Assert.IsFalse(await page.Locator("div.pagination >> text='<'").IsVisibleAsync());
        Assert.IsFalse(await page.Locator("div.pagination >> text='«'").IsVisibleAsync());
    }
    
    private async Task ClickAndWaitForURLChange(IPage page, string selector, string expectedUrlStart)
    {
        await page.Locator(selector).WaitForAsync();
        await page.Locator(selector).ClickAsync();
        await page.WaitForURLAsync(url => url.StartsWith("https://bdsagroup25chirprazor.azurewebsites.net/" + expectedUrlStart));
    }
}