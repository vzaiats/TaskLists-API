using System.ComponentModel.DataAnnotations;
using TaskListsAPI.Application.Constants;

namespace TaskListsAPI.Application.DTOs.RequestDTOs
{
    /// <summary>
    /// DTO for creating a new task item.
    /// </summary>
    public class CreateTaskItemDto
    {
        /// <summary>
        /// Title of the task item.
        /// </summary>
        /// <example>Buy groceries</example>
        [Required(ErrorMessage = ErrorMessages.TaskItemTitleRequired)]
        [StringLength(255, ErrorMessage = ErrorMessages.TaskItemTitleLength)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// ID of the task collection to which this task item belongs.
        /// </summary>
        /// <example>11111111-1111-1111-1111-111111111111</example>
        [Required(ErrorMessage = ErrorMessages.TaskItemCollectionIdRequired)]
        public Guid TaskCollectionId { get; set; }
    }
}