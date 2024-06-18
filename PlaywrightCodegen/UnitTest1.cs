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

        private async Task<IBrowserContext> CreateBrowserContextAsync()
        {
            return await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = "video/",
                RecordVideoSize = new RecordVideoSize
                {
                    Width = 1920,
                    Height = 1080
                },
                ViewportSize = new ViewportSize
                {
                    Width = 1920,
                    Height = 1080
                }
            });
        }


        [Test]
        public async Task CreateTest()

        {
            var context = await CreateBrowserContextAsync();
            var page = await context.NewPageAsync();

            await page.GotoAsync("https://localhost:44371/");
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss1_create_list.png" });
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
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss3_create_After.png" });

            var itemVisible = await page.GetByRole(AriaRole.Cell, new() { Name = "Oranges", Exact = true }).IsVisibleAsync();

            if (itemVisible)
            {
                Console.WriteLine("The new item 'Oranges' was found in the list. Test passed.");
            }
            else
            {
                Assert.Fail("The new item 'Oranges' was not found in the list.");
            }

            await context.CloseAsync();
        }

        [Test]
        public async Task EditTest()
        {
            var context = await CreateBrowserContextAsync();
            var page = await context.NewPageAsync();

            await page.GotoAsync("https://localhost:44371/");
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss4_edit_beforeAdding.png" });
            await page.GetByRole(AriaRole.Link, new() { Name = "Create New" }).ClickAsync();
            await page.GetByLabel("product name").ClickAsync();
            await page.GetByLabel("product name").PressAsync("CapsLock");
            await page.GetByLabel("product name").FillAsync("S");
            await page.GetByLabel("product name").PressAsync("CapsLock");
            await page.GetByLabel("product name").FillAsync("Strawbery");
            await page.GetByLabel("Price").ClickAsync();
            await page.GetByLabel("Price").FillAsync("10.00");
            await page.GetByLabel("description").ClickAsync();
            await page.GetByLabel("description").PressAsync("CapsLock");
            await page.GetByLabel("description").FillAsync("R");
            await page.GetByLabel("description").PressAsync("CapsLock");
            await page.GetByLabel("description").FillAsync("Red");
            await page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss5_edit_AddingIncorrect.png" });
            await page.GetByRole(AriaRole.Link, new() { Name = "Edit" }).Nth(4).ClickAsync();
            await page.GetByLabel("product name").ClickAsync();
            await page.GetByLabel("product name").PressAsync("ArrowLeft");
            await page.GetByLabel("product name").FillAsync("Strawberry");
            await page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss6_edit_EditedCorrectly.png" });

            var itemVisible = await page.GetByRole(AriaRole.Cell, new() { Name = "Strawberry", Exact = true }).IsVisibleAsync();

            if (itemVisible)
            {
                Console.WriteLine("Strawberry was correctly edited");
            }
            else
            {
                Assert.Fail("Strawberry was not found in the list or was incorrectly edited.");
            }

            await context.CloseAsync();

        }

        [Test]
        public async Task DeleteTest()
        {
            var context = await CreateBrowserContextAsync();
            var page = await context.NewPageAsync();

            await page.GotoAsync("https://localhost:44371/");
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss7_delete_beforeDelete.png" });
            await page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).Nth(4).ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots\\ss8_delete_afterDelete.png" });

            var itemVisible = await page.GetByRole(AriaRole.Cell, new() { Name = "Oranges", Exact = true }).IsVisibleAsync();

            if (!itemVisible)
            {
                Console.WriteLine("Oranges was deleted, Test passed.");
            }
            else
            {
                Assert.Fail("The new item 'Oranges' was found in the list. Test failed.");
            }

            await context.CloseAsync();
        }
    }
}