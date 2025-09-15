namespace TaskListsAPI.Application.DTOs.ResponseDTOs
{
    /// <summary>
    /// DTO for a task collection.
    /// </summary>
    public class ReturnTaskCollectionDto
    {
        /// <summary>
        /// Unique identifier of the task collection.
        /// </summary>
        /// <example>11111111-1111-1111-1111-111111111111</example>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the task collection.
        /// </summary>
        /// <example>My Tasks</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID of the user who owns the task collection.
        /// </summary>
        /// <example>22222222-2222-2222-2222-222222222222</example>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Date and time when the collection was created.
        /// </summary>
        /// <example>2025-09-14T12:34:56Z</example>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// List of users with whom this collection is shared.
        /// </summary>
        public List<ReturnShareDto> Shares { get; set; } = new();
    }
}