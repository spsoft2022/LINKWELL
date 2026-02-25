using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ModelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AssignStation()
        {
            return View();
        }
    }
}