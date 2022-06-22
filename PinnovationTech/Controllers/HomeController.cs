using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Mvc;
using PinnovationTech.Interface;
using PinnovationTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinnovationTech.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        public HomeController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        public async Task<DataTable> Index(int start, int length, int column = -1, string direction = null, int roleId = 0)
        {
            column = column == -1 ? Convert.ToInt32(HttpContext.Request.Form["order[0][column]"]) : column;
            direction ??= HttpContext.Request.Form["order[0][dir]"].ToString();

            DataTable datable = new DataTable();
            List<User> users = await userRepository.GetUserList(direction, column, start, length);
            datable.recordsTotal = users.Count() > 0 ? users[0].Total : 0;
            datable.recordsFiltered = users.Count() > 0 ? users[0].Total : 0;
            datable.users = users;
            return datable;
        }
    }
}
