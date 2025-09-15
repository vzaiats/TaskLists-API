namespace TaskListsAPI.Domain.Entities
{
    // Shared user for a TaskCollection
    public class Share
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TaskCollectionId { get; set; }

        public Share() { }

        public Share(Guid userId, Guid collectionId)
        {
            UserId = userId;
            TaskCollectionId = collectionId;
        }
    }
}