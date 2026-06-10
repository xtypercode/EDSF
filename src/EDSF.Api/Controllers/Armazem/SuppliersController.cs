using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Armazem;

[ApiController]
[Route("api/armazem/fornecedores")]
public class SuppliersController(IUnitOfWork uow, ILogger<SuppliersController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<SuppliersController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching Supplier page {Page}", page);
        var query = _uow.Suppliers.Query();
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
        _logger.LogInformation("Fetching Supplier by ID: {Id}", id);
        var s = await _uow.Suppliers.GetByIdAsync(id);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Supplier s)
    {
        var c = await _uow.Suppliers.AddAsync(s);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating Supplier: {Id}", c.Id);
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Supplier s)
    {
        if (id != s.Id)
            return BadRequest();
        _logger.LogInformation("Updating Supplier: {Id}", id);
        await _uow.Suppliers.UpdateAsync(s);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting Supplier: {Id}", id);
        var s = await _uow.Suppliers.GetByIdAsync(id);
        if (s is null)
            return NotFound();
        await _uow.Suppliers.SoftDeleteAsync(s);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.Suppliers.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"fornecedores_{DateTime.Now:yyyyMMdd}.csv");
    }
}
