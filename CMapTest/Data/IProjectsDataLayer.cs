using CMapTest.Models;

namespace CMapTest.Data
{
    public interface IProjectsDataLayer
    {
        Task<Project> GetProjectFromId(int id, CancellationToken cancellationToken);
        Task<IEnumerable<Project>> GetAllProjects(CancellationToken cancellationToken);
        Task<Project> CreateProject(Project project, CancellationToken cancellationToken);
        Task RemoveProject(int id, CancellationToken cancellationToken);
    }
}
