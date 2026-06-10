using EDSF.Core.Enums;
using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Servicos;

[ApiController]
[Route("api/servicos/relatorios")]
public class RelatoriosController(IUnitOfWork uow, ILogger<RelatoriosController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<RelatoriosController> _logger = logger;

    [HttpGet("faturas-por-periodo")]
    public async Task<IActionResult> FaturasPorPeriodo(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim
    )
    {
        _logger.LogInformation("Fetching invoices by period: {Start} - {End}", inicio, fim);
        var faturas = await _uow.Invoices.FindAsync(i =>
            (!inicio.HasValue || i.Date >= inicio.Value) && (!fim.HasValue || i.Date <= fim.Value)
        );
        return Ok(new { total = faturas.Count(), itens = faturas });
    }

    [HttpGet("total-mes")]
    public async Task<IActionResult> TotalMes()
    {
        _logger.LogInformation("Fetching monthly total");
        var now = DateTime.UtcNow;
        var inicio = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var faturas = await _uow.Invoices.FindAsync(i => i.Date >= inicio);
        return Ok(
            new
            {
                mes = now.Month,
                ano = now.Year,
                total = faturas.Sum(i => i.TotalAmount),
            }
        );
    }

    [HttpGet("faturamento-mensal")]
    public async Task<IActionResult> FaturamentoMensal()
    {
        _logger.LogInformation("Fetching monthly billing report");
        var invoices = _uow.Invoices.Query();
        var now = DateTime.UtcNow;
        var labels = new List<string>();
        var data = new List<decimal>();
        for (int i = 5; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            var monthStart = new DateTime(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);
            labels.Add(monthStart.ToString("MMM"));
            data.Add(
                await invoices
                    .Where(inv =>
                        inv.Status == InvoiceStatus.Paid
                        && inv.Date >= monthStart
                        && inv.Date < monthEnd
                    )
                    .SumAsync(inv => (decimal?)inv.TotalAmount)
                    ?? 0
            );
        }
        return Ok(new { labels, data });
    }

    [HttpGet("status-distribution")]
    public async Task<IActionResult> StatusDistribution()
    {
        _logger.LogInformation("Fetching invoice status distribution");
        var status = await _uow
            .Invoices.Query()
            .GroupBy(i => i.Status)
            .Select(g => new { status = g.Key.ToString(), count = g.Count() })
            .ToListAsync();
        return Ok(status);
    }
}
