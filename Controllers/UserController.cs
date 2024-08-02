using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_task.Models;
using test_task.Context;

namespace Controllers
{
    /// <summary>
    /// Контроллер для выполнения действий над Пользователем
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TestTaskContext _context;
        
        /// <summary>
        /// Использование DI контейнера
        /// </summary>
        /// <param name="context"></param>
        public UserController(TestTaskContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Метод для получения информации о пользователе
        /// </summary>
        /// <param name="id">Индентификатор пользователя</param>
        /// <returns>Возвращает NotFound в случае отсутсвия такого пользователя, иначе Ok с информацией о Пользователе </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Метод для получения списка всех Пользователей
        /// </summary>
        /// <param name="UserId">Индентификатор пользвателя</param>
        /// <returns>
        /// NotFound если пользователь не найден
        /// Forbid если пользователь не имеет доступа, иначе Ok
        /// </returns>
        [HttpGet("{UserId}")]

        public async Task<ActionResult<User>> GetAllUsers(int UserId)
        {
            var user = await _context.Users.FindAsync(UserId);

            if (user == null)
                return NotFound("Пользователь не найден");
            
            if (user.UserRole != UserRole.Admin)
                return Forbid();
            
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        /// <summary>
        /// Метод для получения информации о правах пользователя
        /// </summary>
        /// <param name="id">Индентификатор пользвателя</param>
        /// <returns>NotFound если пользователь не найден, иначе Ok</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserRole(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.UserRole);

        }


    }
}
