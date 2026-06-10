using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Armazem;

[ApiController]
[Route("api/armazem/produtos")]
public class ProductsController(IUnitOfWork uow, ILogger<ProductsController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<ProductsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        _logger.LogInformation("Fetching Product page {Page}", page);
        var query = _uow.Products.Query();
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
        _logger.LogInformation("Fetching Product by ID: {Id}", id);
        var product = await _uow
            .Products.Query()
            .Include(p => p.StockMovements)
            .FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        var created = await _uow.Products.AddAsync(product);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Creating Product: {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id)
            return BadRequest();
        _logger.LogInformation("Updating Product: {Id}", id);
        await _uow.Products.UpdateAsync(product);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogWarning("Deleting Product: {Id}", id);
        var product = await _uow.Products.GetByIdAsync(id);
        if (product is null)
            return NotFound();
        await _uow.Products.SoftDeleteAsync(product);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var data = await _uow.Products.GetAllAsync();
        var csv = CsvExportService.ExportToCsv(data.ToList());
        return File(csv, "text/csv", $"produtos_{DateTime.Now:yyyyMMdd}.csv");
    }
}
