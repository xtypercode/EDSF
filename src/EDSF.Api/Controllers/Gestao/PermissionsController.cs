using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Gestao;

[ApiController]
[Route("api/gestao/permissoes")]
public class PermissionsController(IUnitOfWork uow, ILogger<PermissionsController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<PermissionsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching Permission page {Page}", page);
        var query = _uow.Permissions.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Module != null && e.Module.Contains(search));
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
        _logger.LogInformation("Fetching Permission by ID: {Id}", id);
        var perm = await _uow
            .Permissions.Query()
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(p => p.Id == id);
        return perm is null ? NotFound() : Ok(perm);
    }

    [HttpGet("usuario/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        _logger.LogInformation("Fetching Permissions for user: {UserId}", userId);
        return Ok(await _uow.Permissions.FindAsync(p => p.AppUserId == userId));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Permission permission)
    {
        var created = await _uow.Permissions.AddAsync(permission);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating Permission: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Permission permission)
    {
        if (id != permission.Id)
            return BadRequest();
        _logger.LogInformation("Updating Permission: {Id}", id);
        await _uow.Permissions.UpdateAsync(permission);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting Permission: {Id}", id);
        var perm = await _uow.Permissions.GetByIdAsync(id);
        if (perm is null)
            return NotFound();
        await _uow.Permissions.SoftDeleteAsync(perm);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
