using BookStore.RedisLock;
using InventoryService.AdminAddBook.Models;
using InventoryService.Entities;

namespace InventoryService.AdminOperations;

// Handlers for admin operations are omitted for brevity and time savings

public class AdminOperationsService(IBookRepository bookRepository, IDistributedLock distributedLock)
    : IAdminOperationsService
{
    public async Task<AdminAddBookResponse> AddBookAsync(AdminAddBookRequest request,
        CancellationToken cancellationToken)
    {
        var book = new BookEntity
        {
            Title = request.Title,
            Author = request.Author,
            Amount = request.Amount,
            Price = request.Price
        };

        await bookRepository.CreateAsync(book, cancellationToken);
        
        await bookRepository.SaveChangesAsync(cancellationToken);

        return new AdminAddBookResponse
        {
            BookId = book.Id
        };
    }

    public async Task<AdminRemoveBookResponse> RemoveBookAsync(AdminRemoveBookRequest request,
        CancellationToken cancellationToken)
    {
        if (distributedLock.TryAcquireLock(request.BookId, TimeSpan.FromSeconds(50), out string lockId))
        {
            await bookRepository.DeleteAsync(request.BookId, cancellationToken);
            await bookRepository.SaveChangesAsync(cancellationToken);
            
            distributedLock.TryReleaseLock(request.BookId, lockId);
            
            return new AdminRemoveBookResponse();
        }
        else
        {
            throw new SynchronizationLockException("Failed to acquire lock");
        }
    }

    public async Task<AdminChangeBookAmountResponse> ChangeBookAmountAsync(AdminChangeBookAmountRequest request,
        CancellationToken cancellationToken)
    {
        if(distributedLock.TryAcquireLock(request.BookId, TimeSpan.FromSeconds(50), out string lockId))
        {
            var book = await bookRepository.GetAsync(request.BookId, cancellationToken);
            
            if(book.Amount + request.Amount < 0)
            {
                throw new InvalidOperationException("Amount cannot be negative");
            }
            
            book.Amount += request.Amount;
            await bookRepository.UpdateAsync(book, cancellationToken);
            await bookRepository.SaveChangesAsync(cancellationToken);
            
            distributedLock.TryReleaseLock(request.BookId, lockId);
            
            return new AdminChangeBookAmountResponse()
            {
                BookId = book.Id,
                UpdatedAmount = book.Amount
            };
        }
        else
        {
            throw new SynchronizationLockException("Failed to acquire lock");
        }
    }
}