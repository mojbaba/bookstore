namespace UserService.Login;

public interface IUserLoginService
{
    public Task<UserLoginResponse> LoginAsync(UserLoginRequest request, CancellationToken cancellationToken);
}