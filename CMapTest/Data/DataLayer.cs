using CMapTest.Auth;
using CMapTest.Config;
using CMapTest.Exceptions;
using CMapTest.Models;
using CMapTest.Utils;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace CMapTest.Data
{
    // at work the data layer tends to call funcs from an other class for the DB work but in this case beacuse in mem storage is so simple im not gonna do that
    // but if i were in this case i would have some kind of IDataStorage which could be swapped for a DB implementation
    public sealed class DataLayer(IAuthService _auth, IOptionsMonitor<AuthOptions> options) : IDataLayer
    {
        private readonly ConcurrentDictionary<string, AuthUser> _authUsers = [];
        private readonly ConcurrentDictionary<int, User> _users = [];
        private readonly ConcurrentDictionary<int, Project> _projects = [];
        private readonly ConcurrentDictionary<int, Entry> _entries = [];
        private readonly ConcurrentBag<UserClaim> _userClaims = [];
        private AuthOptions _config => options.CurrentValue;

        public Task<Entry> GetEntryFromId(int id, CancellationToken cancellationToken)
        {
            if (_entries.TryGetValue(id, out Entry? entry)) return Task.FromResult(entry);
            throw new OperationFailedException("$Cannot find Entry with Id: {id}");
        }

        public Task<Project> GetProjectFromId(int id, CancellationToken cancellationToken)
        {
            if (_projects.TryGetValue(id, out Project? proj)) return Task.FromResult(proj);
            throw new OperationFailedException("Cannot find Project with Id: {id}");
        }

        public Task<User> GetUserFromId(int id, CancellationToken cancellationToken)
        {
            if (_users.TryGetValue(id, out User? user)) return Task.FromResult(user);
            throw new OperationFailedException("Cannot find User with Id: {id}");
        }

        public Task<IEnumerable<Project>> GetAllProjects(CancellationToken cancellationToken) => Task.FromResult(_projects.Select(vkp => vkp.Value));

        public Task<Project> CreateProject(Project project, CancellationToken cancellationToken)
        {
            if (_projects.Any(kvp => kvp.Value.Name == project.Name)) throw new OperationFailedException("That project name is in use");
            project.Id = nextId(_projects);
            _projects.TryAdd(project.Id, project);
            return Task.FromResult(project);
        }
        public Task<User> CreateUser(User user, CancellationToken cancellationToken)
        {
            user.Id = nextId(_users);
            _users.TryAdd(user.Id, user);
            return Task.FromResult(user);
        }

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

        public void Seed()
        {
            _users.TryAdd(0, new User()
            {
                Id = 0,
                FirstName = "test",
                LastName = "test"
            });
            _authUsers.TryAdd("test", new AuthUser()
            {
                UserId = 0,
                Username = "test",
                Password = _auth.GeneratePasswordHash("test", default).Result
            });
            _userClaims.Add(new UserClaim()
            {
                UserId = 0,
                Type = ClaimTypes.Name,
                Value = "Alexander Ghosh"
            });
            _userClaims.Add(new UserClaim()
            {
                UserId = 0,
                Type = ClaimTypes.UserId,
                Value = "0"
            });
            _projects.TryAdd(0, new()
            {
                Id = 0,
                Name = "testProject",
                Enabled = true,
            });
            _entries.TryAdd(0, new()
            {
                Id = 0,
                UserId = 0,
                ProjectId = 0,
                Date = DateTime.Today,
                StartTime = new TimeOnly(0, 0),
                EndTime = new TimeOnly(1, 0),
                Description = "test entry description"
            });
        }


        private static int nextId<T>(IEnumerable<T> c) => c.Count();

        private bool isDuplicateEntry(Entry candidate)
        {
            return false;
        }

        // could obv separate this into a more general function which takes the message and collection but then id have to rely on the programmer using collections were T has Id
        // or add an interface and use that i think the convience and prettyness of haveing i base func is lost in the Interface and potential miss use
        private void assertUserExists(int userId)
        {
            if (!_users.ContainsKey(userId))
                throw new OperationFailedException($"Cannot find User with id {userId}");
        }

        private void assertProjectExists(int projectId)
        {
            if (!_projects.ContainsKey(projectId))
                throw new OperationFailedException($"Cannot find Project with id {projectId}");
        }

        private void assertEntryExists(int entryId)
        {
            if (!_entries.ContainsKey(entryId))
                throw new OperationFailedException($"Cannot find Entry with id {entryId}");
        }
    }
}
