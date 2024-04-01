using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;


namespace SimpleShoppingApp.Data.Seeders
{
    public class ProductsSeeder : ISeeder
    {
        private readonly IRepository<Product> productsRepo;
        private readonly IRepository<Category> categoriesRepo;
        private readonly IRepository<ApplicationUser> usersRepo;
        private readonly IConfiguration configuration;
        private readonly IDictionary<string, int> categoryDict;
        private string? adminId;

        public ProductsSeeder(IRepository<Product> _productsRepo,
            IRepository<Category> _categoriesRepo,
            IRepository<ApplicationUser> _usersRepo,
            IConfiguration _configuration)
        {
            productsRepo = _productsRepo;
            categoriesRepo = _categoriesRepo;
            usersRepo = _usersRepo;
            configuration = _configuration;
            categoryDict = new Dictionary<string, int>();
        }
        public async Task SeedAsync()
        {
            if (productsRepo.AllAsNoTracking().Count() == 0)
            {
                var categoriesLinks = GetCategoriesLinks();

                foreach (var categoryLink in categoriesLinks)
                {
                    var productLinks = await GetProductsLinksAsync(categoryLink, 20);
                    if (productLinks != null)
                    {
                        foreach (var productLink in productLinks)
                        {
                            var product = await GetProductAsync(productLink);
                            if (product != null)
                            {
                                await productsRepo.AddAsync(product);
                            }
                        }
                        await productsRepo.SaveChangesAsync();
                    }
                }
            }

        }

        public static IEnumerable<string> GetCategoriesLinks(int? numberOfLinks = null)
        {
            var categoriesLinks = new List<string>() {
    "https://www.emag.bg/mobilni-telefoni/c?ref=hp_menu_quick-nav_1_1&type=category",
    "https://www.emag.bg/tablet/c?ref=hp_menu_quick-nav_1_12&type=category",
    "https://www.emag.bg/laptopi/c?ref=hp_menu_quick-nav_1_21&type=category",
    "https://www.emag.bg/smartwatch/c?ref=hp_menu_quick-nav_1_31&type=category",
    "https://www.emag.bg/shopping-assistant/mobilni-telefoni?ref=hp_menu_quick-nav_1_127&type=link",
    "https://www.emag.bg/nastolni-kompjutri/c?ref=hp_menu_quick-nav_21_1&type=category",
    "https://www.emag.bg/procesori/c?ref=hp_menu_quick-nav_21_5&type=category",
    "https://www.emag.bg/office-desktop-prilozhenija/c?ref=hp_menu_quick-nav_21_16&type=category",
    "https://www.emag.bg/usb-pameti/c?ref=hp_menu_quick-nav_21_20&type=category",
    "https://www.emag.bg/lazerni-printeri/c?ref=hp_menu_quick-nav_21_31&type=category",
    "https://www.emag.bg/bezzhichni-ruteri/c?ref=hp_menu_quick-nav_21_36&type=category",
    "https://www.emag.bg/televizori/c?ref=hp_menu_quick-nav_320_1&type=link",
    "https://www.emag.bg/televizori/filter/razmer-na-displeja-f9186,80-81-cm-v29468/c?ref=hp_menu_quick-nav_320_6&type=link",
    "https://www.emag.bg/soundbar/c?ref=hp_menu_quick-nav_320_16&type=link",
    "https://www.emag.bg/prenosimi-tonkoloni/c?ref=hp_menu_quick-nav_320_24&type=link",
    "https://www.emag.bg/videokameri/c?ref=hp_menu_quick-nav_320_31&type=link",
    "https://www.emag.bg/fotoaparati-dslr/c?ref=hp_menu_quick-nav_320_34&type=link",
    "https://www.emag.bg/hardware-prenosimi-konzoli/brand/sony/c?ref=hp_menu_quick-nav_3096_1&type=link",
    "https://www.emag.bg/kontroleri-volani-gaming-slushalki/c?ref=hp_menu_quick-nav_3096_5&type=link",
    "https://www.emag.bg/igri-konzola-kompiutyr/filter/platforma-f7609,playstation-v-7008037/syvmestimo-gejming-ustrojstvo-f7610,playstation-5-v-9180225/c?ref=hp_menu_quick-nav_3096_11&type=link",
    "https://www.emag.bg/klaviaturi/filter/tip-f6330,gejming-v23011/c?ref=hp_menu_quick-nav_3096_17&type=link",
    "https://www.emag.bg/nastolni-kompjutri/filter/prednaznachen-za-f7905,gaming-v-4670356/c?ref=hp_menu_quick-nav_3096_22&type=link",
    "https://www.emag.bg/procesori/c?ref=hp_menu_quick-nav_3096_30&type=link",
    "https://www.emag.bg/laptopi/brand/acer/filter/vid-laptop-f7882,gaming-v-17788/c?ref=hp_menu_quick-nav_3096_37&type=link",
    "https://www.emag.bg/hladilnici/c?ref=hp_menu_quick-nav_418_1&type=category",
    "https://www.emag.bg/sushilni-za-drehi/c?ref=hp_menu_quick-nav_418_9&type=link",
    "https://www.emag.bg/sydomialni/c?ref=hp_menu_quick-nav_418_11&type=link",
    "https://www.emag.bg/peralni/filter/tip-montirane-f7942,vgrazhdane-v-4744918/c?ref=hp_menu_quick-nav_418_13&type=link",
    "https://www.emag.bg/gotvarski-pechki/c?ref=hp_menu_quick-nav_418_20&type=category",
    "https://www.emag.bg/klimatici/c?ref=hp_menu_quick-nav_418_26&type=category",
    "https://www.emag.bg/shopping-assistant/klimatik?ref=hp_menu_quick-nav_418_127&type=link",
    "https://www.emag.bg/prahosmukachki/c?ref=hp_menu_quick-nav_2026_1&type=link",
    "https://www.emag.bg/espreso-mashini/c?ref=hp_menu_quick-nav_2026_10&type=link",
    "https://www.emag.bg/elektricheski-skari/c?ref=hp_menu_quick-nav_2026_16&type=link",
    "https://www.emag.bg/prahosmukachki/filter/tip-produkt-f7960,prahosmukachka-bez-torba-v-7355651/c?ref=hp_menu_quick-nav_2026_33&type=filter",
    "https://www.emag.bg/espreso-mashini/filter/tip-f8257,avtomatichna-v-7313784/forma-kafe-f8769,na-zyrna-v-7305294/c?ref=hp_menu_quick-nav_2026_39&type=link",
    "https://www.emag.bg/label/Selection-of-brands-for-her-?ref=hp_menu_quick-nav_1101_1&type=link",
    "https://www.emag.bg/damski-chasovnici/c?ref=hp_menu_quick-nav_1101_3&type=link",
    "https://www.emag.bg/damski-maratonki/c?ref=hp_menu_quick-nav_1101_7&type=link",
    "https://www.emag.bg/damski-teniski/c?ref=hp_menu_quick-nav_1101_11&type=link",
    "https://www.emag.bg/label/Selection-of-brands-for-him?ref=hp_menu_quick-nav_1101_15&type=link",
    "https://www.emag.bg/myzhki-chasovnici/c?ref=hp_menu_quick-nav_1101_17&type=link",
    "https://www.emag.bg/myzhki-maratonki/c?ref=hp_menu_quick-nav_1101_21&type=link",
    "https://www.emag.bg/myzhki-teniski/c?ref=hp_menu_quick-nav_1101_25&type=link",
    "https://www.emag.bg/kufari/c?ref=hp_menu_quick-nav_1101_33&type=link",
    "https://www.emag.bg/epilatori/c?ref=hp_menu_quick-nav_549_1&type=category",
    "https://www.emag.bg/elektricheski-chetki-za-zybi/c?ref=hp_menu_quick-nav_549_15&type=category",
    "https://www.emag.bg/termometri/c?ref=hp_menu_quick-nav_549_17&type=category",
    "https://www.emag.bg/kremove-za-lice/c?ref=hp_menu_quick-nav_549_25&type=category",
    "https://www.emag.bg/gradinski-stolove-shezlongi/c?ref=hp_menu_quick-nav_612_2&type=link",
    "https://www.emag.bg/ofis-stolove/c?ref=hp_menu_quick-nav_612_7&type=link",
    "https://www.emag.bg/tigani/c?ref=hp_menu_quick-nav_612_21&type=link",
    "https://www.emag.bg/perilni-preparati/c?ref=hp_menu_quick-nav_612_28&type=link",
    "https://www.emag.bg/kilimi/c?ref=hp_menu_quick-nav_612_33&type=link",
    "https://www.emag.bg/shopping-assistant/matraci?ref=hp_menu_quick-nav_612_38&type=link",
    "https://www.emag.bg/hrana-za-kuche/c?ref=hp_menu_quick-nav_612_44&type=category",
    "https://www.emag.bg/legla-vyzglavnici-diusheci-za-domashni-liubimci-1/c?ref=hp_menu_quick-nav_612_50&type=category",
    "https://www.emag.bg/label/Pampers?ref=hp_menu_quick-nav_689_1&type=link",
    "https://www.emag.bg/kolichki/c?ref=hp_menu_quick-nav_689_8&type=link",
    "https://www.emag.bg/mebeli-za-detska-staia/c?ref=hp_menu_quick-nav_689_12&type=category",
    "https://www.emag.bg/stolcheta-za-hranene/c?ref=hp_menu_quick-nav_689_18&type=category",
    "https://www.emag.bg/label/Baby-rush-creams?ref=hp_menu_quick-nav_689_23&type=link",
    "https://www.emag.bg/label/%D0%9F%D0%BE%D0%BC%D0%BF%D0%B8-%D0%B7%D0%B0-%D0%BA%D1%8A%D1%80%D0%BC%D0%B0?ref=hp_menu_quick-nav_689_29&type=link",
    "https://www.emag.bg/label/Baby-toys?ref=hp_menu_quick-nav_689_35&type=link",
    "https://www.emag.bg/batuti/c?ref=hp_menu_quick-nav_689_46&type=link",
    "https://www.emag.bg/veloergometri/c?ref=hp_menu_quick-nav_731_1&type=category",
    "https://www.emag.bg/kamping-artikuli/sd?ref=hp_menu_quick-nav_731_15&type=subdepartment",
    "https://www.emag.bg/kufari/c?ref=hp_menu_quick-nav_731_24&type=category",
    "https://www.emag.bg/sportni-chasovnici/c?ref=hp_menu_quick-nav_731_29&type=category",
    "https://www.emag.bg/velosipedi/c?ref=hp_menu_quick-nav_731_33&type=category",
    "https://www.emag.bg/sportni-obuvki/c?ref=hp_menu_quick-nav_731_38&type=category",
    "https://www.emag.bg/avto-dzhanti/c?ref=hp_menu_quick-nav_760_1&type=link",
    "https://www.emag.bg/videoregistratori/c?ref=hp_menu_quick-nav_760_3&type=link",
    "https://www.emag.bg/avtoboks/c?ref=hp_menu_quick-nav_760_8&type=category",
    "https://www.emag.bg/bormashini-vintoverti/c?ref=hp_menu_quick-nav_760_17&type=category",
    "https://www.emag.bg/kuhnenski-mivki/c?ref=hp_menu_quick-nav_760_24&type=category",
    "https://www.emag.bg/betonobyrkachki-kolichki/c?ref=hp_menu_quick-nav_760_28&type=category",
    "https://www.emag.bg/vodostrujki/c?ref=hp_menu_quick-nav_760_33&type=link",
    "https://www.emag.bg/unishtozhiteli-dokumenti/c?ref=hp_menu_quick-nav_2388_1&type=link",
    "https://www.emag.bg/cvetni-molivi-flumasteri/c?ref=hp_menu_quick-nav_2388_7&type=link",
    "https://www.emag.bg/kafe/filter/forma-f8408,zyrna-v-6124346/c?ref=hp_menu_quick-nav_2388_15&type=link",
    "https://www.emag.bg/mineralna-voda/c?ref=hp_menu_quick-nav_2388_22&type=link",
    "https://www.emag.bg/whiskey/c?ref=hp_menu_quick-nav_2388_28&type=link",
    "https://www.emag.bg/sladki-lakomstva/c?ref=hp_menu_quick-nav_2388_32&type=link"
};

            if (numberOfLinks != null)
            {
                categoriesLinks = categoriesLinks.Take((int)numberOfLinks).ToList();
            }

            return categoriesLinks;
        }

        public static async Task<IEnumerable<string>> GetProductsLinksAsync(string categoryUrl, int numberOfLinks)
        {
            var links = new List<string>();
            string url = categoryUrl;

            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);

            var productLinks = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class, 'card-v2-title')]");

            if (productLinks != null)
            {
                foreach (var productLink in productLinks)
                {
                    if (productLink != null)
                    {
                        var productUrl = productLink.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrWhiteSpace(productUrl))
                        {
                            links.Add(productUrl);
                            if (links.Count == numberOfLinks)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return links;
        }
        private async Task<Product?> GetProductAsync(string productUrl)
        {
            try
            {
                var httpClient = new HttpClient();

                var html = await httpClient.GetStringAsync(productUrl);

                var htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(html);


                var productName = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='page-title']")?.InnerText;
                var productPrice = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'pricing-block')]//p[contains(@class, 'product-new-price')]")?[1]?.InnerText;
                var productDescription = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='specifications-body']//table")?.OuterHtml;
                var categoryName = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='breadcrumb']//span")?.InnerText;
                var imagesLinks = htmlDoc.DocumentNode.SelectNodes("//*[contains(@class, 'product-gallery-inner')]//*[contains(@class, 'thumbnail-wrapper')]//a");


                if (!string.IsNullOrWhiteSpace(productName) && !string.IsNullOrWhiteSpace(productPrice) && !string.IsNullOrWhiteSpace(productDescription) && !string.IsNullOrWhiteSpace(categoryName))
                {
                    productName = productName.Replace("\n", string.Empty).Trim();
                    categoryName = categoryName.Replace("&amp;", "&");
                    productPrice = productPrice.Replace("&#44;", ".").Replace("лв.", string.Empty).TrimEnd();

                    if (!decimal.TryParse(productPrice, out decimal productPriceDecimal))
                    {
                        return null;
                    }

                    if (!categoryDict.ContainsKey(categoryName))
                    {
                        int categoryId = await categoriesRepo.AllAsNoTracking()
                            .Where(c => c.Name.Contains(categoryName))
                            .Select(c => c.Id)
                            .FirstOrDefaultAsync();

                        if (categoryId != 0)
                        {
                            categoryDict[categoryName] = categoryId;
                        }
                    }

                    if (categoryDict.ContainsKey(categoryName))
                    {
                        if (adminId == null)
                        {
                             adminId = await GetAdminIdAsync();
                        }
                        
                        var product = new Product
                        {
                            Name = productName,
                            Description = productDescription,
                            Price = productPriceDecimal,
                            CategoryId = categoryDict[categoryName],
                            Quantity = 1000,
                            UserId = adminId,
                        };

                        if (imagesLinks != null)
                        {
                            foreach (var imageLink in imagesLinks)
                            {
                                if (imageLink != null)
                                {
                                    string imgHref = imageLink.GetAttributeValue("href", string.Empty);
                                    if (!string.IsNullOrWhiteSpace(imgHref))
                                    {
                                        product.Images.Add(new Image
                                        {
                                            ImageUrl = imgHref,
                                        });
                                    }

                                }
                            }
                        }

                        return product;
                    }


                }

                return null;
            }

            catch (Exception ex)
            {
                return null;
            }

        }

        private async Task<string> GetAdminIdAsync()
        {
            var adminEmail = configuration["AdminAccount:Email"];
            return await usersRepo.AllAsNoTracking()
                .Where(u => u.Email == adminEmail)
                .Select(u => u.Id)
                .FirstAsync();
        }
    }
}
