using StackExchange.Redis;

namespace BookStore.RedisLock;

public class RedisDistributedLock(IDatabase redisDatabase) : IDistributedLock
{
    public bool TryAcquireLock(string lockName, TimeSpan expiryTime, out string lockId)
    {
        lockId = Guid.NewGuid().ToString();
        return redisDatabase.LockTake(lockName, lockId, expiryTime);
    }

    public bool TryReleaseLock(string lockName, string lockId)
    {
        return redisDatabase.LockRelease(lockName, lockId);
    }
}