using Microsoft.Extensions.Logging;
using TaskListsAPI.Application.Constants;
using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.DTOs.ResponseDTOs;
using TaskListsAPI.Application.Helpers;
using TaskListsAPI.Application.Interfaces;
using TaskListsAPI.Application.Results;
using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Application.Services
{
    // Service for TaskCollection and sharing operations
    public class TaskCollectionService : ITaskCollectionService
    {
        private readonly ITaskCollectionRepository _repository;
        private readonly ILogger<TaskCollectionService> _logger;

        #region Ctor

        public TaskCollectionService(ITaskCollectionRepository repository, ILogger<TaskCollectionService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        #endregion

        #region Methods

        // Create a new task collection
        public async Task<ServiceResult<ReturnTaskCollectionDto>> CreateAsync(CreateTaskCollectionDto dto)
        {
            try
            {
                var collection = new TaskCollection(dto.Name, dto.OwnerId);
                await _repository.AddAsync(collection);
                await _repository.SaveChangesAsync();

                return ServiceResult<ReturnTaskCollectionDto>.Success(MapToReturnDtoHelper.MapToReturnDto(collection));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating collection for user {OwnerId}", dto.OwnerId);
                return ServiceResult<ReturnTaskCollectionDto>.Error("Failed to create collection");
            }
        }

        // Update collection name
        public async Task<ServiceResult<ReturnTaskCollectionDto>> UpdateAsync(Guid collectionId, Guid userId, UpdateTaskCollectionDto dto)
        {
            try
            {
                var collection = await _repository.GetByIdAsync(collectionId);
                if (collection == null) return ServiceResult<ReturnTaskCollectionDto>.Error(ErrorMessages.CollectionNotFound);

                if (collection.OwnerId != userId && !collection.Shares.Any(s => s.UserId == userId))
                    return ServiceResult<ReturnTaskCollectionDto>.Error(ErrorMessages.AccessDenied);

                collection.Rename(dto.Name);
                await _repository.UpdateAsync(collection);
                await _repository.SaveChangesAsync();

                return ServiceResult<ReturnTaskCollectionDto>.Success(MapToReturnDtoHelper.MapToReturnDto(collection));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating collection {CollectionId} by user {UserId}", collectionId, userId);
                return ServiceResult<ReturnTaskCollectionDto>.Error("Failed to update collection");
            }
        }

        // Delete collection (owner only)
        public async Task<ServiceResult<bool>> DeleteAsync(Guid collectionId, Guid userId)
        {
            try
            {
                var collection = await _repository.GetByIdAsync(collectionId);
                if (collection == null) return ServiceResult<bool>.Error(ErrorMessages.CollectionNotFound);

                if (collection.OwnerId != userId)
                    return ServiceResult<bool>.Error(ErrorMessages.OnlyOwnerCanDelete);

                await _repository.DeleteAsync(collection);
                await _repository.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting collection {CollectionId} by user {UserId}", collectionId, userId);
                return ServiceResult<bool>.Error("Failed to delete collection");
            }
        }

        // Get collection by Id
        public async Task<ServiceResult<ReturnTaskCollectionDto>> GetByIdAsync(Guid collectionId, Guid userId)
        {
            try
            {
                var collection = await _repository.GetByIdAsync(collectionId);
                if (collection == null) return ServiceResult<ReturnTaskCollectionDto>.Error(ErrorMessages.CollectionNotFound);

                if (collection.OwnerId != userId && !collection.Shares.Any(s => s.UserId == userId))
                    return ServiceResult<ReturnTaskCollectionDto>.Error(ErrorMessages.AccessDenied);

                return ServiceResult<ReturnTaskCollectionDto>.Success(MapToReturnDtoHelper.MapToReturnDto(collection));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting collection {CollectionId} by user {UserId}", collectionId, userId);
                return ServiceResult<ReturnTaskCollectionDto>.Error("Failed to get collection");
            }
        }

        // Get all collections for a user with pagination
        public async Task<ServiceResult<List<ReturnTaskCollectionDto>>> GetAllAsync(Guid userId, int page = Constants.Constants.DefaultPage, int pageSize = Constants.Constants.DefaultPageSize)
        {
            try
            {
                if (page <= 0) page = Constants.Constants.DefaultPage;
                if (pageSize <= 0) pageSize = Constants.Constants.DefaultPageSize;

                var allCollections = await _repository.GetAllAsync();
                var userCollections = allCollections
                    .Where(c => c.OwnerId == userId || c.Shares.Any(s => s.UserId == userId))
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToReturnDtoHelper.MapToReturnDto)
                    .ToList();

                return ServiceResult<List<ReturnTaskCollectionDto>>.Success(userCollections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting collections for user {UserId}", userId);
                return ServiceResult<List<ReturnTaskCollectionDto>>.Error("Failed to get collections");
            }
        }

        // Share collection with another user
        public async Task<ServiceResult<ReturnTaskCollectionDto>> ShareAsync(Guid collectionId, Guid userId, ShareTaskCollectionDto dto)
        {
            try
            {
                var collection = await _repository.GetByIdAsync(collectionId);
                if (collection == null) return ServiceResult<ReturnTaskCollectionDto>.Error(ErrorMessages.CollectionNotFound);

                if (collection.OwnerId != userId && !collection.Shares.Any(s => s.UserId == userId))
                    return ServiceResult<ReturnTaskCollectionDto>.Error(ErrorMessages.AccessDenied);

                if (collection.Shares.Count >= Constants.Constants.MaxShares)
                    return ServiceResult<ReturnTaskCollectionDto>.Error(ErrorMessages.MaxThreeUsers);

                if (!collection.Shares.Any(s => s.UserId == dto.UserId))
                    collection.Shares.Add(new Share(dto.UserId, collectionId));

                await _repository.UpdateAsync(collection);
                await _repository.SaveChangesAsync();

                return ServiceResult<ReturnTaskCollectionDto>.Success(MapToReturnDtoHelper.MapToReturnDto(collection));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing collection {CollectionId} by user {UserId}", collectionId, userId);
                return ServiceResult<ReturnTaskCollectionDto>.Error("Failed to share collection");
            }
        }

        // Remove a shared user from collection
        public async Task<ServiceResult<ReturnTaskCollectionDto>> UnshareAsync(Guid collectionId, Guid userId, Guid removeUserId)
        {
            try
            {
                var collection = await _repository.GetByIdAsync(collectionId);
                if (collection == null) return ServiceResult<ReturnTaskCollectionDto>.Error(ErrorMessages.CollectionNotFound);

                if (collection.OwnerId != userId && !collection.Shares.Any(s => s.UserId == userId))
                    return ServiceResult<ReturnTaskCollectionDto>.Error(ErrorMessages.AccessDenied);

                var share = collection.Shares.FirstOrDefault(s => s.UserId == removeUserId);
                if (share != null)
                    collection.Shares.Remove(share);

                await _repository.UpdateAsync(collection);
                await _repository.SaveChangesAsync();

                return ServiceResult<ReturnTaskCollectionDto>.Success(MapToReturnDtoHelper.MapToReturnDto(collection));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsharing collection {CollectionId} by user {UserId}", collectionId, userId);
                return ServiceResult<ReturnTaskCollectionDto>.Error("Failed to unshare collection");
            }
        }

        #endregion
    }
}
