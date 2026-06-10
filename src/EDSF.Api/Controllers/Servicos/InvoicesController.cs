using EDSF.Core.Enums;
using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Servicos;

[ApiController]
[Route("api/servicos/faturas")]
public class InvoicesController(
    IUnitOfWork uow,
    ILogger<InvoicesController> logger,
    SeriesManager series
) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<InvoicesController> _logger = logger;
    private readonly SeriesManager _series = series;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching Invoice page {Page}", page);
        var query = _uow.Invoices.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Number != null && e.Number.Contains(search));
        var total = await query.CountAsync();
        var items = await query
            .Include(i => i.Customer)
            .OrderByDescending(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(
            new
            {
                items,
                total,
                page,
                pageSize,
            }
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Fetching Invoice by ID: {Id}", id);
        var invoice = await _uow
            .Invoices.Query()
            .Include(i => i.Customer)
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == id);
        return invoice is null ? NotFound() : Ok(invoice);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Invoice invoice)
    {
        if (string.IsNullOrEmpty(invoice.Series) || invoice.Series == "0")
            invoice.Series = await _series.NextNumber(invoice.DocumentType);
        if (string.IsNullOrEmpty(invoice.Number))
            invoice.Number = invoice.Series;
        foreach (var line in invoice.Lines)
        {
            line.TaxBase = TaxCalculator.CalcTaxBase(line.UnitPrice, line.Quantity, line.Discount);
            line.TaxAmount = TaxCalculator.CalcTax(line.TaxBase, line.TaxRate);
        }
        invoice.TaxBase = invoice.Lines.Sum(l => l.TaxBase);
        invoice.TaxAmount = invoice.Lines.Sum(l => l.TaxAmount);
        if (invoice.StampTax == 0)
            invoice.StampTax = TaxCalculator.CalcStampTax(invoice.TaxBase, invoice.DocumentType);
        invoice.TotalAmount =
            invoice.TaxBase + invoice.TaxAmount + invoice.StampTax - invoice.WithholdingTax;
        var created = await _uow.Invoices.AddAsync(invoice);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating Invoice: {Id} - {Number}", created.Id, created.Number);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Invoice invoice)
    {
        if (id != invoice.Id)
            return BadRequest();
        _logger.LogInformation("Updating Invoice: {Id}", id);
        foreach (var line in invoice.Lines)
        {
            line.TaxBase = TaxCalculator.CalcTaxBase(line.UnitPrice, line.Quantity, line.Discount);
            line.TaxAmount = TaxCalculator.CalcTax(line.TaxBase, line.TaxRate);
        }
        invoice.TaxBase = invoice.Lines.Sum(l => l.TaxBase);
        invoice.TaxAmount = invoice.Lines.Sum(l => l.TaxAmount);
        if (invoice.StampTax == 0)
            invoice.StampTax = TaxCalculator.CalcStampTax(invoice.TaxBase, invoice.DocumentType);
        invoice.TotalAmount =
            invoice.TaxBase + invoice.TaxAmount + invoice.StampTax - invoice.WithholdingTax;
        await _uow.Invoices.UpdateAsync(invoice);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting Invoice: {Id}", id);
        var invoice = await _uow.Invoices.GetByIdAsync(id);
        if (invoice is null)
            return NotFound();
        await _uow.Invoices.SoftDeleteAsync(invoice);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("periodo")]
    public async Task<IActionResult> GetByPeriod([FromQuery] int ano, [FromQuery] int? mes)
    {
        var query = _uow
            .Invoices.Query()
            .Include(i => i.Customer)
            .Include(i => i.Lines)
            .Where(i => i.Date.Year == ano);
        if (mes.HasValue)
            query = query.Where(i => i.Date.Month == mes.Value);
        return Ok(await query.OrderByDescending(i => i.Date).ToListAsync());
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.Invoices.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"faturas_{DateTime.Now:yyyyMMdd}.csv");
    }
}
