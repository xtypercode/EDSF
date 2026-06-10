using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Tesouraria;

[ApiController]
[Route("api/tesouraria/adiantamentos")]
public class AdvancePaymentsController(IUnitOfWork uow, ILogger<AdvancePaymentsController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<AdvancePaymentsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching AdvancePayment page {Page}", page);
        var query = _uow.AdvancePayments.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.EmployeeName != null && e.EmployeeName.Contains(search));
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
        _logger.LogInformation("Fetching AdvancePayment by ID: {Id}", id);
        var adv = await _uow.AdvancePayments.GetByIdAsync(id);
        return adv is null ? NotFound() : Ok(adv);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdvancePayment adv)
    {
        var created = await _uow.AdvancePayments.AddAsync(adv);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating AdvancePayment: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AdvancePayment adv)
    {
        if (id != adv.Id)
            return BadRequest();
        _logger.LogInformation("Updating AdvancePayment: {Id}", id);
        await _uow.AdvancePayments.UpdateAsync(adv);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting AdvancePayment: {Id}", id);
        var adv = await _uow.AdvancePayments.GetByIdAsync(id);
        if (adv is null)
            return NotFound();
        await _uow.AdvancePayments.SoftDeleteAsync(adv);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.AdvancePayments.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"adiantamentos_{DateTime.Now:yyyyMMdd}.csv");
    }
}
