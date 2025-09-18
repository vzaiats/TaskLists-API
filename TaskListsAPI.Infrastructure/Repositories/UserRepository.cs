using Microsoft.EntityFrameworkCore;
using TaskListsAPI.Application.Interfaces;
using TaskListsAPI.Domain.Entities;
using TaskListsAPI.Infrastructure.Database;

namespace TaskListsAPI.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        #region Ctor

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<List<User>> GetAllAsync() =>
            await _dbContext.Users
                .OrderBy(u => u.Name)
                .ToListAsync();

        public async Task AddAsync(User user) =>
            await _dbContext.Users.AddAsync(user);

        public Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() =>
            await _dbContext.SaveChangesAsync();

        #endregion
    }
}