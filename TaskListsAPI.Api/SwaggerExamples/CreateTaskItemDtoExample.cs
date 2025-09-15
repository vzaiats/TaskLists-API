using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Application.DTOs.RequestDTOs;

namespace TaskListsAPI.Api.SwaggerExamples
{
    public class CreateTaskItemDtoExample : IExamplesProvider<CreateTaskItemDto>
    {
        public CreateTaskItemDto GetExamples()
        {
            return new CreateTaskItemDto
            {
                Title = "Buy milk",
                TaskCollectionId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            };
        }
    }
}