// Controllers/DiffController.cs

using System.Text.Json;
using DiffAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiffAPI.Controllers;

[ApiController]
[Route("v1/diff/{id}")]
public class DiffController : ControllerBase
{
    private readonly IDiffService _diffService;

    public DiffController(IDiffService diffService)
    {
        _diffService = diffService;
    }

    [HttpPut("left")]
    public IActionResult PutLeft(string id, [FromBody] JsonElement data)
    {
        var base64Data = data.GetProperty("data").GetString();
        if (string.IsNullOrEmpty(base64Data)) return BadRequest();

        _diffService.SaveLeft(id, base64Data);
        return CreatedAtAction(nameof(GetDiff), new { id }, null);
    }

    [HttpPut("right")]
    public IActionResult PutRight(string id, [FromBody] JsonElement data)
    {
        var base64Data = data.GetProperty("data").GetString();
        if (string.IsNullOrEmpty(base64Data)) return BadRequest();

        _diffService.SaveRight(id, base64Data);
        return CreatedAtAction(nameof(GetDiff), new { id }, null);
    }

    [HttpGet]
    public IActionResult GetDiff(string id)
    {
        var (exists, diffResultType, diffs) = _diffService.GetDiff(id);
        if (!exists)
        {
            return NotFound();
        }

        if (diffResultType == "Equals")
        {
            return Ok(new { diffResultType });
        }

        if (diffResultType == "SizeDoNotMatch")
        {
            return Ok(new { diffResultType });
        }

        return Ok(new { diffResultType, diffs });
    }
}