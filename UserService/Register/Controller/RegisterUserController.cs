
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Register;

[ApiController]
[Route ("api/[controller]")]
public class RegisterUserController : ControllerBase
{
    private readonly IUserRegisterService _userRegisterService;

    public RegisterUserController(IUserRegisterService userRegisterService)
    {
        _userRegisterService = userRegisterService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(UserRegisterationRequest request)
    {
        try
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            
            var response = await _userRegisterService.RegisterAsync(request);
            return Ok(response);
        }
        catch (UserRegisterException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}