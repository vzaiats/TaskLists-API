using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.DTOs.ResponseDTOs;
using TaskListsAPI.Application.Results;

namespace TaskListsAPI.Application.Interfaces
{
    public interface ITaskItemService
    {
        Task<ServiceResult<ReturnTaskItemDto>> CreateAsync(CreateTaskItemDto dto);

        Task<ServiceResult<ReturnTaskItemDto>> UpdateAsync(Guid taskId, UpdateTaskItemDto dto);

        Task<ServiceResult<bool>> DeleteAsync(Guid taskId);

        Task<ServiceResult<ReturnTaskItemDto>> GetByIdAsync(Guid taskId);

        Task<ServiceResult<List<ReturnTaskItemDto>>> GetAllByCollectionIdAsync(Guid collectionId);
    }
}