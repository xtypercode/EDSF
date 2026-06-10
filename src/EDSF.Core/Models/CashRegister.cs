using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class CashRegister : ISoftDeletable
{
    public int Id { get; set; }
    public DateTime OpeningDate { get; set; } = DateTime.UtcNow;
    [Range(0, double.MaxValue)]
    public decimal InitialBalance { get; set; }
    public decimal? FinalBalance { get; set; }
    public DateTime? ClosingDate { get; set; }
    [StringLength(500)]
    public string? Notes { get; set; }
    public bool IsOpen => ClosingDate == null;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
