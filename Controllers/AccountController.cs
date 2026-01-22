using System.Net;
using System.Net.Mail;
using BCrypt.Net;
using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.Models;
using LinkwellProductionSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;



namespace LinkwellProductionSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AccountController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public IActionResult Login(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login");

            return View();
        }


        [HttpPost]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM model)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            string username = HttpContext.Session.GetString("Username");

            var user = _db.AppUsers.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                TempData["error"] = "User not found";
                return View(model);
            }

            // ✅ VERIFY CURRENT PASSWORD (BCrypt)
            bool passwordValid = BCrypt.Net.BCrypt.Verify(
                                        model.CurrentPassword,
                                        user.PasswordHash);

            if (!passwordValid)
            {
                TempData["error"] = "Current password is incorrect";
                return View(model);
            }

            // ✅ HASH NEW PASSWORD (BCrypt)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            _db.SaveChanges();

            TempData["success"] = "Password changed successfully";

            // Optional security: force logout
            // HttpContext.Session.Clear();
            // return RedirectToAction("Login");

            return RedirectToAction("ChangePassword");
        }






        // GET
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _db.AppUsers.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Email not found!";
                return View();
            }

            // Generate default password
            string defaultPassword = "Admin@123"; // or random generator

            // Hash password (IMPORTANT)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword);

            _db.Update(user);
            await _db.SaveChangesAsync();

            // Send email
            await SendEmail(email, defaultPassword);

            ViewBag.Message = "Default password sent to your email.";

            return View();
        }


        private async Task SendEmail(string toEmail, string password)
        {
            var fromEmail = _config.GetValue<string>("EmailSettings:Email");
            var fromPassword = _config.GetValue<string>("EmailSettings:Password");

            if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromPassword))
            {
                throw new Exception("EmailSettings are missing in appsettings.json");
            }

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:Email"]),
                Subject = "Password Reset",
                Body = $"Your default password is: {password}\n\nPlease change after login.",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var user = _db.AppUsers.FirstOrDefault(x => x.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("Role", user.Role);

                return RedirectToAction("Index","Station");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}