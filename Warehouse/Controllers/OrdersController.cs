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
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto model)
        {
            if (model == null) return BadRequest();

           
            var result = await _mediator.Send(new CreateOrderCommand(model));
            return Ok(new { success = true, orderId = result });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var data = await _mediator.Send(new GetOrderQuery(id));

            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(
                                      int page = 1,
                                      int limit = 10,
                                      string? search = null)
        {
            var query = new GetAllOrderQuery(page, limit, search);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("{id}/confirm-delivery")]
        public async Task<IActionResult> ConfirmDelivery(string id, [FromBody] ConfirmDeliveryDto model)
        {
            if (model == null) return BadRequest();

            // Gửi Command kèm theo id đơn hàng và dữ liệu từ body sang cho MediatR xử lý
            var result = await _mediator.Send(new ConfirmDeliveryCommand(id, model));

            if (!result.IsSuccess) return StatusCode(500, new { success = false, message = "Xử lý thất bại hoặc đơn hàng không tồn tại." });

            return Ok(new
            {
                success = true,
                message = result.Message,       // Kết quả: "Đã gửi thành công"
                exportDate = result.ExportDate   // Kết quả: Chuỗi thời gian dạng "dd/MM/yyyy HH:mm" để frontend render lên UI
            });
        }
    }
}
