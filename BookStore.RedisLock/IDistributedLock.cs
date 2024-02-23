namespace BookStore.RedisLock;

public interface IDistributedLock
{
    bool TryAcquireLock(string lockName, TimeSpan expiryTime, out string lockId);

    Task<string> WaitToAcquireLockAsync(string lockName, TimeSpan expiryTime,
        CancellationToken cancellationToken);

    Task<string> WaitToAcquireLockAsync(string lockName, TimeSpan expiryTime,
        TimeSpan timeout);

    bool TryReleaseLock(string lockName, string lockId);
}