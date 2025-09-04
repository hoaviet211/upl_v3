using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Academy.Crm.Core.Entities;
using Academy.Crm.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Academy.Crm.Api.Services;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    Task<(string accessToken, string refreshToken)> CreateTokensAsync(UserAccount user, CancellationToken ct = default);
}

public class AuthService(IConfiguration cfg, IUnitOfWork uow) : IAuthService
{
    private readonly IConfiguration _cfg = cfg;
    private readonly IUnitOfWork _uow = uow;

    public string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
        => HashPassword(password).Equals(passwordHash, StringComparison.OrdinalIgnoreCase);

    public Task<(string accessToken, string refreshToken)> CreateTokensAsync(UserAccount user, CancellationToken ct = default)
    {
        var issuer = _cfg["Jwt:Issuer"]!;
        var audience = _cfg["Jwt:Audience"]!;
        var key = _cfg["Jwt:Key"]!;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role)
        };

        if (user.StudentId.HasValue)
            claims.Add(new Claim("studentId", user.StudentId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        return Task.FromResult((accessToken, refreshToken));
    }
}

