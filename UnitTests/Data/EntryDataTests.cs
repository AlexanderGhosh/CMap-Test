using CMapTest.Auth;
using CMapTest.Data;
using CMapTest.Exceptions;
using CMapTest.Models;
using Moq;

namespace UnitTests.Data
{
    public class EntryDataTests
    {
        private DataLayer? _dataLayer;

        [Fact]
        public async Task CreateOneEntry()
        {
            IEntriesDataLayer entries = mockEntryLayer();
            IUserDataLayer users = mockUserLayer();
            IProjectsDataLayer projects = mockProjectLayer();

            Entry creating = new()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today
            };
            await Assert.ThrowsAsync<OperationFailedException>(async () => await assertEntryCreation(entries, creating, 0));

            await assertUserCreation(users, new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            }, default);

            await Assert.ThrowsAsync<OperationFailedException>(async () => await assertEntryCreation(entries, creating, 0));

            await assertProjectCreation(projects, new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            }, default);
            Entry created = await assertEntryCreation(entries, creating, 0);

            Entry gotton = await entries.GetEntryFromId(created.Id, default);
            Assert.NotNull(gotton);

            Assert.Equal(created.Id, gotton.Id);
            Assert.Equal(created.UserId, gotton.UserId);
            Assert.Equal(created.ProjectId, gotton.ProjectId);
            Assert.Equal(created.Description, gotton.Description);
            Assert.Equal(created.StartTime, gotton.StartTime);
            Assert.Equal(created.EndTime, gotton.EndTime);
            Assert.Equal(created.Date, gotton.Date);
        }
        [Fact]
        public async Task CreateDuplicateEntry()
        {
            IEntriesDataLayer entries = mockEntryLayer();
            IUserDataLayer users = mockUserLayer();
            IProjectsDataLayer projects = mockProjectLayer();

            Entry creating = new()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today.AddDays(1)
            };
            await Assert.ThrowsAsync<OperationFailedException>(async () => await assertEntryCreation(entries, creating, 0));

            await assertUserCreation(users, new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            }, default);

            await Assert.ThrowsAsync<OperationFailedException>(async () => await assertEntryCreation(entries, creating, 0));

            await assertProjectCreation(projects, new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            }, default);
            Entry created = await assertEntryCreation(entries, creating, 0);
            await Assert.ThrowsAsync<OperationFailedException>(async () => await assertEntryCreation(entries, creating, 0));
        }
        [Fact]
        public async Task CreateTwoEntries()
        {
            IEntriesDataLayer entries = mockEntryLayer();
            IUserDataLayer users = mockUserLayer();
            IProjectsDataLayer projects = mockProjectLayer();

            Entry creating1 = new Entry()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 1",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today
            };
            Entry creating2 = new Entry()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 2",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today.AddDays(1)
            };


            await assertUserCreation(users, new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            }, default);
            await assertProjectCreation(projects, new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            }, default);
            Entry created1 = await assertEntryCreation(entries, creating1, 0);
            Entry created2 = await assertEntryCreation(entries, creating2, 1);

            Entry gotton1 = await entries.GetEntryFromId(created1.Id, default);
            Assert.NotNull(gotton1);

            Assert.Equal(created1.Id, gotton1.Id);
            Assert.Equal(created1.UserId, gotton1.UserId);
            Assert.Equal(created1.ProjectId, gotton1.ProjectId);
            Assert.Equal(created1.Description, gotton1.Description);
            Assert.Equal(created1.StartTime, gotton1.StartTime);
            Assert.Equal(created1.EndTime, gotton1.EndTime);
            Assert.Equal(created1.Date, gotton1.Date);

            Entry gotton2 = await entries.GetEntryFromId(created2.Id, default);
            Assert.NotNull(gotton2);

            Assert.Equal(created2.Id, gotton2.Id);
            Assert.Equal(created2.UserId, gotton2.UserId);
            Assert.Equal(created2.ProjectId, gotton2.ProjectId);
            Assert.Equal(created2.Description, gotton2.Description);
            Assert.Equal(created2.StartTime, gotton2.StartTime);
            Assert.Equal(created2.EndTime, gotton2.EndTime);
            Assert.Equal(created2.Date, gotton2.Date);
        }
        [Theory]
        [InlineData(-2), InlineData(-1), InlineData(0), InlineData(1), InlineData(2)]
        public async Task GetMissingEntry(int entryId)
        {
            IEntriesDataLayer entries = mockEntryLayer();

            await Assert.ThrowsAsync<OperationFailedException>(async () => await entries.GetEntryFromId(entryId, default));
        }
        [Theory]
        [InlineData(-2), InlineData(-1), InlineData(0), InlineData(1), InlineData(2)]
        public async Task DeleteMissingEntry(int entryId)
        {
            IEntriesDataLayer entries = mockEntryLayer();

            await Assert.ThrowsAsync<OperationFailedException>(async () => await entries.RemoveEntry(entryId, default));
        }
        [Fact]
        public async Task DeleteEntry()
        {
            IEntriesDataLayer entries = mockEntryLayer();
            IUserDataLayer users = mockUserLayer();
            IProjectsDataLayer projects = mockProjectLayer();

            Entry creating = new Entry()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today
            };

            await assertUserCreation(users, new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            }, default);
            await assertProjectCreation(projects, new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            }, default);
            Entry p = await assertEntryCreation(entries, creating, 0);
            await entries.RemoveEntry(p.Id, default);
            await Assert.ThrowsAsync<OperationFailedException>(async () => await entries.GetEntryFromId(p.Id, default));
        }
        [Fact]
        public async Task GetAllNon()
        {
            IEntriesDataLayer entries = mockEntryLayer();
            IUserDataLayer users = mockUserLayer();
            IProjectsDataLayer projects = mockProjectLayer();

            Entry creating = new Entry()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today
            };

            await assertUserCreation(users, new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            }, default);
            await assertProjectCreation(projects, new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            }, default);
            var us = await entries.GetAllEntries(default);
            Assert.NotNull(us);
            Assert.Empty(us);
        }
        [Fact]
        public async Task GetAll2()
        {
            IEntriesDataLayer entries = mockEntryLayer();
            IUserDataLayer users = mockUserLayer();
            IProjectsDataLayer projects = mockProjectLayer();

            Entry creating1 = new()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 1",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today
            };
            Entry creating2 = new()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 2",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today.AddDays(1)
            };

            await assertUserCreation(users, new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            }, default);
            await assertProjectCreation(projects, new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            }, default);
            Entry created1 = await assertEntryCreation(entries, creating1, 0);
            Entry created2 = await assertEntryCreation(entries, creating2, 1);

            var es = await entries.GetAllEntries(default);
            Assert.NotNull(es);
            Assert.NotEmpty(es);
            Assert.Contains(es, e => e.Id == creating1.Id);
            Assert.Contains(es, e => e.Id == creating2.Id);
        }
        [Fact]
        public async Task SearchForNon()
        {
            IEntriesDataLayer entries = mockEntryLayer();
            IUserDataLayer users = mockUserLayer();
            IProjectsDataLayer projects = mockProjectLayer();

            Entry creating1 = new()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 1",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today
            };
            Entry creating2 = new()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 2",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today.AddDays(1)
            };

            await assertUserCreation(users, new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            }, default);
            await assertProjectCreation(projects, new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            }, default);
            Entry created1 = await assertEntryCreation(entries, creating1, 0);
            Entry created2 = await assertEntryCreation(entries, creating2, 1);

            var searchRes = await entries.EntrySearch(new EntrySearchContext()
            {
                DateStart = DateTime.Today.AddYears(1000)
            }, default);
            Assert.NotNull(searchRes);
            Assert.Empty(searchRes);
        }
        [Fact]
        public async Task SearchFor2()
        {
            IEntriesDataLayer entries = mockEntryLayer();
            IUserDataLayer users = mockUserLayer();
            IProjectsDataLayer projects = mockProjectLayer();

            Entry creating1 = new()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 1",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today
            };
            Entry creating2 = new()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 2",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today.AddDays(1)
            };

            await assertUserCreation(users, new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            }, default);
            await assertProjectCreation(projects, new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            }, default);
            Entry created1 = await assertEntryCreation(entries, creating1, 0);
            Entry created2 = await assertEntryCreation(entries, creating2, 1);

            var searchRes = await entries.EntrySearch(null, default);
            Assert.NotNull(searchRes);
            Assert.True(searchRes.Count() == 2);

            searchRes = await entries.EntrySearch(new EntrySearchContext()
            {
                UserId = 0
            }, default);
            Assert.NotNull(searchRes);
            Assert.True(searchRes.Count() == 2);

            searchRes = await entries.EntrySearch(new EntrySearchContext()
            {
                UserId = 1
            }, default);
            Assert.NotNull(searchRes);
            Assert.False(searchRes.Any());

            searchRes = await entries.EntrySearch(new EntrySearchContext()
            {
                ProjectId = 0
            }, default);
            Assert.NotNull(searchRes);
            Assert.True(searchRes.Count() == 2);

            searchRes = await entries.EntrySearch(new EntrySearchContext()
            {
                ProjectId = 1
            }, default);
            Assert.NotNull(searchRes);
            Assert.False(searchRes.Any());

            searchRes = await entries.EntrySearch(new EntrySearchContext()
            {
                DateStart = DateTime.Today,
                DateEnd = DateTime.Today.AddDays(1)
            }, default);
            Assert.NotNull(searchRes);
            Assert.True(searchRes.Count() == 2);

            searchRes = await entries.EntrySearch(new EntrySearchContext()
            {
                DateStart = DateTime.Today,
                DateEnd = DateTime.Today
            }, default);
            Assert.NotNull(searchRes);
            Assert.True(searchRes.Count() == 1);
        }
        [Fact]
        public async Task UpdateEntry()
        {
            IEntriesDataLayer entries = mockEntryLayer();
            IUserDataLayer users = mockUserLayer();
            IProjectsDataLayer projects = mockProjectLayer();
            Entry creating = new()
            {
                Id = -1,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 1",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today
            };
            await assertUserCreation(users, new User()
            {
                Id = -1,
                FirstName = "Test FN",
                LastName = "Test LN",
                OtherNames = "Test ON",
                PreferredName = "Test PN"
            }, default);
            await assertProjectCreation(projects, new Project()
            {
                Id = -1,
                Name = "Test N 1",
                Description = "Test D 1",
                Enabled = true
            }, default);
            Entry created = await assertEntryCreation(entries, creating, 0);
            Entry updated = new()
            {
                Id = 0,
                UserId = 0,
                ProjectId = 0,
                Description = "Test D 2",
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                EndTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(1),
                Date = DateTime.Today
            };
            await entries.UpdateEntry(updated, default);
            Entry gotton = await entries.GetEntryFromId(created.Id, default);

            Assert.NotNull(gotton);

            Assert.Equal(updated.Id, gotton.Id);
            Assert.Equal(updated.UserId, gotton.UserId);
            Assert.Equal(updated.ProjectId, gotton.ProjectId);
            Assert.Equal(updated.Description, gotton.Description);
            Assert.Equal(updated.StartTime, gotton.StartTime);
            Assert.Equal(updated.EndTime, gotton.EndTime);
            Assert.Equal(updated.Date, gotton.Date);
        }

        private IUserDataLayer mockUserLayer()
        {
            Mock<IServiceProvider> _servicesMock = new Mock<IServiceProvider>();
            IAuthService authMoc = Mock.Of<IAuthService>();
            _servicesMock.Setup(s => s.GetService(typeof(IAuthService))).Returns(authMoc);
            return mockDataLayer();
        }

        private IProjectsDataLayer mockProjectLayer()
        {
            Mock<IServiceProvider> _servicesMock = new Mock<IServiceProvider>();
            IAuthService authMoc = Mock.Of<IAuthService>();
            _servicesMock.Setup(s => s.GetService(typeof(IAuthService))).Returns(authMoc);
            return mockDataLayer();
        }

        private IEntriesDataLayer mockEntryLayer()
        {
            Mock<IServiceProvider> _servicesMock = new Mock<IServiceProvider>();
            IAuthService authMoc = Mock.Of<IAuthService>();
            _servicesMock.Setup(s => s.GetService(typeof(IAuthService))).Returns(authMoc);
            return mockDataLayer();
        }

        private DataLayer mockDataLayer()
        {
            if (_dataLayer is not null) return _dataLayer;
            Mock<IServiceProvider> _servicesMock = new Mock<IServiceProvider>();
            IAuthService authMoc = Mock.Of<IAuthService>();
            _servicesMock.Setup(s => s.GetService(typeof(IAuthService))).Returns(authMoc);
            _dataLayer = new DataLayer(_servicesMock.Object);
            return _dataLayer;
        }

        private async Task<User> assertUserCreation(IUserDataLayer users, User user, int exceptedId)
        {
            User created = await users.CreateUser(user, default);
            Assert.NotNull(created);

            Assert.Equal(exceptedId, created.Id);
            Assert.Equal(user.FirstName, created.FirstName);
            Assert.Equal(user.LastName, created.LastName);
            Assert.Equal(user.OtherNames, created.OtherNames);
            Assert.Equal(user.PreferredName, created.PreferredName);
            return created;
        }

        private async Task<Project> assertProjectCreation(IProjectsDataLayer projects, Project Project, int exceptedId)
        {
            Project created = await projects.CreateProject(Project, default);
            Assert.NotNull(created);

            Assert.Equal(exceptedId, created.Id);
            Assert.Equal(Project.Name, created.Name);
            Assert.Equal(Project.Description, created.Description);
            Assert.Equal(Project.Enabled, created.Enabled);

            return created;
        }

        private async Task<Entry> assertEntryCreation(IEntriesDataLayer entries, Entry entry, int exceptedId)
        {
            Entry created = await entries.CreateEntry(entry, default);
            Assert.NotNull(created);

            Assert.Equal(exceptedId, created.Id);
            Assert.Equal(entry.UserId, created.UserId);
            Assert.Equal(entry.ProjectId, created.ProjectId);
            Assert.Equal(entry.Description, created.Description);
            Assert.Equal(entry.StartTime, created.StartTime);
            Assert.Equal(entry.EndTime, created.EndTime);
            Assert.Equal(entry.Date, created.Date);

            return created;
        }
    }
}