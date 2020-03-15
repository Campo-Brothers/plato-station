using ImagicleLibrary.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using plato.data.repository;
using Serilog;
using System.Threading.Tasks;

namespace plato.server.Controllers
{
    public class TermoController : Controller
    {
        private readonly ITermoRepository _termoRepository;

        public TermoController(IConfiguration configuration, ITermoRepository termoRepository)
        {
            _termoRepository = termoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var profile = await _termoRepository.GetActiveProfile();
            
            Log.Information<TermoController>(LogBuilder.StartNew("header").KeyValue("valore", 1), this);
            
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