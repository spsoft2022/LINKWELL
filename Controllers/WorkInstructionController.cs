using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers
{
    public class WorkInstructionController : Controller
    {
        // ==============================
        // ADMIN UI PAGE
        // ==============================
        public IActionResult Admin()
        {
            return View();
        }
    }
}
