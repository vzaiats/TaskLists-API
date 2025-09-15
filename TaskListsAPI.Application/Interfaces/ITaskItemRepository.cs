using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Application.Interfaces
{
    public interface ITaskItemRepository
    {
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<List<TaskItem>> GetAllByCollectionIdAsync(Guid collectionId);
        Task AddAsync(TaskItem item);
        Task UpdateAsync(TaskItem item);
        Task DeleteAsync(TaskItem item);
        Task SaveChangesAsync();
    }
}