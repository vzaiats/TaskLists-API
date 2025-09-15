using System.ComponentModel.DataAnnotations;
using TaskListsAPI.Application.Constants;

namespace TaskListsAPI.Application.DTOs.RequestDTOs
{
    /// <summary>
    /// DTO for sharing a task collection with another user.
    /// </summary>
    public class ShareTaskCollectionDto
    {
        /// <summary>
        /// ID of the user to share the task collection with.
        /// </summary>
        /// <example>11111111-1111-1111-1111-111111111111</example>
        [Required(ErrorMessage = ErrorMessages.ShareTaskCollectionUserIdRequired)]
        public Guid UserId { get; set; }
    }
}