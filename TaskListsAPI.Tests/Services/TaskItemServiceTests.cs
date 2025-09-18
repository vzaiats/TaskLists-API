using Microsoft.Extensions.Logging;
using Moq;
using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.Interfaces;
using TaskListsAPI.Application.Services;
using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Tests.Services
{
    public class TaskItemServiceTests
    {
        private readonly Mock<ITaskItemRepository> _taskItemRepository;
        private readonly Mock<ILogger<TaskItemService>> _loggerMock;
        private readonly TaskItemService _taskItemService;

        #region Ctor

        public TaskItemServiceTests()
        {
            _taskItemRepository = new Mock<ITaskItemRepository>();
            _loggerMock = new Mock<ILogger<TaskItemService>>();
            _taskItemService = new TaskItemService(_taskItemRepository.Object, _loggerMock.Object);
        }

        #endregion

        #region CreateAsync

        [Theory]
        [InlineData("Buy milk", "11111111-1111-1111-1111-111111111111")]
        [InlineData("Finish report", "22222222-2222-2222-2222-222222222222")]
        public async Task CreateAsync_ShouldReturnSuccess(string title, string collectionIdStr)
        {
            // Arrange
            var collectionId = Guid.Parse(collectionIdStr);
            var dto = new CreateTaskItemDto { Title = title, TaskCollectionId = collectionId };
            _taskItemRepository.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
            _taskItemRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _taskItemService.CreateAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(title, result.Data.Title);
            Assert.Equal(collectionId, result.Data.TaskCollectionId);
        }

        #endregion

        #region UpdateAsync

        [Theory]
        [InlineData("Updated task", true)]
        [InlineData("Another task", false)]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenTaskExists(string newTitle, bool isCompleted)
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem("Old task", Guid.NewGuid());
            _taskItemRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);
            _taskItemRepository.Setup(r => r.UpdateAsync(task)).Returns(Task.CompletedTask);
            _taskItemRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new UpdateTaskItemDto { Title = newTitle, IsCompleted = isCompleted };

            // Act
            var result = await _taskItemService.UpdateAsync(taskId, dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newTitle, result.Data.Title);
            Assert.Equal(isCompleted, result.Data.IsCompleted);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskItemRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync((TaskItem)null);
            var dto = new UpdateTaskItemDto { Title = "New Task", IsCompleted = true };

            // Act
            var result = await _taskItemService.UpdateAsync(taskId, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Task not found", result.Errors.First());
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenTaskExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem("Test", Guid.NewGuid());
            _taskItemRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);
            _taskItemRepository.Setup(r => r.DeleteAsync(task)).Returns(Task.CompletedTask);
            _taskItemRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _taskItemService.DeleteAsync(taskId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskItemRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskItemService.DeleteAsync(taskId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Task not found", result.Errors.First());
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSuccess_WhenTaskExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem("Test", Guid.NewGuid());
            _taskItemRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            var result = await _taskItemService.GetByIdAsync(taskId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Test", result.Data.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskItemRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskItemService.GetByIdAsync(taskId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Task not found", result.Errors.First());
        }

        #endregion

        #region GetAllByCollectionIdAsync

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111", 3)]
        [InlineData("22222222-2222-2222-2222-222222222222", 2)]
        public async Task GetAllByCollectionIdAsync_ShouldReturnTasks(string collectionIdStr, int expectedCount)
        {
            // Arrange
            var collectionId = Guid.Parse(collectionIdStr);
            var tasks = new List<TaskItem>
            {
                new TaskItem("Task1", collectionId),
                new TaskItem("Task2", collectionId),
                new TaskItem("Task3", collectionId)
            };
            _taskItemRepository.Setup(r => r.GetAllByCollectionIdAsync(collectionId)).ReturnsAsync(tasks.Take(expectedCount).ToList());

            // Act
            var result = await _taskItemService.GetAllByCollectionIdAsync(collectionId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedCount, result.Data.Count);
        }

        [Fact]
        public async Task GetAllByCollectionIdAsync_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            _taskItemRepository.Setup(r => r.GetAllByCollectionIdAsync(collectionId)).ThrowsAsync(new Exception());

            // Act
            var result = await _taskItemService.GetAllByCollectionIdAsync(collectionId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Failed to get tasks", result.Errors.First());
        }

        #endregion
    }
}
