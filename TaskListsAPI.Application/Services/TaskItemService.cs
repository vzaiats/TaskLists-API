using Microsoft.Extensions.Logging;
using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.DTOs.ResponseDTOs;
using TaskListsAPI.Application.Helpers;
using TaskListsAPI.Application.Interfaces;
using TaskListsAPI.Application.Results;
using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Application.Services
{
    // Service for TaskItem operations
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _repository;
        private readonly ILogger<TaskItemService> _logger;

        #region Ctor

        public TaskItemService(ITaskItemRepository repository, ILogger<TaskItemService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        #endregion

        #region Methods

        // Create a new task item
        public async Task<ServiceResult<ReturnTaskItemDto>> CreateAsync(CreateTaskItemDto dto)
        {
            try
            {
                var task = new TaskItem(dto.Title, dto.TaskCollectionId);
                await _repository.AddAsync(task);
                await _repository.SaveChangesAsync();

                return ServiceResult<ReturnTaskItemDto>.Success(MapToReturnDtoHelper.MapToReturnDto(task));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task in collection {CollectionId}", dto.TaskCollectionId);
                return ServiceResult<ReturnTaskItemDto>.Error("Failed to create task");
            }
        }

        // Update an existing task item
        public async Task<ServiceResult<ReturnTaskItemDto>> UpdateAsync(Guid taskId, UpdateTaskItemDto dto)
        {
            try
            {
                var task = await _repository.GetByIdAsync(taskId);
                if (task == null)
                    return ServiceResult<ReturnTaskItemDto>.Error(Constants.ErrorMessages.TaskNotFound);

                task.UpdateTitleAndStatus(dto.Title, dto.IsCompleted);

                await _repository.UpdateAsync(task);
                await _repository.SaveChangesAsync();

                return ServiceResult<ReturnTaskItemDto>.Success(MapToReturnDtoHelper.MapToReturnDto(task));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", taskId);
                return ServiceResult<ReturnTaskItemDto>.Error("Failed to update task");
            }
        }

        // Delete a task item
        public async Task<ServiceResult<bool>> DeleteAsync(Guid taskId)
        {
            try
            {
                var task = await _repository.GetByIdAsync(taskId);
                if (task == null)
                    return ServiceResult<bool>.Error(Constants.ErrorMessages.TaskNotFound);

                await _repository.DeleteAsync(task);
                await _repository.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", taskId);
                return ServiceResult<bool>.Error("Failed to delete task");
            }
        }

        // Get task by Id
        public async Task<ServiceResult<ReturnTaskItemDto>> GetByIdAsync(Guid taskId)
        {
            try
            {
                var task = await _repository.GetByIdAsync(taskId);
                if (task == null)
                    return ServiceResult<ReturnTaskItemDto>.Error(Constants.ErrorMessages.TaskNotFound);

                return ServiceResult<ReturnTaskItemDto>.Success(MapToReturnDtoHelper.MapToReturnDto(task));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task {TaskId}", taskId);
                return ServiceResult<ReturnTaskItemDto>.Error("Failed to get task");
            }
        }

        // Get all tasks for a collection
        public async Task<ServiceResult<List<ReturnTaskItemDto>>> GetAllByCollectionIdAsync(Guid collectionId)
        {
            try
            {
                var tasks = await _repository.GetAllByCollectionIdAsync(collectionId);
                var result = tasks.Select(MapToReturnDtoHelper.MapToReturnDto).ToList();

                return ServiceResult<List<ReturnTaskItemDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for collection {CollectionId}", collectionId);
                return ServiceResult<List<ReturnTaskItemDto>>.Error("Failed to get tasks");
            }
        }

        #endregion
    }
}
