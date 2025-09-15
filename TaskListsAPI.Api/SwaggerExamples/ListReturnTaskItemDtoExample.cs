using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Application.DTOs.ResponseDTOs;

namespace TaskListsAPI.Api.SwaggerExamples
{
    public class ListReturnTaskItemDtoExample : IExamplesProvider<List<ReturnTaskItemDto>>
    {
        public List<ReturnTaskItemDto> GetExamples()
        {
            return new List<ReturnTaskItemDto>
            {
                new ReturnTaskItemDtoExample().GetExamples(),
                new ReturnTaskItemDtoExample().GetExamples()
            };
        }
    }
}