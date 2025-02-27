using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ADOLib;
using static ADOLib.Model.Model;
using Microsoft.AspNetCore.Identity.Data;

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
    }
}
