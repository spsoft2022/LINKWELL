using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinkwellProductionSystem.Data;   // your DbContext namespace
using System.Linq;

namespace LinkwellProductionSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = User.Identity.Name;

            var user = _context.AppUsers.FirstOrDefault(x => x.Username == username);

            ViewBag.ProfileImage = user?.ProfileImagePath;

            return View();
        }
    }
}
