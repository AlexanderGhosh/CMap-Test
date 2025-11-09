using CMapTest.Models;

namespace CMapTest.Data
{
    public interface IUserDataLayer
    {
        Task<User> GetUserFromId(int id, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken);
        Task<User> CreateUser(User user, CancellationToken cancellationToken);
    }
}
