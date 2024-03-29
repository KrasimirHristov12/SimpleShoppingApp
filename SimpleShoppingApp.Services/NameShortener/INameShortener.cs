namespace SimpleShoppingApp.Services.NameShortener
{
    public interface INameShortener
    {
        string Shorten(int symbolsToDisplay, string name);
    }
}
