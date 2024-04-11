namespace SimpleShoppingApp.Services.NameShortener
{
    public interface INameShortenerService
    {
        string Shorten(string name, int symbolsToDisplay = 50);
    }
}
