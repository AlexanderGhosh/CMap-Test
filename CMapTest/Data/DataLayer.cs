using CMapTest.Exceptions;
using CMapTest.Models;
using System.Collections.Concurrent;

namespace CMapTest.Data
{
    // at work the data layer tends to call funcs from an other class for the DB work but in this case beacuse in mem storage is so simple im not gonna do that
    // but if i were in this case i would have some kind of IDataStorage which could be swapped for a DB implementation
    public sealed class DataLayer : IDataLayer
    {
        private readonly ConcurrentDictionary<int, User> _users = [];
        private readonly ConcurrentDictionary<int, Project> _projects = [];
        private readonly ConcurrentDictionary<int, Entry> _entries = [];
        public Task<Entry> CreateEntry(Entry entry, CancellationToken cancellationToken)
        {
            assertUserExists(entry.UserId);
            assertProjectExists(entry.ProjectId);
            if (isDuplicateEntry(entry)) throw new OperationFailedException("Cannot add duplicate entries");
            entry.Id = nextId(_entries); // assigned to variable so i can debug easier if needed
            _entries.TryAdd(entry.Id, entry);
            return Task.FromResult(entry);
        }

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
