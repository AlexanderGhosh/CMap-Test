using CMapTest.Models;
using System.Security.Claims;

namespace CMapTest.Data
{
    public interface IAuthDataLayer
    {

        Task<User> SignUpUser(SignupUser signup, CancellationToken cancellationToken);
        Task<User> LoginUser(LoginRequest loginRequest, CancellationToken cancellationToken);
        Task<IEnumerable<Claim>> GetUserClaims(int userId, CancellationToken cancellationToken);
    }
}
