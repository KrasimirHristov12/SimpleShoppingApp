using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Data;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Users;
using Moq;

namespace SimpleShoppingApp.Tests
{
    public class NotificationsServiceTests
    {
        private ApplicationDbContext db;
        private Repository<Notification> repository;
        private NotificationsService service;
        private IUsersService usersService;
        [SetUp]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.DoesUserExistAsync("NotExistingUser"))
                .ReturnsAsync(false);

            usersServiceMock.Setup(x => x.DoesUserExistAsync("ExistingUser"))
                .ReturnsAsync(true);

            usersServiceMock.Setup(x => x.DoesUserExistAsync("ExistingUser2"))
                .ReturnsAsync(true);

            usersService = usersServiceMock.Object;

            db = new ApplicationDbContext(options);
            repository = new Repository<Notification>(db);
            service = new NotificationsService(usersService, repository);
        }


        [Test]
        public async Task TestAddNotificationWhenSenderUserDoesNotExist()
        {
            var result = await service.AddAsync("NotExistingUser", string.Empty, string.Empty);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestAddNotificationWhenReceiverUserDoesNotExist()
        {
            var result = await service.AddAsync(string.Empty, "NotExistingUser", string.Empty);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestAddNotificationWhenTheSameUserSendsAndReceivesNotification()
        {
            var result = await service.AddAsync("ExistingUser", "ExistingUser", string.Empty);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestSuccessfulAddingOfNotification()
        {
            var result = await service.AddAsync("ExistingUser", "ExistingUser2", "Some text");
            Assert.IsTrue(result);
        }

        [Test]
        public async Task TestGetNotificationsWhenUserDoesNotHaveAnyNotifications()
        {
            for (int i = 0; i < 10; i++)
            {
                await service.AddAsync("ExistingUser", "ExistingUser2", "Some text" + (i + 1).ToString());
            }

            var notifications = await service.GetNotificationsAsync(11, "ExistingUser");

            Assert.That(notifications.Notifications.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task TestSuccessfulGetNotifications()
        {
            for (int i = 0; i < 10; i++)
            {
                await service.AddAsync("ExistingUser", "ExistingUser2", "Some text" + (i+1).ToString());
            }

            var notifications = await service.GetNotificationsAsync(11, "ExistingUser2");

            Assert.That(notifications.Notifications.Count(), Is.EqualTo(10));
        }

        [Test]
        public async Task TestReadNotificationsWhenNotificationDoesNotExist()
        {
            var result = await service.ReadAsync("ExistingUser", 1);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestReadNotificationsWhenUserDoesNotExist()
        {
            await service.AddAsync("ExistingUser", "ExistingUser2", "Some text");

            var result = await service.ReadAsync("NotExistingUser", 1);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestSuccessfulReadNotification()
        {
            await service.AddAsync("ExistingUser", "ExistingUser2", "Some text");
            var result = await service.ReadAsync("ExistingUser2", 1);
            Assert.IsTrue(result);
        }


    }
}
