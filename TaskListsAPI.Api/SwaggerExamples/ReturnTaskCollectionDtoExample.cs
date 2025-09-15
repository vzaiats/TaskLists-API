using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Application.DTOs.ResponseDTOs;

namespace TaskListsAPI.Api.SwaggerExamples
{
    public class ReturnTaskCollectionDtoExample : IExamplesProvider<ReturnTaskCollectionDto>
    {
        public ReturnTaskCollectionDto GetExamples()
        {
            return new ReturnTaskCollectionDto
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Alice Tasks",
                OwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = DateTime.UtcNow,
                Shares = new List<ReturnShareDto>
                {
                    new ReturnShareDto
                    {
                        UserId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                    },
                    new ReturnShareDto
                    {
                        UserId = Guid.Parse("44444444-4444-4444-4444-444444444444")
                    }
                }
            };
        }
    }
}