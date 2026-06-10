using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(IUnitOfWork uow, ILogger<DashboardController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<DashboardController> _logger = logger;

    [HttpGet("resumo")]
    public async Task<IActionResult> Resumo()
    {
        _logger.LogInformation("Fetching dashboard summary");
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var prevMonthStart = monthStart.AddMonths(-1);
        var invoices = _uow.Invoices.Query();
        var revenueMonth =
            await invoices
                .Where(i => i.Status == Core.Enums.InvoiceStatus.Paid && i.Date >= monthStart)
                .SumAsync(i => (decimal?)i.TotalAmount)
            ?? 0;
        var revenuePrev =
            await invoices
                .Where(i =>
                    i.Status == Core.Enums.InvoiceStatus.Paid
                    && i.Date >= prevMonthStart
                    && i.Date < monthStart
                )
                .SumAsync(i => (decimal?)i.TotalAmount)
            ?? 0;
        var pendingInvoices = await invoices.CountAsync(i =>
            i.Status == Core.Enums.InvoiceStatus.Pending
        );
        var pendingPrev = await invoices.CountAsync(i =>
            i.Status == Core.Enums.InvoiceStatus.Pending
            && i.Date >= prevMonthStart
            && i.Date < monthStart
        );
        var products = _uow.Products.Query();
        var lowStock = await products.CountAsync(p => p.StockQuantity > 0 && p.StockQuantity <= 5);
        return Ok(
            new
            {
                revenueMonth,
                revenueMonthPrev = revenuePrev,
                pendingInvoices,
                pendingInvoicesPrev = pendingPrev,
                lowStock,
                lowStockPrev = lowStock,
                totalCustomers = await _uow.Customers.Query().CountAsync(),
                totalProducts = await products.CountAsync(),
                totalInvoices = await invoices.CountAsync(),
            }
        );
    }

    [HttpGet("revenue-trend")]
    public async Task<IActionResult> RevenueTrend()
    {
        _logger.LogInformation("Fetching revenue trend");
        var invoices = _uow.Invoices.Query();
        var now = DateTime.UtcNow;
        var labels = new List<string>();
        var data = new List<decimal>();
        for (int i = 5; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            var monthStart = new DateTime(month.Year, month.Month, 1);
            var monthEnd = monthStart.AddMonths(1);
            labels.Add(monthStart.ToString("MMM"));
            data.Add(
                await invoices
                    .Where(inv =>
                        inv.Status == Core.Enums.InvoiceStatus.Paid
                        && inv.Date >= monthStart
                        && inv.Date < monthEnd
                    )
                    .SumAsync(inv => (decimal?)inv.TotalAmount)
                    ?? 0
            );
        }
        return Ok(new { labels, data });
    }

    [HttpGet("recent")]
    public async Task<IActionResult> Recent()
    {
        _logger.LogInformation("Fetching recent invoices");
        var invoices = await _uow
            .Invoices.Query()
            .OrderByDescending(i => i.Date)
            .Take(5)
            .Select(i => new
            {
                i.Number,
                i.TotalAmount,
                i.Date,
                Status = i.Status.ToString(),
            })
            .ToListAsync();
        return Ok(new { invoices });
    }

    [HttpGet("kpi-summary")]
    public async Task<IActionResult> KpiSummary()
    {
        _logger.LogInformation("Fetching KPI summary");
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var prevMonthStart = monthStart.AddMonths(-1);
        var invoices = _uow.Invoices.Query();
        var revenue =
            await invoices
                .Where(i => i.Status == Core.Enums.InvoiceStatus.Paid && i.Date >= monthStart)
                .SumAsync(i => (decimal?)i.TotalAmount)
            ?? 0;
        var revenuePrev =
            await invoices
                .Where(i =>
                    i.Status == Core.Enums.InvoiceStatus.Paid
                    && i.Date >= prevMonthStart
                    && i.Date < monthStart
                )
                .SumAsync(i => (decimal?)i.TotalAmount)
            ?? 0;
        var products = _uow.Products.Query();
        var customers = _uow.Customers.Query();
        return Ok(
            new
            {
                revenue,
                revenueChange = revenuePrev > 0 ? (revenue - revenuePrev) / revenuePrev * 100 : 0,
                revenuePrev,
                totalCustomers = await customers.CountAsync(),
                totalProducts = await products.CountAsync(),
                totalInvoices = await invoices.CountAsync(),
                pendingInvoices = await invoices.CountAsync(i =>
                    i.Status == Core.Enums.InvoiceStatus.Pending
                ),
                lowStock = await products.CountAsync(p =>
                    p.StockQuantity > 0 && p.StockQuantity <= 5
                ),
            }
        );
    }
}
