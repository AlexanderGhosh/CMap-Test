using CMapTest.Auth;
using CMapTest.Data;
using CMapTest.Exceptions;
using CMapTest.Models;
using Moq;

namespace UnitTests.Data
{
    public class ProjectDataTests
    {
        [Fact]
        public async Task CreateOneProject()
        {
            IProjectsDataLayer projects = mockProjectLayer();

            Project creating = new Project()
            {
                Id = -1,
                Name = "Test N",
                Description = "Test D",
                Enabled = true
            };
            Project created = await assertProjectCreation(projects, creating, 0);

            Project gotton = await projects.GetProjectFromId(created.Id, default);
            Assert.NotNull(gotton);


            Assert.Equal(created.Id, gotton.Id);
            Assert.Equal(created.Name, gotton.Name);
            Assert.Equal(created.Description, gotton.Description);
            Assert.Equal(created.Enabled, gotton.Enabled);
        }

        [Fact]
        public async Task CreateTwoProjects()
        {
            IProjectsDataLayer projects = mockProjectLayer();

            Project creating1 = new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            };
            Project creating2 = new Project()
            {
                Id = -1,
                Name = "Test N 2",
                Description = "Test D 2",
                Enabled = true
            };
            Project created1 = await assertProjectCreation(projects, creating1, 0);
            Project created2 = await assertProjectCreation(projects, creating2, 1);

            Project gotton1 = await projects.GetProjectFromId(created1.Id, default);
            Assert.NotNull(gotton1);

            Assert.Equal(created1.Id, gotton1.Id);
            Assert.Equal(created1.Name, gotton1.Name);
            Assert.Equal(created1.Description, gotton1.Description);
            Assert.Equal(created1.Enabled, gotton1.Enabled);

            Project gotton2 = await projects.GetProjectFromId(created2.Id, default);
            Assert.NotNull(gotton2);

            Assert.Equal(created2.Id, gotton2.Id);
            Assert.Equal(created2.Name, gotton2.Name);
            Assert.Equal(created2.Description, gotton2.Description);
            Assert.Equal(created2.Enabled, gotton2.Enabled);
        }

        [Theory]
        [InlineData(-2), InlineData(-1), InlineData(0), InlineData(1), InlineData(2)]
        public async Task GetMissingProject(int projectId)
        {
            IProjectsDataLayer projects = mockProjectLayer();

            await Assert.ThrowsAsync<OperationFailedException>(async () => await projects.GetProjectFromId(projectId, default));
        }

        [Theory]
        [InlineData(-2), InlineData(-1), InlineData(0), InlineData(1), InlineData(2)]
        public async Task DeleteMissingProject(int projectId)
        {
            IProjectsDataLayer projects = mockProjectLayer();

            await Assert.ThrowsAsync<OperationFailedException>(async () => await projects.RemoveProject(projectId, default));
        }

        [Fact]
        public async Task DeleteProject()
        {
            IProjectsDataLayer projects = mockProjectLayer();

            Project creating = new Project()
            {
                Id = -1,
                Name = "Test N",
                Description = "Test D",
                Enabled = true
            };
            Project p = await assertProjectCreation(projects, creating, 0);
            await projects.RemoveProject(p.Id, default);
            await Assert.ThrowsAsync<OperationFailedException>(async () => await projects.GetProjectFromId(p.Id, default));
        }

        [Fact]
        public async Task GetAllNon()
        {
            IProjectsDataLayer projects = mockProjectLayer();
            var us = await projects.GetAllProjects(default);
            Assert.NotNull(us);
            Assert.Empty(us);
        }

        [Fact]
        public async Task GetAll2()
        {
            IProjectsDataLayer projects = mockProjectLayer();

            Project creating1 = new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            };
            Project creating2 = new Project()
            {
                Id = -1,
                Name = "Test N 2",
                Description = "Test D 2",
                Enabled = true
            };
            Project created1 = await assertProjectCreation(projects, creating1, 0);
            Project created2 = await assertProjectCreation(projects, creating2, 1);
            var ps = await projects.GetAllProjects(default);
            Assert.NotNull(ps);
            Assert.NotEmpty(ps);
            Assert.Contains(ps, p => p == creating1);
            Assert.Contains(ps, p => p == creating2);
        }


        private IProjectsDataLayer mockProjectLayer()
        {
            Mock<IServiceProvider> _servicesMock = new Mock<IServiceProvider>();
            IAuthService authMoc = Mock.Of<IAuthService>();
            _servicesMock.Setup(s => s.GetService(typeof(IAuthService))).Returns(authMoc);
            IProjectsDataLayer Projects = new DataLayer(_servicesMock.Object);
            return Projects;
        }

        private async Task<Project> assertProjectCreation(IProjectsDataLayer Projects, Project Project, int exceptedId)
        {
            Project created = await Projects.CreateProject(Project, default);
            Assert.NotNull(created);

            Assert.Equal(exceptedId, created.Id);
            Assert.Equal(Project.Name, created.Name);
            Assert.Equal(Project.Description, created.Description);
            Assert.Equal(Project.Enabled, created.Enabled);

            return created;
        }
    }
}