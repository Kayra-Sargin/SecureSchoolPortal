using BitirmeProjesiPortal.Entities;
using BitirmeProjesiPortal.Models;
using BitirmeProjesiPortal.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BitirmeProjesiPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.UserAccounts.ToList());
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(UserAccount account)
        {
            if (ModelState.IsValid)
            {
                _context.UserAccounts.Add(account);

                try
                {
                    _context.SaveChanges();

                    ModelState.Clear();
                    ViewBag.Message = $"{account.FirstName} {account.LastName} başarıyla kayıt oldu (Zafiyetli Mod).";
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Email veya Kullanıcı adı zaten var.");
                    return View(account);
                }
            }
            return View(account);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string sqlQuery = $"SELECT * FROM UserAccounts WHERE (UserName = '{model.UserNameOrEmail}' OR Email = '{model.UserNameOrEmail}') AND Password = '{model.Password}'";

                    var user = _context.UserAccounts.FromSqlRaw(sqlQuery).FirstOrDefault();
                    if (user != null)
                    {

                        string userRole = user.IsAdmin ? "Admin" : "User";

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim("Name", user.FirstName),
                            new Claim(ClaimTypes.Role, userRole)
                        };

                        if (user.IsAdmin)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username/Email or Password is not correct.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"SQL Hatası (Syntax Error): {ex.Message}");
                }
                
            }
            return View(model);
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
