using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Application.DTOs.RequestDTOs;

namespace TaskListsAPI.Api.SwaggerExamples
{
    public class UpdateTaskItemDtoExample : IExamplesProvider<UpdateTaskItemDto>
    {
        public UpdateTaskItemDto GetExamples()
        {
            return new UpdateTaskItemDto
            {
                Title = "Buy milk and eggs",
                IsCompleted = true
            };
        }
    }
}