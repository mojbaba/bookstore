namespace UserService;

public interface IUserRepository
{
    Task<UserEntity?> GetUserAsync(string email, CancellationToken cancellationToken);
    Task<UserEntity?> GetUserAsync(Guid id, CancellationToken cancellationToken);
    Task<UserEntity> CreateUserAsync(UserEntity user, CancellationToken cancellationToken);
    Task<UserEntity> UpdateUserAsync(UserEntity user, CancellationToken cancellationToken);
    Task<UserEntity> DeleteUserAsync(Guid id, CancellationToken cancellationToken);
    Task<UserEntity> DeleteUserAsync(string email, CancellationToken cancellationToken);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}