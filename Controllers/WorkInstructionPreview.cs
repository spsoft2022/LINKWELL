using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.DTOs.WorkInstruction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/workinstructions")]
public class WorkInstructionPreviewController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public WorkInstructionPreviewController(IWebHostEnvironment env)
    {
        _env = env;
    }

    private readonly ApplicationDbContext _context;

    public WorkInstructionPreviewController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("preview")]
    public IActionResult Preview(int modelId, int stationId, int? versionNo = null)
    {
        //// 🔹 Get latest version
        //int latestVersion = _context.ModelStationWorkInstruction
        //    .Where(x => x.ModelId == modelId && x.StationId == stationId)
        //    .Max(x => (int?)x.VersionNo) ?? 1;

        //int effectiveVersion = versionNo ?? latestVersion;

        //var data = (
        //    from mswi in _context.ModelStationWorkInstruction
        //    join wi in _context.WorkInstruction
        //        on mswi.WorkInstructionId equals wi.Id
        //    where mswi.ModelId == modelId
        //       && mswi.StationId == stationId
        //       && mswi.VersionNo == effectiveVersion
        //       && mswi.Status != "Archived"
        //    orderby mswi.SequenceNo
        //    select new
        //    {
        //        SequenceNo = mswi.SequenceNo ?? 0,
        //        IsMandatory = mswi.IsMandatory ?? false,
        //        ConditionJson = mswi.ConditionJson ?? "",
        //        ValidationJson = mswi.ValidationJson ?? "",
        //        VersionNo = mswi.VersionNo ?? 1,
        //        Status = mswi.Status ?? "",

        //        InstructionId = wi.Id,
        //        Content = wi.HtmlContent ?? ""
        //    }
        //).ToList();

        //var result = data
        //    .GroupBy(x => x.SequenceNo)
        //    .Select(g => new WorkInstructionPreviewStepDto
        //    {
        //        SequenceNo = g.Key,
        //        Instructions = g.Select(i => new WorkInstructionPreviewItemDto
        //        {
        //            InstructionId = i.InstructionId,
        //            InstructionText = i.h,
        //            IsMandatory = i.IsMandatory,
        //            ConditionJson = i.ConditionJson,
        //            ValidationJson = i.ValidationJson,
        //            VersionNo = i.VersionNo,
        //            Status = i.Status
        //        }).ToList()
        //    })
        //    .OrderBy(x => x.SequenceNo)
        //    .ToList();

        //// ✅ RETURN VERSION INFO ALSO
        return Ok(new
        {
        });
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile upload)
    {
        if (upload == null || upload.Length == 0)
            return BadRequest("No file");

        // validate image
        var allowed = new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp" };
        var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();

        if (!allowed.Contains(ext))
            return BadRequest("Invalid file type");

        // create folder if missing
        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "work-instructions");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        // safe unique file name
        string fileName = $"{Guid.NewGuid()}{ext}";
        string filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await upload.CopyToAsync(stream);

        // CKEditor expects relative URL
        string fileUrl = $"/uploads/work-instructions/{fileName}";

        return Ok(new { url = fileUrl });
    }


}

