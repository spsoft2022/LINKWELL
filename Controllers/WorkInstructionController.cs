using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers
{
    public class WorkInstructionController : Controller
    {
        public IActionResult AddInstructions()
        {
            return View();
        }
    }
}
