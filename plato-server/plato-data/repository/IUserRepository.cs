using plato.data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plato.data.repository
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAll();
        bool Save(User user);
    }
}
