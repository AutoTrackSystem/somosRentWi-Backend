using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomosRentWi.Application.Wallets.Interfaces;

namespace SomosRentWi.Api.Controllers;

[ApiController]
[Route("api/admin/wallet")]
[Authorize(Roles = "Admin")]
public class AdminWalletController : ControllerBase
{
    private readonly IWalletQueryService _walletService;

    public AdminWalletController(IWalletQueryService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var wallet = await _walletService.GetAdminWalletAsync();
        return Ok(wallet);
    }
}
