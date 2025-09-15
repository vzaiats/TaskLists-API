using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Application.DTOs.ResponseDTOs;

namespace TaskListsAPI.Api.SwaggerExamples
{
    public class ReturnTaskItemDtoExample : IExamplesProvider<ReturnTaskItemDto>
    {
        public ReturnTaskItemDto GetExamples()
        {
            return new ReturnTaskItemDto
            {
                Id = Guid.NewGuid(),
                Title = "Buy milk",
                IsCompleted = false,
                TaskCollectionId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            };
        }
    }
}