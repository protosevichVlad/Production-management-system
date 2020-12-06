using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _context;
        public AccountController()
        {
            _context = new ApplicationContext();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        private async System.Threading.Tasks.Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Register()
        {
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "RusName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Register(RegisterModel model)
        {
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "RusName");
            if (ModelState.IsValid)
            {
                User user = null;
                user = _context.Users.FirstOrDefault(u => u.Login == model.Login);
                if (user == null)
                {
                    user = new User();
                    user.Login = model.Login;
                    user.Password = model.Password;
                    user.Role = _context.Roles.FirstOrDefault(r => r.Id == model.RoleId);
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return Redirect("/Account/Show");
                }
                ModelState.AddModelError("Login", "Данный логин уже используется");
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Show()
        {
            ViewBag.Users = _context.Users.Include(u => u.Role);
            return View();
        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "RusName");
            User user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
            RegisterModel regModel = new RegisterModel { Login = user.Login, Password = user.Password, RoleId = user.Role.Id };
            return View(regModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(RegisterModel model)
        {
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "RusName");
            if (ModelState.IsValid)
            {
                User user = null;
                user = _context.Users.FirstOrDefault(u => u.Login == model.Login);
                if (user == null)
                {
                    ModelState.AddModelError("Login", "Данный логин не используется");
                }
                user.Password = model.Password;
                user.Role = _context.Roles.FirstOrDefault(r => r.Id == model.RoleId);
                _context.SaveChanges();
                return Redirect("/Account/Show");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Remove(string login)
        {
            User user = _context.Users.FirstOrDefault(u => u.Login == login);
            _context.Users.Attach(user);
            _context.Users.Remove(user);
            _context.SaveChanges();

            return Redirect("/Account/Show");
        }
    }
}
