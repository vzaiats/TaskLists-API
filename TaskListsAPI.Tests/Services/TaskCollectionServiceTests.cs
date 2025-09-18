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
        private readonly Mock<ITaskCollectionRepository> _taskCollectionRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<TaskCollectionService>> _loggerMock;
        private readonly TaskCollectionService _taskCollectionService;

        public TaskCollectionServiceTests()
        {
            _taskCollectionRepositoryMock = new Mock<ITaskCollectionRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<TaskCollectionService>>();

            _taskCollectionService = new TaskCollectionService(
                _taskCollectionRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        #region CreateAsync

        [Theory]
        [InlineData("My Collection", "11111111-1111-1111-1111-111111111111")]
        public async Task CreateAsync_ShouldReturnSuccess(string name, string ownerIdStr)
        {
            var ownerId = Guid.Parse(ownerIdStr);
            var dto = new CreateTaskCollectionDto { Name = name, OwnerId = ownerId };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId))
                .ReturnsAsync(new User { Id = ownerId, Name = "TestUser" });
            _taskCollectionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskCollection>())).Returns(Task.CompletedTask);
            _taskCollectionRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _taskCollectionService.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(name, result.Data.Name);
            Assert.Equal(ownerId, result.Data.OwnerId);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenUserNotFound()
        {
            var ownerId = Guid.NewGuid();
            var dto = new CreateTaskCollectionDto { Name = "Test", OwnerId = ownerId };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId)).ReturnsAsync((User)null);

            var result = await _taskCollectionService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.UserNotFound, result.Errors);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenUserIsOwner()
        {
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Old Name", ownerId);

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId)).ReturnsAsync(new User { Id = ownerId });
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            _taskCollectionRepositoryMock.Setup(r => r.UpdateAsync(collection)).Returns(Task.CompletedTask);
            _taskCollectionRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new UpdateTaskCollectionDto { Name = "New Name" };

            var result = await _taskCollectionService.UpdateAsync(collectionId, ownerId, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal("New Name", result.Data.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var dto = new UpdateTaskCollectionDto { Name = "New Name" };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _taskCollectionService.UpdateAsync(collectionId, userId, dto);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.UserNotFound, result.Errors);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenCollectionNotFound()
        {
            var userId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync((TaskCollection)null);

            var dto = new UpdateTaskCollectionDto { Name = "New Name" };
            var result = await _taskCollectionService.UpdateAsync(collectionId, userId, dto);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.CollectionNotFound, result.Errors);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUserNotAllowed()
        {
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Old Name", ownerId);

            _userRepositoryMock.Setup(u => u.GetByIdAsync(otherUserId)).ReturnsAsync(new User { Id = otherUserId });
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);

            var dto = new UpdateTaskCollectionDto { Name = "New Name" };
            var result = await _taskCollectionService.UpdateAsync(collectionId, otherUserId, dto);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.AccessDenied, result.Errors);
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenOwnerDeletes()
        {
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId)).ReturnsAsync(new User { Id = ownerId });
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            _taskCollectionRepositoryMock.Setup(r => r.DeleteAsync(collection)).Returns(Task.CompletedTask);
            _taskCollectionRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _taskCollectionService.DeleteAsync(collectionId, ownerId);

            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenNotOwner()
        {
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);

            _userRepositoryMock.Setup(u => u.GetByIdAsync(otherUserId)).ReturnsAsync(new User { Id = otherUserId });
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);

            var result = await _taskCollectionService.DeleteAsync(collectionId, otherUserId);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.OnlyOwnerCanDelete, result.Errors);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenNotFound()
        {
            var userId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync((TaskCollection)null);

            var result = await _taskCollectionService.DeleteAsync(collectionId, userId);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.CollectionNotFound, result.Errors);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSuccess_WhenUserHasAccess()
        {
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId)).ReturnsAsync(new User { Id = ownerId });
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);

            var result = await _taskCollectionService.GetByIdAsync(collectionId, ownerId);

            Assert.True(result.IsSuccess);
            Assert.Equal("Test", result.Data.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _taskCollectionService.GetByIdAsync(collectionId, userId);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.UserNotFound, result.Errors);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenCollectionNotFound()
        {
            var userId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync((TaskCollection)null);

            var result = await _taskCollectionService.GetByIdAsync(collectionId, userId);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.CollectionNotFound, result.Errors);
        }

        #endregion

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_ShouldReturnUserCollections()
        {
            var userId = Guid.NewGuid();
            var collections = new List<TaskCollection>
            {
                new TaskCollection("A", userId),
                new TaskCollection("B", userId)
            };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
            _taskCollectionRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(collections);

            var result = await _taskCollectionService.GetAllAsync(userId);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _taskCollectionService.GetAllAsync(userId);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.UserNotFound, result.Errors);
        }

        #endregion

        #region ShareAsync

        [Fact]
        public async Task ShareAsync_ShouldReturnSuccess_WhenAllowed()
        {
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var userToShare = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId)).ReturnsAsync(new User { Id = ownerId });
            _userRepositoryMock.Setup(u => u.GetByIdAsync(userToShare)).ReturnsAsync(new User { Id = userToShare });
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            _taskCollectionRepositoryMock.Setup(r => r.UpdateAsync(collection)).Returns(Task.CompletedTask);
            _taskCollectionRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new ShareTaskCollectionDto { UserId = userToShare };
            var result = await _taskCollectionService.ShareAsync(collectionId, ownerId, dto);

            Assert.True(result.IsSuccess);
            Assert.Contains(collection.Shares, s => s.UserId == userToShare);
        }

        [Fact]
        public async Task ShareAsync_ShouldReturnError_WhenUserNotFound()
        {
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var userToShare = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId)).ReturnsAsync((User)null);

            var dto = new ShareTaskCollectionDto { UserId = userToShare };
            var result = await _taskCollectionService.ShareAsync(collectionId, ownerId, dto);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.UserNotFound, result.Errors);
        }

        [Fact]
        public async Task ShareAsync_ShouldReturnError_WhenMaxUsersExceeded()
        {
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var collection = new TaskCollection("Test", ownerId);
            collection.Shares.AddRange(new[]
            {
                new Share(Guid.NewGuid(), collectionId),
                new Share(Guid.NewGuid(), collectionId),
                new Share(Guid.NewGuid(), collectionId)
            });

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId)).ReturnsAsync(new User { Id = ownerId });
            _userRepositoryMock.Setup(u => u.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new User());
            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);

            var dto = new ShareTaskCollectionDto { UserId = Guid.NewGuid() };
            var result = await _taskCollectionService.ShareAsync(collectionId, ownerId, dto);

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

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId)).ReturnsAsync(new User { Id = ownerId });
            _userRepositoryMock.Setup(u => u.GetByIdAsync(sharedUserId)).ReturnsAsync(new User { Id = sharedUserId });

            _taskCollectionRepositoryMock.Setup(r => r.GetByIdAsync(collectionId)).ReturnsAsync(collection);
            _taskCollectionRepositoryMock.Setup(r => r.UpdateAsync(collection)).Returns(Task.CompletedTask);
            _taskCollectionRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _taskCollectionService.UnshareAsync(collectionId, ownerId, sharedUserId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain(collection.Shares, s => s.UserId == sharedUserId);
        }

        [Fact]
        public async Task UnshareAsync_ShouldReturnError_WhenUserNotFound()
        {
            var ownerId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var sharedUserId = Guid.NewGuid();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(ownerId)).ReturnsAsync((User)null);

            var result = await _taskCollectionService.UnshareAsync(collectionId, ownerId, sharedUserId);

            Assert.False(result.IsSuccess);
            Assert.Contains(ErrorMessages.UserNotFound, result.Errors);
        }

        #endregion
    }
}
