using System.Security.Claims;
using Auth.Jwt.Models;
using Auth.Jwt.Services;
using Auth.Jwt.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Jwt.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;
    private readonly IJwtAuthenticationManager _jwtManager;

    public AccountController(
        ILogger<AccountController> logger,
        IUserService userService,
        IJwtAuthenticationManager jwtManager
    )
    {
        _logger = logger;
        _userService = userService;
        _jwtManager = jwtManager;
    }

    [AllowAnonymous]
    [HttpPost("[action]", Name = nameof(Login))]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Send Valid Reuest Model.");

        if (!await _userService.IsValidUserCredentials(request.UserName, request.Password))
            return Unauthorized("Incorrect UserName Or Password.");

        var role = await _userService.GetUserRole(request.UserName);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name,request.UserName),
            new Claim(ClaimTypes.Role, role)
        };

        var jwtResult = _jwtManager.GenerateTokens(request.UserName, claims, DateTime.Now);
        _logger.LogInformation($"User [{request.UserName}] Logged in the Application.");

        return Ok(
            new LoginResult
            {
                UserName = request.UserName,
                Role = role,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.Token
            }
        );
    }

    [HttpPost("[action]")]
    [Authorize]
    public ActionResult Logout()
    {
        var userName = User.Identity.Name;
        _jwtManager.RemoveRefreshTokenByUserName(userName);
        _logger.LogInformation($"User [{userName}] logged out the system.");
        return Ok();
    }
}
