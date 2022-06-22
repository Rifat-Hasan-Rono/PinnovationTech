using Microsoft.Extensions.Configuration;
using PinnovationTech.Interface;
using PinnovationTech.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace PinnovationTech.Repository
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IConfiguration _configuration;

        public LoginRepository(IConfiguration config)
        {
            _configuration = config;
        }

        public bool Login(HttpContext httpContext, Login login)
        {
            bool authenticated = false;
            User user;
            if (login.EmailNo == "admin@admin.com" && login.Password == "@123@")
            {
                user = new User { FName = "Admin", LName = "Admin", EmailNo = login.EmailNo, UserId = 0 };
            }
            else
            {
                string query = @"Select * From TblUsers Where emailNo = '" + login.EmailNo + "' And password = '" + login.Password + "'";
                using IDbConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                user = con.Query<User>(query, new DynamicParameters()).FirstOrDefault();
            }

            if (user != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                authenticated = true;
                httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
            return authenticated;
        }

        public void LogOut(HttpContext httpContext)
        {
            httpContext.SignOutAsync();
        }

        public async Task<string> CreateUser(User user)
        {
            var values = new
            {
                user.UserId,
                user.FName,
                user.LName,
                user.PhoneNo,
                user.EmailNo,
                user.UserCity,
                user.UserImg,
                user.UserCV,
                user.Dob,
                user.Password,
                user.Gender,
                ProcessType = "Insert"
            };
            using IDbConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return (await con.QueryAsync<string>("MasterCRUD", values, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FName+" "+user.LName),
                new Claim(ClaimTypes.Email, user.EmailNo),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };
            return claims;
        }

        public List<Country> GetCountry()
        {
            string query = @"Select * From TblCountry";
            using IDbConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return con.Query<Country>(query, new DynamicParameters()).ToList();
        }

        public List<City> GetCity(int countryId)
        {
            string query = @"Select cityId,cityName From TblCity Where countryId = "+ countryId;
            using IDbConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return con.Query<City>(query, new DynamicParameters()).ToList();
        }

        public async Task<User> GetUser(int userId)
        {
            string query = @"SELECT * From dbo.ITF_User(@Parm)";
            using IDbConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return (await con.QueryAsync<User>(query, new { Parm = userId }, commandType: CommandType.Text)).FirstOrDefault();
        }

        public User EditUser(int userId)
        {
            var values = new
            {
                UserId = userId.ToString(),
                ProcessType = "Select"
            };
            using IDbConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return con.Query<User>("MasterCRUD", values, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public User DeleteUser(int userId)
        {
            var values = new
            {
                UserId = userId,
                ProcessType = "Delete"
            };
            using IDbConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return con.Query<User>("MasterCRUD", values, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
