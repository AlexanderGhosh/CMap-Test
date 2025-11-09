using CMapTest.Exceptions;
using CMapTest.Models;
using System.Collections.Concurrent;

namespace CMapTest.Data
{
    public sealed partial class DataLayer : IUserDataLayer
    {
        private readonly ConcurrentDictionary<int, User> _users = [];

        public Task<User> GetUserFromId(int id, CancellationToken cancellationToken)
        {
            if (_users.TryGetValue(id, out User? user)) return Task.FromResult(user);
            throw new OperationFailedException("Cannot find User with Id: {id}");
        }
        public Task<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken) => Task.FromResult(_users.Select(vkp => vkp.Value));
        public Task<User> CreateUser(User user, CancellationToken cancellationToken)
        {
            user.Id = nextId(_users);
            _users.TryAdd(user.Id, user);
            return Task.FromResult(user);
        }

        private void assertUserExists(int userId)
        {
            if (!_users.ContainsKey(userId))
                throw new OperationFailedException($"Cannot find User with id {userId}");
        }
    }
}
