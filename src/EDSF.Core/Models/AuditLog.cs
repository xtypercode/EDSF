using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class AuditLog : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(200)]
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    [Required, StringLength(100)]
    public string Action { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    [StringLength(100)]
    public string? UserName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    [StringLength(45)]
    public string? IpAddress { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
