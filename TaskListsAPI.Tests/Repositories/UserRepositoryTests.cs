using Microsoft.EntityFrameworkCore;
using TaskListsAPI.Domain.Entities;
using TaskListsAPI.Infrastructure.Database;
using TaskListsAPI.Infrastructure.Repositories;

namespace TaskListsAPI.Tests.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserRepository _userRepository;

        #region Ctor

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _userRepository = new UserRepository(_context);

            SeedDatabase().Wait();
        }

        #endregion

        #region Seed Data

        private async Task SeedDatabase()
        {
            var users = new List<User>
            {
                new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Alice" },
                new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Bob" }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region GetByIdAsync Tests

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111", "Alice")]
        [InlineData("22222222-2222-2222-2222-222222222222", "Bob")]
        public async Task GetByIdAsync_ShouldReturnUser_WhenExists(string idStr, string expectedName)
        {
            // Arrange
            var id = Guid.Parse(idStr);

            // Act
            var result = await _userRepository.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedName, result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _userRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Act
            var result = await _userRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Name == "Alice");
            Assert.Contains(result, u => u.Name == "Bob");
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            // Arrange
            var newUser = new User { Id = Guid.NewGuid(), Name = "Charlie" };

            // Act
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            var added = await _context.Users.FindAsync(newUser.Id);

            // Assert
            Assert.NotNull(added);
            Assert.Equal("Charlie", added!.Name);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            // Arrange
            var user = await _context.Users.FirstAsync();
            user.Name = "Updated Name";

            // Act
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            var updated = await _context.Users.FindAsync(user.Id);

            // Assert
            Assert.Equal("Updated Name", updated!.Name);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            // Arrange
            var user = await _context.Users.FirstAsync();

            // Act
            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();

            var deleted = await _context.Users.FindAsync(user.Id);

            // Assert
            Assert.Null(deleted);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion
    }
}
