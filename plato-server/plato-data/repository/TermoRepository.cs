using ImagicleLibrary.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using plato.data.database;
using plato.data.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace plato.data.repository
{
    public class TermoRepository : ITermoRepository
    {
        private readonly PlatoStationDbContext _context;
        private ILogger _logger;

        public TermoRepository(IConfiguration config)
        {
            _logger = Log.Logger;
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
                TemperatureProfile profile = null;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {
                    var profiles = _context.TemperatureProfiles.Include(p => p.Schedules).ToList();
                    profile = profiles.Where(p => p.Current).First();
                    profile.Schedules = profile.Schedules.OrderBy(s => s.DayOfWeek).ThenBy(s => s.Start).ToList();
                    _logger.Information(
                        LogBuilder.StartNew("Active profile")
                        .KeyValue("Name", profile.Name)
                        .KeyValue("Configurations", profile.Schedules.Count)
                        .ElapsedTime(sw));
                }
                catch (Exception err)
                {
                    _logger.Error(err, LogBuilder.StartNew("Errore nel recupero del profilo corrente").ElapsedTime(sw));
                }
                finally
                {
                    sw.Stop();
                }

                return profile;
            });
        }

        public async Task<IEnumerable<TemperatureProfile>> GetAllProfiles()
        {
            _logger.Information(LogBuilder.StartNew("Recupero di tutti i profili"));
            try
            {
                return await Task.Run(() => _context.TemperatureProfiles.Include(s => s.Schedules));
            }
            catch (Exception err)
            {
                _logger.Error(err, LogBuilder.StartNew("Si è verificato un errore nel recupero dei profili"));
                return null;
            }
        }
    }
}
