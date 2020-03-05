using Microsoft.AspNetCore.Mvc;
using plato.data.repository;
using System.Threading.Tasks;

namespace plato.server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TermoController : ControllerBase
    {
        private ITermoRepository _termoService;

        public TermoController(ITermoRepository termoService)
        {
            _termoService = termoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profiles = await _termoService.GetAllProfiles();
            return Ok(profiles);
        }
    }
}