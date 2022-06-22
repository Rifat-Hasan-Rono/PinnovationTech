using System;
using PinnovationTech.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PinnovationTech.Interface;
using System.Linq;
using Microsoft.Web.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PinnovationTech.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginRepository loginRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        public LoginController(ILoginRepository loginRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.loginRepository = loginRepository;
            this.webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            List<Country> countries = loginRepository.GetCountry();
            ViewBag.Countries = countries;
            return View();
        }

        // LOGIN NEW USER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Login login)
        {
            if (!ModelState.IsValid)
                return View();
            
            bool isAuthenticated = loginRepository.Login(this.HttpContext, login);
            if (isAuthenticated)
                return login.EmailNo != "admin@admin.com" ? RedirectToRoute("View") : RedirectToRoute("Home");
            else {
                List<Country> countries = loginRepository.GetCountry();
                ViewBag.Message = "Invalid Email or Password";
                ViewBag.Countries = countries;
                return View(); 
            }
        }

        //CREATE NEW USER
        [HttpPost]
        [AjaxOnly]
        public async Task<JsonResult> RegisterUser(IFormCollection formCollection,User user)
        {
            bool isAuthenticated = User.Identity.IsAuthenticated;
            if (isAuthenticated)
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                User deletedUser = loginRepository.DeleteUser(userId);
                if (deletedUser != null)
                {
                    if (deletedUser.UserImg != null) System.IO.File.Delete(this.GetPathAndFilename(deletedUser.UserImg, true));
                    if (deletedUser.UserCV != null) System.IO.File.Delete(this.GetPathAndFilename(deletedUser.UserCV, true));
                }
            }
            
            var imgFile = formCollection.Files["userImgFile"];
            var cvFile = formCollection.Files["userCVFile"];
            if (imgFile != null && imgFile.Length > 0) user.UserImg = ContentDispositionHeaderValue.Parse(formCollection.Files.GetFile("userImgFile").ContentDisposition).FileName.Trim('"');
            if (cvFile != null && cvFile.Length > 0) user.UserCV = ContentDispositionHeaderValue.Parse(formCollection.Files.GetFile("userCVFile").ContentDisposition).FileName.Trim('"');

            if (ModelState.IsValid)
            {
                string userId = await loginRepository.CreateUser(user);
                if (userId != "")
                {
                    foreach (IFormFile source in formCollection.Files)
                    {
                        string filename = "";
                        if (source.Name == "userImgFile") filename = "\\images\\" + userId + "_" + this.EnsureCorrectFilename(user.UserImg);
                        if (source.Name == "userCVFile") filename = "\\cvs\\" + userId + "_" + this.EnsureCorrectFilename(user.UserCV);
                        using FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename));
                        await source.CopyToAsync(output);
                    }
                }
                return new JsonResult(Convert.ToInt32(userId));
            }
            else
            {
                return new JsonResult(ModelState.ToDictionary(
                    e => e.Key.Substring(0, 1).ToLower() + e.Key.Substring(1),
                    e => e.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                ));
            }
        }

        // REMOVE PATH SIGN
        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        // ADD ROOT PATH TO FILE NAME
        private string GetPathAndFilename(string filename, bool delete = false)
        {
            return delete ? this.webHostEnvironment.WebRootPath + filename : 
                this.webHostEnvironment.WebRootPath + "\\uploads\\" + filename;
        }

        // GET CITY LIST
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCities(int countryId)
        {
            return new JsonResult(loginRepository.GetCity(countryId));
        }

        // LOGOUT CURRENT USER
        [Authorize]
        public IActionResult LogOut()
        {
            loginRepository.LogOut(this.HttpContext);
            return RedirectToRoute("LogIn");
        }

        // USER PROFILE DATA
        [Authorize]
        public async Task<IActionResult> GetDetail(int id)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            userId = userId == 0 ? id : userId;
            User user = await loginRepository.GetUser(userId);
            ViewData["User"] = user;
            List<Country> countries = loginRepository.GetCountry();
            ViewData["Countries"] = countries;
            return View();
        }

        // USER INFORMATION TO UPDATE
        [HttpPost]
        [AjaxOnly]
        public JsonResult EditUser(int userId)
        {
            return new JsonResult(loginRepository.EditUser(userId));
        }
    }
}