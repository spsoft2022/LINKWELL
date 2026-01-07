using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace LinkwellProductionSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModelController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Optional: server-side admin check
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            return View();
        }



        [HttpGet("get-models")]
        public IActionResult GetModels()
        {
            var data = _context.Models
                .FromSqlRaw("EXEC usp_Model_GetAll")
                .AsNoTracking()
                .ToList();

            return Ok(data);
        }

        [HttpPost("add")]
        public IActionResult AddModel([FromBody] ModelVM model)
        {
            _context.Database.ExecuteSqlRaw(
                "EXEC usp_Model_Insert @ModelCode,@ModelName,@Description,@CreatedBy",
                new SqlParameter("@ModelCode", model.ModelCode),
                new SqlParameter("@ModelName", model.ModelName),
                new SqlParameter("@Description", model.Description ?? ""),
                new SqlParameter("@CreatedBy", User.Identity.Name ?? "admin")
            );

            return Ok("Model created successfully");
        }

        [HttpPut("update")]
        public IActionResult UpdateModel([FromBody] ModelVM model)
        {
            _context.Database.ExecuteSqlRaw(
                "EXEC usp_Model_Update @Id,@ModelName,@Description,@ModifiedBy",
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@ModelName", model.ModelName),
                new SqlParameter("@Description", model.Description ?? ""),
                new SqlParameter("@ModifiedBy", User.Identity.Name ?? "admin")
            );

            return Ok("Model updated successfully");
        }

        [HttpPut("toggle")]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            _context.Database.ExecuteSqlRaw(
                "EXEC usp_Model_ToggleStatus @Id,@IsActive,@ModifiedBy",
                new SqlParameter("@Id", id),
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@ModifiedBy", User.Identity.Name ?? "admin")
            );

            return Ok("Status updated");
        }
    }

}
