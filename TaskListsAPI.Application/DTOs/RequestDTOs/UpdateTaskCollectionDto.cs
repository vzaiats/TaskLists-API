using System.ComponentModel.DataAnnotations;
using TaskListsAPI.Application.Constants;

namespace TaskListsAPI.Application.DTOs.RequestDTOs
{
    /// <summary>
    /// DTO for updating a task collection's name.
    /// </summary>
    public class UpdateTaskCollectionDto
    {
        /// <summary>
        /// New name for the task collection.
        /// </summary>
        /// <example>My Updated Tasks</example>
        [Required(ErrorMessage = ErrorMessages.UpdateTaskCollectionNameRequired)]
        [StringLength(255, MinimumLength = 1, ErrorMessage = ErrorMessages.UpdateTaskCollectionNameLength)]
        public string Name { get; set; } = string.Empty;
    }
}