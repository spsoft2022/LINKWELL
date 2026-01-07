// Controllers/DashboardController.cs
using LinkwellProductionSystem.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db) => _db = db;

        public IActionResult ASSY01() => View("Dashboard", "ASSY01");
        public IActionResult ASSY02() => View("Dashboard", "ASSY02");
        public IActionResult QC01() => View("Dashboard", "QC01");
        public IActionResult PACKING01() => View("Dashboard", "PACKING01");
        public IActionResult FINALTEST01() => View("Dashboard", "FINALTEST01");
    }
}