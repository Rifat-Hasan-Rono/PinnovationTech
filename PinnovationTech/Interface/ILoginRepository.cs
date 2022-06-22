using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PinnovationTech.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PinnovationTech.Interface
{
    public interface ILoginRepository
    {
        bool Login(HttpContext httpContext, [FromBody] Login login);
        void LogOut(HttpContext httpContext);
        Task<string> CreateUser([FromBody] User user);
        List<Country> GetCountry();
        List<City> GetCity(int countryId);
        Task<User> GetUser(int userId);
        User EditUser(int userId);
        User DeleteUser(int userId);
    }
}
