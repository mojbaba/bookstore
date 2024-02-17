using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Login;

[ApiController]
[Route("api/user")]
public class UserLoginController(IUserLoginService userLoginService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(UserLoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var response = await userLoginService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UserLoginException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}