using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Armazem;

[ApiController]
[Route("api/armazem/compras")]
public class PurchaseOrdersController(IUnitOfWork uow, ILogger<PurchaseOrdersController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<PurchaseOrdersController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching PurchaseOrder page {Page}", page);
        var query = _uow.PurchaseOrders.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Status != null && e.Status.Contains(search));
        var total = await query.CountAsync();
        var items = await query
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
        _logger.LogInformation("Fetching PurchaseOrder by ID: {Id}", id);
        var po = await _uow
            .PurchaseOrders.Query()
            .Include(p => p.Supplier)
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id);
        return po is null ? NotFound() : Ok(po);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PurchaseOrder po)
    {
        var c = await _uow.PurchaseOrders.AddAsync(po);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating PurchaseOrder: {Id}", c.Id);
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PurchaseOrder po)
    {
        if (id != po.Id)
            return BadRequest();
        _logger.LogInformation("Updating PurchaseOrder: {Id}", id);
        await _uow.PurchaseOrders.UpdateAsync(po);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting PurchaseOrder: {Id}", id);
        var po = await _uow.PurchaseOrders.GetByIdAsync(id);
        if (po is null)
            return NotFound();
        await _uow.PurchaseOrders.SoftDeleteAsync(po);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] PurchaseOrderStatusUpdateRequest request)
    {
        _logger.LogInformation("Updating PurchaseOrder status: {Id}", id);
        var po = await _uow.PurchaseOrders.GetByIdAsync(id);
        if (po is null)
            return NotFound();
        po.Status = request.Status;
        await _uow.PurchaseOrders.UpdateAsync(po);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.PurchaseOrders.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"compras_{DateTime.Now:yyyyMMdd}.csv");
    }
}

public record PurchaseOrderStatusUpdateRequest(string Status);
