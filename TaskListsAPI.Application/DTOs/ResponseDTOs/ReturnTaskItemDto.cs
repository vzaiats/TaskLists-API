namespace TaskListsAPI.Application.DTOs.ResponseDTOs
{
    /// <summary>
    /// DTO for a task item within a collection.
    /// </summary>
    public class ReturnTaskItemDto
    {
        /// <summary>
        /// Unique identifier of the task item.
        /// </summary>
        /// <example>11111111-1111-1111-1111-111111111111</example>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the task item.
        /// </summary>
        /// <example>Buy groceries</example>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the task item is completed.
        /// </summary>
        /// <example>false</example>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// ID of the collection this task item belongs to.
        /// </summary>
        /// <example>22222222-2222-2222-2222-222222222222</example>
        public Guid TaskCollectionId { get; set; }
    }
}