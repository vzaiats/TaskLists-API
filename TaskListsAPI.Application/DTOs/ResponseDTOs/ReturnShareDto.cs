namespace TaskListsAPI.Application.DTOs.ResponseDTOs
{
    /// <summary>
    /// DTO for a user with whom a task collection is shared.
    /// </summary>
    public class ReturnShareDto
    {
        /// <summary>
        /// ID of the shared user.
        /// </summary>
        /// <example>11111111-1111-1111-1111-111111111111</example>
        public Guid UserId { get; set; }
    }
}