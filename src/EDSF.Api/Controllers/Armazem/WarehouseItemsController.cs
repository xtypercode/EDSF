using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Armazem;

[ApiController]
[Route("api/armazem/itens")]
public class WarehouseItemsController(IUnitOfWork uow, ILogger<WarehouseItemsController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<WarehouseItemsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching WarehouseItem page {Page}", page);
        var query = _uow.WarehouseItems.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Name != null && e.Name.Contains(search));
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
        _logger.LogInformation("Fetching WarehouseItem by ID: {Id}", id);
        var item = await _uow.WarehouseItems.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(WarehouseItem item)
    {
        var created = await _uow.WarehouseItems.AddAsync(item);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating WarehouseItem: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, WarehouseItem item)
    {
        if (id != item.Id)
            return BadRequest();
        _logger.LogInformation("Updating WarehouseItem: {Id}", id);
        await _uow.WarehouseItems.UpdateAsync(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting WarehouseItem: {Id}", id);
        var item = await _uow.WarehouseItems.GetByIdAsync(id);
        if (item is null)
            return NotFound();
        await _uow.WarehouseItems.SoftDeleteAsync(item);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.WarehouseItems.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"itens-armazem_{DateTime.Now:yyyyMMdd}.csv");
    }
}
