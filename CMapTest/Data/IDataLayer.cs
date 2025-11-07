using CMapTest.Models;

namespace CMapTest.Data
{
    // this is async because future implementations might be better served by it
    public interface IDataLayer
    {
        Task<User> GetUserFromId(int id, CancellationToken cancellationToken);
        Task<Project> GetProjectFromId(int id, CancellationToken cancellationToken);
        Task<Entry> GetEntryFromId(int id, CancellationToken cancellationToken);

        Task<IEnumerable<Project>> GetAllProjects(CancellationToken cancellationToken);

        Task<Entry> CreateEntry(Entry entry, CancellationToken cancellationToken);
        Task<Project> CreateProject(Project project, CancellationToken cancellationToken);

        Task<User> LoginUser(LoginRequest loginRequest, CancellationToken cancellationToken);
    }
}
