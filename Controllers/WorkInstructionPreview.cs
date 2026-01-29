using LinkwellProductionSystem.Data;
using Microsoft.AspNetCore.Mvc;

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

}


