using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TokenService.QueryUserBalance;

[ApiController]
[Route("api/book-purchase-token")]
[Authorize]
public class UserBalanceController(IUserBalanceQueryHandler userBalanceQueryHandler) : ControllerBase
{
    [HttpGet("balance")]
    public async Task<IActionResult> QueryUserBalanceAsync(CancellationToken cancellationToken = default)
    {
        var request = new QueryUserBalanceRequest
        {
            UserId = User.FindFirstValue(ClaimTypes.Sid)
        };

        var response = await userBalanceQueryHandler.QueryUserBalanceAsync(request, cancellationToken);
        return Ok(response);
    }
}