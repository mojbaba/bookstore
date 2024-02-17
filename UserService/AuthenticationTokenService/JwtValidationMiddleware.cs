using System.Net;

namespace UserService;

public class JwtValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenValidationService _tokenValidationService;

    public JwtValidationMiddleware(RequestDelegate next, ITokenValidationService tokenValidationService)
    {
        _next = next;
        _tokenValidationService = tokenValidationService;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        
        if (await _tokenValidationService.ValidateTokenAsync(token,default))
        {
            await _next(context);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

    }
}