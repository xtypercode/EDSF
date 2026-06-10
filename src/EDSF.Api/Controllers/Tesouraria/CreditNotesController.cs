using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Tesouraria;

[ApiController]
[Route("api/tesouraria/notas-credito")]
public class CreditNotesController(IUnitOfWork uow, ILogger<CreditNotesController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<CreditNotesController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching CreditNote page {Page}", page);
        var query = _uow.CreditNotes.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Number != null && e.Number.Contains(search));
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
        _logger.LogInformation("Fetching CreditNote by ID: {Id}", id);
        var note = await _uow
            .CreditNotes.Query()
            .Include(c => c.Customer)
            .Include(c => c.Invoice)
            .FirstOrDefaultAsync(c => c.Id == id);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreditNote note)
    {
        var created = await _uow.CreditNotes.AddAsync(note);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating CreditNote: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreditNote note)
    {
        if (id != note.Id)
            return BadRequest();
        _logger.LogInformation("Updating CreditNote: {Id}", id);
        await _uow.CreditNotes.UpdateAsync(note);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting CreditNote: {Id}", id);
        var note = await _uow.CreditNotes.GetByIdAsync(id);
        if (note is null)
            return NotFound();
        await _uow.CreditNotes.SoftDeleteAsync(note);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.CreditNotes.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"notas-credito_{DateTime.Now:yyyyMMdd}.csv");
    }
}
