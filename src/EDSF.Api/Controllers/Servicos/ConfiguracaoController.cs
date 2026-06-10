using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EDSF.Api.Controllers.Servicos;

[ApiController]
[Route("api/servicos/configuracao")]
public class ConfiguracaoController(IUnitOfWork uow, ILogger<ConfiguracaoController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<ConfiguracaoController> _logger = logger;

    [HttpGet("servicos-padrao")]
    public async Task<IActionResult> GetServicosPadrao()
    {
        _logger.LogInformation("Fetching default services");
        return Ok(await _uow.Services.GetAllAsync());
    }
}
