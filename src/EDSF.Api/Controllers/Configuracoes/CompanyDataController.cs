using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EDSF.Api.Controllers.Configuracoes;

[ApiController]
[Route("api/configuracoes/empresa")]
public class CompanyDataController(IUnitOfWork uow, ILogger<CompanyDataController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<CompanyDataController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("Fetching company data");
        var data = await _uow.CompanyData.GetAllAsync();
        var company = data.FirstOrDefault();
        return company is null ? NotFound() : Ok(company);
    }

    [HttpPut]
    public async Task<IActionResult> Update(CompanyData company)
    {
        _logger.LogInformation("Updating company data");
        var data = await _uow.CompanyData.GetAllAsync();
        var existing = data.FirstOrDefault();
        if (existing is null)
        {
            company.UpdatedAt = DateTime.UtcNow;
            await _uow.CompanyData.AddAsync(company);
        }
        else
        {
            existing.Name = company.Name;
            existing.Nif = company.Nif;
            existing.Address = company.Address;
            existing.Phone = company.Phone;
            existing.Email = company.Email;
            existing.TaxRegime = company.TaxRegime;
            existing.LogoUrl = company.LogoUrl;
            existing.UpdatedAt = DateTime.UtcNow;
            await _uow.CompanyData.UpdateAsync(existing);
        }
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
