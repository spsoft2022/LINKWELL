using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers
{
    [Authorize]
    public class WorkInstructionController : Controller
    {
        public IActionResult AddInstructions()
        {
            return View();
        }
    }
}
