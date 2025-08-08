using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rentline_backend.Services;

namespace rentline_backend.Controllers;

[Authorize(Policy = "OwnerOrManager")]
[Route("api/landlords")]
public class LandlordsController : ControllerBase
{
    private readonly ILandlordService _landlords;
    public LandlordsController(ILandlordService landlords) => _landlords = landlords;

    [HttpGet]
    public async Task<IActionResult> GetLandlords()
    {
        var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
        var landlords = await _landlords.GetLandlordsAsync(orgId);
        return Ok(landlords);
    }
}
