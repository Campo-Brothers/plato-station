using Microsoft.AspNetCore.Mvc;
using plato.data.repository;
using System.Threading.Tasks;

namespace plato.server.Controllers
{
    public class TermoController : Controller
    {
        private ITermoRepository _termoService;

        public TermoController(ITermoRepository termoService)
        {
            _termoService = termoService;
        }

        public async Task<IActionResult> Index()
        {
            var profiles = await _termoService.GetAllProfiles();
            
            return View(profiles);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var profiles = await _termoService.GetAllProfiles();
        //    return Ok(profiles);
        //}
    }
}