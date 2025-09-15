namespace TaskListsAPI.Domain.Entities
{
    // Single task inside a collection
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public Guid TaskCollectionId { get; set; }

        public TaskItem() { }

        public TaskItem(string title, Guid collectionId)
        {
            Title = title;
            TaskCollectionId = collectionId;
        }

        public void UpdateTitleAndStatus(string title, bool isCompleted)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;
            IsCompleted = isCompleted;
        }
    }
}