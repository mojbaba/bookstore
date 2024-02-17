namespace UserService;

public interface ITokenService
{
    string GenerateToken(string username,string id);
}