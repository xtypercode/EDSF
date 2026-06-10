using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Tesouraria;

[ApiController]
[Route("api/tesouraria/relatorios")]
public class RelatoriosController(IUnitOfWork uow, ILogger<RelatoriosController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<RelatoriosController> _logger = logger;

    [HttpGet("resumo-financeiro")]
    public async Task<IActionResult> ResumoFinanceiro()
    {
        _logger.LogInformation("Fetching financial summary report");
        var records = _uow.FinanceRecords.Query();
        return Ok(
            new
            {
                receitas = await records.Where(r => r.Amount > 0).SumAsync(r => (decimal?)r.Amount)
                    ?? 0,
                despesas = await records
                    .Where(r => r.Amount < 0)
                    .SumAsync(r => (decimal?)Math.Abs(r.Amount))
                    ?? 0,
                saldo = await records.SumAsync(r => (decimal?)r.Amount) ?? 0,
            }
        );
    }

    [HttpGet("contas-pagar")]
    public async Task<IActionResult> ContasPagar()
    {
        _logger.LogInformation("Fetching accounts payable report");
        var debitos = _uow.DebitNotes.Query();
        return Ok(
            new
            {
                total = await debitos.SumAsync(d => (decimal?)d.Amount) ?? 0,
                itens = await debitos.ToListAsync(),
            }
        );
    }

    [HttpGet("contas-receber")]
    public async Task<IActionResult> ContasReceber()
    {
        _logger.LogInformation("Fetching accounts receivable report");
        var creditos = _uow.CreditNotes.Query();
        return Ok(
            new
            {
                total = await creditos.SumAsync(c => (decimal?)c.Amount) ?? 0,
                itens = await creditos.ToListAsync(),
            }
        );
    }
}
