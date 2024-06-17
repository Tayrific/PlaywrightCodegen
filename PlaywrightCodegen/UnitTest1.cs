using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightCodegen
{
    public class Tests : PageTest
    {
        [SetUp]
        public async Task Setup()
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        [TearDown]
        public async Task TearDown()
        {
            await Context.Tracing.StopAsync(new()
            {
                Path = Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    "playwright-traces",
                    $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
                )
            });
        }


        [Test]
        public async Task MyTest()

        {
            var context = await Browser.NewContextAsync(new BrowserNewContextOptions()
            {
                RecordVideoDir = "video/",
                RecordVideoSize = new RecordVideoSize()
                {
                    Width = 1920,
                    Height = 1080
                },
                ViewportSize = new ViewportSize()
                {
                    Width = 1920,
                    Height = 1080
                }
            });


            var page = await context.NewPageAsync();

            await page.GotoAsync("https://localhost:44371/");
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss1_list.png" });
            await page.GetByRole(AriaRole.Link, new() { Name = "Create New" }).ClickAsync();
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss2_createPage.png" });
            await page.GetByLabel("product name").ClickAsync();
            await page.GetByLabel("product name").PressAsync("CapsLock");
            await page.GetByLabel("product name").FillAsync("O");
            await page.GetByLabel("product name").PressAsync("CapsLock");
            await page.GetByLabel("product name").FillAsync("Oranges");
            await page.GetByLabel("Price").ClickAsync();
            await page.GetByLabel("Price").FillAsync("10.00");
            await page.GetByLabel("description").ClickAsync();
            await page.GetByLabel("description").PressAsync("CapsLock");
            await page.GetByLabel("description").FillAsync("S");
            await page.GetByLabel("description").PressAsync("CapsLock");
            await page.GetByLabel("description").FillAsync("Sweet oranges");
            await page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss3_After.png" });

            var table = page.Locator("table");
            var row = table.Locator("tr", new() { HasText = "Oranges" });

            var nameCell = row.Locator("td").First;

            var nameText = await nameCell.TextContentAsync();
            System.Console.WriteLine($"Text content of the first cell in the row: {nameText}");

            var nameIsCorrect = nameText.Trim() == "Oranges";
            Assert.IsTrue(nameIsCorrect, $"The new item 'Oranges' was not found. Found text: {nameText}");

            await context.CloseAsync(); 
        }
    }
}