using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.Models;
using LinkwellProductionSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LinkwellProductionSystem.Controllers.Api
{
    [Authorize]
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
            return User.IsInRole("Admin");
        }

        private string CurrentUser()
        {
            return User.Identity?.Name ?? "system";
        }

        // ==============================
        // GET MODELS
        // ==============================
        [HttpGet("get-models")]
        public IActionResult GetModels()
        {
            var data = _context.Models
                .FromSqlRaw("EXEC usp_Model_GetAll")
                .AsNoTracking()
                .ToList();

            return Ok(data);
        }

        // ==============================
        // INSERT / UPDATE MODEL (ADMIN)
        // ==============================
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
                      @CategoryId,
                      @ModelCode,
                      @ModelName,
                      @Description,
                      @IsActive,
                      @CreatedBy,
                      @UserRole",
                    new SqlParameter("@CategoryId", model.CategoryId),
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
                // UPDATE
                _context.Database.ExecuteSqlRaw(
                    @"EXEC usp_Admin_UpdateModel
                      @Id,
                      @ModelCode,
                      @ModelName,
                      @CategoryId,
                      @Description,
                      @IsActive,
                      @ModifiedBy,
                      @UserRole",
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@ModelCode", model.ModelCode),
                    new SqlParameter("@ModelName", model.ModelName),
                    new SqlParameter("@CategoryId", model.CategoryId),
                    new SqlParameter("@Description", (object?)model.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@ModifiedBy", CurrentUser()),
                    new SqlParameter("@UserRole", "Admin")
                );

                return Ok(new { success = true, message = "Model updated successfully" });
            }
        }

        // ==============================
        // TOGGLE MODEL STATUS (ADMIN)
        // ==============================
        [HttpPut("toggle")]
        public IActionResult ToggleStationStatus([FromBody] ModelStatusVM model)
        {
            if (!IsAdmin())
                return Unauthorized("Admin access only");

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

        // ==============================
        // GET MAPPED STATIONS
        // ==============================
        [HttpGet("get-mapped-stations/{modelId}")]
        public IActionResult GetMappedStations(int modelId)
        {
            return Ok(
                _context.ModelStationMap
                    .Where(x => x.ModelId == modelId && x.IsActive)
                    .Select(x => x.StationId)
                    .ToList()
            );
        }

        // ==============================
        // ASSIGN STATIONS
        // ==============================
        [HttpPost("assignstations")]
        public IActionResult AssignStations([FromBody] ModelStationAssignVM vm)
        {
            var existing = _context.ModelStationMap
                .Where(x => x.ModelId == vm.ModelId)
                .ToList();

            _context.ModelStationMap.RemoveRange(existing);

            foreach (var stationId in vm.ids)
            {
                _context.ModelStationMap.Add(new ModelStationMap
                {
                    ModelId = vm.ModelId,
                    StationId = stationId,
                    IsActive = true,
                    CreatedOn = DateTime.Now
                });
            }

            _context.SaveChanges();
            return Ok();
        }
    }
}
