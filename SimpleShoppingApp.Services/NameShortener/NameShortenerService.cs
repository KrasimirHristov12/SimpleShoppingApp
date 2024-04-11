namespace SimpleShoppingApp.Services.NameShortener
{
    public class NameShortenerService : INameShortenerService
    {
        public string Shorten(string name, int symbolsToDisplay = 50)
        {
            int length = symbolsToDisplay >= name.Length ? name.Length : symbolsToDisplay;
            string shortenedName = name.Substring(0, length);
            return shortenedName == name ? name : shortenedName + "...";
        }
    }
}
