using System.Text.Json;
using DiffAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiffAPI.Controllers
{
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
        public async Task<IActionResult> PutLeft(string id, [FromBody] JsonElement data)
        {
            var base64Data = data.GetProperty("data").GetString();
            if (string.IsNullOrEmpty(base64Data)) return BadRequest();

            await _diffService.SaveLeftAsync(id, base64Data);

            // Return a 201 Created response with a link to the GetDiff endpoint
            return CreatedAtAction(nameof(GetDiff), new { id }, null);
        }

        [HttpPut("right")]
        public async Task<IActionResult> PutRight(string id, [FromBody] JsonElement data)
        {
            var base64Data = data.GetProperty("data").GetString();
            if (string.IsNullOrEmpty(base64Data)) return BadRequest();

            await _diffService.SaveRightAsync(id, base64Data);

            // Return a 201 Created response with a link to the GetDiff endpoint
            return CreatedAtAction(nameof(GetDiff), new { id }, null);
        }

        [HttpGet]
        public async Task<IActionResult> GetDiff(string id)
        {
            var (exists, diffResultType, diffs) = await _diffService.GetDiffAsync(id);

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

            // Return the detailed differences
            return Ok(new { diffResultType, diffs });
        }
    }
}
