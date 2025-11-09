using CMapTest.Models;
using CMapTest.Utils;
using System.Security.Claims;

namespace CMapTest.Data
{
    // at work the data layer tends to call funcs from an other class for the DB work but in this case beacuse in mem storage is so simple im not gonna do that
    // but if i were in this case i would have some kind of IDataStorage which could be swapped for a DB implementation
    public sealed partial class DataLayer(IServiceProvider _services)
#if DEBUG
        : IDataLayer
#endif
    {
        public void Seed()
        {
            _users.TryAdd(0, new User()
            {
                Id = 0,
                FirstName = "test user 0",
                LastName = "test user 0"
            });
            _users.TryAdd(1, new User()
            {
                Id = 1,
                FirstName = "test user 1",
                LastName = "test user 1"
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
            _userClaims.Add(new UserClaim()
            {
                UserId = 0,
                Type = ClaimTypes.UserRole,
                Value = UserRole.Admin.ToString()
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
            _entries.TryAdd(1, new()
            {
                Id = 1,
                UserId = 1,
                ProjectId = 0,
                Date = DateTime.Today.AddDays(1),
                StartTime = new TimeOnly(0, 0),
                EndTime = new TimeOnly(1, 0),
                Description = "test entry description"
            });
        }
        private static int nextId<T>(IEnumerable<T> c)
        {
            lock (c)
            {
                return c.Count();
            }
        }

    }
}
