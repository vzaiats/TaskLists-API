using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Infrastructure.Configurations
{
    public class TaskCollectionConfiguration : IEntityTypeConfiguration<TaskCollection>
    {
        public void Configure(EntityTypeBuilder<TaskCollection> entity)
        {
            entity.ToTable("TaskCollections");

            entity.HasKey(tc => tc.Id);

            entity.Property(tc => tc.Name)
                .IsRequired()
                .HasMaxLength(Constants.Constants.MaxNameLength);

            entity.Property(tc => tc.OwnerId).IsRequired();

            entity.Property(tc => tc.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasMany(tc => tc.Shares)
                .WithOne()
                .HasForeignKey(s => s.TaskCollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(tc => tc.Tasks)
                .WithOne()
                .HasForeignKey(t => t.TaskCollectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}