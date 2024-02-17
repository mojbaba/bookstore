using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserService.Register;

public class UserRegisterService(UserServiceDbContext dbContext) : IUserRegisterService
{
    public async Task<UserRegisterationResponse> RegisterAsync(UserRegisterationRequest request)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user != null)
        {
            throw new UserRegisterException("Email is already registered");
        }
        
        var passwordHasher = new PasswordHasher<UserEntity>();
        var randomSalt = Guid.NewGuid().ToString();
        var passwordHash = passwordHasher.HashPassword(null, request.Password + randomSalt);
        
        var newUser = new UserEntity
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            Password = passwordHash,
            Salt = randomSalt
        };
        
        dbContext.Users.Add(newUser);
        
        await dbContext.SaveChangesAsync();
        
        return new UserRegisterationResponse
        {
            Email = newUser.Email
        };
    }
}