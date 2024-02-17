using Microsoft.EntityFrameworkCore;

namespace UserService;

public class EntityFrameworkUserRepository(UserServiceDbContext dbContext) : IUserRepository
{
    public Task<UserEntity?> GetUserAsync(string email, CancellationToken cancellationToken)
    {
        return dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public Task<UserEntity?> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Users.FirstOrDefaultAsync(x => x.Id == id.ToString(), cancellationToken);
    }

    public Task<UserEntity> CreateUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Users.Add(user).Entity);
    }

    public Task<UserEntity> UpdateUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Users.Update(user).Entity);
    }

    public async Task<UserEntity> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id.ToString(), cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return dbContext.Users.Remove(user).Entity;
    }

    public async Task<UserEntity> DeleteUserAsync(string email, CancellationToken cancellationToken)
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