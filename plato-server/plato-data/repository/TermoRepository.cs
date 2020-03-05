using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using plato.data.database;
using plato.data.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plato.data.repository
{
    public class TermoRepository : ITermoRepository
    {
        private readonly PlatoStationDbContext _context;

        public TermoRepository(IConfiguration config)
        {
            DbContextOptionsBuilder<PlatoStationDbContext> builder = new DbContextOptionsBuilder<PlatoStationDbContext>();
            var options = builder.UseMySql(config.GetConnectionString("DefaultConnection"),
                mySqlOptions => mySqlOptions.ServerVersion(new ServerVersion(new Version(8, 0, 18), ServerType.MySql))
            ).Options;
            _context = new PlatoStationDbContext(options);
        }

        public async Task<IEnumerable<TemperatureProfile>> GetAllProfiles()
        {
            return await Task.Run(() => _context.TemperatureProfiles.Include(s => s.Schedules));
        }
    }
}
