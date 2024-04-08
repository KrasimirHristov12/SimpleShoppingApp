using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SimpleShoppingApp.Data;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Services.Addresses;

namespace SimpleShoppingApp.Tests
{
    public class AddressesServiceTests
    {
        private ApplicationDbContext db;
        private Repository<ShippingAddress> repository;
        private AddressesService service;

        [SetUp]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            db = new ApplicationDbContext(options);
            repository = new Repository<ShippingAddress>(db);
            service = new AddressesService(repository);
        }

        [Test]
        public async Task TestAddAddress()
        {
            for (int i = 0; i < 10; i++)
            {
                await service.AddAsync("Address " + (i + 1).ToString(), string.Empty);
            }

            var addressesCount = await repository.AllAsNoTracking().CountAsync();

            Assert.That(addressesCount, Is.EqualTo(10));
        }

        [Test]
        public async Task TestAddressViewModel()
        {
            var address = await service.AddAsync("Address 1", string.Empty);

            Assert.IsNotNull(address);

            Assert.That(address.Name, Is.EqualTo("Address 1"));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestDeleteAddressWhenIdIsNotAPositiveNumber(int id)
        {
            var result = await service.DeleteAsync(id, string.Empty);

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        [TestCase(1)]
        [TestCase(100)]
        [TestCase(10000)]
        public async Task TestDeleteAddressWhenAddressDoesNotExist(int id)
        {
            var result = await service.DeleteAsync(id, string.Empty);

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestDeleteAddressWhenAnotherUserTriesToDeleteIt()
        {
            await service.AddAsync("Address 1", "CreatorUserId");
            var result = await service.DeleteAsync(1, "AnotherUserId");
            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Forbidden));
        }

        [Test]
        public async Task TestSuccessfulDelete()
        {
            await service.AddAsync("Address 1", "CreatorUserId");
            var result = await service.DeleteAsync(1, "CreatorUserId");
            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Success));
        }

        [Test]
        public async Task TestGettingAddressesForUser() 
        {
            for (int i = 0; i < 10; i++)
            {
                await service.AddAsync("Address " + i.ToString(), "CreatorUserId");
            }

            var addresses = await service.GetAllForUserAsync("CreatorUserId");

            Assert.That(addresses.Count(), Is.EqualTo(10));
        
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestGetByIdAsyncWhenIdIsNotAPositiveNumber(int id)
        {
            var result = await service.GetByIdAsync(id);

            Assert.IsNull(result);
        }

        public async Task TestGetByIdWhenAddressExists()
        {
            for (int i = 0; i < 10; i++)
            {
                await service.AddAsync("Address " + i.ToString(), string.Empty);
            }

            for (int i = 1; i <= 10; i++)
            {
                var result = await service.GetByIdAsync(i);
                Assert.IsNotNull(result);
            }
            
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestIfAddressExistsWhenIdIsNotAPositiveNumber(int id)
        {
            var result = await service.DoesAddressExistAsync(id);
            Assert.IsFalse(result);
        }
        [Test]
        [TestCase(2)]
        [TestCase(50)]
        [TestCase(100)]
        public async Task TestIfAddressExistsWhenIdIsInvalid(int id)
        {
            await service.AddAsync("Address 1", string.Empty);
            var result = await service.DoesAddressExistAsync(id);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestIfAddressExistsWhenIdIsValid()
        {
            for (int i = 0; i < 10; i++)
            {
                await service.AddAsync("Address " + (i + 1).ToString(), string.Empty);
            }

            for (int i = 1; i <= 10; i++)
            {
                var result = await service.DoesAddressExistAsync(i);
                Assert.IsTrue(result);
            }
            
        }
    }
}
