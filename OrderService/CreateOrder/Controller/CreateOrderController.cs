using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.CreateOrder;

[ApiController]
[Authorize]
[Route("api/order")]
public class CreateOrderController(ICreateOrderService createOrderService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder(CreateOrderControllerRequest controllerRequest)
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest();
        }

        var jwtTokenInHeader = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var request = new CreateOrderRequest()
        {
            BookIds = controllerRequest.BookIds,
            UserId = User.FindFirstValue(ClaimTypes.Sid),
            AuthenticationToken = jwtTokenInHeader
        };

        try
        {
            var response = await createOrderService.CreateOrderAsync(request, CancellationToken.None);
            return Ok(response);
        }
        catch (CreateOrderException e)
        {
            return BadRequest(e.Message);
        }
    }
}

public record CreateOrderControllerRequest
{
    public IEnumerable<string> BookIds { get; set; }
}