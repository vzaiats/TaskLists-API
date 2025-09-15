using TaskListsAPI.Application.DTOs.ResponseDTOs;
using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Application.Helpers
{
    public static class MapToReturnDtoHelper
    {
        #region Methods

        public static ReturnTaskCollectionDto MapToReturnDto(TaskCollection collection)
        {
            return new ReturnTaskCollectionDto
            {
                Id = collection.Id,
                Name = collection.Name,
                OwnerId = collection.OwnerId,
                CreatedAt = collection.CreatedAt,
                Shares = collection.Shares
                    .Select(s => new ReturnShareDto { UserId = s.UserId })
                    .ToList()
            };
        }

        public static ReturnTaskItemDto MapToReturnDto(TaskItem task)
        {
            return new ReturnTaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                IsCompleted = task.IsCompleted,
                TaskCollectionId = task.TaskCollectionId
            };
        }

        #endregion
    }
}