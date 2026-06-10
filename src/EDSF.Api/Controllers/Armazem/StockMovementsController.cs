using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Armazem;

[ApiController]
[Route("api/armazem/stock")]
public class StockMovementsController(IUnitOfWork uow, ILogger<StockMovementsController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<StockMovementsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching StockMovement page {Page}", page);
        var query = _uow.StockMovements.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Type.ToString().Contains(search));
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
        _logger.LogInformation("Fetching StockMovement by ID: {Id}", id);
        var m = await _uow
            .StockMovements.Query()
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
        return m is null ? NotFound() : Ok(m);
    }

    [HttpPost]
    public async Task<IActionResult> Create(StockMovement movement)
    {
        var created = await _uow.StockMovements.AddAsync(movement);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating StockMovement: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, StockMovement movement)
    {
        if (id != movement.Id)
            return BadRequest();
        _logger.LogInformation("Updating StockMovement: {Id}", id);
        await _uow.StockMovements.UpdateAsync(movement);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting StockMovement: {Id}", id);
        var m = (await _uow.StockMovements.FindAsync(x => x.Id == id)).FirstOrDefault();
        if (m is null)
            return NotFound();
        await _uow.StockMovements.SoftDeleteAsync(m);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.StockMovements.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"stock_{DateTime.Now:yyyyMMdd}.csv");
    }
}
