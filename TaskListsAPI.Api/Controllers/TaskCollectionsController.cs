using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Api.SwaggerExamples;
using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.DTOs.ResponseDTOs;

namespace TaskListsAPI.Api.Controllers
{
    /// <summary>
    /// Controller for managing task collections
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TaskCollectionsController : ControllerBase
    {
        private readonly ITaskCollectionService _service;

        #region Ctor

        public TaskCollectionsController(ITaskCollectionService service)
        {
            _service = service;
        }

        #endregion

        #region Methods

        /// <summary>Create a new task collection</summary>
        /// <param name="dto">Data for the new task collection</param>
        [HttpPost]
        [SwaggerRequestExample(typeof(CreateTaskCollectionDto), typeof(CreateTaskCollectionDtoExample))]
        [SwaggerResponseExample(200, typeof(ReturnTaskCollectionDtoExample))]
        [ProducesResponseType(typeof(ReturnTaskCollectionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTaskCollectionDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Update a task collection's name</summary>
        /// <param name="id">ID of the task collection to update</param>
        /// <param name="userId">ID of the user making the request</param>
        /// <param name="dto">New data for the task collection</param>
        [HttpPut("{id:guid}")]
        [SwaggerRequestExample(typeof(UpdateTaskCollectionDto), typeof(UpdateTaskCollectionDtoExample))]
        [SwaggerResponseExample(200, typeof(ReturnTaskCollectionDtoExample))]
        [ProducesResponseType(typeof(ReturnTaskCollectionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromQuery] Guid userId, [FromBody] UpdateTaskCollectionDto dto)
        {
            var result = await _service.UpdateAsync(id, userId, dto);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Delete a task collection (owner only)</summary>
        /// <param name="id">ID of the task collection to delete</param>
        /// <param name="userId">ID of the user making the request</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid userId)
        {
            var result = await _service.DeleteAsync(id, userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Get a task collection by ID</summary>
        /// <param name="id">ID of the task collection to retrieve</param>
        /// <param name="userId">ID of the user making the request</param>
        [HttpGet("{id:guid}")]
        [SwaggerResponseExample(200, typeof(ReturnTaskCollectionDtoExample))]
        [ProducesResponseType(typeof(ReturnTaskCollectionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] Guid userId)
        {
            var result = await _service.GetByIdAsync(id, userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Get all task collections for a user</summary>
        /// <param name="userId">ID of the user whose collections to retrieve</param>
        /// <param name="page">Page number for pagination (default 1)</param>
        /// <param name="pageSize">Number of items per page (default 20)</param>
        [HttpGet]
        [SwaggerResponseExample(200, typeof(ListReturnTaskCollectionDtoExample))]
        [ProducesResponseType(typeof(IEnumerable<ReturnTaskCollectionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] Guid userId, [FromQuery] int page = Constants.Constants.DefaultPage, [FromQuery] int pageSize = Constants.Constants.DefaultPageSize)
        {
            var result = await _service.GetAllAsync(userId, page, pageSize);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Share a task collection with another user</summary>
        /// <param name="id">ID of the task collection to share</param>
        /// <param name="userId">ID of the user making the request</param>
        /// <param name="dto">Data of the user to share with</param>
        [HttpPost("{id:guid}/share")]
        [SwaggerRequestExample(typeof(ShareTaskCollectionDto), typeof(ShareTaskCollectionDtoExample))]
        [SwaggerResponseExample(200, typeof(ReturnTaskCollectionDtoExample))]
        [ProducesResponseType(typeof(ReturnTaskCollectionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Share(Guid id, [FromQuery] Guid userId, [FromBody] ShareTaskCollectionDto dto)
        {
            var result = await _service.ShareAsync(id, userId, dto);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Remove a shared user from a task collection</summary>
        /// <param name="id">ID of the task collection</param>
        /// <param name="userId">ID of the user making the request</param>
        /// <param name="shareUserId">ID of the user to remove from the collection</param>
        [HttpDelete("{id:guid}/share/{shareUserId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Unshare(Guid id, [FromQuery] Guid userId, Guid shareUserId)
        {
            var result = await _service.UnshareAsync(id, userId, shareUserId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        #endregion
    }
}
