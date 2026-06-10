using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class Budget : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Number { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime? ValidUntil { get; set; }
    [Required, StringLength(50)]
    public string Status { get; set; } = "Draft";
    [StringLength(500)]
    public string? Notes { get; set; }
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }
    public ICollection<BudgetItem> Items { get; set; } = new List<BudgetItem>();

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}

public class BudgetItem
{
    public int Id { get; set; }
    public int BudgetId { get; set; }
    public Budget Budget { get; set; } = null!;
    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
