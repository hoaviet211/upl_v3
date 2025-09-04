using Academy.Crm.Api.Services;
using Academy.Crm.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academy.Crm.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IUnitOfWork uow, IAuthService auth) : ControllerBase
{
    public record LoginRequest(string UserName, string Password);
    public record LoginResponse(string AccessToken, string RefreshToken);

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var user = await uow.UserAccounts.Query()
            .FirstOrDefaultAsync(x => x.UserName == req.UserName && !x.IsDeleted, ct);
        if (user == null) return Unauthorized();
        if (!auth.VerifyPassword(req.Password, user.PasswordHash)) return Unauthorized();
        var (access, refresh) = await auth.CreateTokensAsync(user, ct);
        return Ok(new LoginResponse(access, refresh));
    }
}
