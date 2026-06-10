using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Armazem;

[ApiController]
[Route("api/armazem/relatorios")]
public class RelatoriosController(IUnitOfWork uow, ILogger<RelatoriosController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<RelatoriosController> _logger = logger;

    [HttpGet("stock-atual")]
    public async Task<IActionResult> StockAtual()
    {
        _logger.LogInformation("Fetching current stock report");
        var items = await _uow
            .WarehouseItems.Query()
            .Select(i => new
            {
                i.Code,
                i.Name,
                i.Quantity,
                i.Location,
            })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("movimentos")]
    public async Task<IActionResult> Movimentos(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim
    )
    {
        _logger.LogInformation("Fetching stock movements report: {Start} - {End}", inicio, fim);
        var movimentos = await _uow.StockMovements.FindAsync(m =>
            (!inicio.HasValue || m.Date >= inicio.Value) && (!fim.HasValue || m.Date <= fim.Value)
        );
        return Ok(movimentos);
    }
}
