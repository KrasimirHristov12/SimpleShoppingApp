using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Data;
using SimpleShoppingApp.Services.Categories;

namespace SimpleShoppingApp.Tests
{
    public class CategoriesServiceTests
    {
        private ApplicationDbContext db;
        private Repository<Category> repository;
        private CategoriesService service;

        [SetUp]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            db = new ApplicationDbContext(options);
            repository = new Repository<Category>(db);
            service = new CategoriesService(repository);
        }

        [Test]
        public async Task TestAddingACategoryWhenThereIsCategoryWithTheSameName()
        {
            await service.AddAsync("cat 1");
            var result = await service.AddAsync("cat 1");
            Assert.IsFalse(result);
        }

        public async Task TestAddingACategorySuccessfully()
        {
            var result = await service.AddAsync("cat 1");
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestIfCategoryDoesNotExistWhenIdIsNotAPositiveNumber(int id)
        {
            var result = await service.DoesCategoryExistAsync(id);

            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(2)]
        [TestCase(50)]
        [TestCase(100)]
        public async Task TestIfCategoryDoesNotExistWhenIdIsInvalid(int id)
        {
            await service.AddAsync("cat 1");

            var result = await service.DoesCategoryExistAsync(id);

            Assert.IsFalse(result);

        }

        [Test]
        public async Task TestIfCategoryExistsWhenIdIsValid()
        {
            for (int i = 0; i < 10; i++)
            {
                await service.AddAsync("cat " + (i+1).ToString());
            }

            for (int i = 1; i <= 10; i++)
            {
                var result = await service.DoesCategoryExistAsync(i);
                Assert.IsTrue(result);
            }
        }

        [Test]

        public async Task TestGetCount()
        {
            for (int i = 0; i < 10; i++)
            {
                await service.AddAsync("cat " + (i + 1).ToString());
            }

            var result = await service.GetCountAsync();

            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public async Task TestGetAll()
        {
            for (int i = 0; i < 10; i++)
            {
                await service.AddAsync("cat " + (i + 1).ToString());
            }

            var result = await service.GetAllAsync();

            var resultToList = result.ToList();

            Assert.That(resultToList.Count, Is.EqualTo(10));

            for (int i = 0; i < resultToList.Count; i++)
            {
                Assert.That(resultToList[i].Name, Is.EqualTo("cat " + (i + 1).ToString()));
            }
        }
    }
}
