using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskListsAPI.Domain.Entities;

namespace TaskListsAPI.Infrastructure.Configurations
{
    public class ShareConfiguration : IEntityTypeConfiguration<Share>
    {
        public void Configure(EntityTypeBuilder<Share> entity)
        {
            entity.ToTable("Shares");

            entity.HasKey(s => new { s.TaskCollectionId, s.UserId });

            entity.Property(s => s.UserId).IsRequired();
            entity.Property(s => s.TaskCollectionId).IsRequired();
        }
    }
}