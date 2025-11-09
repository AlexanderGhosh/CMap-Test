using CMapTest.Auth;
using CMapTest.Exceptions;
using CMapTest.Models;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace CMapTest.Data
{
    public sealed partial class DataLayer : IAuthDataLayer
    {
        private readonly IAuthService _auth = _services.GetRequiredService<IAuthService>();
        private readonly ConcurrentDictionary<string, AuthUser> _authUsers = [];
        private readonly ConcurrentBag<UserClaim> _userClaims = [];
        public Task<User> LoginUser(LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!_authUsers.TryGetValue(loginRequest.Username, out AuthUser? authUser))
                throw new OperationFailedException("Username not found");

            _auth.VerifyPassword(loginRequest.Password, authUser.Password, cancellationToken);

            if (!_users.TryGetValue(authUser.UserId, out User? user))
                throw new Exception("Data corruption exception: Auth User exists but there is not corresponding User");
            return Task.FromResult(user);
        }

        public Task<IEnumerable<Claim>> GetUserClaims(int userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            assertUserExists(userId);
            return Task.FromResult(_userClaims.Where(c => c.UserId == userId).Select(c => new Claim(c.Type, c.Value)));
        }
        //Task.FromResult<IEnumerable<Claim>>([
        //    new Claim(ClaimTypes.Name, "Alexander Khumar Ghosh", null, _config.AuthenticationIssuer)
        //]);

        public async Task<User> SignUpUser(SignupUser signup, CancellationToken cancellationToken)
        {
            if (_authUsers.Any(kvp => kvp.Value.Username == signup.Username)) throw new OperationFailedException("That username is in use");
            var res = await _auth.IsPasswordStrongEnough(signup.Password, cancellationToken);
            if (!res.IsAdequate) throw new OperationFailedException($"Password does not meet the following criteria: {(res.FailedReason ?? "Unknown")}");
            User u = new()
            {
                Id = nextId(_users),
                FirstName = signup.FirstName,
                LastName = signup.LastName,
                OtherNames = signup.OtherNames,
                PreferredName = signup.PreferredName
            };
            await CreateUser(u, default);
            AuthUser authUser = new()
            {
                UserId = u.Id,
                Username = signup.Username,
                Password = await _auth.GeneratePasswordHash(signup.Password, default)
            };
            return u;
        }
    }
}
