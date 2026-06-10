using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EDSF.Api.Services;
using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EDSF.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(
    IUnitOfWork uow,
    IOptions<JwtSettings> jwt,
    ILogger<AuthController> logger,
    PasswordService password,
    LoginThrottle throttle
) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly JwtSettings _jwt = jwt.Value;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly PasswordService _password = password;
    private readonly LoginThrottle _throttle = throttle;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            return BadRequest(new { message = "Username e password são obrigatórios." });
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        if (_throttle.IsBlocked(ip))
        {
            _logger.LogWarning("Login blocked for IP {IP} - too many attempts", ip);
            return StatusCode(
                429,
                new { message = "Demasiadas tentativas. Tente novamente dentro de 15 minutos." }
            );
        }
        var user = await _uow.AppUsers.FindAsync(u => u.Username == request.Username);
        var found = user.FirstOrDefault();
        if (found == null || !_password.Verify(request.Password, found.PasswordHash))
        {
            _throttle.RecordAttempt(ip);
            _logger.LogWarning("Failed login attempt for user: {Username}", request.Username);
            return Unauthorized(new { message = "Credenciais inválidas." });
        }
        _throttle.Reset(ip);
        var token = GenerateToken(found);
        _logger.LogInformation("User {Username} logged in successfully", found.Username);
        return Ok(
            new
            {
                token,
                username = found.Username,
                role = found.Role ?? "user",
            }
        );
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for user: {Username}", request.Username);
        if (request.Password.Length < 4)
            return BadRequest(new { message = "Password deve ter pelo menos 4 caracteres." });
        var existing = await _uow.AppUsers.FindAsync(u => u.Username == request.Username);
        if (existing.Any())
        {
            _logger.LogWarning(
                "Registration failed - username already exists: {Username}",
                request.Username
            );
            return BadRequest(new { message = "Username já existe." });
        }
        var user = new AppUser
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _password.Hash(request.Password),
            Role = "user",
        };
        await _uow.AppUsers.AddAsync(user);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("User {Username} registered successfully", user.Username);
        return Ok(new { message = "Utilizador criado com sucesso." });
    }

    private string GenerateToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role ?? "user"),
        };
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Username, string Password);

public record RegisterRequest(string Username, string Password, string Email);
