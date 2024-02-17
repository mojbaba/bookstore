namespace UserService.Register;

public interface IUserRegisterService
{
    public Task<UserRegisterationResponse> RegisterAsync(UserRegisterationRequest request, CancellationToken cancellationToken);
}