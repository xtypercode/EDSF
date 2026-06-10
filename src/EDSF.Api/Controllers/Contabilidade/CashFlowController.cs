using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EDSF.Api.Controllers.Contabilidade;

[ApiController]
[Route("api/contabilidade/fluxo-caixa")]
public class CashFlowController(IUnitOfWork uow, ILogger<CashFlowController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<CashFlowController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
    {
        _logger.LogInformation("Fetching cash flow: {Start} - {End}", inicio, fim);
        var records = await _uow.FinanceRecords.FindAsync(r =>
            (!inicio.HasValue || r.Date >= inicio.Value) && (!fim.HasValue || r.Date <= fim.Value)
        );
        return Ok(
            new
            {
                entradas = records.Where(r => r.Amount > 0).Sum(r => r.Amount),
                saidas = records.Where(r => r.Amount < 0).Sum(r => Math.Abs(r.Amount)),
                movimentos = records.OrderBy(r => r.Date),
            }
        );
    }
}
