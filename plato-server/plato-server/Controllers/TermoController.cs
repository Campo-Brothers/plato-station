using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using plato.data.repository;
using System.Threading.Tasks;

namespace plato.server.Controllers
{
    public class TermoController : Controller
    {
        private readonly ITermoRepository _termoService;
        private readonly ILogger _logger;

        public TermoController(ITermoRepository termoService, ILogger<TermoController> logger)
        {
            _termoService = termoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var profile = await _termoService.GetActiveProfile();
            
            _logger.BeginScope(typeof(TermoController));
            _logger.LogInformation($"Chiamata GET con oggetto di ritorno: {@profile}");
            
            return View(profile);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var profiles = await _termoService.GetAllProfiles();
        //    return Ok(profiles);
        //}
    }
}