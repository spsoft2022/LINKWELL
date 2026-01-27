using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.DTOs.WorkInstruction;
using LinkwellProductionSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/workinstructions")]
public class WorkInstructionPreviewController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _context;

    public WorkInstructionPreviewController(IWebHostEnvironment env, ApplicationDbContext context)
    {
        _env = env;
        _context = context;
    }



    [HttpGet("preview")]
    public IActionResult Preview(int modelId, int stationId, int? versionNo = null)
    {
        try
        {
            var stationName = _context.Stations
                .Where(x => x.Id == stationId.ToString())
                .Select(x => x.StationName)
                .FirstOrDefault();

            var modelName = _context.Models
                .Where(x => x.Id == modelId)
                .Select(x => x.ModelName)
                .FirstOrDefault();

            // ====== RESOLVE VERSION ======

            int versionToLoad;

            if (versionNo.HasValue)
            {
                // Use version sent from URL
                versionToLoad = versionNo.Value;
            }
            else
            {
                // Fallback to latest published
                versionToLoad = _context.WorkInstruction
                    .Where(x => x.ModelId == modelId
                             && x.StationId == stationId
                             && x.Status == "Published")
                    .OrderByDescending(x => x.VersionNo)
                    .Select(x => x.VersionNo)
                    .FirstOrDefault() ?? 0;
            }

            if (versionToLoad == 0)
            {
                return Ok(new
                {
                    success = false,
                    message = "No published versions found"
                });
            }

            // ====== LOAD INSTRUCTIONS ======
            var data = _context.WorkInstruction
                .Where(x => x.ModelId == modelId
                         && x.StationId == stationId
                         && x.VersionNo == versionToLoad)
                .Select(x => new
                {
                    htmlContent = x.HtmlContent,
                    version = x.VersionNo,
                    status = x.Status
                })
                .ToList();

            if (!data.Any())
            {
                return Ok(new
                {
                    success = false,
                    message = $"Version {versionToLoad} not found for this Model/Station"
                });
            }

            return Ok(new
            {
                success = true,
                stationName,
                modelName,
                version = versionToLoad,
                data
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }






    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile upload)
    {
        if (upload == null || upload.Length == 0)
            return BadRequest("No file uploaded");

        var uploads = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
        var filePath = Path.Combine(uploads, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await upload.CopyToAsync(stream);

        return Ok(new { url = "/uploads/" + fileName });
    }

}


