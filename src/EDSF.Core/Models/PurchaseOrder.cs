using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class PurchaseOrder : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Number { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    [Required, StringLength(50)]
    public string Status { get; set; } = "Pending";
    [StringLength(500)]
    public string? Notes { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}

public class PurchaseOrderItem
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
