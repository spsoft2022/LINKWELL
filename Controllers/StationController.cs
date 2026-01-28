using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers
{
   
    public class StationController : Controller
    {

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") == null)
                return RedirectToAction("Login", "Account");

            // Optional: server-side admin check
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            return View();
        }
   
    }
}
