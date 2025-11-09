using CMapTest.Exceptions;
using CMapTest.Models;
using System.Collections.Concurrent;

namespace CMapTest.Data
{
    public sealed partial class DataLayer : IProjectsDataLayer
    {
        private readonly ConcurrentDictionary<int, Project> _projects = [];
        public Task<Project> GetProjectFromId(int id, CancellationToken cancellationToken)
        {
            if (_projects.TryGetValue(id, out Project? proj)) return Task.FromResult(proj);
            throw new OperationFailedException("Cannot find Project with Id: {id}");
        }
        public Task<IEnumerable<Project>> GetAllProjects(CancellationToken cancellationToken) => Task.FromResult(_projects.Select(vkp => vkp.Value));
        public Task<Project> CreateProject(Project project, CancellationToken cancellationToken)
        {
            if (_projects.Any(kvp => kvp.Value.Name == project.Name)) throw new OperationFailedException("That project name is in use");
            project.Id = nextId(_projects);
            _projects.TryAdd(project.Id, project);
            return Task.FromResult(project);
        }
        public Task RemoveProject(int id, CancellationToken cancellationToken)
        {
            assertProjectExists(id);
            _projects.TryRemove(id, out _);
            return Task.CompletedTask;
        }
        private void assertProjectExists(int projectId, bool requireEnabled = true)
        {
            if (!_projects.TryGetValue(projectId, out Project? p))
                throw new OperationFailedException($"Cannot find Project with id {projectId}");
            else if (requireEnabled && !p.Enabled)
                throw new OperationFailedException($"Project with id {projectId} is not enabled");
        }
    }
}
