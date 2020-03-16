using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using plato.data.repository;
using System.Threading.Tasks;

namespace plato.server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userService;

        public UsersController(IConfiguration configuration)
        {
            _userService = new UserRepository(configuration); ;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }
    }
}