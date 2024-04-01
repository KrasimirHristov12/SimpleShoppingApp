namespace SimpleShoppingApp.Services.NameShortener
{
    public interface INameShortenerService
    {
        string Shorten(int symbolsToDisplay, string name);
    }
}
