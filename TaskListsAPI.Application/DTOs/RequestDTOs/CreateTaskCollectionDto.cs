using System.ComponentModel.DataAnnotations;
using TaskListsAPI.Application.Constants;

namespace TaskListsAPI.Application.DTOs.RequestDTOs
{
    /// <summary>
    /// DTO for creating a new task collection.
    /// </summary>
    public class CreateTaskCollectionDto
    {
        /// <summary>
        /// Name of the task collection.
        /// </summary>
        /// <example>My Tasks</example>
        [Required(ErrorMessage = ErrorMessages.NameRequired)]
        [StringLength(255, MinimumLength = 1, ErrorMessage = ErrorMessages.NameLength)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID of the user who will own the task collection.
        /// </summary>
        /// <example>11111111-1111-1111-1111-111111111111</example>
        [Required(ErrorMessage = ErrorMessages.OwnerIdRequired)]
        public Guid OwnerId { get; set; }
    }
}