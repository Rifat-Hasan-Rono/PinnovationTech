using PinnovationTech.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PinnovationTech.Interface
{
    public interface IUserRepository
    {
        Task<List<User>> GetUserList(string order, int column, int start, int length);
    }
}
