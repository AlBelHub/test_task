using System.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_task.Context;
using test_task.Models;

namespace Controllers
{
    /// <summary>
    /// Контроллер для выполнения действий над Заказами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly TestTaskContext _context;

        public OrderController(TestTaskContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Метод для получения всех заказов
        /// </summary>
        /// <returns>Список всех заказов с их позициями</returns>
        [HttpGet]
        [Route("allOrders/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound("Пользователь не найден");
            
            if (user.UserRole != UserRole.Admin)
                return Forbid();
            
            return await _context.Orders.Include(o => o.OrderItems).ToListAsync();
        }

        /// <summary>
        /// Метод для получения всех заказов конкретного пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список заказов пользователя с их позициями</returns>
        [HttpGet]
        [Route("userOrders/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(int userId)
        {
            return await _context.Orders.Where(order => order.UserId == userId).Include(o => o.OrderItems).ToListAsync();
        }

        /// <summary>
        /// Метод для создания заказа
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>
        /// NotFound если пользователь не найден
        /// Ok если заказ успешно создан
        /// </returns>
        [HttpPost]
        [Route("createOrder/{userId}")]
        public async Task<ActionResult<Order>> CreateOrder(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Метод для отмены заказа
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>
        /// NotFound если заказ не найден
        /// Ok если заказ и связанные с ним позиции успешно удалены
        /// </returns>
        [HttpDelete]
        [Route("cancelOrder/{orderId}")]
        public async Task<ActionResult> CancelOrder(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            // Удаление связанных позиций заказа
            _context.OrderItems.RemoveRange(order.OrderItems);

            // Удаление самого заказа
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return Ok("Заказ отменён");
        }

        /// <summary>
        /// Метод для размещения заказа
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>
        /// NotFound если заказ или продукт не найден
        /// Ok если заказ успешно размещен и итоговая стоимость рассчитана
        /// </returns>
        [HttpPut]
        [Route("placeOrder/{orderId}")]
        public async Task<ActionResult> PlaceOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            decimal totalOrderPrice = 0;

            foreach (var orderItem in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(orderItem.ProductId);

                if (product == null)
                {
                    return NotFound($"Товар не найден для Позиции {orderItem.OrderItemId}");
                }

                totalOrderPrice += orderItem.Quantity * product.Price;
            }

            order.OrderPrice = totalOrderPrice;

            await _context.SaveChangesAsync();
            
            return Ok("Заказ размещён");
        }

        /// <summary>
        /// Метод для добавления продукта в заказ
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="productId">Идентификатор Товара</param>
        /// <param name="quantity">Количество Товара</param>
        /// <returns>
        /// NotFound если заказ или Товар не найден
        /// Ok если Товар успешно добавлен в Заказ и стоимость пересчитана
        /// </returns>
        [HttpPost]
        [Route("addProduct/{orderId}")]
        public async Task<ActionResult<Order>> AddProductToOrder(int orderId, int productId, int quantity)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                                             .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Товар не найден");
            }

            var ExistingOrderItem = order.OrderItems.FirstOrDefault(item => item.ProductId == productId);
            if (ExistingOrderItem != null)
            {
                ExistingOrderItem.Quantity = quantity;
                _context.OrderItems.Update(ExistingOrderItem);
            }
            else
            {
                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    ProductId = productId,
                    Quantity = quantity
                };

                order.OrderItems.Add(orderItem);
            }

            // Пересчет стоимости заказа
            order.OrderPrice = order.OrderItems.Sum(oi => oi.Quantity * product.Price);

            await _context.SaveChangesAsync();

            return Ok("Товар добавлен, стоимость пересчитана");
        }

        /// <summary>
        /// Метод для удаления продукта из заказа
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="productId">Идентификатор продукта</param>
        /// <returns>
        /// NotFound если заказ или продукт не найден
        /// Ok если продукт успешно удален из заказа и стоимость пересчитана
        /// </returns>
        [HttpDelete]
        [Route("removeProduct/{orderId}")]
        public async Task<ActionResult> RemoveProductFromOrder(int orderId, int productId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                                             .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);

            if (orderItem == null)
            {
                return NotFound($"Позиция заказа не существует {orderItem.OrderItemId}");
            }

            order.OrderItems.Remove(orderItem);

            // Пересчет стоимости заказа
            var product = await _context.Products.FindAsync(productId);
            order.OrderPrice = order.OrderItems.Sum(oi => oi.Quantity * product.Price);

            await _context.SaveChangesAsync();

            return Ok("Товар убран, стоимость пересчитана");
        }
    }

}
