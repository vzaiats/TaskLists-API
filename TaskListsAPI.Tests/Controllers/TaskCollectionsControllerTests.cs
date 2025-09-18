using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskListsAPI.Api.Controllers;
using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.DTOs.ResponseDTOs;
using TaskListsAPI.Application.Results;

namespace TaskListsAPI.Tests.Controllers
{
    public class TaskCollectionsControllerTests
    {
        private readonly Mock<ITaskCollectionService> _taskCollectionServiceMock;
        private readonly TaskCollectionsController _taskCollectionsController;

        #region Ctor

        public TaskCollectionsControllerTests()
        {
            _taskCollectionServiceMock = new Mock<ITaskCollectionService>();
            _taskCollectionsController = new TaskCollectionsController(_taskCollectionServiceMock.Object);
        }

        #endregion

        #region Create

        [Theory]
        [InlineData("My Collection", "11111111-1111-1111-1111-111111111111")]
        public async Task Create_ReturnsOk_WhenServiceSuccess(string name, string ownerId)
        {
            // Arrange
            var dto = new CreateTaskCollectionDto { Name = name, OwnerId = Guid.Parse(ownerId) };
            var returnDto = new ReturnTaskCollectionDto { Id = Guid.NewGuid(), Name = name, OwnerId = Guid.Parse(ownerId) };
            _taskCollectionServiceMock.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ServiceResult<ReturnTaskCollectionDto>.Success(returnDto));

            // Act
            var result = await _taskCollectionsController.Create(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(returnDto, okResult.Value);
        }

        [Theory]
        [InlineData("Invalid Collection", "11111111-1111-1111-1111-111111111111")]
        public async Task Create_ReturnsBadRequest_WhenServiceFails(string name, string ownerId)
        {
            // Arrange
            var dto = new CreateTaskCollectionDto { Name = name, OwnerId = Guid.Parse(ownerId) };
            _taskCollectionServiceMock.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ServiceResult<ReturnTaskCollectionDto>.Error("Error"));

            // Act
            var result = await _taskCollectionsController.Create(dto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", (List<string>)badResult.Value);
        }

        #endregion

        #region Update

        [Theory]
        [InlineData("22222222-2222-2222-2222-222222222222", "11111111-1111-1111-1111-111111111111", "New Name")]
        public async Task Update_ReturnsOk_WhenServiceSuccess(string collectionId, string userId, string newName)
        {
            // Arrange
            var dto = new UpdateTaskCollectionDto { Name = newName };
            var returnDto = new ReturnTaskCollectionDto { Id = Guid.Parse(collectionId), Name = newName, OwnerId = Guid.Parse(userId) };
            _taskCollectionServiceMock.Setup(s => s.UpdateAsync(Guid.Parse(collectionId), Guid.Parse(userId), dto))
                .ReturnsAsync(ServiceResult<ReturnTaskCollectionDto>.Success(returnDto));

            // Act
            var result = await _taskCollectionsController.Update(Guid.Parse(collectionId), Guid.Parse(userId), dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(returnDto, okResult.Value);
        }

        [Theory]
        [InlineData("33333333-3333-3333-3333-333333333333", "44444444-4444-4444-4444-444444444444", "New Name")]
        public async Task Update_ReturnsBadRequest_WhenServiceFails(string collectionId, string userId, string newName)
        {
            // Arrange
            var dto = new UpdateTaskCollectionDto { Name = newName };
            _taskCollectionServiceMock.Setup(s => s.UpdateAsync(Guid.Parse(collectionId), Guid.Parse(userId), dto))
                .ReturnsAsync(ServiceResult<ReturnTaskCollectionDto>.Error("Error"));

            // Act
            var result = await _taskCollectionsController.Update(Guid.Parse(collectionId), Guid.Parse(userId), dto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", (List<string>)badResult.Value);
        }

        #endregion

        #region Delete

        [Theory]
        [InlineData("55555555-5555-5555-5555-555555555555", "55555555-5555-5555-5555-555555555555")]
        public async Task Delete_ReturnsOk_WhenServiceSuccess(string collectionId, string userId)
        {
            // Arrange
            _taskCollectionServiceMock.Setup(s => s.DeleteAsync(Guid.Parse(collectionId), Guid.Parse(userId)))
                .ReturnsAsync(ServiceResult<bool>.Success(true));

            // Act
            var result = await _taskCollectionsController.Delete(Guid.Parse(collectionId), Guid.Parse(userId));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Theory]
        [InlineData("55555555-5555-5555-5555-555555555555", "11111111-1111-1111-1111-111111111111")]
        public async Task Delete_ReturnsBadRequest_WhenServiceFails(string collectionId, string userId)
        {
            // Arrange
            _taskCollectionServiceMock.Setup(s => s.DeleteAsync(Guid.Parse(collectionId), Guid.Parse(userId)))
                .ReturnsAsync(ServiceResult<bool>.Error("Error"));

            // Act
            var result = await _taskCollectionsController.Delete(Guid.Parse(collectionId), Guid.Parse(userId));

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", (List<string>)badResult.Value);
        }

        #endregion

        #region GetById

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222")]
        public async Task GetById_ReturnsOk_WhenServiceSuccess(string collectionId, string userId)
        {
            // Arrange
            var returnDto = new ReturnTaskCollectionDto { Id = Guid.Parse(collectionId), Name = "Name", OwnerId = Guid.Parse(userId) };
            _taskCollectionServiceMock.Setup(s => s.GetByIdAsync(Guid.Parse(collectionId), Guid.Parse(userId)))
                .ReturnsAsync(ServiceResult<ReturnTaskCollectionDto>.Success(returnDto));

            // Act
            var result = await _taskCollectionsController.GetById(Guid.Parse(collectionId), Guid.Parse(userId));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(returnDto, okResult.Value);
        }

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111", "33333333-3333-3333-3333-333333333333")]
        public async Task GetById_ReturnsBadRequest_WhenServiceFails(string collectionId, string userId)
        {
            // Arrange
            _taskCollectionServiceMock.Setup(s => s.GetByIdAsync(Guid.Parse(collectionId), Guid.Parse(userId)))
                .ReturnsAsync(ServiceResult<ReturnTaskCollectionDto>.Error("Error"));

            // Act
            var result = await _taskCollectionsController.GetById(Guid.Parse(collectionId), Guid.Parse(userId));

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", (List<string>)badResult.Value);
        }

        #endregion

        #region GetAll

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111")]
        public async Task GetAll_ReturnsOk_WhenServiceSuccess(string userId)
        {
            // Arrange
            var collections = new List<ReturnTaskCollectionDto>
            {
                new ReturnTaskCollectionDto { Id = Guid.NewGuid(), Name = "Test", OwnerId = Guid.Parse(userId) }
            };
            _taskCollectionServiceMock.Setup(s => s.GetAllAsync(Guid.Parse(userId), 1, 20))
                .ReturnsAsync(ServiceResult<List<ReturnTaskCollectionDto>>.Success(collections));

            // Act
            var result = await _taskCollectionsController.GetAll(Guid.Parse(userId));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(collections, okResult.Value);
        }

        #endregion

        #region Share

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222", "33333333-3333-3333-3333-333333333333")]
        public async Task Share_ReturnsOk_WhenServiceSuccess(string collectionId, string userId, string shareUserId)
        {
            // Arrange
            var dto = new ShareTaskCollectionDto { UserId = Guid.Parse(shareUserId) };
            var returnDto = new ReturnTaskCollectionDto { Id = Guid.Parse(collectionId), Name = "Shared", OwnerId = Guid.Parse(userId) };
            _taskCollectionServiceMock.Setup(s => s.ShareAsync(Guid.Parse(collectionId), Guid.Parse(userId), dto))
                .ReturnsAsync(ServiceResult<ReturnTaskCollectionDto>.Success(returnDto));

            // Act
            var result = await _taskCollectionsController.Share(Guid.Parse(collectionId), Guid.Parse(userId), dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(returnDto, okResult.Value);
        }

        #endregion

        #region Unshare

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222", "33333333-3333-3333-3333-333333333333")]
        public async Task Unshare_ReturnsOk_WhenServiceSuccess(string collectionId, string userId, string shareUserId)
        {
            // Arrange
            var returnDto = new ReturnTaskCollectionDto { Id = Guid.Parse(collectionId), Name = "Shared", OwnerId = Guid.Parse(userId) };
            _taskCollectionServiceMock.Setup(s => s.UnshareAsync(Guid.Parse(collectionId), Guid.Parse(userId), Guid.Parse(shareUserId)))
                .ReturnsAsync(ServiceResult<ReturnTaskCollectionDto>.Success(returnDto));

            // Act
            var result = await _taskCollectionsController.Unshare(Guid.Parse(collectionId), Guid.Parse(userId), Guid.Parse(shareUserId));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(returnDto, okResult.Value);
        }

        #endregion
    }
}
