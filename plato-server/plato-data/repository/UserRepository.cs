namespace plato.data.repository
{
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

    /// <summary>
    /// Defines the <see cref="UserRepository" />
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Defines the _context
        /// </summary>
        private readonly PlatoStationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="config">The config<see cref="IConfiguration"/></param>
        public UserRepository(IConfiguration config)
        {
            DbContextOptionsBuilder<PlatoStationDbContext> builder = new DbContextOptionsBuilder<PlatoStationDbContext>();
            var options = builder.UseMySql(config.GetConnectionString("DefaultConnection"),
                mySqlOptions => mySqlOptions.ServerVersion(new ServerVersion(new Version(8, 0, 18), ServerType.MySql))
            ).Options;
            _context = new PlatoStationDbContext(options);
        }

        /// <summary>
        /// The Authenticate
        /// </summary>
        /// <param name="username">The username<see cref="string"/></param>
        /// <param name="password">The password<see cref="string"/></param>
        /// <returns>The <see cref="User"/></returns>
        public async Task<User> Authenticate(string username, string password)
        {
            return await Task.Run(() => _context.Users.Where(user => user.Username == username & user.Password == password).First());
        }

        /// <summary>
        /// The GetAll
        /// </summary>
        /// <returns>The <see cref="IEnumerable{User}"/></returns>
        public async Task<IEnumerable<User>> GetAll()
        {
            return await Task.Run(() => _context.Users);
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="user">The user<see cref="User"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool Save(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChanges() == 1;
        }
    }
}
