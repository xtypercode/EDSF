using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Servicos;

[ApiController]
[Route("api/servicos/clientes")]
public class ClientesController(IUnitOfWork uow, ILogger<ClientesController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<ClientesController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching Customer page {Page}", page);
        var query = _uow.Customers.Query();
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
        _logger.LogInformation("Fetching Customer by ID: {Id}", id);
        var c = await _uow
            .Customers.Query()
            .Include(cu => cu.Invoices)
            .FirstOrDefaultAsync(cu => cu.Id == id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        var created = await _uow.Customers.AddAsync(customer);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating Customer: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Customer customer)
    {
        if (id != customer.Id)
            return BadRequest();
        _logger.LogInformation("Updating Customer: {Id}", id);
        await _uow.Customers.UpdateAsync(customer);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting Customer: {Id}", id);
        var c = await _uow.Customers.GetByIdAsync(id);
        if (c is null)
            return NotFound();
        await _uow.Customers.SoftDeleteAsync(c);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.Customers.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"clientes_{DateTime.Now:yyyyMMdd}.csv");
    }
}
