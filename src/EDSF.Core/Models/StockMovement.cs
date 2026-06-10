using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;
using EDSF.Core.Enums;

namespace EDSF.Core.Models;

public class StockMovement : ISoftDeletable
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    [StringLength(500)]
    public string? Notes { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
