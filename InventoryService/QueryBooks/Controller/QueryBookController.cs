using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.QueryBooks;

[ApiController]
[Route("api/query-books")]
[Authorize]
public class QueryBookController(IBookQueryHandler bookQueryHandler) : ControllerBase
{
    [HttpGet("query")]
    public async Task<IActionResult> QueryBooksAsync([FromQuery] IEnumerable<string> bookIds,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new QueryBookRequest()
            {
                BookIds = bookIds ?? new string[] { },
            };

            var response = await bookQueryHandler.HandleAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (QueryBookException e)
        {
            return BadRequest(e.Message);
        }
    }
}