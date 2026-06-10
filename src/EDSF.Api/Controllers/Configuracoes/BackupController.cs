using Microsoft.AspNetCore.Mvc;

namespace EDSF.Api.Controllers.Configuracoes;

[ApiController]
[Route("api/configuracoes/backup")]
public class BackupController(IWebHostEnvironment env, ILogger<BackupController> logger)
    : ControllerBase
{
    private readonly IWebHostEnvironment _env = env;
    private readonly ILogger<BackupController> _logger = logger;

    [HttpPost("criar")]
    public IActionResult Criar()
    {
        _logger.LogInformation("Creating database backup");
        var dbPath = Path.Combine(_env.ContentRootPath, "edsf.db");
        if (!System.IO.File.Exists(dbPath))
            return NotFound("Base de dados não encontrada.");
        var backupDir = Path.Combine(_env.ContentRootPath, "Backups");
        Directory.CreateDirectory(backupDir);
        var backupName = $"edsf_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
        var backupPath = Path.Combine(backupDir, backupName);
        System.IO.File.Copy(dbPath, backupPath, overwrite: true);
        return Ok(new { backup = backupName, path = backupPath });
    }
}
