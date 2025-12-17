using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomosRentWi.Application.Wallets.Interfaces;

namespace SomosRentWi.Api.Controllers;

[ApiController]
[Route("api/company/wallet")]
[Authorize(Roles = "Company")]
public class CompanyWalletController : ControllerBase
{
    private readonly IWalletQueryService _walletService;

    public CompanyWalletController(IWalletQueryService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // Fallback to NameIdentifier if Sub not found (standard mapping often maps sub -> nameidentifier)
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { error = "User ID not found in token" });
        }

        var wallet = await _walletService.GetCompanyWalletAsync(userId);
        return Ok(wallet);
    }
}
