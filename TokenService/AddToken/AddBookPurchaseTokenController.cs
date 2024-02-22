using Microsoft.AspNetCore.Mvc;

namespace TokenService.AddToken;

[ApiController]
[Route("api/book-purchase-token")]
public class AddBookPurchaseTokenController(IAddBookPurchaseTokenService bookPurchaseTokenService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddBookPurchaseTokenAsync(AddBookPurchaseTokenReqeust request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var response = await bookPurchaseTokenService.AddBookPurchaseTokenAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AddBookPurchaseTokenException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}