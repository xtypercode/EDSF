using EDSF.Core.Enums;
using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EDSF.Api.Controllers.Configuracoes;

[ApiController]
[Route("api/configuracoes/regime")]
public class RegimeController(IUnitOfWork uow, ILogger<RegimeController> logger) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<RegimeController> _logger = logger;

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RegimeUpdateRequest request)
    {
        _logger.LogInformation("Updating tax regime");
        var data = await _uow.CompanyData.GetAllAsync();
        var company = data.FirstOrDefault();
        if (company is null)
            return NotFound();
        company.TaxRegime = request.Regime;
        company.UpdatedAt = DateTime.UtcNow;
        await _uow.CompanyData.UpdateAsync(company);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}

public record RegimeUpdateRequest(TaxRegime Regime);
