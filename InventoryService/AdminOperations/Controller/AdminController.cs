using InventoryService.AdminAddBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.AdminOperations;

[ApiController]
[Route("api/admin")]
public class AdminController(IAdminOperationsService adminOperationsService) : ControllerBase
{
    [HttpPost("add-book")]
    public async Task<IActionResult> AddBookAsync(AdminAddBookRequest request, CancellationToken cancellationToken)
    {
        var response = await adminOperationsService.AddBookAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("remove-book")]
    public async Task<IActionResult> RemoveBookAsync(AdminRemoveBookRequest request, CancellationToken cancellationToken)
    {
        var response = await adminOperationsService.RemoveBookAsync(request, cancellationToken);
        return Ok(response);
    }
}