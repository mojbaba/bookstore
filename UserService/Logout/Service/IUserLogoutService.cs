namespace UserService.Logout;

public interface IUserLogoutService
{
     Task<bool> LogoutAsync(UserLogoutRequest request, CancellationToken cancellationToken);
}