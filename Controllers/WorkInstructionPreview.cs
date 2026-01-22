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
            // Get Station Name
            var stationName = _context.Stations
            .Where(x => x.Id == stationId.ToString())
            .Select(x => x.StationName)
            .FirstOrDefault();


            // Get Model Name
            var modelName = _context.Models
                .Where(x => x.Id == modelId)
                .Select(x => x.ModelName)
                .FirstOrDefault();

            // Get Instructions
            var data = _context.WorkInstruction
                .Where(x => x.ModelId == modelId
                         && x.StationId == stationId
                         && x.Status == "Published"
                         ) // usually true = active
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new
                {
                    htmlContent = x.HtmlContent
                })
                .ToList();

            return Ok(new
            {
                success = true,
                stationName = stationName,
                modelName = modelName,
                data = data
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

