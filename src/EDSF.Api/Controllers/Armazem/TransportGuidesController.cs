using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Armazem;

[ApiController]
[Route("api/armazem/guias-transporte")]
public class TransportGuidesController(IUnitOfWork uow, ILogger<TransportGuidesController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<TransportGuidesController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching TransportGuide page {Page}", page);
        var query = _uow.TransportGuides.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Number != null && e.Number.Contains(search));
        var total = await query.CountAsync();
        var items = await query
            .Include(t => t.Customer)
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
        _logger.LogInformation("Fetching TransportGuide by ID: {Id}", id);
        var guide = await _uow
            .TransportGuides.Query()
            .Include(t => t.Customer)
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id);
        return guide is null ? NotFound() : Ok(guide);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TransportGuide guide)
    {
        var created = await _uow.TransportGuides.AddAsync(guide);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating TransportGuide: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TransportGuide guide)
    {
        if (id != guide.Id)
            return BadRequest();
        _logger.LogInformation("Updating TransportGuide: {Id}", id);
        await _uow.TransportGuides.UpdateAsync(guide);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting TransportGuide: {Id}", id);
        var guide = await _uow.TransportGuides.GetByIdAsync(id);
        if (guide is null)
            return NotFound();
        await _uow.TransportGuides.SoftDeleteAsync(guide);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.TransportGuides.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"guias-transporte_{DateTime.Now:yyyyMMdd}.csv");
    }
}
