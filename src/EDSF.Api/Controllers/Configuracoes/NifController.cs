using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EDSF.Api.Controllers.Configuracoes;

[ApiController]
[Route("api/configuracoes/nif")]
public class NifController(IUnitOfWork uow, ILogger<NifController> logger) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<NifController> _logger = logger;

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] NifUpdateRequest request)
    {
        _logger.LogInformation("Updating NIF");
        var data = await _uow.CompanyData.GetAllAsync();
        var company = data.FirstOrDefault();
        if (company is null)
            return NotFound();
        company.Nif = request.Nif;
        company.UpdatedAt = DateTime.UtcNow;
        await _uow.CompanyData.UpdateAsync(company);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}

public record NifUpdateRequest(string Nif);
