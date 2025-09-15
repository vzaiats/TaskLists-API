using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Application.Interfaces
{
    public interface ITaskCollectionRepository
    {
        Task<TaskCollection?> GetByIdAsync(Guid id);
        Task<List<TaskCollection>> GetAllAsync();
        Task AddAsync(TaskCollection collection);
        Task UpdateAsync(TaskCollection collection);
        Task DeleteAsync(TaskCollection collection);
        Task SaveChangesAsync();
    }
}