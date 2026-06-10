using EDSF.Core.Interfaces;
using EDSF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Tesouraria;

[ApiController]
[Route("api/tesouraria/saft")]
public class SaftController(IUnitOfWork uow, ILogger<SaftController> logger) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<SaftController> _logger = logger;

    [HttpPost("exportar")]
    public async Task<IActionResult> Exportar([FromQuery] int fiscalYear)
    {
        _logger.LogInformation("Generating A-SAF-T for fiscal year {Year}", fiscalYear);
        var company = (await _uow.CompanyData.GetAllAsync()).FirstOrDefault();
        if (company is null)
            return BadRequest(new { error = "Dados da empresa não configurados." });
        var invoices = await _uow
            .Invoices.Query()
            .Include(i => i.Lines)
            .Include(i => i.Customer)
            .Where(i => i.Date.Year == fiscalYear && !i.IsDeleted)
            .ToListAsync();
        var customers = await _uow.Customers.Query().Where(c => !c.IsDeleted).ToListAsync();
        var generator = new ASaftGenerator();
        var xml = generator.Generate(company, invoices, customers, fiscalYear);
        return File(
            System.Text.Encoding.UTF8.GetBytes(xml),
            "application/xml",
            $"SAFT_{fiscalYear}.xml"
        );
    }

    [HttpPost("exportar-periodo")]
    public async Task<IActionResult> ExportarPeriodo(
        [FromQuery] DateTime inicio,
        [FromQuery] DateTime fim
    )
    {
        _logger.LogInformation("Generating A-SAF-T for period: {Start} - {End}", inicio, fim);
        var company = (await _uow.CompanyData.GetAllAsync()).FirstOrDefault();
        if (company is null)
            return BadRequest(new { error = "Dados da empresa não configurados." });
        var invoices = await _uow
            .Invoices.Query()
            .Include(i => i.Lines)
            .Include(i => i.Customer)
            .Where(i => i.Date >= inicio && i.Date <= fim && !i.IsDeleted)
            .ToListAsync();
        var customers = await _uow.Customers.Query().Where(c => !c.IsDeleted).ToListAsync();
        var generator = new ASaftGenerator();
        var xml = generator.Generate(company, invoices, customers, inicio.Year);
        return File(
            System.Text.Encoding.UTF8.GetBytes(xml),
            "application/xml",
            $"SAFT_{inicio:yyyyMMdd}-{fim:yyyyMMdd}.xml"
        );
    }
}
