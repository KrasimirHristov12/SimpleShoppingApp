using NUnit.Framework;
using SimpleShoppingApp.Services.NameShortener;

namespace SimpleShoppingApp.Tests
{
    public class NameShortenerServiceTests
    {
        private INameShortenerService service;

        [SetUp]
        public void Initialize()
        {
            service = new NameShortenerService();
        }

        [Test]
        public void TestWhenTheNameIsShortAndWantToGetTheWholeName()
        {
            string name = "abcd";

            string shortenedName = service.Shorten(name);

            Assert.That(shortenedName, Is.EqualTo(name));
        }

        [Test]
        public void TestWhenNameIsShortButWantToGetPartOfTheWholeName()
        {
            string name = "abcd";
            string shortenedName = service.Shorten(name, 3);
            Assert.That(shortenedName, Is.EqualTo("abc" + "..."));
        }

        [Test]
        public void TestWhenNameIsShortAndWantToGetMoreThanNameLength()
        {
            string name = "abcd";
            string shortenedName = service.Shorten(name, 50);
            Assert.That(shortenedName, Is.EqualTo("abcd"));
        }

        [Test]
        public void TestWhenNameIsLongAndWantToGetTheWholeName()
        {
            string name = new string('a', 5000);
            string shortenedName = service.Shorten(name);
            Assert.That(shortenedName, Is.EqualTo(new string('a', 50) + "..."));
        }

        [Test]
        public void TestWhenNameIsLongAndWantToGetPartOfTheWholeName()
        {
            string name = new string('a', 5000);
            string shortenedName = service.Shorten(name, 5);
            Assert.That(shortenedName, Is.EqualTo(new string('a', 5) + "..."));
        }

        [Test]
        public void TestWhenNameIsLongAndWantToGetMoreThanNameLength()
        {
            string name = new string('a', 5000);
            string shortenedName = service.Shorten(name, 50000);
            Assert.That(shortenedName, Is.EqualTo(new string('a', 5000)));
        }
    }
}
