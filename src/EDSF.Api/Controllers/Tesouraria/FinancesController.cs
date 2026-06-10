using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Tesouraria;

[ApiController]
[Route("api/tesouraria/financas")]
public class FinancesController(IUnitOfWork uow, ILogger<FinancesController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<FinancesController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching FinanceRecord page {Page}", page);
        var query = _uow.FinanceRecords.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Description != null && e.Description.Contains(search));
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
        _logger.LogInformation("Fetching FinanceRecord by ID: {Id}", id);
        var record = await _uow.FinanceRecords.GetByIdAsync(id);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpPost]
    public async Task<IActionResult> Create(FinanceRecord record)
    {
        var created = await _uow.FinanceRecords.AddAsync(record);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating FinanceRecord: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, FinanceRecord record)
    {
        if (id != record.Id)
            return BadRequest();
        _logger.LogInformation("Updating FinanceRecord: {Id}", id);
        await _uow.FinanceRecords.UpdateAsync(record);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting FinanceRecord: {Id}", id);
        var record = await _uow.FinanceRecords.GetByIdAsync(id);
        if (record is null)
            return NotFound();
        await _uow.FinanceRecords.SoftDeleteAsync(record);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.FinanceRecords.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"financas_{DateTime.Now:yyyyMMdd}.csv");
    }
}
