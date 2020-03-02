using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using plato_data;

namespace plato_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PlatoStationDbContext _dbContext;

        public UsersController(ILogger<HomeController> logger, PlatoStationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET api/sync
        [HttpGet]
        public IActionResult Get()
        {
            return GetUsers();
        }

        private ObjectResult GetUsers()
        {
            var users = _dbContext.Users;
            return new ObjectResult(users);
        }
    }
}