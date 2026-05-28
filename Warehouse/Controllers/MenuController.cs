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
    [Route("api/customers")]

    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenuController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Lấy danh sách menu phân cấp cho Sidebar
        /// </summary>
        [HttpGet("sidebar")]
        public async Task<IActionResult> GetSidebar()
        {
            try
            {
                // Gửi request đến Handler thông qua MediatR
                var result = await _mediator.Send(new GetSidebarMenuQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log error ở đây (nếu có logger)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
