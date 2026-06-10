using EDSF.Api.Services;
using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EDSF.Api.Controllers.MinhaConta;

[ApiController]
[Route("api/minha-conta/senha")]
public class PasswordController(
    IUnitOfWork uow,
    ILogger<PasswordController> logger,
    PasswordService password
) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<PasswordController> _logger = logger;
    private readonly PasswordService _password = password;

    [HttpPut("alterar")]
    public async Task<IActionResult> Alterar([FromBody] AlterarSenhaRequest request)
    {
        _logger.LogInformation("Changing password for user: {Username}", request.Username);
        var users = await _uow.AppUsers.FindAsync(u => u.Username == request.Username);
        var user = users.FirstOrDefault();
        if (user is null)
            return NotFound("Utilizador não encontrado.");
        if (!_password.Verify(request.CurrentPassword, user.PasswordHash))
            return BadRequest("Palavra-passe atual incorreta.");
        user.PasswordHash = _password.Hash(request.NewPassword);
        await _uow.AppUsers.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("recuperar")]
    [AllowAnonymous]
    public IActionResult Recuperar([FromBody] RecuperarSenhaRequest request)
    {
        _logger.LogInformation("Password recovery requested for email: {Email}", request.Email);
        return Ok(new { mensagem = $"Instruções enviadas para {request.Email} (simulado)." });
    }
}

public class AlterarSenhaRequest
{
    public string Username { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class RecuperarSenhaRequest
{
    public string Email { get; set; } = string.Empty;
}
