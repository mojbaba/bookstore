namespace BookStore.RedisLock;

public interface IDistributedLock
{
    bool TryAcquireLock(string lockName, TimeSpan expiryTime, out string lockId);
    
    bool TryReleaseLock(string lockName, string lockId);
}