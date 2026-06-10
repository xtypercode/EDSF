using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Tesouraria;

[ApiController]
[Route("api/tesouraria/caixa")]
public class CashRegistersController(IUnitOfWork uow, ILogger<CashRegistersController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<CashRegistersController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Fetching CashRegister page {Page}", page);
        var query = _uow.CashRegisters.Query();
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
        _logger.LogInformation("Fetching CashRegister by ID: {Id}", id);
        var reg = await _uow.CashRegisters.GetByIdAsync(id);
        return reg is null ? NotFound() : Ok(reg);
    }

    [HttpPost("abrir")]
    public async Task<IActionResult> Abrir([FromBody] CashRegister register)
    {
        register.OpeningDate = DateTime.UtcNow;
        var created = await _uow.CashRegisters.AddAsync(register);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating CashRegister: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}/fechar")]
    public async Task<IActionResult> Fechar(int id, [FromQuery] decimal finalBalance)
    {
        _logger.LogInformation("Closing CashRegister: {Id}", id);
        var reg = await _uow.CashRegisters.GetByIdAsync(id);
        if (reg is null)
            return NotFound();
        reg.FinalBalance = finalBalance;
        reg.ClosingDate = DateTime.UtcNow;
        await _uow.CashRegisters.UpdateAsync(reg);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
