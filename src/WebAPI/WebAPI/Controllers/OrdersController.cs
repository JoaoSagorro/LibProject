using ADOLib;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly Orders _orderService;

        public OrdersController(Orders orderService)
        {
            _orderService = orderService;
        }

        [HttpGet()]
        public IActionResult GetOrders(int userId)
        {
            var orders = _orderService.CheckOrderState(userId);
            return Ok(orders);
        }

        [HttpGet("returned")]
        public IActionResult GetReturnedOrders(int userId, int? libraryId = null) // Make libraryId optional
        {
            var orders = _orderService.GetReturnedOrders(userId, libraryId); // Pass libraryId to the service
            return Ok(orders);
        }
    }
}

