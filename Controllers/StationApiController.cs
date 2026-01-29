using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LinkwellProductionSystem.Controllers
{
    [ApiController]
    [Route("api/station")]
    public class StationApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StationController> _logger;

        public StationApiController(ApplicationDbContext context, ILogger<StationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ==============================
        // ADMIN CHECK (COMMON)
        // ==============================
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        private string CurrentUser()
        {
            return HttpContext.Session.GetString("Username") ?? "system";
        }

        // ==============================
        // GET ALL STATIONS (ADMIN)
        // ==============================
        [HttpGet("get-stations")]
        public IActionResult GetStations()
        {
            _logger.LogInformation("GetStations API called");

            if (!IsAdmin())
            {
                _logger.LogWarning("Unauthorized access attempt to GetStations API");
                return Unauthorized(new { message = "Admin access only" });
            }

            var stations = _context.StationVMs
                .FromSqlRaw(
                    "EXEC usp_Admin_GetStations @UserRole",
                    new SqlParameter("@UserRole", "Admin")
                )
                .AsNoTracking()
                .ToList();

            _logger.LogInformation("GetStations API executed successfully. Records: {Count}", stations.Count);

            return Ok(new
            {
                success = true,
                data = stations
            });
        }



        [HttpPost("save")]
        public IActionResult SaveStation([FromBody] StationUpsertVM model)
        {
            if (!IsAdmin())
                return Unauthorized("Admin access only");

            if (model.StationId == null)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw(
                        @"EXEC usp_Admin_InsertStation
          @StationCode, @StationName, @Description, @Location, @CreatedBy, @UserRole",
                        new SqlParameter("@StationCode", model.StationCode),
                        new SqlParameter("@StationName", model.StationName),
                        new SqlParameter("@Description", (object?)model.Description ?? DBNull.Value),
                        new SqlParameter("@Location", (object?)model.Location ?? DBNull.Value),
                        new SqlParameter("@CreatedBy", CurrentUser()),
                        new SqlParameter("@UserRole", "Admin")
                    );

                    return Ok(new { success = true, message = "Station created successfully" });
                }
                catch (SqlException ex)
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }

            }
            else
            {

                try
                {
                    // UPDATE
                    _context.Database.ExecuteSqlRaw(
                        @"EXEC usp_Admin_UpdateStation
              @StationId, @StationCode, @StationName, @Description, @Location, @ModifiedBy,@UserRole",
                        new SqlParameter("@StationId", model.StationId),
                        new SqlParameter("@StationCode", model.StationCode),
                        new SqlParameter("@StationName", model.StationName),
                        new SqlParameter("@Description", (object?)model.Description ?? DBNull.Value),
                        new SqlParameter("@Location", (object?)model.Location ?? DBNull.Value),
                        new SqlParameter("@ModifiedBy", CurrentUser()),
                        new SqlParameter("@UserRole", "Admin")
                    );

                    return Ok(new { success = true, message = "Station updated successfully" });

                }
                catch (SqlException ex)
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }

            }
        }


        // ==============================
        // TOGGLE STATION (ADMIN)
        // ==============================
        [HttpPut("toggle-status")]
        public IActionResult ToggleStationStatus([FromBody] StationStatusVM model)
        {
            if (!IsAdmin())
                return Unauthorized("Admin access only");

            try
            {
                _context.Database.ExecuteSqlRaw(
                    @"EXEC usp_Admin_ToggleStationStatus
                @StationId,
                @IsActive,
                @ModifiedBy",
                    new SqlParameter("@StationId", model.StationId),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@ModifiedBy", CurrentUser())
                );

                string msg = model.IsActive ? "Station enabled" : "Station disabled";
                return Ok(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


    }
}
