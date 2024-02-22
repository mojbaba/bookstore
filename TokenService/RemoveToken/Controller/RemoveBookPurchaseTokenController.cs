using Microsoft.AspNetCore.Mvc;

namespace TokenService.RemoveToken;

[ApiController]
[Route("api/book-purchase-token")]
public class RemoveBookPurchaseTokenController(IRemoveBookPurchaseTokenService bookPurchaseTokenService) : ControllerBase
{
    [HttpDelete]
    public async Task<IActionResult> RemoveBookPurchaseTokenAsync(RemoveBookPurchaseTokenRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var response = await bookPurchaseTokenService.RemoveBookPurchaseTokenAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (RemoveBookPurchaseTokenException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}