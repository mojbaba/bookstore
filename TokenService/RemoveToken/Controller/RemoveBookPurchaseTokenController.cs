using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TokenService.RemoveToken;

public class RemoveTokenRequest
{
    public long Amount { get; set; }
}

[ApiController]
[Route("api/book-purchase-token")]
public class RemoveBookPurchaseTokenController(IRemoveBookPurchaseTokenService bookPurchaseTokenService) : ControllerBase
{
    [HttpPost("remove")]
    public async Task<IActionResult> RemoveBookPurchaseTokenAsync(RemoveTokenRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            
            var removeBookPurchaseTokenRequest = new RemoveBookPurchaseTokenRequest()
            {
                Amount = request.Amount,
                UserId = User.FindFirstValue(ClaimTypes.Sid)
            };

            var response = await bookPurchaseTokenService.RemoveBookPurchaseTokenAsync(removeBookPurchaseTokenRequest, cancellationToken);
            return Ok(response);
        }
        catch (RemoveBookPurchaseTokenException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}