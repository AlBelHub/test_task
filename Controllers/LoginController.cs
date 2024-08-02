using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using test_task.Context;

namespace test_task.Controllers
{
    /// <summary>
    /// Метод для авторизации
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private readonly TestTaskContext _context;
        
        public LoginController(TestTaskContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Метод для авторизации Пользователя
        /// </summary>
        /// <param name="UserLogin">Логин Пользователя</param>
        /// <param name="UserPassword">Пароль Пользователя</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/login")]
        public async Task<IActionResult> Login(string UserLogin, string UserPassword)
        {
            var user = await _context.Users.FindAsync(UserLogin);

            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            if (user.UserPassword.IsNullOrEmpty())
            {
                user.UserPassword = BCrypt.Net.BCrypt.HashPassword(UserPassword);
                _context.Users.Update(user);
                _context.SaveChanges();
                return Ok("Пароль создан");
            }
            
            if (!BCrypt.Net.BCrypt.Verify(UserPassword, user.UserPassword))
            {
                return Unauthorized();
            }

            return Ok();
        }
    }

}