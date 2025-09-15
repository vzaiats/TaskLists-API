using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(Constants.Constants.MaxNameLength);

            entity.HasMany(u => u.OwnedCollections)
                .WithOne()
                .HasForeignKey(tc => tc.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.SharedCollections)
                .WithOne()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}