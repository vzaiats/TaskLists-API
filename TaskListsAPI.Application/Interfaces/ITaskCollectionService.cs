using TaskListsAPI.Application.Constants;
using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.DTOs.ResponseDTOs;
using TaskListsAPI.Application.Results;

public interface ITaskCollectionService
{
    Task<ServiceResult<ReturnTaskCollectionDto>> CreateAsync(CreateTaskCollectionDto dto);
    Task<ServiceResult<ReturnTaskCollectionDto>> UpdateAsync(Guid collectionId, Guid userId, UpdateTaskCollectionDto dto);
    Task<ServiceResult<bool>> DeleteAsync(Guid collectionId, Guid userId);
    Task<ServiceResult<ReturnTaskCollectionDto>> GetByIdAsync(Guid collectionId, Guid userId);
    Task<ServiceResult<List<ReturnTaskCollectionDto>>> GetAllAsync(Guid userId, int page = Constants.DefaultPage, int pageSize = Constants.DefaultPageSize);
    Task<ServiceResult<ReturnTaskCollectionDto>> ShareAsync(Guid collectionId, Guid userId, ShareTaskCollectionDto dto);
    Task<ServiceResult<ReturnTaskCollectionDto>> UnshareAsync(Guid collectionId, Guid userId, Guid removeUserId);
}