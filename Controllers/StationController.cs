using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LinkwellProductionSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationController : Controller
    {
        private readonly ApplicationDbContext _context;


        public StationController(ApplicationDbContext context)
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


        [HttpGet("get-all")]
        public IActionResult GetStations()
        {
            return Ok(_context.Stations
                .Where(x => x.IsActive)
                .Select(x => new {
                    id = x.Id,
                    stationName = x.StationName
                }).ToList());
        }


        // ==============================
        // GET ALL STATIONS (ADMIN)
        // ==============================
       // [HttpGet("get-all")]
        //public IActionResult GetStations()
        //{
        //    if (!IsAdmin())
        //        return Unauthorized(new { message = "Admin access only" });

        //    try
        //    {
        //        var stations = _context.StationVMs
        //        .FromSqlRaw(
        //        "EXEC usp_Admin_GetStations @UserRole",
        //        new SqlParameter("@UserRole", "Admin")
        //        )
        //        .AsNoTracking()
        //        .ToList();


        //        return Ok(new
        //        {
        //            success = true,
        //            data = stations
        //        });
        //    }
        //    catch (SqlException ex)
        //    {
        //        return BadRequest(new
        //        {
        //            success = false,
        //            message = ex.Message
        //        });
        //    }
        //}


        [HttpPost("save")]
        public IActionResult SaveStation([FromBody] StationUpsertVM model)
        {
            if (!IsAdmin())
                return Unauthorized("Admin access only");

            if (model.StationId == null)
            {
                // INSERT
                _context.Database.ExecuteSqlRaw(
                    @"EXEC usp_Admin_InsertStation
              @StationCode, @StationName, @Description, @Location, @CreatedBy,@UserRole",
                    new SqlParameter("@StationCode", model.StationCode),
                    new SqlParameter("@StationName", model.StationName),
                    new SqlParameter("@Description", (object?)model.Description ?? DBNull.Value),
                    new SqlParameter("@Location", (object?)model.Location ?? DBNull.Value),
                    new SqlParameter("@CreatedBy", CurrentUser()),
                     new SqlParameter("@UserRole", "Admin")
                );

                return Ok(new { success = true, message = "Station created successfully" });
            }
            else
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
