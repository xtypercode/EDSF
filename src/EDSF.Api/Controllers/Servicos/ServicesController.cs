using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Servicos;

[ApiController]
[Route("api/servicos/servicos")]
public class ServicesController(IUnitOfWork uow, ILogger<ServicesController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<ServicesController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching Service page {Page}", page);
        var query = _uow.Services.Query();
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
        _logger.LogInformation("Fetching Service by ID: {Id}", id);
        var s = await _uow.Services.GetByIdAsync(id);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Service service)
    {
        var created = await _uow.Services.AddAsync(service);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating Service: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Service service)
    {
        if (id != service.Id)
            return BadRequest();
        _logger.LogInformation("Updating Service: {Id}", id);
        await _uow.Services.UpdateAsync(service);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting Service: {Id}", id);
        var s = await _uow.Services.GetByIdAsync(id);
        if (s is null)
            return NotFound();
        await _uow.Services.SoftDeleteAsync(s);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.Services.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"servicos_{DateTime.Now:yyyyMMdd}.csv");
    }
}
