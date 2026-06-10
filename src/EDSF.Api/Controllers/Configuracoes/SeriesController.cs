using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Configuracoes;

[ApiController]
[Route("api/configuracoes/series")]
public class SeriesController(IUnitOfWork uow, ILogger<SeriesController> logger) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<SeriesController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _uow
            .InvoiceSeries.Query()
            .OrderByDescending(s => s.FiscalYear)
            .ThenBy(s => s.Series)
            .ToListAsync();
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(InvoiceSeries series)
    {
        var created = await _uow.InvoiceSeries.AddAsync(series);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, InvoiceSeries series)
    {
        if (id != series.Id)
            return BadRequest();
        await _uow.InvoiceSeries.UpdateAsync(series);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _uow.InvoiceSeries.GetByIdAsync(id);
        if (s is null)
            return NotFound();
        await _uow.InvoiceSeries.DeleteAsync(s);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
