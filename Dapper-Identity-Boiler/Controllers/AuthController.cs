using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace DIB.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationUserRole> _roleManager;
        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationUserRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); 
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            { 
                ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                if (user==null)
                {
                    ModelState.AddModelError(string.Empty, "Login Invalid.");
                    return View(model);
                }
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)  { 
                    return Redirect("/");
                } else {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            } 
            return View(model);
        } 
        public async Task<string> Roles()
        {
            IList<ApplicationUser> results = await _userManager.GetUsersInRoleAsync("User");
            return "roles";
        } 
        public IActionResult Register()
        {
            ViewBag.ErrorMessage = null;
            RegisterUser user = new RegisterUser();
            user.Email = "timkopp@gmail.com";
            user.Password = "P@ssword1";
            user.Username = "Tim.Kopp"; 
            return View(user);
        } 
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Username))
                    throw new Exception("The username cannot be empty"); 
                if (string.IsNullOrEmpty(user.Email))
                    throw new Exception("Email cannot be empty"); 
                if (string.IsNullOrEmpty(user.Password))
                    throw new Exception("The password cannot be empty"); 
                if (user.Password != user.ConfirmPassword)
                    throw new Exception("The Passwords do not match");

                var appUser = new ApplicationUser()
                {
                    UserName = user.Username,
                    Email = user.Email 
                }; 
                await _userManager.CreateAsync(appUser, user.Password);
            

                if (!_roleManager.RoleExistsAsync("User").Result)
                {
                    ApplicationUserRole role = new ApplicationUserRole() { Name="User" }; 
                    IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
                }
                if (!_userManager.IsInRoleAsync(appUser, "User").Result)
                {
                    _userManager.AddToRoleAsync(appUser, "User").Wait();
                }  
                return View(new RegisterUser());
            }
            catch(Exception ex)
            {
                throw ex; 
            }
        }
    }
}