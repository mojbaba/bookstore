
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Register;

[ApiController]
[Route ("api/user")]
public class RegisterUserController(IUserRegisterService userRegisterService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(UserRegisterationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            
            var response = await userRegisterService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UserRegisterException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}