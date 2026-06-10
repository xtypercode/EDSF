using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Contabilidade;

[ApiController]
[Route("api/contabilidade/inventarios")]
public class InventoriesController(IUnitOfWork uow, ILogger<InventoriesController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<InventoriesController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all Inventory");
        return Ok(await _uow.Inventories.Query().ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Fetching Inventory by ID: {Id}", id);
        var inv = await _uow.Inventories.GetByIdAsync(id);
        return inv is null ? NotFound() : Ok(inv);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Inventory inventory)
    {
        var created = await _uow.Inventories.AddAsync(inventory);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating Inventory: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Inventory inventory)
    {
        if (id != inventory.Id)
            return BadRequest();
        _logger.LogInformation("Updating Inventory: {Id}", id);
        await _uow.Inventories.UpdateAsync(inventory);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting Inventory: {Id}", id);
        var inv = await _uow.Inventories.GetByIdAsync(id);
        if (inv is null)
            return NotFound();
        await _uow.Inventories.SoftDeleteAsync(inv);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
