using plato.data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plato.data.repository
{
    public interface ITermoRepository
    {
        Task<IEnumerable<TemperatureProfile>> GetAllProfiles();
    }
}
