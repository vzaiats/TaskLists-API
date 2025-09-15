using Microsoft.EntityFrameworkCore;
using TaskListsAPI.Application.Interfaces;
using TaskListsAPI.Domain.Entities;
using TaskListsAPI.Infrastructure.Database;

namespace TaskListsAPI.Infrastructure.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly AppDbContext _dbContext;

        #region Ctor

        public TaskItemRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        public async Task<TaskItem?> GetByIdAsync(Guid id) =>
            await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<List<TaskItem>> GetAllByCollectionIdAsync(Guid collectionId) =>
            await _dbContext.TaskItems
                .Where(t => t.TaskCollectionId == collectionId)
                .ToListAsync();

        public async Task<List<TaskItem>> GetAllAsync() =>
            await _dbContext.TaskItems.ToListAsync();

        public async Task AddAsync(TaskItem task) => await _dbContext.TaskItems.AddAsync(task);

        public Task UpdateAsync(TaskItem task)
        {
            _dbContext.TaskItems.Update(task);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TaskItem task)
        {
            _dbContext.TaskItems.Remove(task);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}