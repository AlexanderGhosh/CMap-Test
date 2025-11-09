using CMapTest.Auth;
using CMapTest.Data;
using CMapTest.Exceptions;
using CMapTest.Models;
using CMapTest.Utils;
using Moq;
using System.Security.Claims;

namespace UnitTests.Data
{
    public class AuthDataTests
    {
        [Fact]
        public async Task LoginNonExistentUser()
        {
            IAuthDataLayer auth = mockAuthLayer();
            LoginRequest req = new()
            {
                Username = "Test UN",
                Password = "Test PW"
            };
            await Assert.ThrowsAsync<OperationFailedException>(async () => await auth.LoginUser(req, default));
        }
        [Fact]
        public async Task LoginUser()
        {
            IAuthDataLayer auth = mockAuthLayer();
            SignupUser signup = new()
            {
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN",
                Username = "Test UN",
                Password = "Test PW"
            };
            await assertSignup(auth, signup);
            LoginRequest req = new()
            {
                Username = signup.Username,
                Password = signup.Password
            };
            User u = await auth.LoginUser(req, default);
            Assert.NotNull(u);

            Assert.Equal(signup.FirstName, u.FirstName);
            Assert.Equal(signup.LastName, u.LastName);
            Assert.Equal(signup.OtherNames, u.OtherNames);
            Assert.Equal(signup.PreferredName, u.PreferredName);
        }

        [Fact]
        public async Task SignupNewUser()
        {
            IAuthDataLayer auth = mockAuthLayer();
            SignupUser signup = new()
            {
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN",
                Username = "Test UN",
                Password = "Test PW"
            };
            await assertSignup(auth, signup);
        }

        [Fact]
        public async Task GetUserClaims()
        {
            IAuthDataLayer auth = mockAuthLayer();
            SignupUser signup = new()
            {
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN",
                Username = "Test UN",
                Password = "Test PW"
            };
            User u = await assertSignup(auth, signup);
            var claims = await auth.GetUserClaims(u.Id, default);
            Assert.NotNull(claims);
            Assert.NotEmpty(claims);
            Claim? claim = claims.FirstOrDefault(static c => c.Type == Extensions.get_UserId());
            Assert.NotNull(claim);
            Assert.Equal("0", claim.Value);
        }

        [Fact]
        public async Task SignupExistingUser()
        {
            IAuthDataLayer auth = mockAuthLayer();
            SignupUser signup = new()
            {
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN",
                Username = "Test UN",
                Password = "Test PW"
            };
            await assertSignup(auth, signup);
            await Assert.ThrowsAsync<OperationFailedException>(async () => await assertSignup(auth, signup));
        }

        private async Task<User> assertSignup(IAuthDataLayer auth, SignupUser signup)
        {
            User created = await auth.SignUpUser(signup, default);
            Assert.NotNull(created);

            Assert.Equal(signup.FirstName, created.FirstName);
            Assert.Equal(signup.LastName, created.LastName);
            Assert.Equal(signup.OtherNames, created.OtherNames);
            Assert.Equal(signup.PreferredName, created.PreferredName);
            return created;
        }

        private IAuthDataLayer mockAuthLayer()
        {
            Mock<IServiceProvider> _servicesMock = new Mock<IServiceProvider>();
            Mock<IAuthService> authMock = new Mock<IAuthService>();
            authMock.Setup(a => a.VerifyPassword(It.IsAny<string>(), It.IsAny<byte[]>(), default)).Returns(Task.FromResult(true));
            authMock.Setup(a => a.IsPasswordStrongEnough(It.IsAny<string>(), default)).Returns(Task.FromResult(PasswordStrengthResult.Pass()));
            authMock.Setup(a => a.GeneratePasswordHash(It.IsAny<string>(), default)).Returns(Task.FromResult(Array.Empty<byte>()));
            _servicesMock.Setup(s => s.GetService(typeof(IAuthService))).Returns(authMock.Object);
            IAuthDataLayer auth = new DataLayer(_servicesMock.Object);
            return auth;
        }
    }
}
