using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Servicos;

[ApiController]
[Route("api/servicos/orcamentos")]
public class BudgetsController(IUnitOfWork uow, ILogger<BudgetsController> logger) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<BudgetsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching Budget page {Page}", page);
        var query = _uow.Budgets.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Status != null && e.Status.Contains(search));
        var total = await query.CountAsync();
        var items = await query
            .Include(b => b.Customer)
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
        _logger.LogInformation("Fetching Budget by ID: {Id}", id);
        var b = await _uow
            .Budgets.Query()
            .Include(bu => bu.Customer)
            .Include(bu => bu.Items)
            .FirstOrDefaultAsync(bu => bu.Id == id);
        return b is null ? NotFound() : Ok(b);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Budget b)
    {
        var c = await _uow.Budgets.AddAsync(b);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating Budget: {Id}", c.Id);
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Budget b)
    {
        if (id != b.Id)
            return BadRequest();
        _logger.LogInformation("Updating Budget: {Id}", id);
        await _uow.Budgets.UpdateAsync(b);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting Budget: {Id}", id);
        var b = await _uow.Budgets.GetByIdAsync(id);
        if (b is null)
            return NotFound();
        await _uow.Budgets.SoftDeleteAsync(b);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] BudgetStatusUpdateRequest request)
    {
        _logger.LogInformation("Updating Budget status: {Id}", id);
        var b = await _uow.Budgets.GetByIdAsync(id);
        if (b is null)
            return NotFound();
        b.Status = request.Status;
        await _uow.Budgets.UpdateAsync(b);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.Budgets.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"orcamentos_{DateTime.Now:yyyyMMdd}.csv");
    }
}

public record BudgetStatusUpdateRequest(string Status);
