using EDSF.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Api.Controllers.Configuracoes;

[ApiController]
[Route("api/configuracoes/auditoria")]
public class AuditoriaController(IUnitOfWork uow, ILogger<AuditoriaController> logger)
    : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<AuditoriaController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? entityName = null,
        [FromQuery] string? action = null
    )
    {
        _logger.LogInformation("Fetching audit logs page {Page}", page);
        var query = _uow.AuditLogs.Query();
        if (!string.IsNullOrEmpty(entityName))
            query = query.Where(a => a.EntityName.Contains(entityName));
        if (!string.IsNullOrEmpty(action))
            query = query.Where(a => a.Action.Equals(action));
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.Timestamp)
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
}
