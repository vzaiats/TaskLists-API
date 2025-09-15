namespace TaskListsAPI.Domain.Entities
{
    // Collection of tasks
    public class TaskCollection
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Share> Shares { get; set; } = new();

        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public TaskCollection() { }

        public TaskCollection(string name, Guid ownerId)
        {
            Name = name;
            OwnerId = ownerId;
        }

        public void Rename(string newName)
        {
            if (!string.IsNullOrWhiteSpace(newName))
                Name = newName;
        }
    }
}