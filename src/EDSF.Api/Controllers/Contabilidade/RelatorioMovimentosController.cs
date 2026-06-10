using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EDSF.Api.Controllers.Contabilidade;

[ApiController]
[Route("api/contabilidade/relatorio-movimentos")]
public class RelatorioMovimentosController(
    IUnitOfWork uow,
    ILogger<RelatorioMovimentosController> logger
) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<RelatorioMovimentosController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
    {
        _logger.LogInformation("Fetching movement report: {Start} - {End}", inicio, fim);
        var stock = await _uow.StockMovements.FindAsync(m =>
            (!inicio.HasValue || m.Date >= inicio.Value) && (!fim.HasValue || m.Date <= fim.Value)
        );
        var finance = await _uow.FinanceRecords.FindAsync(r =>
            (!inicio.HasValue || r.Date >= inicio.Value) && (!fim.HasValue || r.Date <= fim.Value)
        );
        return Ok(
            new
            {
                stockMovements = stock.OrderBy(m => m.Date),
                financeRecords = finance.OrderBy(r => r.Date),
            }
        );
    }
}
