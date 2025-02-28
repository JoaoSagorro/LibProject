using ADOLib;
using Microsoft.AspNetCore.Mvc;
using static ADOLib.Model.Model;

namespace WebAPI.Controllers
{
    public class OrdersController
    {
        private readonly Orders _orders;
        
        public OrdersController()
        {
            _orders = new Orders();
        }

        [HttpGet("orders")]
        public List<Order> GetOrdersByUserId(int userId)
        {
            try
            {
                List<Order> orders = _orders.GetOrdersByUserId(userId);
                return orders;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        } 
    }
}
