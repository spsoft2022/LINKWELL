using LinkwellProductionSystem.Core.Entities;
using LinkwellProductionSystem.Data;
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
        [HttpGet("by-model-station")]
        public IActionResult GetByModelStation(int modelId, int stationId)
        {
            var data = from ms in _context.ModelStationWorkInstruction
                       join wi in _context.WorkInstruction
                           on ms.WorkInstructionId equals wi.Id
                       where ms.ModelId == modelId
                          && ms.StationId == stationId
                       orderby ms.SequenceNo
                       select new
                       {
                           id = ms.Id,
                           sequenceNo = ms.SequenceNo,

                           // FROM WorkInstruction (MASTER)
                           title = wi.Title,
                           instructionType = wi.InstructionType,
                           content = wi.Content,
                           isActive = wi.IsActive,

                           // FROM MAPPING
                           isMandatory = ms.IsMandatory,
                           status = ms.Status,
                           versionNo = ms.VersionNo
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
        public IActionResult Add([FromBody] AddWorkInstructionRequest request)
        {
            // ==============================
            // 1. INSERT INTO WorkInstruction (MASTER)
            // ==============================
            var workInstruction = new WorkInstruction
            {
                Title = request.Title,
                InstructionType = request.InstructionType,
                Content = request.Content,
                IsActive = request.IsActive,
                CreatedBy = request.CreatedBy,
                CreatedOn = DateTime.Now
            };

            _context.WorkInstruction.Add(workInstruction);
            _context.SaveChanges();

            // ==============================
            // 2. INSERT INTO ModelStationWorkInstruction (MAPPING)
            // ==============================
            var modelStationWI = new ModelStationWorkInstruction
            {
                ModelId = request.ModelId,
                StationId = request.StationId,
                WorkInstructionId = workInstruction.Id,
                SequenceNo = request.SequenceNo,
                IsMandatory = request.IsMandatory,
                VersionNo = request.VersionNo,
                Status = request.Status
            };

            _context.ModelStationWorkInstruction.Add(modelStationWI);
            _context.SaveChanges();

            return Ok(new { success = true, id = modelStationWI.Id });
        }

        // =====================================================
        // 3️⃣ PUT: Update Instruction
        // =====================================================
        [HttpPut("update")]
        public IActionResult Update([FromBody] UpdateWorkInstructionRequest request)
        {
            var ms = _context.ModelStationWorkInstruction
                .FirstOrDefault(x => x.Id == request.ModelStationWorkInstructionId);

            if (ms == null)
                return NotFound("Instruction mapping not found");

            var wi = _context.WorkInstruction
                .FirstOrDefault(x => x.Id == ms.WorkInstructionId);

            if (wi == null)
                return NotFound("WorkInstruction not found");

            // Update execution info
            ms.SequenceNo = request.SequenceNo;
            ms.IsMandatory = request.IsMandatory;
            ms.Status = request.Status;

            _context.SaveChanges();

            return Ok(new { success = true });
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
