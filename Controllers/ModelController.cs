using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace LinkwellProductionSystem.Controllers
{
    
    public class ModelController : Controller
    {
     

        public IActionResult Index()
        {
            // Optional: server-side admin check
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            return View();
        }

        public IActionResult AssignStation()
        {
            // Optional: server-side admin check
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            return View();
        }


    }

}
