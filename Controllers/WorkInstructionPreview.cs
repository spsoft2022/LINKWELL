using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.DTOs.WorkInstruction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/workinstructions")]
public class WorkInstructionPreviewController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WorkInstructionPreviewController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("preview")]
    public IActionResult Preview(int modelId, int stationId, int? versionNo = null)
    {
        // 🔹 Get latest version
        int latestVersion = _context.ModelStationWorkInstruction
            .Where(x => x.ModelId == modelId && x.StationId == stationId)
            .Max(x => (int?)x.VersionNo) ?? 1;

        int effectiveVersion = versionNo ?? latestVersion;

        var data = (
            from mswi in _context.ModelStationWorkInstruction
            join wi in _context.WorkInstruction
                on mswi.WorkInstructionId equals wi.Id
            where mswi.ModelId == modelId
               && mswi.StationId == stationId
               && mswi.VersionNo == effectiveVersion
               && mswi.Status != "Archived"
            orderby mswi.SequenceNo
            select new
            {
                SequenceNo = mswi.SequenceNo ?? 0,
                IsMandatory = mswi.IsMandatory ?? false,
                ConditionJson = mswi.ConditionJson ?? "",
                ValidationJson = mswi.ValidationJson ?? "",
                VersionNo = mswi.VersionNo ?? 1,
                Status = mswi.Status ?? "",

                InstructionId = wi.Id,
                InstructionType = wi.InstructionType ?? "Text",
                Content = wi.Content ?? ""
            }
        ).ToList();

        var result = data
            .GroupBy(x => x.SequenceNo)
            .Select(g => new WorkInstructionPreviewStepDto
            {
                SequenceNo = g.Key,
                Instructions = g.Select(i => new WorkInstructionPreviewItemDto
                {
                    InstructionId = i.InstructionId,
                    InstructionType = i.InstructionType,
                    InstructionText = i.InstructionType == "Text" ? i.Content : null,
                    AttachmentPath = i.InstructionType == "Image" ? i.Content : null,
                    IsMandatory = i.IsMandatory,
                    ConditionJson = i.ConditionJson,
                    ValidationJson = i.ValidationJson,
                    VersionNo = i.VersionNo,
                    Status = i.Status
                }).ToList()
            })
            .OrderBy(x => x.SequenceNo)
            .ToList();

        // ✅ RETURN VERSION INFO ALSO
        return Ok(new
        {
            version = effectiveVersion,
            steps = result
        });
    }




}

