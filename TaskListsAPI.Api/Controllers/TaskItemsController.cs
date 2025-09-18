using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using TaskListsAPI.Api.SwaggerExamples;
using TaskListsAPI.Application.DTOs.RequestDTOs;
using TaskListsAPI.Application.DTOs.ResponseDTOs;
using TaskListsAPI.Application.Interfaces;

namespace TaskListsAPI.Api.Controllers
{
    /// <summary>
    /// Controller for managing task items within task collections
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TaskItemsController : ControllerBase
    {
        private readonly ITaskItemService _service;

        #region Ctor

        public TaskItemsController(ITaskItemService service)
        {
            _service = service;
        }

        #endregion

        #region Methods

        /// <summary>Create a new task</summary>
        /// <param name="dto">Data for the new task</param>
        [HttpPost]
        [SwaggerRequestExample(typeof(CreateTaskItemDto), typeof(CreateTaskItemDtoExample))]
        [SwaggerResponseExample(200, typeof(ReturnTaskItemDtoExample))]
        [ProducesResponseType(typeof(ReturnTaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTaskItemDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Update an existing task</summary>
        /// <param name="id">ID of the task to update</param>
        /// <param name="dto">Updated task data</param>
        [HttpPut("{id:guid}")]
        [SwaggerRequestExample(typeof(UpdateTaskItemDto), typeof(UpdateTaskItemDtoExample))]
        [SwaggerResponseExample(200, typeof(ReturnTaskItemDtoExample))]
        [ProducesResponseType(typeof(ReturnTaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskItemDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Delete a task</summary>
        /// <param name="id">ID of the task to delete</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Get a task by ID</summary>
        /// <param name="id">ID of the task to retrieve</param>
        [HttpGet("{id:guid}")]
        [SwaggerResponseExample(200, typeof(ReturnTaskItemDtoExample))]
        [ProducesResponseType(typeof(ReturnTaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        /// <summary>Get all tasks in a collection</summary>
        /// <param name="collectionId">ID of the task collection</param>
        [HttpGet("collection/{collectionId:guid}")]
        [SwaggerResponseExample(200, typeof(ListReturnTaskItemDtoExample))]
        [ProducesResponseType(typeof(IEnumerable<ReturnTaskItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllByCollectionId(Guid collectionId)
        {
            var result = await _service.GetAllByCollectionIdAsync(collectionId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
        }

        #endregion
    }
}
