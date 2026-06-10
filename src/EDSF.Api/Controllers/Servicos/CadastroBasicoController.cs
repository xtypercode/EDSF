using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EDSF.Api.Controllers.Servicos;

[ApiController]
[Route("api/servicos/cadastro")]
public class CadastroBasicoController(IUnitOfWork uow, ILogger<CadastroBasicoController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<CadastroBasicoController> _logger = logger;

    [HttpGet("clientes")]
    public async Task<IActionResult> GetClientes()
    {
        _logger.LogInformation("Fetching basic customer list");
        return Ok(await _uow.Customers.GetAllAsync());
    }

    [HttpGet("servicos")]
    public async Task<IActionResult> GetServicos()
    {
        _logger.LogInformation("Fetching basic service list");
        return Ok(await _uow.Services.GetAllAsync());
    }

    [HttpGet("produtos")]
    public async Task<IActionResult> GetProdutos()
    {
        _logger.LogInformation("Fetching basic product list");
        return Ok(await _uow.Products.GetAllAsync());
    }
}
