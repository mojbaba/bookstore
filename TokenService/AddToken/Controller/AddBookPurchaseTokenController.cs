using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TokenService.AddToken;

public class AddRequest
{
    [Required]
    [Range(1, long.MaxValue)]
    public long Amount { get; set; }
}

[ApiController]
[Route("api/book-purchase-token")]
[Authorize]
public class AddBookPurchaseTokenController(IAddBookPurchaseTokenService bookPurchaseTokenService) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> AddBookPurchaseTokenAsync(
        [FromBody] AddRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var addBookPurchaseTokenReqeust = new AddBookPurchaseTokenReqeust()
            {
                Amount = request.Amount,
                UserId = User.FindFirstValue(ClaimTypes.Sid)
            };

            var response = await bookPurchaseTokenService.AddBookPurchaseTokenAsync(addBookPurchaseTokenReqeust, cancellationToken);
            return Ok(response);
        }
        catch (AddBookPurchaseTokenException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}