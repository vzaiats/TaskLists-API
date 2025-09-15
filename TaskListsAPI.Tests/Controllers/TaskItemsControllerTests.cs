using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskListsAPI.Api.Controllers;
using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.DTOs.ResponseDTOs;
using TaskListsAPI.Application.Interfaces;
using TaskListsAPI.Application.Results;

namespace TaskListsAPI.Tests.Controllers
{
    public class TaskItemsControllerTests
    {
        private readonly Mock<ITaskItemService> _serviceMock;
        private readonly TaskItemsController _controller;

        #region Ctor

        public TaskItemsControllerTests()
        {
            _serviceMock = new Mock<ITaskItemService>();
            _controller = new TaskItemsController(_serviceMock.Object);
        }

        #endregion

        #region Create

        [Theory]
        [InlineData("Buy milk", "11111111-1111-1111-1111-111111111111")]
        public async Task Create_ReturnsOk_WhenServiceSuccess(string title, string collectionId)
        {
            // Arrange
            var dto = new CreateTaskItemDto { Title = title, TaskCollectionId = Guid.Parse(collectionId) };
            var returnDto = new ReturnTaskItemDto { Id = Guid.NewGuid(), Title = title, TaskCollectionId = Guid.Parse(collectionId) };
            _serviceMock.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ServiceResult<ReturnTaskItemDto>.Success(returnDto));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(returnDto, okResult.Value);
        }

        [Theory]
        [InlineData("Invalid Task", "11111111-1111-1111-1111-111111111111")]
        public async Task Create_ReturnsBadRequest_WhenServiceFails(string title, string collectionId)
        {
            // Arrange
            var dto = new CreateTaskItemDto { Title = title, TaskCollectionId = Guid.Parse(collectionId) };
            _serviceMock.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ServiceResult<ReturnTaskItemDto>.Error("Error"));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", (List<string>)badResult.Value);
        }

        #endregion

        #region Update

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111", "New Task", true)]
        public async Task Update_ReturnsOk_WhenServiceSuccess(string taskId, string newTitle, bool isCompleted)
        {
            // Arrange
            var dto = new UpdateTaskItemDto { Title = newTitle, IsCompleted = isCompleted };
            var returnDto = new ReturnTaskItemDto { Id = Guid.Parse(taskId), Title = newTitle, IsCompleted = isCompleted };
            _serviceMock.Setup(s => s.UpdateAsync(Guid.Parse(taskId), dto))
                .ReturnsAsync(ServiceResult<ReturnTaskItemDto>.Success(returnDto));

            // Act
            var result = await _controller.Update(Guid.Parse(taskId), dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(returnDto, okResult.Value);
        }

        [Theory]
        [InlineData("22222222-2222-2222-2222-222222222222", "Invalid Task", false)]
        public async Task Update_ReturnsBadRequest_WhenServiceFails(string taskId, string newTitle, bool isCompleted)
        {
            // Arrange
            var dto = new UpdateTaskItemDto { Title = newTitle, IsCompleted = isCompleted };
            _serviceMock.Setup(s => s.UpdateAsync(Guid.Parse(taskId), dto))
                .ReturnsAsync(ServiceResult<ReturnTaskItemDto>.Error("Error"));

            // Act
            var result = await _controller.Update(Guid.Parse(taskId), dto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", (List<string>)badResult.Value);
        }

        #endregion

        #region Delete

        [Theory]
        [InlineData("33333333-3333-3333-3333-333333333333")]
        public async Task Delete_ReturnsOk_WhenServiceSuccess(string taskId)
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(Guid.Parse(taskId)))
                .ReturnsAsync(ServiceResult<bool>.Success(true));

            // Act
            var result = await _controller.Delete(Guid.Parse(taskId));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Theory]
        [InlineData("44444444-4444-4444-4444-444444444444")]
        public async Task Delete_ReturnsBadRequest_WhenServiceFails(string taskId)
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(Guid.Parse(taskId)))
                .ReturnsAsync(ServiceResult<bool>.Error("Error"));

            // Act
            var result = await _controller.Delete(Guid.Parse(taskId));

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", (List<string>)badResult.Value);
        }

        #endregion

        #region GetById

        [Theory]
        [InlineData("55555555-5555-5555-5555-555555555555")]
        public async Task GetById_ReturnsOk_WhenServiceSuccess(string taskId)
        {
            // Arrange
            var returnDto = new ReturnTaskItemDto { Id = Guid.Parse(taskId), Title = "Task", IsCompleted = false };
            _serviceMock.Setup(s => s.GetByIdAsync(Guid.Parse(taskId)))
                .ReturnsAsync(ServiceResult<ReturnTaskItemDto>.Success(returnDto));

            // Act
            var result = await _controller.GetById(Guid.Parse(taskId));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(returnDto, okResult.Value);
        }

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111112")]
        public async Task GetById_ReturnsBadRequest_WhenServiceFails(string taskId)
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(Guid.Parse(taskId)))
                .ReturnsAsync(ServiceResult<ReturnTaskItemDto>.Error("Error"));

            // Act
            var result = await _controller.GetById(Guid.Parse(taskId));

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", (List<string>)badResult.Value);
        }

        #endregion

        #region GetAllByCollectionId

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111")]
        public async Task GetAllByCollectionId_ReturnsOk_WhenServiceSuccess(string collectionId)
        {
            // Arrange
            var tasks = new List<ReturnTaskItemDto>
            {
                new ReturnTaskItemDto { Id = Guid.NewGuid(), Title = "Task1", TaskCollectionId = Guid.Parse(collectionId) }
            };
            _serviceMock.Setup(s => s.GetAllByCollectionIdAsync(Guid.Parse(collectionId)))
                .ReturnsAsync(ServiceResult<List<ReturnTaskItemDto>>.Success(tasks));

            // Act
            var result = await _controller.GetAllByCollectionId(Guid.Parse(collectionId));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(tasks, okResult.Value);
        }

        [Theory]
        [InlineData("22222222-2222-2222-2222-222222222222")]
        public async Task GetAllByCollectionId_ReturnsBadRequest_WhenServiceFails(string collectionId)
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllByCollectionIdAsync(Guid.Parse(collectionId)))
                .ReturnsAsync(ServiceResult<List<ReturnTaskItemDto>>.Error("Error"));

            // Act
            var result = await _controller.GetAllByCollectionId(Guid.Parse(collectionId));

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", (List<string>)badResult.Value);
        }

        #endregion
    }
}
