using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.Models;
using LinkwellProductionSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LinkwellProductionSystem.Controllers.Api
{
    [ApiController]
    [Route("api/model")]
    public class ModelApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ModelApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        private string CurrentUser()
        {
            return HttpContext.Session.GetString("Username") ?? "system";
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


        [HttpPost("save")]
        public IActionResult SaveModel([FromBody] ModelUpsertVM model)
        {
            if (!IsAdmin())
                return Unauthorized("Admin access only");

            if (model.Id == null)
            {
                // INSERT
                _context.Database.ExecuteSqlRaw(
                    @"EXEC usp_Admin_InsertModel
              @ModelCode, @ModelName, @Description, @IsActive, @CreatedBy, @UserRole",
                    new SqlParameter("@ModelCode", model.ModelCode),
                    new SqlParameter("@ModelName", model.ModelName),
                    new SqlParameter("@Description", (object?)model.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@CreatedBy", CurrentUser()),
                    new SqlParameter("@UserRole", "Admin")
                );

                return Ok(new { success = true, message = "Model created successfully" });
            }
            else
            {
                // UPDATEMicrosoft.Data.SqlClient.SqlException: 'Invalid object name 'Model'.'
                _context.Database.ExecuteSqlRaw(
                    @"EXEC usp_Admin_UpdateModel
              @Id, @ModelCode, @ModelName, @Description, @IsActive, @ModifiedBy, @UserRole",
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@ModelCode", model.ModelCode),
                    new SqlParameter("@ModelName", model.ModelName),
                    new SqlParameter("@Description", (object?)model.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@ModifiedBy", CurrentUser()),
                    new SqlParameter("@UserRole", "Admin")
                );

                return Ok(new { success = true, message = "Model updated successfully" });
            }
        }


        // ==============================
        // TOGGLE STATION (ADMIN)
        // ==============================
        [HttpPut("toggle")]
        public IActionResult ToggleStationStatus([FromBody] ModelStatusVM model)
        {
            if (!IsAdmin())
                return Unauthorized("Admin access only");

            try
            {
                _context.Database.ExecuteSqlRaw(
                    @"EXEC usp_Model_ToggleStatus
                @Id,
                @IsActive,
                @ModifiedBy",
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@ModifiedBy", CurrentUser())
                );

                string msg = model.IsActive ? "Model enabled" : "Model disabled";
                return Ok(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult AssignStations([FromBody] ModelStationAssignVM vm)
        {
            // Remove old mappings
            var existing = _context.ModelStationMap
                .Where(x => x.ModelId == vm.ModelId);

            _context.ModelStationMap.RemoveRange(existing);

            // Insert new mappings
            foreach (var stationId in vm.StationIds)
            {
                _context.ModelStationMap.Add(new ModelStationMap
                {
                    ModelId = vm.ModelId,
                    StationId = stationId,
                    CreatedBy = "admin",
                    CreatedOn = DateTime.Now,
                    IsActive = true
                });
            }

            _context.SaveChanges();
            return Ok();
        }


        
    }
}
