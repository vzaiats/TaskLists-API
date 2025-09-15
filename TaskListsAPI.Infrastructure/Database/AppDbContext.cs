using Microsoft.EntityFrameworkCore;
using TaskListsAPI.Domain.Entities;
using TaskListsAPI.Infrastructure.Configurations;

namespace TaskListsAPI.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<TaskCollection> TaskCollections { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Share> Shares { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new TaskCollectionConfiguration());
            builder.ApplyConfiguration(new TaskItemConfiguration());
            builder.ApplyConfiguration(new ShareConfiguration());
        }
    }
}