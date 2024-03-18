using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;


namespace SimpleShoppingApp.Data.Seeders
{
    public class ProductsSeeder : ISeeder
    {
        private readonly IRepository<Product> productsRepo;
        private readonly IRepository<Category> categoriesRepo;

        public ProductsSeeder(IRepository<Product> _productsRepo,
            IRepository<Category> _categoriesRepo)
        {
            productsRepo = _productsRepo;
            categoriesRepo = _categoriesRepo;
        }
        public async Task SeedAsync()
        {
            if (productsRepo.AllAsNoTracking().Count() == 0)
            {
                var categoriesLinks = GetCategoriesLinks(10);

                foreach (var categoryLink in categoriesLinks)
                {
                    var productLinks = await GetProductsLinksAsync(categoryLink, 2);
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
                    }
                }
                await productsRepo.SaveChangesAsync();
            }

        }

        public static IEnumerable<string> GetCategoriesLinks(int? numberOfLinks = null)
        {
            var categoriesLinks = new List<string>() {
    "/mobilni-telefoni/c?ref=hp_menu_quick-nav_1_1&type=category",
    "/kalyfi-za-mobilni-telefoni/c?ref=hp_menu_quick-nav_1_2&type=category",
    "/zashtitni-folia-za-mobilni-telefoni/c?ref=hp_menu_quick-nav_1_3&type=category",
    "/zariadni-ustrojstva-za-mobilni-telefoni/c?ref=hp_menu_quick-nav_1_4&type=link",
    "/baterii-za-mobilni-telefoni/c?ref=hp_menu_quick-nav_1_5&type=category",
    "/bluetooth-slushalki/c?ref=hp_menu_quick-nav_1_6&type=category",
    "/data-kabeli-za-mobilni-telefoni/c?ref=hp_menu_quick-nav_1_7&type=category",
    "/postavki-docking-stancii-za-mobilni-telefoni/c?ref=hp_menu_quick-nav_1_8&type=category",
    "/vynshni-baterii-za-mobilni-telefoni/c?ref=hp_menu_quick-nav_1_9&type=category",
    "/karti-pamet/c?ref=hp_menu_quick-nav_1_10&type=category",
    "/tablet/c?ref=hp_menu_quick-nav_1_12&type=category",
    "/kalyfi-tableti/c?ref=hp_menu_quick-nav_1_13&type=category",
    "/zaschitni-folia-za-tableti/c?ref=hp_menu_quick-nav_1_14&type=category",
    "/zarjadni-ustrojstva-za-tableti/c?ref=hp_menu_quick-nav_1_15&type=category",
    "/postavki-za-kola-i-doking/c?ref=hp_menu_quick-nav_1_16&type=category",
    "/klaviaturi-za-tableti/c?ref=hp_menu_quick-nav_1_17&type=category",
    "/kabeli-i-adapteri-za-tableti/c?ref=hp_menu_quick-nav_1_18&type=category",
    "/drugi-aksesoari-za-tableti/c?ref=hp_menu_quick-nav_1_19&type=category",
    "/laptopi/c?ref=hp_menu_quick-nav_1_21&type=category",
    "/chanti-laptop/c?ref=hp_menu_quick-nav_1_22&type=category",
    "/pameti-za-laptopi/c?ref=hp_menu_quick-nav_1_23&type=category",
    "/hard-diskove-za-laptopi/c?ref=hp_menu_quick-nav_1_24&type=category",
    "/smartwatch/c?ref=hp_menu_quick-nav_1_31&type=category",
    "/fitnes-grivni/c?ref=hp_menu_quick-nav_1_32&type=category",
    "/drugi-dzhadzhi/c?ref=hp_menu_quick-nav_1_33&type=category",
    "/vr-ochila/c?ref=hp_menu_quick-nav_1_34&type=category",
    "/smart-home-reshenia-senzori/c?ref=hp_menu_quick-nav_1_35&type=category",
    "/shopping-assistant/mobilni-telefoni?ref=hp_menu_quick-nav_1_127&type=link",
    "/nastolni-kompjutri/c?ref=hp_menu_quick-nav_21_1&type=category",
    "/lcd---led-monitori/c?ref=hp_menu_quick-nav_21_2&type=category",
    "/monitorni-aksesoari/c?ref=hp_menu_quick-nav_21_3&type=link",
    "/procesori/c?ref=hp_menu_quick-nav_21_5&type=category",
    "/video-karti/c?ref=hp_menu_quick-nav_21_6&type=category",
    "/dynni-platki/c?ref=hp_menu_quick-nav_21_7&type=category",
    "/operativna-pamet/c?ref=hp_menu_quick-nav_21_8&type=category",
    "/hard-diskove/c?ref=hp_menu_quick-nav_21_9&type=category",
    "/kompiutyrni-kutii/c?ref=hp_menu_quick-nav_21_10&type=category",
    "/zahranvashti-blokove/c?ref=hp_menu_quick-nav_21_11&type=category",
    "/solid-state-drive--ssd/c?ref=hp_menu_quick-nav_21_12&type=category",
    "/it-aksesoari/c?ref=hp_menu_quick-nav_21_13&type=category",
    "/zvukovi-platki/c?ref=hp_menu_quick-nav_21_14&type=category",
    "/office-desktop-prilozhenija/c?ref=hp_menu_quick-nav_21_16&type=category",
    "/operacionni-sistemi/c?ref=hp_menu_quick-nav_21_17&type=category",
    "/antivirusni-programi/c?ref=hp_menu_quick-nav_21_18&type=category",
    "/usb-pameti/c?ref=hp_menu_quick-nav_21_20&type=category",
    "/vynshni-hard-diskove/c?ref=hp_menu_quick-nav_21_21&type=category",
    "/vnshni-ssd/c?ref=hp_menu_quick-nav_21_22&type=category",
    "/klaviaturi/c?ref=hp_menu_quick-nav_21_23&type=category",
    "/mishki/c?ref=hp_menu_quick-nav_21_24&type=category",
    "/tonkoloni-kompiutyr/c?ref=hp_menu_quick-nav_21_25&type=category",
    "/slushalki-kompiutyr/c?ref=hp_menu_quick-nav_21_26&type=category",
    "/vynshni-optichni-ustrojstva/c?ref=hp_menu_quick-nav_21_27&type=category",
    "/ueb-kameri/c?ref=hp_menu_quick-nav_21_28&type=category",
    "/grafichni-tableti/c?ref=hp_menu_quick-nav_21_29&type=category",
    "/lazerni-printeri/c?ref=hp_menu_quick-nav_21_31&type=category",
    "/printeri-multifunkcionalni-ustrojstva/c?ref=hp_menu_quick-nav_21_32&type=category",
    "/printeri-multifunkcionalni-ustrojstva/filter/tip-produkt-f8594,foto-printer-v-6632604/c?ref=hp_menu_quick-nav_21_33&type=link",
    "/mastileni-kaseti-za-printeri/c?ref=hp_menu_quick-nav_21_34&type=category",
    "/bezzhichni-ruteri/c?ref=hp_menu_quick-nav_21_36&type=category",
    "/mrezhovi-anteni-aksesoari/c?ref=hp_menu_quick-nav_21_37&type=category",
    "/bezzhichni-adapteri/c?ref=hp_menu_quick-nav_21_38&type=category",
    "/kameri-za-nabljudenie/c?ref=hp_menu_quick-nav_21_39&type=category",
    "/nas/c?ref=hp_menu_quick-nav_21_40&type=category",
    "/televizori/c?ref=hp_menu_quick-nav_320_1&type=link",
    "/televizori/brand/samsung/c?ref=hp_menu_quick-nav_320_2&type=link",
    "/televizori/brand/lg/c?ref=hp_menu_quick-nav_320_3&type=link",
    "/televizori/filter/razmer-na-displeja-f9186,80-81-cm-v29468/c?ref=hp_menu_quick-nav_320_6&type=link",
    "/televizori/filter/razmer-na-displeja-f9186,82-110-cm-v29469/c?ref=hp_menu_quick-nav_320_7&type=link",
    "/televizori/filter/razmer-na-displeja-f9186,127-152-cm-v29471/c?ref=hp_menu_quick-nav_320_8&type=link",
    "/soundbar/c?ref=hp_menu_quick-nav_320_16&type=link",
    "/audio-sistemi/c?ref=hp_menu_quick-nav_320_17&type=link",
    "/smart-audio-sistemi/c?ref=hp_menu_quick-nav_320_18&type=link",
    "/multimedijni-pleyri/c?ref=hp_menu_quick-nav_320_19&type=link",
    "/tonkoloni/c?ref=hp_menu_quick-nav_320_20&type=link",
    "/gramofoni/c?ref=hp_menu_quick-nav_320_21&type=link",
    "/pleyri-resijvyri/c?ref=hp_menu_quick-nav_320_22&type=link",
    "/prenosimi-tonkoloni/c?ref=hp_menu_quick-nav_320_24&type=link",
    "/videokameri/c?ref=hp_menu_quick-nav_320_31&type=link",
    "/sportni-videokameri/c?ref=hp_menu_quick-nav_320_32&type=link",
    "/fotoaparati-dslr/c?ref=hp_menu_quick-nav_320_34&type=link",
    "/fotoaparati-mirrorless/c?ref=hp_menu_quick-nav_320_35&type=link",
    "/kompaktni-fotoaparati/c?ref=hp_menu_quick-nav_320_36&type=link",
    "/fotoaparati-za-momentni-snimki/c?ref=hp_menu_quick-nav_320_37&type=link",
    "/hardware-prenosimi-konzoli/brand/sony/c?ref=hp_menu_quick-nav_3096_1&type=link",
    "/hardware-prenosimi-konzoli/brand/microsoft/c?ref=hp_menu_quick-nav_3096_2&type=link",
    "/hardware-prenosimi-konzoli/brand/nintendo/c?ref=hp_menu_quick-nav_3096_3&type=link",
    "/kontroleri-volani-gaming-slushalki/c?ref=hp_menu_quick-nav_3096_5&type=link",
    "/kontroleri-volani-gaming-slushalki/filter/tip-produkt-f7552,gejmyrski-volan-v-11214852/c?ref=hp_menu_quick-nav_3096_6&type=link",
    "/vr-gaming-ochila/c?ref=hp_menu_quick-nav_3096_7&type=link",
    "/vr-gaming-aksiesoari/c?ref=hp_menu_quick-nav_3096_8&type=link",
    "/igri-konzola-kompiutyr/filter/platforma-f7609,playstation-v-7008037/syvmestimo-gejming-ustrojstvo-f7610,playstation-5-v-9180225/c?ref=hp_menu_quick-nav_3096_11&type=link",
    "/igri-konzola-kompiutyr/filter/platforma-f7609,playstation-v-7008037/syvmestimo-gejming-ustrojstvo-f7610,playstation-4-v-4419468/c?ref=hp_menu_quick-nav_3096_12&type=link",
    "/klaviaturi/filter/tip-f6330,gejming-v23011/c?ref=hp_menu_quick-nav_3096_17&type=link",
    "/mishki/filter/tip-f6329,gejming-v23008/c?ref=hp_menu_quick-nav_3096_18&type=link",
    "/podlozhki-za-mishka/filter/tip-f6327,gaming-v-2956968/c?ref=hp_menu_quick-nav_3096_19&type=link",
    "/slushalki-kompiutyr/filter/tip-f6328,gaming-v23004/c?ref=hp_menu_quick-nav_3096_20&type=link",
    "/nastolni-kompjutri/filter/prednaznachen-za-f7905,gaming-v-4670356/c?ref=hp_menu_quick-nav_3096_22&type=link",
    "/nastolni-kompjutri/brand/acer/filter/prednaznachen-za-f7905,gaming-v-4670356/c?ref=hp_menu_quick-nav_3096_23&type=link",
    "/nastolni-kompjutri/brand/lenovo/filter/prednaznachen-za-f7905,gaming-v-4670356/c?ref=hp_menu_quick-nav_3096_24&type=link",
    "/nastolni-kompjutri/brand/msi/filter/prednaznachen-za-f7905,gaming-v-4670356/c?ref=hp_menu_quick-nav_3096_25&type=link",
    "/nastolni-kompjutri/brand/serioux/filter/prednaznachen-za-f7905,gaming-v-4670356/c?ref=hp_menu_quick-nav_3096_26&type=link",
    "/procesori/c?ref=hp_menu_quick-nav_3096_30&type=link",
    "/solid-state-drive--ssd/c?ref=hp_menu_quick-nav_3096_31&type=link",
    "/hard-diskove/c?ref=hp_menu_quick-nav_3096_32&type=link",
    "/video-karti/c?ref=hp_menu_quick-nav_3096_33&type=link",
    "/operativna-pamet/c?ref=hp_menu_quick-nav_3096_34&type=link",
    "/dynni-platki/c?ref=hp_menu_quick-nav_3096_35&type=link",
    "/laptopi/brand/acer/filter/vid-laptop-f7882,gaming-v-17788/c?ref=hp_menu_quick-nav_3096_37&type=link",
    "/laptopi/brand/asus/filter/vid-laptop-f7882,gaming-v-17788/c?ref=hp_menu_quick-nav_3096_38&type=link",
    "/laptopi/brand/dell/filter/vid-laptop-f7882,gaming-v-17788/c?ref=hp_menu_quick-nav_3096_39&type=link",
    "/laptopi/brand/hewlett_packard/filter/vid-laptop-f7882,gaming-v-17788/c?ref=hp_menu_quick-nav_3096_40&type=link",
    "/hladilnici/c?ref=hp_menu_quick-nav_418_1&type=category",
    "/hladilnici-s-frizer/c?ref=hp_menu_quick-nav_418_2&type=category",
    "/hladilnici-side-by-side/c?ref=hp_menu_quick-nav_418_3&type=category",
    "/hladilni-vitrini/c?ref=hp_menu_quick-nav_418_4&type=category",
    "/vertikalni-frizeri/c?ref=hp_menu_quick-nav_418_5&type=category",
    "/frizerni-rakli/c?ref=hp_menu_quick-nav_418_6&type=category",
    "/sushilni-za-drehi/c?ref=hp_menu_quick-nav_418_9&type=link",
    "/sydomialni/c?ref=hp_menu_quick-nav_418_11&type=link",
    "/peralni/filter/tip-montirane-f7942,vgrazhdane-v-4744918/c?ref=hp_menu_quick-nav_418_13&type=link",
    "/gotvarski-pechki/c?ref=hp_menu_quick-nav_418_20&type=category",
    "/kotloni/c?ref=hp_menu_quick-nav_418_21&type=category",
    "/elektricheski-furni/c?ref=hp_menu_quick-nav_418_22&type=category",
    "/mikrovylnovi-furni/c?ref=hp_menu_quick-nav_418_23&type=category",
    "/absorbatori/c?ref=hp_menu_quick-nav_418_24&type=link",
    "/klimatici/c?ref=hp_menu_quick-nav_418_26&type=category",
    "/shopping-assistant/klimatik?ref=hp_menu_quick-nav_418_127&type=link",
    "/shopping-assistant/peralni?ref=hp_menu_quick-nav_418_127&type=link",
    "/shopping-assistant/hladilnitsi-side-by-side?ref=hp_menu_quick-nav_418_127&type=link",
    "/prahosmukachki/c?ref=hp_menu_quick-nav_2026_1&type=link",
    "/iutii-parogeneratori-gladachni-presi/c?ref=hp_menu_quick-nav_2026_2&type=link",
    "/parochistachki-elektricheski-mopove/c?ref=hp_menu_quick-nav_2026_3&type=link",
    "/shevni-mashini/c?ref=hp_menu_quick-nav_2026_4&type=link",
    "/aksesoari-chasti-za-prahosmukachki/c?ref=hp_menu_quick-nav_2026_5&type=link",
    "/koshove-za-prane-legeni/c?ref=hp_menu_quick-nav_2026_6&type=link",
    "/dyski-za-gladene/c?ref=hp_menu_quick-nav_2026_7&type=link",
    "/espreso-mashini/c?ref=hp_menu_quick-nav_2026_10&type=link",
    "/kafemashini/c?ref=hp_menu_quick-nav_2026_11&type=link",
    "/kafemelachki/c?ref=hp_menu_quick-nav_2026_12&type=link",
    "/elektricheski-skari/c?ref=hp_menu_quick-nav_2026_16&type=link",
    "/fritiurnici/c?ref=hp_menu_quick-nav_2026_17&type=link",
    "/hlebopekarni/c?ref=hp_menu_quick-nav_2026_18&type=link",
    "/mikseri/c?ref=hp_menu_quick-nav_2026_19&type=link",
    "/mikseri/filter/tip-produkt-f7960,pasator-v-7192683/c?ref=hp_menu_quick-nav_2026_20&type=link",
    "/blenderi-chopyri/c?ref=hp_menu_quick-nav_2026_21&type=link",
    "/mesomelachki/c?ref=hp_menu_quick-nav_2026_22&type=link",
    "/elektricheski-furni/c?ref=hp_menu_quick-nav_2026_23&type=link",
    "/kuhnenski-roboti/c?ref=hp_menu_quick-nav_2026_24&type=link",
    "/multikukyri/c?ref=hp_menu_quick-nav_2026_25&type=link",
    "/tosteri/c?ref=hp_menu_quick-nav_2026_26&type=link",
    "/uredi-za-sandvichi/c?ref=hp_menu_quick-nav_2026_27&type=link",
    "/prahosmukachki/filter/tip-produkt-f7960,prahosmukachka-bez-torba-v-7355651/c?ref=hp_menu_quick-nav_2026_33&type=filter",
    "/prahosmukachki/filter/tip-produkt-f7960,prahosmukachka-s-torba-v-7355650/c?ref=hp_menu_quick-nav_2026_34&type=filter",
    "/prahosmukachki/filter/tip-produkt-f7960,vertikalna-prahosmukachka-v-7355652/c?ref=hp_menu_quick-nav_2026_35&type=filter",
    "/prahosmukachki/filter/tip-produkt-f7960,rychna-prahosmukachka-v-7355653/c?ref=hp_menu_quick-nav_2026_36&type=filter",
    "/prahosmukachki/filter/tip-produkt-f7960,robot-prahosmukachka-v-7355655/c?ref=hp_menu_quick-nav_2026_37&type=filter",
    "/espreso-mashini/filter/tip-f8257,avtomatichna-v-7313784/forma-kafe-f8769,na-zyrna-v-7305294/c?ref=hp_menu_quick-nav_2026_39&type=link",
    "/espreso-mashini/filter/tip-f8257,rychna-v-7313785/forma-kafe-f8769,dozi-v-7314156/forma-kafe-f8769,mljano-v-7305293/c?ref=hp_menu_quick-nav_2026_40&type=link",
    "/label/Selection-of-brands-for-her-?ref=hp_menu_quick-nav_1101_1&type=link",
    "/damski-chasovnici/c?ref=hp_menu_quick-nav_1101_3&type=link",
    "/damski-chanti/c?ref=hp_menu_quick-nav_1101_4&type=link",
    "/damski-bizhuta/c?ref=hp_menu_quick-nav_1101_5&type=link",
    "/damski-maratonki/c?ref=hp_menu_quick-nav_1101_7&type=link",
    "/obuvki-zheni/c?ref=hp_menu_quick-nav_1101_8&type=category",
    "/damski-sandali/c?ref=hp_menu_quick-nav_1101_9&type=link",
    "/damski-teniski/c?ref=hp_menu_quick-nav_1101_11&type=link",
    "/rokli/c?ref=hp_menu_quick-nav_1101_12&type=link",
    "/damski-dynki/c?ref=hp_menu_quick-nav_1101_13&type=link",
    "/label/Selection-of-brands-for-him?ref=hp_menu_quick-nav_1101_15&type=link",
    "/myzhki-chasovnici/c?ref=hp_menu_quick-nav_1101_17&type=link",
    "/m-zhki-ranici/c?ref=hp_menu_quick-nav_1101_18&type=link",
    "/myzhki-portfejli/c?ref=hp_menu_quick-nav_1101_19&type=link",
    "/myzhki-maratonki/c?ref=hp_menu_quick-nav_1101_21&type=link",
    "/obuvki-za-myzhe/c?ref=hp_menu_quick-nav_1101_22&type=link",
    "/myzhki-chehli/c?ref=hp_menu_quick-nav_1101_23&type=link",
    "/myzhki-teniski/c?ref=hp_menu_quick-nav_1101_25&type=link",
    "/myzhki-dynki/c?ref=hp_menu_quick-nav_1101_26&type=link",
    "/myzhki-pantaloni/c?ref=hp_menu_quick-nav_1101_27&type=link",
    "/kufari/c?ref=hp_menu_quick-nav_1101_33&type=link",
    "/pytni-chanti/c?ref=hp_menu_quick-nav_1101_34&type=link",
    "/aksesoari-za-pytuvane/c?ref=hp_menu_quick-nav_1101_35&type=link",
    "/epilatori/c?ref=hp_menu_quick-nav_549_1&type=category",
    "/aparati-za-grizha-poddryzhka-na-tialoto/c?ref=hp_menu_quick-nav_549_2&type=category",
    "/uredi-za-manikjur-i-pedikjur/c?ref=hp_menu_quick-nav_549_3&type=category",
    "/presi-za-kosa/c?ref=hp_menu_quick-nav_549_4&type=category",
    "/seshoari/c?ref=hp_menu_quick-nav_549_5&type=category",
    "/mashi-za-kosa/c?ref=hp_menu_quick-nav_549_6&type=category",
    "/rolki-za-kosa/c?ref=hp_menu_quick-nav_549_7&type=category",
    "/elektricheski-chetki-za-kosa/c?ref=hp_menu_quick-nav_549_8&type=category",
    "/mashinki-za-podstrigvane-trimeri/c?ref=hp_menu_quick-nav_549_9&type=category",
    "/elektricheski-samobrysnachki/c?ref=hp_menu_quick-nav_549_10&type=category",
    "/klasicheski-samobrysnachki/c?ref=hp_menu_quick-nav_549_11&type=category",
    "/aksesoari-za-elektricheski-samobrysnachki/c?ref=hp_menu_quick-nav_549_12&type=category",
    "/elektricheski-chetki-za-zybi/c?ref=hp_menu_quick-nav_549_15&type=category",
    "/termometri/c?ref=hp_menu_quick-nav_549_17&type=category",
    "/kantari/c?ref=hp_menu_quick-nav_549_18&type=category",
    "/uredi-za-masaj/c?ref=hp_menu_quick-nav_549_19&type=category",
    "/kremove-za-lice/c?ref=hp_menu_quick-nav_549_25&type=category",
    "/serumi-terapiq-za-lice/c?ref=hp_menu_quick-nav_549_26&type=link",
    "/pochistvashti-produkti-za-lice/c?ref=hp_menu_quick-nav_549_27&type=link",
    "/dush-gelove/c?ref=hp_menu_quick-nav_549_28&type=link",
    "/losioni-kremove-za-tialo/c?ref=hp_menu_quick-nav_549_29&type=link",
    "/dezodoranti-antiperspiranti/c?ref=hp_menu_quick-nav_549_30&type=link",
    "/shampoani/c?ref=hp_menu_quick-nav_549_31&type=link",
    "/maski-terapia-za-kosa/c?ref=hp_menu_quick-nav_549_32&type=category",
    "/boi-za-kosa-oksidanti/c?ref=hp_menu_quick-nav_549_33&type=category",
    "/slyncezashtitni-produkti/c?ref=hp_menu_quick-nav_549_34&type=category",
    "/prezervativi/c?ref=hp_menu_quick-nav_549_35&type=category",
    "/klasicheski-samobrysnachki/c?ref=hp_menu_quick-nav_549_37&type=category",
    "/aksesoari-za-klasicheski-samobrysnachki/c?ref=hp_menu_quick-nav_549_38&type=category",
    "/gradinski-stolove-shezlongi/c?ref=hp_menu_quick-nav_612_2&type=link",
    "/komplekti-gradinski-mebeli/c?ref=hp_menu_quick-nav_612_3&type=link",
    "/hamaci-liulki/c?ref=hp_menu_quick-nav_612_4&type=link",
    "/basejni/c?ref=hp_menu_quick-nav_612_5&type=link",
    "/ofis-stolove/c?ref=hp_menu_quick-nav_612_7&type=link",
    "/stolove/c?ref=hp_menu_quick-nav_612_8&type=link",
    "/kuhnensko-obzavezhdane/c?ref=hp_menu_quick-nav_612_9&type=link",
    "/matraci/c?ref=hp_menu_quick-nav_612_10&type=link",
    "/obzavezhdane-za-spalnia/c?ref=hp_menu_quick-nav_612_11&type=link",
    "/fotiojli/c?ref=hp_menu_quick-nav_612_12&type=link",
    "/divani/c?ref=hp_menu_quick-nav_612_13&type=link",
    "/tigani/c?ref=hp_menu_quick-nav_612_21&type=link",
    "/tendzheri/c?ref=hp_menu_quick-nav_612_22&type=link",
    "/nozhove-komplekti-nozhove/c?ref=hp_menu_quick-nav_612_23&type=link",
    "/filtrirashti-kani/c?ref=hp_menu_quick-nav_612_24&type=link",
    "/servizi-za-hranene/c?ref=hp_menu_quick-nav_612_25&type=link",
    "/stykleni-chashi/c?ref=hp_menu_quick-nav_612_26&type=link",
    "/perilni-preparati/c?ref=hp_menu_quick-nav_612_28&type=link",
    "/omekotiteli/c?ref=hp_menu_quick-nav_612_29&type=link",
    "/kilimi/c?ref=hp_menu_quick-nav_612_33&type=link",
    "/dekorativni-vyzglavnici/c?ref=hp_menu_quick-nav_612_34&type=link",
    "/kalyfi-stolove-divani/c?ref=hp_menu_quick-nav_612_35&type=link",
    "/kartini/c?ref=hp_menu_quick-nav_612_36&type=link",
    "/shopping-assistant/matraci?ref=hp_menu_quick-nav_612_38&type=link",
    "/shopping-assistant/nameri-podhodyashite-mebeli-za-gradinata?ref=hp_menu_quick-nav_612_39&type=link",
    "/shopping-assistant/baseyni?ref=hp_menu_quick-nav_612_40&type=link",
    "/shopping-assistant/barbekiuta?ref=hp_menu_quick-nav_612_41&type=link",
    "/hrana-za-kuche/c?ref=hp_menu_quick-nav_612_44&type=category",
    "/hrana-za-kotki/c?ref=hp_menu_quick-nav_612_45&type=category",
    "/hrana-za-ptici-grizachi/c?ref=hp_menu_quick-nav_612_46&type=category",
    "/hrana-za-ribi-vlechugi/c?ref=hp_menu_quick-nav_612_47&type=category",
    "/biskviti-i-nagradi/c?ref=hp_menu_quick-nav_612_48&type=category",
    "/legla-vyzglavnici-diusheci-za-domashni-liubimci-1/c?ref=hp_menu_quick-nav_612_50&type=category",
    "/chanti-transportni-artikuli-za-domashni-liubimci-1/c?ref=hp_menu_quick-nav_612_51&type=category",
    "/kletki-kyshtichki-koshari-kafezi/c?ref=hp_menu_quick-nav_612_52&type=category",
    "/kytove-za-igra-za-domashni-liubimci/c?ref=hp_menu_quick-nav_612_53&type=category",
    "/label/Pampers?ref=hp_menu_quick-nav_689_1&type=link",
    "/peleni-gashtichki/brand/huggies/c?ref=hp_menu_quick-nav_689_2&type=link",
    "/label/Pufies?ref=hp_menu_quick-nav_689_3&type=link",
    "/label/Petit-Dragon?ref=hp_menu_quick-nav_689_4&type=link",
    "/label/baby-wet-wipes?ref=hp_menu_quick-nav_689_5&type=link",
    "/kolichki/c?ref=hp_menu_quick-nav_689_8&type=link",
    "/detski-stolcheta-za-kola/c?ref=hp_menu_quick-nav_689_9&type=link",
    "/kolani-za-prohozhdane-bebeshko-kenguru/c?ref=hp_menu_quick-nav_689_10&type=category",
    "/mebeli-za-detska-staia/c?ref=hp_menu_quick-nav_689_12&type=category",
    "/bebeshki-legla-koshari/c?ref=hp_menu_quick-nav_689_13&type=link",
    "/stolcheta-za-hranene/c?ref=hp_menu_quick-nav_689_18&type=category",
    "/label/Baby-sterilizers-bottle-warmers-and-thermoses?ref=hp_menu_quick-nav_689_19&type=link",
    "/nakrajnici-aksesoari-za-hranene-bebeta/c?ref=hp_menu_quick-nav_689_20&type=category",
    "/label/Baby-bottles-teats-and-accessories?ref=hp_menu_quick-nav_689_21&type=link",
    "/label/Baby-rush-creams?ref=hp_menu_quick-nav_689_23&type=link",
    "/label/Hygiene-and-baby-care?ref=hp_menu_quick-nav_689_24&type=link",
    "/label/Bathroom-and-accessories?ref=hp_menu_quick-nav_689_25&type=link",
    "/label/Baby-and-children-scales?ref=hp_menu_quick-nav_689_26&type=link",
    "/label/%D0%9F%D0%BE%D0%BC%D0%BF%D0%B8-%D0%B7%D0%B0-%D0%BA%D1%8A%D1%80%D0%BC%D0%B0?ref=hp_menu_quick-nav_689_29&type=link",
    "/label/Maternity-cosmetics-and-Breastfeeding-accessories?ref=hp_menu_quick-nav_689_30&type=link",
    "/label/Baby-toys?ref=hp_menu_quick-nav_689_35&type=link",
    "/interaktivni-bebeshki-igrachki/c?ref=hp_menu_quick-nav_689_36&type=category",
    "/konstruktori/c?ref=hp_menu_quick-nav_689_37&type=category",
    "/batuti/c?ref=hp_menu_quick-nav_689_46&type=link",
    "/detski-basejni/c?ref=hp_menu_quick-nav_689_47&type=link",
    "/koli-prevozni-sredstva-za-deca/c?ref=hp_menu_quick-nav_689_48&type=category",
    "/detski-velosipedi/c?ref=hp_menu_quick-nav_689_49&type=category",
    "/veloergometri/c?ref=hp_menu_quick-nav_731_1&type=category",
    "/biagashti-pyteki/c?ref=hp_menu_quick-nav_731_2&type=category",
    "/multifunkcionalni-fitnes-uredi/c?ref=hp_menu_quick-nav_731_3&type=category",
    "/grebni-trenazhori/c?ref=hp_menu_quick-nav_731_4&type=category",
    "/uredi-za-koremni-presi/c?ref=hp_menu_quick-nav_731_5&type=category",
    "/tezhesti/c?ref=hp_menu_quick-nav_731_6&type=category",
    "/giri-dymbeli/c?ref=hp_menu_quick-nav_731_7&type=category",
    "/steperi/c?ref=hp_menu_quick-nav_731_8&type=category",
    "/fitnes-topki/c?ref=hp_menu_quick-nav_731_9&type=category",
    "/fitnes-aksesoari/c?ref=hp_menu_quick-nav_731_10&type=category",
    "/fitnes-lastici/c?ref=hp_menu_quick-nav_731_11&type=category",
    "/postelki-za-uprazhnenia/c?ref=hp_menu_quick-nav_731_12&type=category",
    "/vibro-platformi/c?ref=hp_menu_quick-nav_731_13&type=category",
    "/kamping-artikuli/sd?ref=hp_menu_quick-nav_731_15&type=subdepartment",
    "/lov-ribolov/sd?ref=hp_menu_quick-nav_731_16&type=subdepartment",
    "/roleri-trotinetki-skejtbordove/sd?ref=hp_menu_quick-nav_731_17&type=subdepartment",
    "/vodni-sportove/sd?ref=hp_menu_quick-nav_731_18&type=subdepartment",
    "/otborni-sportove/sd?ref=hp_menu_quick-nav_731_19&type=subdepartment",
    "/tenis-badminton-tenis-na-masa/sd?ref=hp_menu_quick-nav_731_20&type=subdepartment",
    "/sport-na-zakrito/sd?ref=hp_menu_quick-nav_731_21&type=subdepartment",
    "/zimni-sportove/sd?ref=hp_menu_quick-nav_731_22&type=subdepartment",
    "/kufari/c?ref=hp_menu_quick-nav_731_24&type=category",
    "/turisticheski-ranici/c?ref=hp_menu_quick-nav_731_25&type=category",
    "/sportni-ranici/c?ref=hp_menu_quick-nav_731_26&type=category",
    "/sportni-fitnes-chanti/c?ref=hp_menu_quick-nav_731_27&type=category",
    "/sportni-chasovnici/c?ref=hp_menu_quick-nav_731_29&type=category",
    "/sportni-ochila-leshti/c?ref=hp_menu_quick-nav_731_30&type=category",
    "/zashtitni-kaski-za-sport/c?ref=hp_menu_quick-nav_731_31&type=category",
    "/velosipedi/c?ref=hp_menu_quick-nav_731_33&type=category",
    "/instrumenti-za-velosiped/c?ref=hp_menu_quick-nav_731_34&type=category",
    "/svetlini-za-velosiped/c?ref=hp_menu_quick-nav_731_35&type=category",
    "/detski-velosipedi/c?ref=hp_menu_quick-nav_731_36&type=link",
    "/sportni-obuvki/c?ref=hp_menu_quick-nav_731_38&type=category",
    "/sportni-teniski/c?ref=hp_menu_quick-nav_731_39&type=category",
    "/sportni-pantaloni/c?ref=hp_menu_quick-nav_731_40&type=category",
    "/avto-dzhanti/c?ref=hp_menu_quick-nav_760_1&type=link",
    "/videoregistratori/c?ref=hp_menu_quick-nav_760_3&type=link",
    "/radio--cd--dvd-ple-ri-za-kola/c?ref=hp_menu_quick-nav_760_4&type=category",
    "/radiostancii-radarni-detektori/c?ref=hp_menu_quick-nav_760_5&type=category",
    "/avtoboks/c?ref=hp_menu_quick-nav_760_8&type=category",
    "/avto-stojka-za-velosiped/c?ref=hp_menu_quick-nav_760_9&type=category",
    "/avto-hladilnici/c?ref=hp_menu_quick-nav_760_10&type=link",
    "/prahosmukachki-za-avtomobil/c?ref=hp_menu_quick-nav_760_11&type=link",
    "/avtokozmetika/c?ref=hp_menu_quick-nav_760_12&type=link",
    "/bormashini-vintoverti/c?ref=hp_menu_quick-nav_760_17&type=category",
    "/perforatori/c?ref=hp_menu_quick-nav_760_18&type=category",
    "/elektricheski-trioni/c?ref=hp_menu_quick-nav_760_19&type=category",
    "/ygloshlajfi/c?ref=hp_menu_quick-nav_760_20&type=category",
    "/kompresori/c?ref=hp_menu_quick-nav_760_21&type=category",
    "/zavarychni-aparati/c?ref=hp_menu_quick-nav_760_22&type=category",
    "/kuhnenski-mivki/c?ref=hp_menu_quick-nav_760_24&type=category",
    "/label/%D0%A1%D0%BC%D0%B5%D1%81%D0%B8%D1%82%D0%B5%D0%BB%D0%B8?ref=hp_menu_quick-nav_760_25&type=link",
    "/ogledala-mebeli-za-bania/c?ref=hp_menu_quick-nav_760_26&type=category",
    "/betonobyrkachki-kolichki/c?ref=hp_menu_quick-nav_760_28&type=category",
    "/label/%D0%91%D0%BE%D0%B8-%D0%B8-%D0%9C%D0%B0%D0%B7%D0%B8%D0%BB%D0%BA%D0%B8?ref=hp_menu_quick-nav_760_29&type=link",
    "/label/%D0%92%D1%80%D0%B0%D1%82%D0%B8-%D0%B8-%D0%9F%D1%80%D0%BE%D0%B7%D0%BE%D1%80%D1%86%D0%B8-?ref=hp_menu_quick-nav_760_30&type=link",
    "/sejfove-kaseti-za-cennosti/c?ref=hp_menu_quick-nav_760_31&type=link",
    "/vodostrujki/c?ref=hp_menu_quick-nav_760_33&type=link",
    "/verizhni-trioni/c?ref=hp_menu_quick-nav_760_34&type=category",
    "/kosachki-za-treva/c?ref=hp_menu_quick-nav_760_35&type=category",
    "/vodni-pompi/c?ref=hp_menu_quick-nav_760_36&type=category",
    "/elektricheski-motorni-kosi/c?ref=hp_menu_quick-nav_760_37&type=category",
    "/motofrezi-motokultivatori/c?ref=hp_menu_quick-nav_760_39&type=category",
    "/unishtozhiteli-dokumenti/c?ref=hp_menu_quick-nav_2388_1&type=link",
    "/laminatori/c?ref=hp_menu_quick-nav_2388_2&type=link",
    "/konferentni-dyski/c?ref=hp_menu_quick-nav_2388_3&type=link",
    "/kopirna-hartia/c?ref=hp_menu_quick-nav_2388_4&type=link",
    "/pisheschi-sredstva-premium/c?ref=hp_menu_quick-nav_2388_5&type=link",
    "/cvetni-molivi-flumasteri/c?ref=hp_menu_quick-nav_2388_7&type=link",
    "/uchenicheski-ranici/c?ref=hp_menu_quick-nav_2388_8&type=link",
    "/akvarielni-boi-chietki-i-blokchieta-za-risuvanie/c?ref=hp_menu_quick-nav_2388_9&type=link",
    "/kafe/filter/forma-f8408,zyrna-v-6124346/c?ref=hp_menu_quick-nav_2388_15&type=link",
    "/kafe/filter/forma-f8408,kapsuli-v-6124344/c?ref=hp_menu_quick-nav_2388_16&type=link",
    "/kafe/filter/forma-f8408,dozi-v-6124347/c?ref=hp_menu_quick-nav_2388_17&type=link",
    "/kafe/filter/forma-f8408,smljano-v-6124343/c?ref=hp_menu_quick-nav_2388_18&type=link",
    "/kafe/filter/forma-f8408,raztvorimo-v-6124345/c?ref=hp_menu_quick-nav_2388_19&type=link",
    "/chajove/c?ref=hp_menu_quick-nav_2388_20&type=link",
    "/mineralna-voda/c?ref=hp_menu_quick-nav_2388_22&type=link",
    "/gazirani-napitki/c?ref=hp_menu_quick-nav_2388_23&type=link",
    "/naturalni-sokove/c?ref=hp_menu_quick-nav_2388_24&type=link",
    "/vino2/c?ref=hp_menu_quick-nav_2388_25&type=link",
    "/biri/c?ref=hp_menu_quick-nav_2388_26&type=link",
    "/whiskey/c?ref=hp_menu_quick-nav_2388_28&type=link",
    "/vodka/c?ref=hp_menu_quick-nav_2388_29&type=link",
    "/koniak-brendi/c?ref=hp_menu_quick-nav_2388_30&type=link",
    "/sladki-lakomstva/c?ref=hp_menu_quick-nav_2388_32&type=link",
    "/dietichni-naturalni-produkti/c?ref=hp_menu_quick-nav_2388_33&type=link",
    "/podpravki-miksove/c?ref=hp_menu_quick-nav_2388_34&type=link",
    "/konservi-byrzi-hrani/c?ref=hp_menu_quick-nav_2388_35&type=link",
    "/osnovni-hrani/c?ref=hp_menu_quick-nav_2388_36&type=link",
    "/chipsove-snaksove/c?ref=hp_menu_quick-nav_2388_37&type=link",
    "/zyrneni-hrani/c?ref=hp_menu_quick-nav_2388_38&type=link"
};
            categoriesLinks = categoriesLinks.Select(l => "https://www.emag.bg" + l).ToList();

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
            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(productUrl);

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);

            var productName = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='page-title']")?.InnerText;
            var productPrice = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='product-new-price']")?.InnerText;
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

                int categoryId = await categoriesRepo.AllAsNoTracking()
                    .Where(c => c.Name.Contains(categoryName))
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();

                if (categoryId != 0)
                {
                    var product = new Product
                    {
                        Name = productName,
                        Description = productDescription,
                        Price = productPriceDecimal,
                        CategoryId = categoryId,
                        Quantity = 1000,
                        UserId = "e6cc76b9-19a0-4753-bcac-cea4a41df3b3",
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
    }
}
