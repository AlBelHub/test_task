using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using test_task.Context;
using test_task.Models;

namespace Controllers
{
    /// <summary>
    /// Контроллер для выполнения действий над Товарами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly TestTaskContext _context;

        /// <summary>
        /// Использование DI контейнера
        /// </summary>
        /// <param name="context"></param>
        public ProductController(TestTaskContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Метод для получения списка всех Товаров
        /// </summary>
        /// <param name="userId">Индентификатор пользователя</param>
        /// <returns>
        /// NotFound если пользователь не найден
        /// Forbid если пользователь не имеет доступа, иначе возвращается список всех Товаров
        /// </returns>
        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            if (user.UserRole != UserRole.Admin)
            {
                return Forbid();
            }
            
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// Метод для получения Товара по id
        /// </summary>
        /// <param name="id">Индентификатор Товара</param>
        /// <returns>
        /// NotFound если Товар не найден, иначе возвращается Товар
        /// </returns>
        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Такой Товар не найден");
            }

            return product;
        }

        /// <summary>
        /// Метод для созания Товара
        /// </summary>
        /// <param name="product">Сущность Товара</param>
        /// <returns>
        /// NotFound если Товар не был передан
        /// BadRequest если название Товара не указано \ Стоимость Товара не указана
        /// </returns>
        [HttpPost]
        [Route("api/product/create")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return NotFound("Отсутсвует товар");
            } 
            else if (product.ProductName.IsNullOrEmpty())
            {
                return BadRequest("Не указано название Товара");
            }
            else if (product.Price <= 0 )
            {
                return BadRequest("Не указана стоимость Товара");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        /// <summary>
        /// Метод для обновления существующего товара
        /// </summary>
        /// <param name="product">Сущность Товара</param>
        /// <returns>
        /// NotFound если Товар не найден
        /// Ok если обновление прошло успешно
        /// </returns>
        [HttpPut]
        [Route("api/product/update/{id}")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var EFproduct = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (EFproduct == null)
            {
                return NotFound();
            }

            EFproduct.Price = product.Price;
            EFproduct.ProductName = product.ProductName;

            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Метод для удаления Товара по id
        /// </summary>
        /// <param name="id">Идентификатор Товара</param>
        /// <returns>
        /// NotFound если Товар не найден
        /// NoContent если удаление прошло успешно
        /// </returns>
        [HttpDelete]
        [Route("api/product/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

}
