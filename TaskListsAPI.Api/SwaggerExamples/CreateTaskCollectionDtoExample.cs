using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Application.DTOs.RequestDTOs;

namespace TaskListsAPI.Api.SwaggerExamples
{
    public class CreateTaskCollectionDtoExample : IExamplesProvider<CreateTaskCollectionDto>
    {
        public CreateTaskCollectionDto GetExamples()
        {
            return new CreateTaskCollectionDto
            {
                Name = "Alice's New Tasks",
                OwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111") // Alice
            };
        }
    }
}