using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;
using EDSF.Core.Enums;

namespace EDSF.Core.Models;

public class FinanceRecord : ISoftDeletable
{
    public int Id { get; set; }
    public FinanceType Type { get; set; }
    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    [StringLength(100)]
    public string? Category { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
