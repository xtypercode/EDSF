using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Gestao;

[ApiController]
[Route("api/gestao/funcionarios")]
public class EmployeesController(IUnitOfWork uow, ILogger<EmployeesController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<EmployeesController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching Employee page {Page}", page);
        var query = _uow.Employees.Query();
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
        _logger.LogInformation("Fetching Employee by ID: {Id}", id);
        var e = await _uow.Employees.GetByIdAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Employee e)
    {
        var c = await _uow.Employees.AddAsync(e);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating Employee: {Id}", c.Id);
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Employee e)
    {
        if (id != e.Id)
            return BadRequest();
        _logger.LogInformation("Updating Employee: {Id}", id);
        await _uow.Employees.UpdateAsync(e);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting Employee: {Id}", id);
        var e = await _uow.Employees.GetByIdAsync(id);
        if (e is null)
            return NotFound();
        await _uow.Employees.SoftDeleteAsync(e);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.Employees.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"funcionarios_{DateTime.Now:yyyyMMdd}.csv");
    }
}
