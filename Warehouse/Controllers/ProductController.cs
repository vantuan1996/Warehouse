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
    [Route("api/products")]

    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var data = await _mediator.Send(new GetProductQuery(id));
                return Ok(data);
            }
            catch (Exception ex)
            {
                var xxx = ex;
                throw;
            }
           

           
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto dto)
        {
            var command = new CreateProductCommand(dto);

            var id = await _mediator.Send(command);

            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromForm] ProductCreateDto dto)
        {
            //dto.Id = id;

            var command = new UpdateProductCommand(dto, id);

            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok(id);
            }
            return Ok(null);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var command = new DeleteProductCommand(id);

            var result = await _mediator.Send(command);

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(
                                       int page = 1,
                                       int limit = 10,
                                       string? search = null)
        {
            var query = new GetALLProductQuery(page, limit, search);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteProductIdsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
