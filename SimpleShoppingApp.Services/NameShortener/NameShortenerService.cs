namespace SimpleShoppingApp.Services.NameShortener
{
    public class NameShortenerService : INameShortenerService
    {
        public string Shorten(int symbolsToDisplay, string name)
        {
            int length = symbolsToDisplay >= name.Length ? name.Length : symbolsToDisplay;
            string shortenedName = name.Substring(0, length);
            return shortenedName == name ? name : shortenedName + "...";
        }
    }
}
