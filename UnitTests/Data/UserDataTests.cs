using CMapTest.Auth;
using CMapTest.Data;
using CMapTest.Exceptions;
using CMapTest.Models;
using Moq;

namespace UnitTests.Data
{
    public class UserDataTests
    {
        [Fact]
        public async Task CreateOneUser()
        {
            IUserDataLayer users = mockUserLayer();

            User creating = new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            };
            User created = await assertUserCreation(users, creating, 0);

            User gotton = await users.GetUserFromId(created.Id, default);
            Assert.NotNull(gotton);

            Assert.Equal(created.Id, gotton.Id);
            Assert.Equal(created.FirstName, gotton.FirstName);
            Assert.Equal(created.LastName, gotton.LastName);
            Assert.Equal(created.OtherNames, gotton.OtherNames);
            Assert.Equal(created.PreferredName, gotton.PreferredName);
        }

        [Fact]
        public async Task CreateTwoUsers()
        {
            IUserDataLayer users = mockUserLayer();

            User creating1 = new User()
            {
                Id = -1,
                FirstName = "Test FN 1",
                LastName = "Test LN 1",
                OtherNames = "Test ON 1",
                PreferredName = "Test PN 1"
            };
            User creating2 = new User()
            {
                Id = -1,
                FirstName = "Test FN 2",
                LastName = "Test LN 2",
                OtherNames = "Test ON 2",
                PreferredName = "Test PN 2"
            };
            User created1 = await assertUserCreation(users, creating1, 0);
            User created2 = await assertUserCreation(users, creating2, 1);

            User gotton1 = await users.GetUserFromId(created1.Id, default);
            Assert.NotNull(gotton1);

            Assert.Equal(created1.Id, gotton1.Id);
            Assert.Equal(created1.FirstName, gotton1.FirstName);
            Assert.Equal(created1.LastName, gotton1.LastName);
            Assert.Equal(created1.OtherNames, gotton1.OtherNames);
            Assert.Equal(created1.PreferredName, gotton1.PreferredName);

            User gotton2 = await users.GetUserFromId(created2.Id, default);
            Assert.NotNull(gotton2);

            Assert.Equal(created2.Id, gotton2.Id);
            Assert.Equal(created2.FirstName, gotton2.FirstName);
            Assert.Equal(created2.LastName, gotton2.LastName);
            Assert.Equal(created2.OtherNames, gotton2.OtherNames);
            Assert.Equal(created2.PreferredName, gotton2.PreferredName);
        }

        [Theory]
        [InlineData(-2), InlineData(-1), InlineData(0), InlineData(1), InlineData(2)]
        public async Task GetMissingUser(int userId)
        {
            IUserDataLayer users = mockUserLayer();

            await Assert.ThrowsAsync<OperationFailedException>(async () => await users.GetUserFromId(userId, default));
        }

        [Fact]
        public async Task GetAllNon()
        {
            IUserDataLayer users = mockUserLayer();
            var us = await users.GetAllUsers(default);
            Assert.NotNull(us);
            Assert.Empty(us);
        }

        [Fact]
        public async Task GetAll2()
        {
            IUserDataLayer users = mockUserLayer();

            User creating1 = new User()
            {
                Id = 0,
                FirstName = "Test FN 1",
                LastName = "Test LN 1",
                OtherNames = "Test ON 1",
                PreferredName = "Test PN 1"
            };
            User creating2 = new User()
            {
                Id = 1,
                FirstName = "Test FN 2",
                LastName = "Test LN 2",
                OtherNames = "Test ON 2",
                PreferredName = "Test PN 2"
            };
            User created1 = await assertUserCreation(users, creating1, 0);
            User created2 = await assertUserCreation(users, creating2, 1);
            var us = await users.GetAllUsers(default);
            Assert.NotNull(us);
            Assert.NotEmpty(us);
            Assert.Contains(us, u => u == creating1);
            Assert.Contains(us, u => u == creating2);
        }


        private IUserDataLayer mockUserLayer()
        {
            Mock<IServiceProvider> _servicesMock = new Mock<IServiceProvider>();
            IAuthService authMoc = Mock.Of<IAuthService>();
            _servicesMock.Setup(s => s.GetService(typeof(IAuthService))).Returns(authMoc);
            IUserDataLayer users = new DataLayer(_servicesMock.Object);
            return users;
        }

        private async Task<User> assertUserCreation(IUserDataLayer users, User user, int exceptedId)
        {
            User created = await users.CreateUser(user, default);
            Assert.NotNull(created);

            Assert.Equal(exceptedId, created.Id);
            Assert.Equal(user.FirstName, created.FirstName);
            Assert.Equal(user.LastName, created.LastName);
            Assert.Equal(user.OtherNames, created.OtherNames);
            Assert.Equal(user.PreferredName, created.PreferredName);
            return created;
        }
    }
}