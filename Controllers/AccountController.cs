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

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity.Name;

            var user = await _db.AppUsers
                .FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
                return RedirectToAction("Login");

            var vm = new ProfilePageVM
            {
                Profile = new Profile
                {
                    UserName = user.Username,
                    Email = user.Email,
                    Role=user.Role
                },
                ChangePassword = new ChangePasswordVM()
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfilePageVM vm)
        {

            // ✅ remove password validation completely
            ModelState.Remove("ChangePassword");

            if (!ModelState.IsValid)
                return View("Profile", vm);

            var username = User.Identity.Name;

            var user = await _db.AppUsers
                .FirstOrDefaultAsync(x => x.Username == username);

            user.Username = vm.Profile.UserName;
            user.Email = vm.Profile.Email;

            await _db.SaveChangesAsync();

            // ✅ recreate auth cookie
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            TempData["success"] = "Profile Updated Successfully";

            return RedirectToAction("Profile");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
    [Bind(Prefix = "ChangePassword")] ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid password data";
                return RedirectToAction("Profile");
            }

            var username = User.Identity.Name;

            var user = await _db.AppUsers
                .FirstOrDefaultAsync(x => x.Username == username);

            if (!BCrypt.Net.BCrypt.Verify(
                    model.CurrentPassword,
                    user.PasswordHash))
            {
                TempData["error"] = "Current password incorrect";
                return RedirectToAction("Profile");
            }

            user.MustChangePassword = false; ;
            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            await _db.SaveChangesAsync();

            TempData["success"] = "Password updated successfully";

            return RedirectToAction("Profile");
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
                return RedirectToAction("Profile");

            return RedirectToAction("AddInstructions", "WorkInstruction");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}