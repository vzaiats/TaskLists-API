using Microsoft.EntityFrameworkCore;
using TaskListsAPI.Domain.Entities;
using TaskListsAPI.Infrastructure.Database;
using TaskListsAPI.Infrastructure.Repositories;

namespace TaskListsAPI.Tests.Repositories
{
    public class TaskItemRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TaskItemRepository _taskItemRepository;

        #region Ctor

        public TaskItemRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _taskItemRepository = new TaskItemRepository(_context);

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
        [InlineData("Task 1")]
        [InlineData("Task 2")]
        [InlineData("Task 3")]
        public async Task GetByIdAsync_ShouldReturnTask_WhenExists(string taskTitle)
        {
            // Arrange
            var task = await _context.TaskItems.FirstAsync(t => t.Title == taskTitle);

            // Act
            var result = await _taskItemRepository.GetByIdAsync(task.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskTitle, result!.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _taskItemRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllByCollectionIdAsync Tests

        [Theory]
        [InlineData("Alice Tasks", 2)]
        [InlineData("Bob Tasks", 1)]
        public async Task GetAllByCollectionIdAsync_ShouldReturnTasksForCollection(string collectionName, int expectedCount)
        {
            // Arrange
            var collection = await _context.TaskCollections.FirstAsync(c => c.Name == collectionName);

            // Act
            var result = await _taskItemRepository.GetAllByCollectionIdAsync(collection.Id);

            // Assert
            Assert.Equal(expectedCount, result.Count);
            Assert.All(result, t => Assert.Equal(collection.Id, t.TaskCollectionId));
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTasks()
        {
            // Act
            var result = await _taskItemRepository.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count);
        }

        #endregion

        #region AddAsync Tests

        [Theory]
        [InlineData("New Task", "11111111-1111-1111-1111-111111111111")]
        public async Task AddAsync_ShouldAddTask(string title, string collectionIdStr)
        {
            // Arrange
            var collectionId = Guid.Parse(collectionIdStr);
            var newTask = new TaskItem(title, collectionId);

            // Act
            await _taskItemRepository.AddAsync(newTask);
            await _taskItemRepository.SaveChangesAsync();

            var added = await _context.TaskItems.FindAsync(newTask.Id);

            // Assert
            Assert.NotNull(added);
            Assert.Equal(title, added!.Title);
            Assert.Equal(collectionId, added.TaskCollectionId);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTask()
        {
            // Arrange
            var task = await _context.TaskItems.FirstAsync();
            task.UpdateTitleAndStatus("Updated Task", true);

            // Act
            await _taskItemRepository.UpdateAsync(task);
            await _taskItemRepository.SaveChangesAsync();

            var updated = await _context.TaskItems.FindAsync(task.Id);

            // Assert
            Assert.Equal("Updated Task", updated!.Title);
            Assert.True(updated.IsCompleted);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTask()
        {
            // Arrange
            var task = await _context.TaskItems.FirstAsync();

            // Act
            await _taskItemRepository.DeleteAsync(task);
            await _taskItemRepository.SaveChangesAsync();

            var deleted = await _context.TaskItems.FindAsync(task.Id);

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
