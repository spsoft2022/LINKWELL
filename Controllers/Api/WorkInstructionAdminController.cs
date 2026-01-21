using LinkwellProductionSystem.Core.Entities;
using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.DTOs.WorkInstruction;
using LinkwellProductionSystem.ViewModels.WorkInstructions.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LinkwellProductionSystem.Controllers.Api
{
    [ApiController]
    [Route("api/admin/work-instructions")]
    public class WorkInstructionAdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkInstructionAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // 1️⃣ GET: Instructions by Model + Station
        // =====================================================
        [HttpGet("getInstructions")]
        public IActionResult GetAllInstructions()
        {
            var data =
                from wi in _context.WorkInstruction

                    // LEFT JOIN mapping
                from ms in _context.ModelStationWorkInstruction
                    .Where(x => x.WorkInstructionId == wi.Id)
                    .DefaultIfEmpty()

                    // INNER JOIN model & station (these must exist)
                from s in _context.Stations
                    .Where(x => x.Id == wi.StationId.ToString())
                from m in _context.Models
                .Where(x => x.Id == wi.ModelId)

                orderby (ms.SequenceNo ?? wi.Id)

                select new
                {
                    id = wi.Id,
                    modelId = wi.ModelId,
                    modelName = m.ModelName,
                    stationId = wi.StationId,
                    stationName = s.StationName,
                    htmlContent = wi.HtmlContent,
                    status = wi.Status,
                    isActive = wi.IsActive,
                };

            return Ok(data.ToList());
        }




        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var uploads = Path.Combine("wwwroot", "uploads", "workinstr");
            Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            // auto detect base URL from current request
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(new
            {
                relativePath = $"/uploads/workinstr/{fileName}",
                absoluteUrl = $"{baseUrl}/uploads/workinstr/{fileName}"
            });
        }



        // =====================================================
        // 2️⃣ POST: Add New Work Instruction
        // =====================================================
        [HttpPost("add")]
        public async Task<IActionResult> AddInstruction([FromBody] WorkInstructionDto dto)
        {
            if (dto is null)
                return BadRequest("Invalid payload");

            var entity = new WorkInstruction
            {
                HtmlContent = dto.HtmlContent,
                ModelId = dto.ModelId,
                StationId = dto.StationId,
                Status = dto.Status,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow.ToString(),
            };

            _context.WorkInstruction.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Instruction added", id = entity.Id });
        }



        // =====================================================
        // UPDATE WORK INSTRUCTION
        // =====================================================
        [HttpPost("update")]
        public async Task<IActionResult> UpdateInstruction([FromBody] WorkInstructionDto dto)
        {
            if (dto.Id <= 0)
                return BadRequest("Invalid Id");

            var entity = await _context.WorkInstruction.FindAsync(dto.Id);

            if (entity == null)
                return NotFound("Instruction not found");

            entity.HtmlContent = dto.HtmlContent;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Instruction updated" });
        }





        [HttpGet("versions")]
        public IActionResult GetVersions(int modelId, int stationId)
        {
            var versions = _context.ModelStationWorkInstruction
                .Where(x => x.ModelId == modelId && x.StationId == stationId)
                .Select(x => x.VersionNo)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            return Ok(versions);
        }



        // =====================================================
        // 4️⃣ DELETE: Remove Instruction
        // =====================================================
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var ms = _context.ModelStationWorkInstruction
                .FirstOrDefault(x => x.Id == id);

            if (ms == null)
                return NotFound();

            var wi = _context.WorkInstruction
                .FirstOrDefault(x => x.Id == ms.WorkInstructionId);

            _context.ModelStationWorkInstruction.Remove(ms);

            if (wi != null)
                _context.WorkInstruction.Remove(wi);

            _context.SaveChanges();

            return Ok(new { success = true });
        }
    }
}
