using TaskListsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TaskListsAPI.Infrastructure.Database
{
    public static class SeedData
    {
        public static async Task ApplyAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            // Seed Users
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Alice" },
                    new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Bob" },
                    new User { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Charlie" },
                    new User { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Diana" },
                    new User { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Eve" }
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

            // Seed TaskCollections
            if (!context.TaskCollections.Any())
            {
                var collections = new List<TaskCollection>
                {
                    new TaskCollection("Alice Tasks", Guid.Parse("11111111-1111-1111-1111-111111111111")),
                    new TaskCollection("Bob Tasks", Guid.Parse("22222222-2222-2222-2222-222222222222")),
                    new TaskCollection("Charlie Tasks", Guid.Parse("33333333-3333-3333-3333-333333333333")),
                    new TaskCollection("Shared Project", Guid.Parse("11111111-1111-1111-1111-111111111111"))
                };

                context.TaskCollections.AddRange(collections);
                await context.SaveChangesAsync();

                var sharedCollection = collections.First(c => c.Name == "Shared Project");

                var shares = new List<Share>
                {
                    new Share(Guid.Parse("22222222-2222-2222-2222-222222222222"), sharedCollection.Id), // Bob
                    new Share(Guid.Parse("44444444-4444-4444-4444-444444444444"), sharedCollection.Id)  // Diana
                };

                context.Shares.AddRange(shares);
                await context.SaveChangesAsync();

                // Seed TaskItems
                if (!context.TaskItems.Any())
                {
                    var tasks = new List<TaskItem>
                    {
                        new TaskItem("Buy milk", collections[0].Id),
                        new TaskItem("Finish report", collections[0].Id),
                        new TaskItem("Call Bob", collections[1].Id),
                        new TaskItem("Email client", collections[1].Id),
                        new TaskItem("Prepare slides", collections[2].Id),
                        new TaskItem("Setup meeting", sharedCollection.Id),
                        new TaskItem("Deploy project", sharedCollection.Id)
                    };

                    context.TaskItems.AddRange(tasks);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
