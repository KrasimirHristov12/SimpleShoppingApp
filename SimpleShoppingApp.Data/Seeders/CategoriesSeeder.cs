using HtmlAgilityPack;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;

namespace SimpleShoppingApp.Data.Seeders
{
    public class CategoriesSeeder : ISeeder
    {
        private readonly IRepository<Category> categoriesRepo;

        public CategoriesSeeder(IRepository<Category> _categoriesRepo)
        {
            categoriesRepo = _categoriesRepo;
        }
        public async Task SeedAsync()
        {
            if (categoriesRepo.AllAsNoTracking().Count() == 0)
            {
                var categories = await GetCategoriesAsync();
                if (categories.Count() > 0)
                {
                    foreach (var categoryName in categories)
                    {
                        var category = new Category
                        {
                            Name = categoryName,
                        };
                        await categoriesRepo.AddAsync(category);
                    }
                    await categoriesRepo.SaveChangesAsync();
                }
            }
        }

        private async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            var categories = new List<string>();
            string url = "https://www.emag.bg/";

            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);

            var categoriesFromWeb = htmlDoc.DocumentNode.SelectNodes("//span[@class='megamenu-list-department__department-name']");

            for (int i = 1; i < categoriesFromWeb.Count; i++)
            {
                categories.Add(categoriesFromWeb[i].InnerText);
            }
            return categories;
        }
    }
}
