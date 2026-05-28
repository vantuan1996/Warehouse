using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Core.Commands;
using Warehouse.Core.DTOs;
using Warehouse.Core.Queries;

//using Warehouse.Core.Services;
using Warehouse.Domain.Entities;

namespace Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/categories")]

    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var data = await _mediator.Send(new GetCategoryQuery(id));

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateDto dto)
        {
            var command = new CreateCategoryCommand(dto);

            var id = await _mediator.Send(command);

            return Ok(id);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromForm] CategoryUpdateDto dto)
        {
            //dto.Id = id;

            var command = new UpdateCategoryCommand(dto, id);

            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok(id);
            }
            return Ok(null);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var command = new DeleteCategoryCommand(id);

            var result = await _mediator.Send(command);

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(
                                       int page = 1,
                                       int limit = 10,
                                       string? search = null)
        {
            var query = new GetALLCategoryQuery(page, limit, search);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteCategoryIdsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
