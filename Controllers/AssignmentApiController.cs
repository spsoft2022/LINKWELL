using System;
using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.DTOs;
using LinkwellProductionSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers
{
    [ApiController]
    [Route("api/admin/assignments")]
    public class AssignmentApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentApiController(ApplicationDbContext context)
        {
            _context = context;
        }


        // POST /api/admin/assignments
        [HttpPost]
        public IActionResult AssignModel([FromBody] AssignModelDto dto)
        {

            if (dto.stationId <= 0 || dto.ModelId <= 0)
                return BadRequest("Invalid station or model");

            var existing = _context.StationAssignment
                                   .FirstOrDefault(x => x.StationId == dto.stationId);

            if (existing == null)
            {
                var newAssign = new StationAssignment
                {
                    StationId = dto.stationId,
                    ModelId = dto.ModelId,
                    AssignedAt = DateTime.Now,
                    AssignedBy = User?.Identity?.Name ?? "Admin"
                };

                _context.StationAssignment.Add(newAssign);
            }
            else
            {
                existing.ModelId = dto.ModelId;
                existing.AssignedAt = DateTime.Now;
                existing.AssignedBy = User?.Identity?.Name ?? "Admin";
            }

            _context.SaveChanges();
            return Ok(new { success = true });
        }

        // GET /api/admin/assignments
        [HttpGet]
        public IActionResult GetAssignments()
        {
            var data =
                (from sa in _context.StationAssignment
                 join s in _context.Stations on sa.StationId equals Convert.ToInt32(s.Id)
                 join m in _context.Models on sa.ModelId equals m.Id
                 select new
                 {
                     stationId = Convert.ToInt32(s.Id),
                     stationName = s.StationCode + " - " + s.StationName,
                     modelId = m.Id,
                     modelName = m.ModelName,
                     updatedAt = sa.AssignedAt.ToString("yyyy-MM-dd HH:mm")
                 })
                .OrderBy(x => x.stationName)
                .ToList();

            return Ok(data);
        }


    }

}
