using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Application.DTOs.RequestDTOs;

namespace TaskListsAPI.Api.SwaggerExamples
{
    public class ShareTaskCollectionDtoExample : IExamplesProvider<ShareTaskCollectionDto>
    {
        public ShareTaskCollectionDto GetExamples()
        {
            return new ShareTaskCollectionDto
            {
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            };
        }
    }
}