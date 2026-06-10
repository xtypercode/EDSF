using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class WarehouseItem : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [StringLength(1000)]
    public string? Description { get; set; }
    [StringLength(100)]
    public string? Category { get; set; }
    [StringLength(300)]
    public string? Location { get; set; }
    public int Quantity { get; set; }
    [Range(0, double.MaxValue)]
    public decimal UnitCost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
