using InventoryService.AdminAddBook.Models;

namespace InventoryService.AdminOperations;

// Single Responsibility Principle is broken here. This class should only have one responsibility. TIME LIMITS

public interface IAdminOperationsService
{
    Task<AdminAddBookResponse> AddBookAsync(AdminAddBookRequest request, CancellationToken cancellationToken);
    Task<AdminRemoveBookResponse> RemoveBookAsync(AdminRemoveBookRequest request, CancellationToken cancellationToken);
}