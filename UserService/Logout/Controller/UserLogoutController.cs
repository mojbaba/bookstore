using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Logout;

[ApiController]
[Route("api/user")]
public class UserLogoutController(IUserLogoutService userLogoutService) : ControllerBase
{
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        var logoutRequest = new UserLogoutRequest
        {
            Id = User.FindFirstValue(ClaimTypes.Sid),
            Email = User.FindFirstValue(ClaimTypes.Name),
            Token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")
        };

        await userLogoutService.LogoutAsync(logoutRequest, cancellationToken);
        return Ok();
    }
}