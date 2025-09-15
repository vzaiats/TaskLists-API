using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Application.DTOs.ResponseDTOs;

namespace TaskListsAPI.Api.SwaggerExamples
{
    public class ListReturnTaskCollectionDtoExample : IExamplesProvider<List<ReturnTaskCollectionDto>>
    {
        public List<ReturnTaskCollectionDto> GetExamples()
        {
            return new List<ReturnTaskCollectionDto>
            {
                new ReturnTaskCollectionDtoExample().GetExamples(),
                new ReturnTaskCollectionDtoExample().GetExamples()
            };
        }
    }
}