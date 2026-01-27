using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers
{
   
    public class StationController : Controller
    {

        public IActionResult Index()
        {
            // Optional: server-side admin check
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            return View();
        }
   
    }
}
