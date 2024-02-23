using StackExchange.Redis;

namespace BookStore.RedisLock;

public class RedisDistributedLock(IDatabase redisDatabase) : IDistributedLock
{
    public bool TryAcquireLock(string lockName, TimeSpan expiryTime, out string lockId)
    {
        lockId = Guid.NewGuid().ToString();
        return redisDatabase.LockTake(lockName, lockId, expiryTime);
    }

    public Task<string> WaitToAcquireLockAsync(string lockName, TimeSpan expiryTime,
        CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                if (this.TryAcquireLock(lockName,expiryTime, out var lockId))
                {
                    return lockId;
                }
                
                await Task.Delay(100, cancellationToken);
            }
        });
    }

    public async Task<string> WaitToAcquireLockAsync(string lockName, TimeSpan expiryTime, TimeSpan timeout)
    {
        var cts = new CancellationTokenSource(timeout);
        
        try
        {
            return await WaitToAcquireLockAsync(lockName, expiryTime, cts.Token);
        }
        catch (TaskCanceledException e)
        {
            throw new TimeoutException("Failed to acquire lock within the specified timeout", e);
        }
    }

    public bool TryReleaseLock(string lockName, string lockId)
    {
        return redisDatabase.LockRelease(lockName, lockId);
    }
}