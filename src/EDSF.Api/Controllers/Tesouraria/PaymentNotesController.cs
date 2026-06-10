using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Tesouraria;

[ApiController]
[Route("api/tesouraria/notas-pagamento")]
public class PaymentNotesController(IUnitOfWork uow, ILogger<PaymentNotesController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<PaymentNotesController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching PaymentNote page {Page}", page);
        var query = _uow.PaymentNotes.Query();
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
        _logger.LogInformation("Fetching PaymentNote by ID: {Id}", id);
        var note = await _uow
            .PaymentNotes.Query()
            .Include(p => p.Customer)
            .FirstOrDefaultAsync(p => p.Id == id);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PaymentNote note)
    {
        var created = await _uow.PaymentNotes.AddAsync(note);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating PaymentNote: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PaymentNote note)
    {
        if (id != note.Id)
            return BadRequest();
        _logger.LogInformation("Updating PaymentNote: {Id}", id);
        await _uow.PaymentNotes.UpdateAsync(note);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting PaymentNote: {Id}", id);
        var note = await _uow.PaymentNotes.GetByIdAsync(id);
        if (note is null)
            return NotFound();
        await _uow.PaymentNotes.SoftDeleteAsync(note);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.PaymentNotes.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"notas-pagamento_{DateTime.Now:yyyyMMdd}.csv");
    }
}
