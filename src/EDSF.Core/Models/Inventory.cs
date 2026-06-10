using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class Inventory : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    [StringLength(500)]
    public string? Notes { get; set; }
    public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}

public class InventoryItem
{
    public int Id { get; set; }
    public int InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int ExpectedQuantity { get; set; }
    public int ActualQuantity { get; set; }
    public int Difference => ActualQuantity - ExpectedQuantity;

    [ConcurrencyCheck] public int Version { get; set; }
}
