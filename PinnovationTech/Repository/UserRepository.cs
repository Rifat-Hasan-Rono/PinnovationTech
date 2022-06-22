using PinnovationTech.Interface;
using PinnovationTech.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace PinnovationTech.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task<List<User>> GetUserList(string order = "desc", int column = 1, int start = 0, int length = -1)
        {
            string lengthQuery = length == -1 ? "" : " WHERE RowNum BETWEEN " + (start + 1) + " AND  " + (length + start);
            Dictionary<int, string> dict = new Dictionary<int, string>() { { 0, "RowNum" }, { 1, "userId" }, { 2, "fName" }, { 3, "lName" } };
            string orderColumn = dict[column];

            string query = @"WITH Results AS(Select *,Total=COUNT(*) OVER(),ROW_NUMBER() OVER(ORDER BY " + orderColumn + " " + order + ") AS RowNum " +
                "From TblUsers) SELECT * FROM Results " + lengthQuery;

            using IDbConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return (await con.QueryAsync<User>(query, new DynamicParameters())).ToList();
        }
    }
}
