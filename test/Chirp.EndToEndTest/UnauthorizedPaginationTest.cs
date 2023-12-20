namespace Chirp.EndToEndTest;

/// <summary>
/// Tests if a user can navigate the page using an extensive selection of pagination options
/// </summary>
[TestFixture]
public class UnauthorizedPaginationTest
{
    /// <summary>
    /// Uses the numbers in pagination
    /// </summary>
    [Test]
    public async Task UnauthorizedUserUsingNumberPagination()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true, //Set to false if you want to watch what happens in the browser. Keep true if youre also
                             //tired of that already
        });
        var context = await browser.NewContextAsync();

        var page = await context.NewPageAsync();

        // Navigate to the website
        await page.GotoAsync("https://bdsagroup25chirprazor.azurewebsites.net/");

        int currentPage = 1;
        int totalPages = 3; // Set to the number of pages you want to move back and forth between

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

    //Helper method that establishes the navigation logic. It is used in the loops, that move back and forth between pages
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
    /// Uses the arrow buttons for navigation
    /// </summary>
    [Test]
    public async Task UnauthorizedUserNavigatesPaginationUsingArrowButtons()
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

    //helper method for navigation
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
    /// Uses the double-arrows, that jumps all the way to the end, or beginning, for pagination
    /// </summary>
    [Test]
    public async Task TestJumpToFirstAndLastPagePagination()
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
