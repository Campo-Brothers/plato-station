using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using systemStatController = Plato.System.Stats.System.SystemStatusController;

namespace plato.server.Controllers
{
    public class SystemStatController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public SystemStatController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult SystemStat()
        {
            return base.View(systemStatController.GetMetrics());
        }
    }
}
