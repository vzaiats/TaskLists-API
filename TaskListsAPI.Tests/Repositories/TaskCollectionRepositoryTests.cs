using Microsoft.EntityFrameworkCore;
using TaskListsAPI.Domain.Entities;
using TaskListsAPI.Infrastructure.Database;
using TaskListsAPI.Infrastructure.Repositories;

namespace TaskListsAPI.Tests.Repositories
{
    public class TaskCollectionRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TaskCollectionRepository _taskCollectionRepository;

        #region Ctor

        public TaskCollectionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _taskCollectionRepository = new TaskCollectionRepository(_context);

            SeedDatabase().Wait();
        }

        #endregion

        #region Seed Data

        private async Task SeedDatabase()
        {
            var users = new List<User>
            {
                new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Alice" },
                new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Bob" }
            };

            var collections = new List<TaskCollection>
            {
                new TaskCollection("Alice Tasks", users[0].Id),
                new TaskCollection("Bob Tasks", users[1].Id)
            };

            _context.Users.AddRange(users);
            _context.TaskCollections.AddRange(collections);
            await _context.SaveChangesAsync();

            var tasks = new List<TaskItem>
            {
                new TaskItem("Task 1", collections[0].Id),
                new TaskItem("Task 2", collections[0].Id),
                new TaskItem("Task 3", collections[1].Id)
            };

            _context.TaskItems.AddRange(tasks);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region GetByIdAsync Tests

        [Theory]
        [InlineData("Alice Tasks")]
        [InlineData("Bob Tasks")]
        public async Task GetByIdAsync_ShouldReturnCollection_WhenExists(string collectionName)
        {
            // Arrange
            var collection = await _context.TaskCollections.FirstAsync(c => c.Name == collectionName);

            // Act
            var result = await _taskCollectionRepository.GetByIdAsync(collection.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(collectionName, result!.Name);
            Assert.NotNull(result.Tasks);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _taskCollectionRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCollections()
        {
            // Act
            var result = await _taskCollectionRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.NotNull(c.Name));
        }

        #endregion

        #region AddAsync Tests

        [Theory]
        [InlineData("New Collection", "11111111-1111-1111-1111-111111111111")]
        public async Task AddAsync_ShouldAddCollection(string name, string ownerIdStr)
        {
            // Arrange
            var ownerId = Guid.Parse(ownerIdStr);
            var newCollection = new TaskCollection(name, ownerId);

            // Act
            await _taskCollectionRepository.AddAsync(newCollection);
            await _taskCollectionRepository.SaveChangesAsync();

            var added = await _context.TaskCollections.FindAsync(newCollection.Id);

            // Assert
            Assert.NotNull(added);
            Assert.Equal(name, added!.Name);
            Assert.Equal(ownerId, added.OwnerId);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCollection()
        {
            // Arrange
            var collection = await _context.TaskCollections.FirstAsync();
            collection.Rename("Updated Name");

            // Act
            await _taskCollectionRepository.UpdateAsync(collection);
            await _taskCollectionRepository.SaveChangesAsync();

            var updated = await _context.TaskCollections.FindAsync(collection.Id);

            // Assert
            Assert.Equal("Updated Name", updated!.Name);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCollection()
        {
            // Arrange
            var collection = await _context.TaskCollections.FirstAsync();

            // Act
            await _taskCollectionRepository.DeleteAsync(collection);
            await _taskCollectionRepository.SaveChangesAsync();

            var deleted = await _context.TaskCollections.FindAsync(collection.Id);

            // Assert
            Assert.Null(deleted);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion
    }
}
