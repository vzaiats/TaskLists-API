using System.ComponentModel.DataAnnotations;
using TaskListsAPI.Application.Constants;

namespace TaskListsAPI.Application.DTOs.RequestDTOs
{
    /// <summary>
    /// DTO for updating an existing task item.
    /// </summary>
    public class UpdateTaskItemDto
    {
        /// <summary>
        /// Title of the task item.
        /// </summary>
        /// <example>Buy groceries</example>
        [Required(ErrorMessage = ErrorMessages.UpdateTaskItemTitleRequired)]
        [StringLength(255, ErrorMessage = ErrorMessages.UpdateTaskItemTitleLength)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the task item is completed.
        /// </summary>
        /// <example>false</example>
        [Required(ErrorMessage = ErrorMessages.UpdateTaskItemIsCompletedRequired)]
        public bool IsCompleted { get; set; }
    }
}