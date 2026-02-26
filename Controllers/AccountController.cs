using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Claims;
using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



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

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM model)
        {
        
            if (!ModelState.IsValid)
                return View(model);

            string username = User.Identity.Name;

            var user = _db.AppUsers.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                TempData["error"] = "User not found";
                return View(model);
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                TempData["error"] = "Current password is incorrect";
                return View(model);
            }

            // Prevent same password reuse
            if (BCrypt.Net.BCrypt.Verify(model.NewPassword, user.PasswordHash))
            {
                TempData["error"] = "New password cannot be same as current password";
                return View(model);
            }

            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

                user.MustChangePassword = false;
                _db.SaveChanges();

                TempData["success"] = "Password changed successfully";

                // Optional: Force logout
                // HttpContext.Session.Clear();
                // return RedirectToAction("Login");

                return RedirectToAction("ChangePassword");
            }
            catch
            {
                TempData["error"] = "Something went wrong. Please try again.";
                return View(model);
            }
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
                TempData["error"] = "Email not found!";
                return View();
            }

            string tempPassword = GenerateTempPassword();

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            user.MustChangePassword = true;

            _db.Update(user);
            await _db.SaveChangesAsync();

            await SendEmail(email, tempPassword);

            TempData["success"] = "Temporary password sent to your email.";

            return RedirectToAction("ForgotPassword");
        }




        private string GenerateTempPassword()
        {
            return "Adm@" + Guid.NewGuid().ToString("N").Substring(0, 6);
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
                Subject = "Your Account Credentials",
                Body = $"""
                <p>Dear User,</p>

                <p>A temporary password has been assigned to your account:</p>

                <p><strong>{password}</strong></p>

                <p>For security reasons, please update your password upon first login.</p>

                <p>Regards,<br/>Support Team</p>
                """,
                IsBodyHtml = true
                };


            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.AppUsers
                .FirstOrDefaultAsync(x => x.Username == model.Username);

            if (user == null)
            {
                TempData["error"] = "Invalid username or password";
                return View(model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                TempData["error"] = "Invalid username or password";
                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("FullName", user.FullName)
    };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (user.MustChangePassword)
                return RedirectToAction("ChangePassword");

            return RedirectToAction("Index", "Station");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}