using Microsoft.EntityFrameworkCore;

namespace UserService;

public class EntityFrameworkUserRepository(UserServiceDbContext dbContext) : IUserRepository
{
    public Task<UserEntity?> GetAsync(string email, CancellationToken cancellationToken)
    {
        return dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public Task<UserEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Users.FirstOrDefaultAsync(x => x.Id == id.ToString(), cancellationToken);
    }

    public Task<UserEntity> CreateAsync(UserEntity user, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Users.Add(user).Entity);
    }

    public Task<UserEntity> UpdateAsync(UserEntity user, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Users.Update(user).Entity);
    }

    public async Task<UserEntity> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id.ToString(), cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return dbContext.Users.Remove(user).Entity;
    }

    public async Task<UserEntity> DeleteAsync(string email, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        
        return dbContext.Users.Remove(user).Entity;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}