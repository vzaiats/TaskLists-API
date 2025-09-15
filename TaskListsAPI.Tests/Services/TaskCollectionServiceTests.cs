using Microsoft.Extensions.Logging;
using Moq;
using TaskListsAPI.Application.Constants;
using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.Interfaces;
using TaskListsAPI.Application.Services;
using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Tests.Services
{
    public class TaskCollectionServiceTests
    {
        private readonly Mock<ITaskCollectionRepository> _repositoryMock;
        private readonly Mock<ILogger<TaskCollectionService>> _loggerMock;
        private readonly TaskCollectionService _service;

        #region Ctor

        public TaskCollectionServiceTests()
        {
            _repositoryMock = new Mock<ITaskCollectionRepository>();
            _loggerMock = new Mock<ILogger<TaskCollectionService>>();
            _service = new TaskCollectionService(_repositoryMock.Object, _loggerMock.Object);
        }

        #endregion

        #region CreateAsync

        [Theory]
        [InlineData("My Collection", "11111111-1111-1111-1111-111111111111")]
        [InlineData("Work Tasks", "22222222-2222-2222-2222-222222222222")]
        public async Task CreateAsync_ShouldReturnSuccess(string name, string ownerIdStr)
        {
            // Arrange
            var ownerId = Guid.Parse(ownerIdStr);
            var dto = new CreateTaskCollectionDto { Name = name, OwnerId = ownerId };
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskCollection>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(name, result.Data.Name);
            Assert.Equal(ownerId, result.Data.OwnerId);
        }

        #endregion

        #region UpdateAsync

        [Theory]
        [InlineData("Updated Collection", "11111111-1111-1111-1111-111111111111")]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenUserIsOwner(string newName, string userIdStr)
        {
            // Arrange
            var userId = Guid.Parse(userIdStr);
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Old Name", userId);
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            _repositoryMock.Setup(r => r.UpdateAsync(collection)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new UpdateTaskCollectionDto { Name = newName };

            // Act
            var result = await _service.UpdateAsync(collectionId, userId, dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newName, result.Data.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenCollectionNotFound()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync((TaskCollection)null);
            var dto = new UpdateTaskCollectionDto { Name = "New Name" };

            // Act
            var result = await _service.UpdateAsync(collectionId, userId, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.CollectionNotFound, result.Errors);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUserNotAllowed()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var collection = new TaskCollection("Name", ownerId);
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            var dto = new UpdateTaskCollectionDto { Name = "New Name" };

            // Act
            var result = await _service.UpdateAsync(collectionId, otherUserId, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.AccessDenied, result.Errors);
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenOwnerDeletes()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            _repositoryMock.Setup(r => r.DeleteAsync(collection)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(collectionId, ownerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenNotOwner()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);

            // Act
            var result = await _service.DeleteAsync(collectionId, otherUserId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.OnlyOwnerCanDelete, result.Errors);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenNotFound()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync((TaskCollection)null);

            // Act
            var result = await _service.DeleteAsync(collectionId, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.CollectionNotFound, result.Errors);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSuccess_WhenUserHasAccess()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);

            // Act
            var result = await _service.GetByIdAsync(collectionId, ownerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Test", result.Data.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenNotFound()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync((TaskCollection)null);

            // Act
            var result = await _service.GetByIdAsync(collectionId, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.CollectionNotFound, result.Errors);
        }

        #endregion

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_ShouldReturnUserCollections()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var collections = new List<TaskCollection>
            {
                new TaskCollection("A", userId),
                new TaskCollection("B", userId)
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(collections);

            // Act
            var result = await _service.GetAllAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.Count);
        }

        #endregion

        #region ShareAsync

        [Fact]
        public async Task ShareAsync_ShouldReturnSuccess_WhenAllowed()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var userToShare = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            _repositoryMock.Setup(r => r.UpdateAsync(collection)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new ShareTaskCollectionDto { UserId = userToShare };

            // Act
            var result = await _service.ShareAsync(collectionId, ownerId, dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains(collection.Shares, s => s.UserId == userToShare);
        }

        [Fact]
        public async Task ShareAsync_ShouldReturnError_WhenMaxUsersExceeded()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);
            collection.Shares.AddRange(new[]
            {
                new Share(Guid.NewGuid(), collectionId),
                new Share(Guid.NewGuid(), collectionId),
                new Share(Guid.NewGuid(), collectionId)
            });
            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            var dto = new ShareTaskCollectionDto { UserId = Guid.NewGuid() };

            // Act
            var result = await _service.ShareAsync(collectionId, ownerId, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.MaxThreeUsers, result.Errors);
        }

        #endregion

        #region UnshareAsync

        [Fact]
        public async Task UnshareAsync_ShouldReturnSuccess_WhenUserRemoved()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var sharedUserId = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);
            collection.Shares.Add(new Share(sharedUserId, collectionId));

            _repositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            _repositoryMock.Setup(r => r.UpdateAsync(collection)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UnshareAsync(collectionId, ownerId, sharedUserId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain(collection.Shares, s => s.UserId == sharedUserId);
        }

        #endregion
    }
}
