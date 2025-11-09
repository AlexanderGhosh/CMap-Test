using CMapTest.Auth;
using CMapTest.Config;
using CMapTest.Utils;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests
{
    // TODO write generator to make test passwords
    public class AuthServiceTests
    {
        [Theory]
        [InlineData("Alex's test password")]
        public async Task HashTheSamePassword(string password)
        {
            AuthOptions o = new AuthOptions()
            {
                AuthenticationAudience = "",
                AuthenticationIssuer = "",
                CookieExpiry = TimeSpan.FromHours(10),
                PasswordLength = new IntRange(10),
                PasswordDigits = new IntRange(1),
                AllowNonAlphaNumeric = false,
                PasswordLowerCase = new IntRange(1),
                PasswordUpperCase = new IntRange(1)
            };
            IAuthService auth = mockAuthService(o);
            byte[] p1 = await auth.GeneratePasswordHash(password, default);
            byte[] p2 = await auth.GeneratePasswordHash(password, default);
            Assert.NotNull(p1);
            Assert.NotNull(p2);
            Assert.NotEqual(p1, p2);
        }

        [Theory]
        [InlineData("_IAmApassword001!", true)]
        [InlineData("123456789", false)] // too short
        [InlineData("1234567890", false)] // no characters
        [InlineData("abcdefghij", false)] // no numbers
        [InlineData("abcdefghij0", false)] // no upper
        [InlineData("ABCDEFGHIJ0", false)] // no lower
        [InlineData("Abcdefghij@#", false)] // non alpha numeric
        public async Task PasswordStrength(string password, bool success)
        {
            AuthOptions o = new AuthOptions()
            {
                AuthenticationAudience = "",
                AuthenticationIssuer = "",
                CookieExpiry = TimeSpan.FromHours(10),
                PasswordLength = new IntRange(10),
                PasswordDigits = new IntRange(1),
                AllowNonAlphaNumeric = false,
                PasswordLowerCase = new IntRange(1),
                PasswordUpperCase = new IntRange(1)
            };
            IAuthService auth = mockAuthService(o);
            PasswordStrengthResult res = await auth.IsPasswordStrongEnough(password, default);
            Assert.NotNull(res);
            Assert.Equal(success, res.IsAdequate);
        }

        [Theory]
        [InlineData("Alex's test password")]
        public async Task PasswordsMatchTheHash(string password)
        {
            AuthOptions o = new AuthOptions()
            {
                AuthenticationAudience = "",
                AuthenticationIssuer = "",
                CookieExpiry = TimeSpan.FromHours(10),
                PasswordLength = new IntRange(10),
                PasswordDigits = new IntRange(1),
                AllowNonAlphaNumeric = false,
                PasswordLowerCase = new IntRange(1),
                PasswordUpperCase = new IntRange(1)
            };
            IAuthService auth = mockAuthService(o);
            byte[] hash = await auth.GeneratePasswordHash(password, default);
            bool match = await auth.VerifyPassword(password, hash, default);
            Assert.True(match);
        }


        [Theory]
        [InlineData(0, 0)]
        [InlineData(-10, 10)]
        [InlineData(10, -10)]
        [InlineData(null, null)]
        [InlineData(0, null)]
        [InlineData(null, 10)]
        public void InRange(int? min, int? max)
        {
            IntRange range = new(min, max);
            min ??= int.MinValue;
            max ??= int.MaxValue;
            for (int i = -100; i < 100; i++)
            {
                bool a = range.InRange(i);
                if (i < min.Value || i > max.Value) Assert.False(a);
                else Assert.True(a);
            }
        }

        private IAuthService mockAuthService(AuthOptions options)
        {
            var optionsMonitorMock = new Mock<IOptionsMonitor<AuthOptions>>();
            optionsMonitorMock.Setup(o => o.CurrentValue).Returns(options);

            return new AuthService(optionsMonitorMock.Object);
        }
    }
}
