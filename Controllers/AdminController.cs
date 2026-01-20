using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("floor-preview")]
        public IActionResult FloorPreview(int station, int model)
        {
            ViewBag.StationId = station;
            ViewBag.ModelId = model;
            return View();
        }

    }
}
