using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Application.DTOs.RequestDTOs;

namespace TaskListsAPI.Api.SwaggerExamples
{
    public class UpdateTaskCollectionDtoExample : IExamplesProvider<UpdateTaskCollectionDto>
    {
        public UpdateTaskCollectionDto GetExamples()
        {
            return new UpdateTaskCollectionDto
            {
                Name = "Alice's Updated Tasks"
            };
        }
    }
}