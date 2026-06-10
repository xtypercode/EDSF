using System.Text;
using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Gestao;

[ApiController]
[Route("api/gestao/usuarios")]
public class UsersController(
    IUnitOfWork uow,
    ICsvExportService csvExport,
    ILogger<UsersController> logger
) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ICsvExportService _csvExport = csvExport;
    private readonly ILogger<UsersController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching User page {Page}", page);
        var query = _uow.AppUsers.Query();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Username != null && e.Username.Contains(search));
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

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var users = await _uow.AppUsers.GetAllAsync();
        var csv = _csvExport.Export(users, ["Id", "Username", "Email", "IsActive", "CreatedAt"]);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", "users.csv");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Fetching User by ID: {Id}", id);
        var user = await _uow.AppUsers.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AppUser user)
    {
        user.CreatedAt = DateTime.UtcNow;
        var created = await _uow.AppUsers.AddAsync(user);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating User: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AppUser user)
    {
        if (id != user.Id)
            return BadRequest();
        _logger.LogInformation("Updating User: {Id}", id);
        await _uow.AppUsers.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting User: {Id}", id);
        var user = await _uow.AppUsers.GetByIdAsync(id);
        if (user is null)
            return NotFound();
        await _uow.AppUsers.SoftDeleteAsync(user);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
