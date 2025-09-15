using Microsoft.EntityFrameworkCore;
using TaskListsAPI.Application.Interfaces;
using TaskListsAPI.Domain.Entities;
using TaskListsAPI.Infrastructure.Database;

namespace TaskListsAPI.Infrastructure.Repositories
{
    public class TaskCollectionRepository : ITaskCollectionRepository
    {
        private readonly AppDbContext _dbContext;

        #region Ctor

        public TaskCollectionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        public async Task<TaskCollection?> GetByIdAsync(Guid id)
        {
            return await _dbContext.TaskCollections
                .Include(tc => tc.Shares)
                .Include(tc => tc.Tasks)
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task<List<TaskCollection>> GetAllAsync()
        {
            return await _dbContext.TaskCollections
                .Include(tc => tc.Shares)
                .Include(tc => tc.Tasks)
                .OrderByDescending(tc => tc.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(TaskCollection collection)
        {
            await _dbContext.TaskCollections.AddAsync(collection);
        }

        public Task UpdateAsync(TaskCollection collection)
        {
            _dbContext.TaskCollections.Update(collection);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TaskCollection collection)
        {
            _dbContext.TaskCollections.Remove(collection);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}