using Microsoft.AspNetCore.Mvc;
using SomosRentWi.Api.Security;
using SomosRentWi.Application.Auth.DTOs;
using SomosRentWi.Application.Auth.Interfaces;

namespace SomosRentWi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwt;

    public AuthController(IAuthService authService, IJwtTokenGenerator jwt)
    {
        _authService = authService;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterClientRequest request)
    {
        var result = await _authService.RegisterClientAsync(request);
        var token = _jwt.Generate(result.UserId, result.Role);

        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        var token = _jwt.Generate(result.UserId, result.Role);

        return Ok(new { token });
    }
}