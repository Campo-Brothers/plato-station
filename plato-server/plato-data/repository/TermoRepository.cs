using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using plato.data.database;
using plato.data.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<TemperatureProfile> FillProfileWithDefault(TemperatureProfile profile)
        {
            return await Task.Run(() =>
            {
                _context.TemperatureProfiles.Add(profile);
                _context.SaveChanges();

                for(int dayOfWeek = 1; dayOfWeek <= 7; dayOfWeek++)
                {
                    var sched = new TemperatureSchedule
                    {
                        DayOfWeek = dayOfWeek,
                        Profile = profile,
                        Start = new TimeSpan(0, 0, 0),
                        End = new TimeSpan(6, 29, 59),
                        Temperature = 16.0
                    };

                    var sched2 = new TemperatureSchedule
                    {
                        DayOfWeek = dayOfWeek,
                        Profile = profile,
                        Start = new TimeSpan(6, 30, 0),
                        End = new TimeSpan(7, 29, 59),
                        Temperature = 19.0
                    };

                    var sched3 = new TemperatureSchedule
                    {
                        DayOfWeek = dayOfWeek,
                        Profile = profile,
                        Start = new TimeSpan(7, 30, 0),
                        End = new TimeSpan(17, 29, 59),
                        Temperature = 16.0
                    };

                    var sched4 = new TemperatureSchedule
                    {
                        DayOfWeek = dayOfWeek,
                        Profile = profile,
                        Start = new TimeSpan(17, 30, 0),
                        End = new TimeSpan(22, 29, 59),
                        Temperature = 19.0
                    };

                    var sched5 = new TemperatureSchedule
                    {
                        DayOfWeek = dayOfWeek,
                        Profile = profile,
                        Start = new TimeSpan(22, 30, 0),
                        End = new TimeSpan(23, 59, 59),
                        Temperature = 16.0
                    };
                    
                    profile.Schedules.Add(sched);
                    profile.Schedules.Add(sched2);
                    profile.Schedules.Add(sched3);
                    profile.Schedules.Add(sched4);
                    profile.Schedules.Add(sched5);
                }

                _context.SaveChanges();

                return profile;
            });
        }

        public async Task<TemperatureProfile> GetActiveProfile()
        {
            return await Task.Run(() =>
            {
                var profiles = _context.TemperatureProfiles.Include(p => p.Schedules).ToList();
                var profile = profiles.Where(p => p.Current).First();
                profile.Schedules = profile.Schedules.OrderBy(s => s.DayOfWeek).ThenBy(s => s.Start).ToList();
                return profile;
            });
        }

        public async Task<IEnumerable<TemperatureProfile>> GetAllProfiles()
        {
            return await Task.Run(() => _context.TemperatureProfiles.Include(s => s.Schedules));
        }
    }
}
